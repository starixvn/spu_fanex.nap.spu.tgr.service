namespace Fanex.NAP.SPU.TGRS.JobService.Services
{
    using Fanex.LetstalkService.Client;
    using Fanex.NAP.SPU.TGR.Features.AutoTrading;
    using Fanex.NAP.SPU.TGR.JobService.Jobs;
    using Fanex.NAP.SPU.TGR.JobService.Services;
    using Fanex.NAP.SPU.TGR.Shares;
    using Microsoft.Extensions.DependencyInjection;

    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {
            RegisterAutoTradingService(services);            
        }

        private static void RegisterAutoTradingService(this IServiceCollection services)
        {            
            services.AddSingleton<LetstalkClientServiceImp, LetstalkClientServiceImp>();
            services.AddSingleton<ILetstalkNotify, LetstalkNotify>();
            services.AddSingleton<IAutoTradingService, AutoTradingService>();            
            services.AddSingleton<IAutoTradingViaLetstalkJobService, AutoTradingViaLetstalkJobService>();
            services.AddSingleton<IAutoTradingViaEmailJobService, AutoTradingViaEmailJobService>();            
        }        
    }
}
