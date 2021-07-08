namespace Fanex.NAP.SPU.DCS.JobService
{
    using Fanex.NAP.SPU.TGRS.JobService;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Sentry;
    using System;
    using System.IO;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            SentrySdk.CaptureMessage(string.Format("{0} has been started at {1}", typeof(Program).FullName, DateTime.Now));            

            host.Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .Build();

            return WebHost.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     config.Sources.Clear();

                     config.AddConfiguration(configuration);
                 })
                  .UseKestrel()
                  .UseUrls($"http://0.0.0.0:{configuration["HostingPort"]}")
                  .UseSentry()
                  .UseIISIntegration()
                  .UseStartup<Startup>();
        }
             
    }
}
