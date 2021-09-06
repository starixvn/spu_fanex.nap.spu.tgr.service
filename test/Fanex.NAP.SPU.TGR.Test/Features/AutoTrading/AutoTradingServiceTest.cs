namespace Fanex.NAP.SPU.TGR.Tests.Features.AutoTrading
{
    using Fanex.NAP.SPU.TGR.Features.AutoTrading;
    using Fanex.NAP.SPU.TGR.Shares;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using Xunit;

    public class AutoTradingServiceTest
    {
        private readonly IAutoTradingService service;

        public AutoTradingServiceTest()
        {
            service = new AutoTradingService();
        }

        [Theory]
        [MemberData(nameof(AutoTradingExpectedData.ExpectedMoveFileToArcFolder), MemberType = typeof(AutoTradingExpectedData))]
        public void MoveFileToArcFolder_WhenSuccess(string directorySaveFile, string urlAPI, string filePassword)
        {
            service.DirectorySaveFile = directorySaveFile;
            service.UrlAPI = urlAPI;
            service.FilePassword = filePassword;

            service.MoveFileToArcFolder();
        }

        [Theory]
        [MemberData(nameof(AutoTradingExpectedData.ExpectedGenerateAutoTradingToFileImage), MemberType = typeof(AutoTradingExpectedData))]
        public void GenerateAutoTradingToFileImage_WhenSuccess(string directorySaveFile, string urlAPI, string endpoint, string saveFileName)
        {
            service.DirectorySaveFile = directorySaveFile;
            service.UrlAPI = urlAPI;            

            service.GenerateAutoTradingToFile(endpoint, saveFileName, FileType.Image);
        }

        [Theory]
        [MemberData(nameof(AutoTradingExpectedData.ExpectedGenerateAutoTradingToFileExcel), MemberType = typeof(AutoTradingExpectedData))]
        public void GenerateAutoTradingToFileExcel_WhenSuccess(string directorySaveFile, string urlAPI, string filePassword, string endpoint, string saveFileName)
        {
            service.DirectorySaveFile = directorySaveFile;
            service.UrlAPI = urlAPI;
            service.FilePassword = filePassword;

            service.GenerateAutoTradingToFile(endpoint, saveFileName, FileType.Excel);
        }

    }
}
