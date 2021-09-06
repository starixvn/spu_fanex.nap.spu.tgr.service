namespace Fanex.NAP.SPU.TGR.JobService.Jobs
{
    using Fanex.MailDelivery.Client.Jobs;
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.SPU.TGR.Features.AutoTrading;
    using Fanex.NAP.SPU.TGR.JobService.Services;
    using Microsoft.Extensions.Configuration;    
    using Quartz;
    using System;
    using System.IO;

    public class AutoTradingViaEmailJob : JobBase
    {
        protected override JobMessage Run(Schedule schedule, IJobExecutionContext jobContext)
        {
            var configuration = new ConfigurationBuilder()
                                  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                  .AddJsonFile("appsettings.json", false, false)
                                  .Build();

            var appSetting = new AppSettings(configuration);

            var autoTradingViaEmailJobService = new AutoTradingViaEmailJobService(appSetting, new AutoTradingService());

            autoTradingViaEmailJobService.Execute();

            var fromDate = "07-Nov-2019";
            var toDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToString("dd-MMM-yyyy");
            var saveFileName = "Model TA Performance Event Details - " + fromDate + " to " + toDate + ".xlsx";

            return new JobMessage
            {
                Mail = autoTradingViaEmailJobService.GenerateMail(saveFileName),
                Schedule = schedule,
                JobStatus = autoTradingViaEmailJobService.IsSuccessAPIResponse ? JobStatus.Success : JobStatus.JobFailed
            };

        }    
              
    }
}
