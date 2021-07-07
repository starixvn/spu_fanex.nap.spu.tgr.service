namespace Fanex.NAP.SPU.TGR.JobService.Services
{
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.TGR.Features.AutoTrading;
    using Fanex.NAP.SPU.TGR.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAutoTradingViaEmailJobService
    {
        Task<bool> Execute();
    }

    public class AutoTradingViaEmailJobService : IAutoTradingViaEmailJobService
    {
        private const string Endpoint_EventLeaguesExcel = "AutoTrading/GetAutoTradingEventLeaguesExcel";
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IAutoTradingService autoTradingService;
        private string notifiedMessage = string.Empty;

        public AutoTradingViaEmailJobService(ISentryClient sentryClient, IAppSettings appSettings, IAutoTradingService autoTradingService)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.autoTradingService = autoTradingService;
        }

        public Task<bool> Execute()
        {
            try
            {
                var fromDate = "07-Nov-2019";
                var toDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToString("dd-MMM-yyyy");
                var saveFileName = "Model TA Performance Event Details - " + fromDate + " to " + toDate + ".xlsx";
                var directorySaveFile = AppDomain.CurrentDomain.BaseDirectory + @"Attachment\";

                this.autoTradingService.DirectorySaveFile = directorySaveFile;
                this.autoTradingService.UrlAPI = this.appSettings.TGRSAPIBaseAddress;

                this.autoTradingService.MoveFileToArcFolder();
                this.autoTradingService.GenerateAutoTradingToFile(Endpoint_EventLeaguesExcel, saveFileName, FileType.Excel);

                this.GenerateMail(saveFileName);

                this.NotifyMessage();
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);                
            }

            return Task.FromResult(true);
        }

        public Mail GenerateMail(string saveFileName)
        {
            if (this.autoTradingService.IsSuccessAPIResponse)
            {
                var toDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToString("dd/MMM/yyyy");
                var subject = "Automail Report - Live Performance Report " + toDate;
                var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"MailTemplate\MailInformTemplate.cshtml";                

                var body = Utilities.ReadTextFile(mailTemplatePath);

                body = string.Format(body, saveFileName);

                return new Mail
                {
                    Subject = subject,
                    Title = subject,
                    Body = body,
                    Attachments = GetMailAttachments(this.autoTradingService.DirectorySaveFile)
                };
            }

            return new Mail();
        }

        private List<MailAttachment> GetMailAttachments(string pathAttachment)
        {
            var attatchments = new List<MailAttachment>();

            var filePaths = Directory.GetFiles(pathAttachment).ToList();

            filePaths.ForEach(fileName =>
            {
                attatchments.Add(new MailAttachment(fileName, FileExtension.MS_EXCEL));
            });

            return attatchments;
        }

        private void NotifyMessage()
        {
            //this.notifiedMessage = $"Auto special licensee job run COMPLETED." +
            //     $"\n\r - OtherSports - Customers have passed Performance Check: {this.specialLicenseeService.PunterBasketballUpdated}" +
            //     $"\n\r - Special SportBook - Customers have passed Performance Check: {this.specialLicenseeService.PunterSportsbookUpdated}";

            var telegramService = new NotificationService();
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001400862084")
               .Message(new MessageModel("TGRS - Auto Trading Via Email", DateTime.Now, appSettings.EnvironmentString, notifiedMessage))
               .SendWithoutDelay(isInfo: true);
        }
    }
}
