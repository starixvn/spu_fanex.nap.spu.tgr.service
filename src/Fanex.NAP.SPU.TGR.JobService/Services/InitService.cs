using Microsoft.Extensions.DependencyInjection;

namespace Fanex.NAP.SPU.VR.JobService.Services
{
    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {
            RegisterAutoTradingService(services);            
        }

        private static void RegisterAutoTradingService(this IServiceCollection services)
        {
          
        }        
    }
}
