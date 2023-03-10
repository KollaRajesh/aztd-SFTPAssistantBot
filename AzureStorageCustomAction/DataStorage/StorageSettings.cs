using Microsoft.Extensions.Configuration;


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

        public string ConnectionString { get { return Config.GetConnectionString("DefaultConnection"); } }
        public string ContainerName  { get { return Config.GetValue<string>("ContainerName"); } }
        //public static string ConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1;";

    }
}
