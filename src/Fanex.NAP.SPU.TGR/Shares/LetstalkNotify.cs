namespace Fanex.NAP.SPU.TGR.Shares
{
    using Fanex.LetstalkService.Client;
    using Fanex.LetstalkService.Core.Models;
    using Fanex.LetstalkService.Core.Services;    
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;

    public interface ILetstalkNotify
    {
        void Info(string message, string target, string group);

        void Error(string message, string target, string group);

        bool CheckExistFileAttach();

        bool SendSingleFileAttachMessage(string subject, string content);
    }

    public class LetstalkNotify : ILetstalkNotify
    {
        private LetstalkClientService letstalkService;
        private LetstalkModel letstalkModel;

        public LetstalkNotify()
        {
            letstalkService = new LetstalkClientServiceImp();
        }

        public LetstalkNotify(string targetName, string group, string pathAttachment)
        {
            letstalkService = new LetstalkClientServiceImp();

            letstalkModel = new LetstalkModel();
            letstalkModel.LetstalkAttachments = GetAttachmentList(pathAttachment);
            letstalkModel.TargetName = targetName;
            letstalkModel.Group = group;
        }

        public void Info(string message, string target, string group)
        {
            this.SendMessage("Info", message, target, group);
        }

        public void Error(string message, string target, string group)
        {
            this.SendMessage("Error", message, target, group);
        }

        private List<LetstalkAttachment> GetAttachmentList(string pathAttachment)
        {
            var attatchment = new List<LetstalkAttachment>();

            var filePaths = Directory.GetFiles(pathAttachment).ToList();

            filePaths.ForEach(fileName =>
            {
                attatchment.Add(new LetstalkAttachment
                {
                    FileName = fileName,
                    Bytes = FileBinaryService.ConvertFileToBinary(fileName)
                });
            });

            return attatchment;
        }

        private void SendMessage(string subject, string content, string target, string group)
        {
            LetstalkModel messsage = new LetstalkModel()
            {
                TargetName = target,
                Subject = subject,
                Content = content,
                Group = group,
                LetstalkAttachments = new List<LetstalkAttachment>()
            };

            letstalkService.SendMessage(messsage);
        }

        public bool CheckExistFileAttach()
        {
            if (letstalkModel != null && letstalkModel.LetstalkAttachments.Any())
            {
                return true;
            }

            return false;
        }

        public bool SendSingleFileAttachMessage(string subject, string content)
        {
            if (letstalkModel != null && letstalkModel.LetstalkAttachments.Any())
            {
                var countSuccess = 0;
                var model = new LetstalkModel
                {
                    Subject = subject,
                    Content = content,
                    LetstalkAttachments = new List<LetstalkAttachment> { letstalkModel.LetstalkAttachments[0] },
                    TargetName = letstalkModel.TargetName,
                    Group = letstalkModel.Group
                };

                var isSuccess = letstalkService.SendMessage(model);
                countSuccess = isSuccess ? countSuccess + 1 : countSuccess;

                var countAttachment = letstalkModel.LetstalkAttachments.Count;
                for (int i = 1; i < letstalkModel.LetstalkAttachments.Count; i++)
                {
                    model.LetstalkAttachments = new List<LetstalkAttachment> { letstalkModel.LetstalkAttachments[i] };
                    model.Subject = string.Empty;
                    model.Content = string.Empty;

                    isSuccess = letstalkService.SendMessage(model);
                    countSuccess = isSuccess ? countSuccess + 1 : countSuccess;
                };

                return countSuccess == countAttachment;
            }

            return false;
        }

        private List<LetstalkAttachment> GetAttachmentList(string pathAttachment, string fileName)
        {
            var attatchment = new List<LetstalkAttachment>();

            var filePaths = Directory.GetFiles(pathAttachment).ToList();

            filePaths.ForEach(file =>
            {
                var xfileName = Path.GetFileName(file);

                if (xfileName == fileName)
                {
                    attatchment.Add(new LetstalkAttachment
                    {
                        FileName = file,
                        Bytes = FileBinaryService.ConvertFileToBinary(file)
                    });
                }
            });

            return attatchment;
        }

    }
}
