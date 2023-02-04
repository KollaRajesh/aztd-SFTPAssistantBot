using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace AzureStorageCustomAction.DataStorage
{
    public class StorageSettings
    {
        public StorageSettings() { }
        public StorageSettings(IConfiguration configuration)
        {
            Config = configuration;
        }
        private readonly IConfiguration Config;

        //public string ConnectionString { get { return Config.GetConnectionString("DefaultConnection"); } }
        //public string ContainerName  { get { return Config.GetValue<string>("ContainerName"); } }
        //public static string ConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1;";

        public string ConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1;";

        public string ContainerName = "sftpdeploymentsfiles";
    }
}
