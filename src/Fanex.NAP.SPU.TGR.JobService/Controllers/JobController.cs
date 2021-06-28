namespace Fanex.NAP.SPU.TGR.JobService.Controllers
{
    using Fanex.NAP.SPU.VR.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
       

        public JobController(ISentryClient sentryClient)
        {
            this.sentryClient = sentryClient;          
        }        
    }
}
