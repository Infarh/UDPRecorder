using MathCore.Hosting;
using MathCore.WPF.Services;
using Microsoft.Extensions.DependencyInjection;

namespace UDPRecorder.Services.Interfaces
{
    [Service(ServiceLifetime.Transient, Implementation = typeof(UserDialogService))]
    public interface IUserDialog : IUserDialogBase
    {
        
    }
}
