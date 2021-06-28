using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;

namespace Fanex.NAP.SPU.TGR.JobService
{
    public interface IAppSettings
    {
        int Environment { get; }

        string EnvironmentString { get; }

        int PunterChunkSize { get; }

        string SPName { get; }
    }
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        private readonly string environment;

        private readonly int punterChunkSize;

        private readonly string spName;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            environment = GetValue<string>(nameof(Environment));

            punterChunkSize = GetValue<int>(nameof(PunterChunkSize));

            spName = GetValue<string>(nameof(SPName));
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
