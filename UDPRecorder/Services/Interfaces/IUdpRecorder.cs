using MathCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace UDPRecorder.Services.Interfaces
{
    [Service(ServiceLifetime.Transient, Implementation = typeof(UpdRecorderService))]
    public interface IUdpRecorder
    {
    }
}
