namespace Fanex.NAP.SPU.TGR.JobService
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.ComponentModel;

    public interface IAppSettings
    {
        int Environment { get; }

        string EnvironmentString { get; }

        int PunterChunkSize { get; }

        string SPName { get; }

        string AutoTradingLetstalkTarget { get; }

        string AutoTradingLetstalkGroups { get; }        

        string AutoTradingFilePassword { get; }

        string TGRSAPIBaseAddress { get; }

        string SentryDsn { get;  }
    }
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        private readonly string environment;

        private readonly int punterChunkSize;

        private readonly string spName;

        private readonly string tgrsAPIBaseAddress;

        private readonly string autoTradingLetstalkTarget;

        private readonly string autoTradingLetstalkGroups;        

        private readonly string autoTradingFilePassword;

        private readonly string sentryDsn;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            environment = GetValue<string>(nameof(Environment));

            punterChunkSize = GetValue<int>(nameof(PunterChunkSize));

            spName = GetValue<string>(nameof(SPName));

            tgrsAPIBaseAddress = GetValue<string>(nameof(TGRSAPIBaseAddress));

            autoTradingLetstalkTarget = GetValue<string>(nameof(AutoTradingLetstalkTarget));

            autoTradingLetstalkGroups = GetValue<string>(nameof(AutoTradingLetstalkGroups));

            autoTradingFilePassword = GetValue<string>(nameof(AutoTradingFilePassword));

            sentryDsn = GetValue<string>("Sentry:Dsn");
        }

        public int Environment
        {
            get
            {
                switch (environment)
                {
                    case "LOCAL":
                        return 1;
                    case "UAT":
                        return 2;
                    default:
                        return 3;
                }
            }
        }

        public string EnvironmentString
        {
            get
            {
                return environment;
            }
        }

        public string SPName => spName;

        public int PunterChunkSize => punterChunkSize;

        public string TGRSAPIBaseAddress => tgrsAPIBaseAddress;

        public string AutoTradingLetstalkTarget => autoTradingLetstalkTarget;

        public string AutoTradingLetstalkGroups => autoTradingLetstalkGroups;

        public string AutoTradingFilePassword => autoTradingFilePassword;

        public string SentryDsn => sentryDsn;

        public T GetValue<T>(string key)
        {
            try
            {
                var value = configuration[key];

                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }

        public T GetSection<T>(string key)
        {
            var schedulerSection = configuration.GetSection(key);

            return schedulerSection.Get<T>();
        }

    }
}
