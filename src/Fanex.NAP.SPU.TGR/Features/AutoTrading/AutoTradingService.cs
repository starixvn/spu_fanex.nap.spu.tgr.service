namespace Fanex.NAP.SPU.TGR.Features.AutoTrading
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.TGR.Shares;
    using OfficeOpenXml;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public interface IAutoTradingService
    {
        bool IsSuccessAPIResponse { get; set; }

        string DirectorySaveFile { get; set; }

        string UrlAPI { get; set; }

        string FilePassword { get; set; }

        void MoveFileToArcFolder();

        void GenerateAutoTradingToFile(string endpoint, string saveFileName, FileType fileType);

    }

    public class AutoTradingService : IAutoTradingService
    {       
        public bool IsSuccessAPIResponse { get; set; }
        public string DirectorySaveFile { get; set; }
        public string UrlAPI { get; set; }
        public string FilePassword { get; set; }      

        public void MoveFileToArcFolder()
        {
            var sourceFolder = this.DirectorySaveFile;

            if (!Directory.Exists(sourceFolder))
            {
                Directory.CreateDirectory(sourceFolder);
            }

            var desArcFolder = AppDomain.CurrentDomain.BaseDirectory + @"Arc_Folder\";

            if (!Directory.Exists(desArcFolder))
            {
                Directory.CreateDirectory(desArcFolder);
            }

            var fileAtsourceFolders = Directory.GetFiles(sourceFolder).ToList();
            var fileAtDesArcFolders = Directory.GetFiles(desArcFolder).ToList();

            try
            {
                fileAtDesArcFolders.ForEach(filePath =>
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                });

                fileAtsourceFolders.ForEach(filePath =>
                {
                    if (File.Exists(filePath))
                    {
                        var fileName = Path.GetFileName(filePath);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            File.Move(filePath, desArcFolder + fileName, true);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                LogService.Warn($"TGRS - Auto Trading: MoveFileToArcFolder failed - { ex.ToString() }");
            }
        }

        public void GenerateAutoTradingToFile(string endpoint, string saveFileName, FileType fileType)
        {
            this.IsSuccessAPIResponse = false;
            var savePath = Path.Combine(this.DirectorySaveFile + saveFileName);            

            try
            {
                using (var client = new HttpClient())
                {
                    var requestUri = this.UrlAPI + endpoint;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.GetAsync(requestUri).Result;

                    using (var content = response.Content)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var bytes = content.ReadAsByteArrayAsync().Result;

                            if (fileType == FileType.Excel)
                                SaveExcel(bytes, savePath);
                            else
                                SaveImage(bytes, savePath);

                            IsSuccessAPIResponse = true;
                        }                       
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"TGRS - Auto Trading: GenereateAutoTradingToFile failed - endpoint: { endpoint }  {logMessage}");                
            }
        }

        private void SaveExcel(byte[] bytes, string savePath)
        {
            if (bytes != null)
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    var excelPackage = new ExcelPackage(memoryStream);

                    excelPackage.SaveAs(new FileInfo(savePath), this.FilePassword);
                }
            }
        }

        private void SaveImage(byte[] bytes, string savePath)
        {
            if (bytes != null)
            {
                var memoryStream = new MemoryStream(bytes);

                var image = Image.FromStream(memoryStream, true, true);

                image.Save(savePath);
            }           
        }
    }
}
