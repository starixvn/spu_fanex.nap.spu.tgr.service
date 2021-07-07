namespace Fanex.NAP.SPU.TGR.Tests.Features.AutoTrading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class AutoTradingExpectedData
    {        
        public static IEnumerable<object[]> ExpectedMoveFileToArcFolder
        {
            get
            {
                return new List<object[]> {
                    new object[] { AppDomain.CurrentDomain.BaseDirectory + @"Attachment\", "http://ha.nexdev.net:7724/TGRS/", "Star168" }                    
                };
            }
        }
        
        public static IEnumerable<object[]> ExpectedGenerateAutoTradingToFileImage
        {
            get
            {
                return new List<object[]> {
                    new object[] { AppDomain.CurrentDomain.BaseDirectory + @"Attachment\", "http://ha.nexdev.net:7724/TGRS/", "AutoTrading/GetAutoTradingTAMonthly", "AT2TAMonthly.png" },
                    new object[] { AppDomain.CurrentDomain.BaseDirectory + @"Attachment\", "http://ha.nexdev.net:7724/TGRS/", "AutoTrading/GetAutoTradingPTAndAfterVRAdjustment", "AT1PTAndAfterVRAdjustment.png" },
                    new object[] { AppDomain.CurrentDomain.BaseDirectory + @"Attachment\", "http://ha.nexdev.net:7724/TGRS/", "AutoTrading/GetAutoTradingRT3AndTAComparition", "AT3RT3AndTAComparition.png" },
                };
            }
        }

        public static IEnumerable<object[]> ExpectedGenerateAutoTradingToFileExcel
        {
            get
            {
                return new List<object[]> {
                    new object[] { AppDomain.CurrentDomain.BaseDirectory + @"Attachment\", "http://ha.nexdev.net:7724/TGRS/", "Star168", "AutoTrading/GetAutoTradingEventLeaguesExcel", "test.xlsx" }                  
                };
            }
        }
    }
}
