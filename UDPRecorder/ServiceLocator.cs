using System;
using Microsoft.Extensions.DependencyInjection;
using UDPRecorder.ViewModels;

namespace UDPRecorder
{
    public class ServiceLocator : MathCore.Hosting.ServiceLocator
    {
        protected override IServiceProvider Services => App.Services;

        public MainWindowViewModel MainModel => Services.GetRequiredService<MainWindowViewModel>();
    }
}
