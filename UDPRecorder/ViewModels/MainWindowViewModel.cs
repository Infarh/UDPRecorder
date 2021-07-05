using MathCore.Hosting;
using MathCore.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using UDPRecorder.Services.Interfaces;

namespace UDPRecorder.ViewModels
{
    [Service(ServiceLifetime.Scoped)]
    public class MainWindowViewModel : TitledViewModel
    {
        [Inject]
        public IUserDialog UserDialog { get; init; }

        public MainWindowViewModel() => Title = "Главное окно программы";

        #region Status : string - Статус

        /// <summary>Статус</summary>
        private string _Status = "Готов.";

        /// <summary>Статус</summary>
        public string Status { get => _Status; set => Set(ref _Status, value); }

        #endregion
    }
}
