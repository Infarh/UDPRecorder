using System;
using System.Threading;
using System.Threading.Tasks;
using MathCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using UDPRecorder.Services.Models;

namespace UDPRecorder.Services.Interfaces
{
    [Service(ServiceLifetime.Transient, Implementation = typeof(UpdRecorderService))]
    public interface IUdpRecorder
    {
        Task StartRecordingAsync(int Port, string DataFile, IProgress<RecordProcessInfo> Progress, CancellationToken Cancel);
    }
}
