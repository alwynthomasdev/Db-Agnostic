using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DbAgnostic.Tests
{
    static class ConfigurationHelper
    {
        static IConfigurationRoot _Config;
        static ConfigurationHelper()
        {
            _Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }

        public static string GetConnectionString()
        {
            return _Config["ConnectionString"];
        }
    }
}
