namespace Fanex.NAP.SPU.TGR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.TGR.Features.AutoTrading;
    using Fanex.NAP.SPU.TGR.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAutoTradingViaLetstalkJobService
    {
        Task<bool> Execute();
    }

    public class AutoTradingViaLetstalkJobService : IAutoTradingViaLetstalkJobService
    {
        private const string FileName_AutoTrading_PTAndAfterVRAdjustment = "AT1PTAndAfterVRAdjustment.png";
        private const string FileName_AutoTrading_TAMonthly = "AT2TAMonthly.png";
        private const string FileName_AutoTrading_RT3AndTAComparition = "AT3RT3AndTAComparition.png";
        private const string Endpoint_TAMonthly = "AutoTrading/GetAutoTradingTAMonthly";
        private const string Endpoint_TAndAfterVRAdjustment = "AutoTrading/GetAutoTradingPTAndAfterVRAdjustment";
        private const string Endpoint_RT3AndTAComparition = "AutoTrading/GetAutoTradingRT3AndTAComparition";

        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IAutoTradingService autoTradingService;
        private ILetstalkNotify letstalkNotify;
        private string notifiedMessage = string.Empty;

        public AutoTradingViaLetstalkJobService(ISentryClient sentryClient, 
            IAppSettings appSettings,
            IAutoTradingService autoTradingService)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.autoTradingService = autoTradingService;
        }

        public Task<bool> Execute()
        {
            try
            {
                var directorySaveImage = AppDomain.CurrentDomain.BaseDirectory + @"Attachment/";
                if (!Directory.Exists(directorySaveImage))
                {
                    Directory.CreateDirectory(directorySaveImage);
                }

                this.autoTradingService.DirectorySaveFile = directorySaveImage;
                this.autoTradingService.UrlAPI = this.appSettings.TGRSAPIBaseAddress;

                this.autoTradingService.MoveFileToArcFolder();

                var tasks = new List<Task>();
                tasks.Add(Task.Run(() => this.autoTradingService.GenerateAutoTradingToFile(Endpoint_TAndAfterVRAdjustment, FileName_AutoTrading_PTAndAfterVRAdjustment, FileType.Image)));
                tasks.Add(Task.Run(() => this.autoTradingService.GenerateAutoTradingToFile(Endpoint_TAMonthly, FileName_AutoTrading_TAMonthly, FileType.Image)));
                tasks.Add(Task.Run(() => this.autoTradingService.GenerateAutoTradingToFile(Endpoint_RT3AndTAComparition, FileName_AutoTrading_RT3AndTAComparition, FileType.Image)));

                Task.WhenAll(tasks.ToArray()).Wait();

                if (this.autoTradingService.IsSuccessAPIResponse)
                {
                    if (this.SendFileMessageViaLetstalk(
                        this.appSettings.LetstalkServiceApi,
                        this.appSettings.AutoTradingLetstalkTarget,
                        this.appSettings.AutoTradingLetstalkGroups,
                        directorySaveImage))
                    {
                        this.notifiedMessage = "Auto Trading via letstalk job run COMPLETED.";

                        LogService.Info($"SendFileMessageViaLestalk sucessfully - { DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") }");
                    }
                    else
                        this.notifiedMessage = "Auto Trading via letstalk job send files FAILED.";
                }
                else
                    this.notifiedMessage = "Auto Trading via letstalk job generate files FAILED.";
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                this.notifiedMessage = "Auto Trading via letstalk job Execute FAILED.";

                return Task.FromResult(false);
            }

            NotifyMessage();

            return Task.FromResult(true);
        }

        public bool SendFileMessageViaLetstalk(string uriString, string target, string group, string pathAttachment)
        {
            try
            {
                var isSendSuccess = true;
                var sundayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);

                var subject = "Automation Trading Report";
                var content = "Good day Sir, TA report update till " + sundayOfLastWeek.ToString("dd/MM/yyyy");

                if (!string.IsNullOrEmpty(group))
                {
                    var arrGroupName = group.Split(';');

                    foreach (var groupName in arrGroupName)
                    {
                        letstalkNotify = new LetstalkNotify(uriString, target, groupName, pathAttachment);

                        if (!letstalkNotify.CheckExistFileAttach())
                        {
                            LogService.Warn($"SendFileMessageViaLestalk to {groupName} does not exit file attach at path: {pathAttachment}");
                        }

                        isSendSuccess = letstalkNotify.SendSingleFileAttachMessage(subject, content);

                        if (!isSendSuccess)
                        {
                            LogService.Warn($"SendFileMessageViaLestalk to {groupName} failed");
                        }
                    }
                }

                return isSendSuccess;
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"TGRS - SendFileMessageViaLestalk failed target: {target} , group: {group} , path { pathAttachment }, message: {logMessage}");

                return false;
            }
        }

        private void NotifyMessage()
        {
            var telegramService = new NotificationService();
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001400862084")
               .Message(new MessageModel("TGRS - Auto Trading Via Letstalk", DateTime.Now, appSettings.EnvironmentString, notifiedMessage))
               .SendWithoutDelay(isInfo: true);
        }
    }
}
