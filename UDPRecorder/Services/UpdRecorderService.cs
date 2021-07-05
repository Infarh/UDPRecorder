using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using UDPRecorder.Services.Interfaces;
using UDPRecorder.Services.Models;

namespace UDPRecorder.Services
{
    public class UpdRecorderService : IUdpRecorder
    {
        private readonly ReadOnlyMemory<byte> _Sync = new(new byte[] { 0, 1, 2, 3 });

        public async Task StartRecordingAsync(int Port, string DataFile, IProgress<RecordProcessInfo> Progress, CancellationToken Cancel)
        {
            Cancel.ThrowIfCancellationRequested();

            using var client = new UdpClient(Port);
            await using var cancellation = Cancel.Register(() => client.Dispose());

            await using var file = File.Create(DataFile);
            var timer = Stopwatch.StartNew();
            var time_buffer = new byte[8];
            var bytes_count = 0L;
            while (!Cancel.IsCancellationRequested)
                try
                {
                    var received = await client.ReceiveAsync().ConfigureAwait(false);
                    await file.WriteAsync(_Sync, Cancel).ConfigureAwait(false);
                    BitConverter.TryWriteBytes(new Span<byte>(time_buffer), timer.ElapsedMilliseconds);
                    await file.WriteAsync(time_buffer, Cancel).ConfigureAwait(false);
                    await file.WriteAsync(received.Buffer, Cancel).ConfigureAwait(false);

                    bytes_count += received.Buffer.Length;
                    Progress?.Report(new RecordProcessInfo
                    {
                        ReceivedBytesCount = bytes_count,
                    });
                }
                catch (ObjectDisposedException) when (Cancel.IsCancellationRequested)
                {
                }
            Cancel.ThrowIfCancellationRequested();
        }
    }
}
