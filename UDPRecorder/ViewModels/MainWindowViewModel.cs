using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using MathCore.Hosting;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using UDPRecorder.Services.Interfaces;
using UDPRecorder.Services.Models;

namespace UDPRecorder.ViewModels
{
    [Service(ServiceLifetime.Scoped)]
    public class MainWindowViewModel : TitledViewModel
    {
        [Inject] public IUserDialog UserDialog { get; init; }

        [Inject] public IUdpRecorder UdpRecorder { get; init; }

        public MainWindowViewModel() => Title = "Главное окно программы";

        #region Status : string - Статус

        /// <summary>Статус</summary>
        private string _Status = "Готов.";

        /// <summary>Статус</summary>
        public string Status { get => _Status; set => Set(ref _Status, value); }

        #endregion

        #region Port : int - Порт

        /// <summary>Порт</summary>
        private int _Port;

        /// <summary>Порт</summary>
        public int Port { get => _Port; set => Set(ref _Port, value, port => port is > 0 and <= 65535); }

        #endregion

        #region DataFilePath : string - Файл данных для записи

        /// <summary>Файл данных для записи</summary>
        private string _DataFilePath = "data.bin";

        /// <summary>Файл данных для записи</summary>
        public string DataFilePath { get => _DataFilePath; private set => Set(ref _DataFilePath, value); }

        #endregion

        [DependencyOn(nameof(DataFilePath))]
        public string DataFileName => Path.GetFileName(DataFilePath ?? "");

        #region Command SelectDataFileCommand - Выбрать файл для записи

        /// <summary>Выбрать файл для записи</summary>
        private Command _SelectDataFileCommand;

        /// <summary>Выбрать файл для записи</summary>
        public ICommand SelectDataFileCommand => _SelectDataFileCommand ??= Command.New(
            () =>
            {
                if (UserDialog.SaveFile("Выбор файла", DefaultFilePath: "data.bin") is not { } file) return;
                DataFilePath = file.FullName;
            });

        #endregion

        #region DataFileNameAddTime : bool - Добавлять время начала записи в имя файла

        /// <summary>Добавлять время начала записи в имя файла</summary>
        private bool _DataFileNameAddTime = true;

        /// <summary>Добавлять время начала записи в имя файла</summary>
        public bool DataFileNameAddTime { get => _DataFileNameAddTime; set => Set(ref _DataFileNameAddTime, value); }

        #endregion

        #region Command StartRecordCommand - Начать запись

        /// <summary>Начать запись</summary>
        private Command _StartRecordCommand;

        /// <summary>Начать запись</summary>
        public ICommand StartRecordCommand => _StartRecordCommand ??= Command.New(OnStartRecordCommandExecuted, CanStartRecordCommandExecute);

        /// <summary>Проверка возможности выполнения - Начать запись</summary>
        private bool CanStartRecordCommandExecute() => DataFileName is { Length: > 0 };

        /// <summary>Логика выполнения - Начать запись</summary>
        private async Task OnStartRecordCommandExecuted()
        {
            var cancellation = new CancellationTokenSource();
            _RecordProcessCancellation = cancellation;
            var progress = new Progress<RecordProcessInfo>(OnRecordProgress);
            _RecordProcessTask = UdpRecorder.StartRecordingAsync(Port, DataFilePath, progress, cancellation.Token);
            OnPropertyChanged(nameof(IsRecordStarted));
            await _RecordProcessTask;
            _RecordProcessCancellation = null;
            _RecordProcessTask = null;
            OnPropertyChanged(nameof(IsRecordStarted));
        }

        private void OnRecordProgress(RecordProcessInfo progress) => RecordedBytesCount = progress.ReceivedBytesCount;

        #endregion

        private CancellationTokenSource _RecordProcessCancellation;
        private Task _RecordProcessTask;

        public bool IsRecordStarted => _RecordProcessCancellation != null;

        #region Command StopRecordCommand - Остановить запись

        /// <summary>Остановить запись</summary>
        private Command _StopRecordCommand;

        /// <summary>Остановить запись</summary>
        public ICommand StopRecordCommand => _StopRecordCommand ??= Command.New(
            () => _RecordProcessCancellation?.Cancel(),
            () => IsRecordStarted);

        #endregion

        #region RecordedBytesCount : long - Число записанных байт

        /// <summary>Число записанных байт</summary>
        private long _RecordedBytesCount;

        /// <summary>Число записанных байт</summary>
        public long RecordedBytesCount { get => _RecordedBytesCount; private set => Set(ref _RecordedBytesCount, value); }

        #endregion
    }
}
