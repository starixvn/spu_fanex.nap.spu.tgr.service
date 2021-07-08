namespace Fanex.NAP.SPU.TGR.JobService.Controllers
{
    using Fanex.NAP.SPU.TGR.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IAutoTradingViaLetstalkJobService autoTradingViaLetstalkJobService;                 

        public JobController(ISentryClient sentryClient, 
            IAutoTradingViaLetstalkJobService autoTradingViaLetstalkJobService)
        {
            this.sentryClient = sentryClient;
            this.autoTradingViaLetstalkJobService = autoTradingViaLetstalkJobService;            
        }

        [HttpPost("autotradingvialetstalk")]
        public async Task<bool> AutoTradingViaLetstalk(string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await this.autoTradingViaLetstalkJobService.Execute();
        }

        [HttpPost("WriteLog")]
        public void WriteLog(string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

        }
    }
}
