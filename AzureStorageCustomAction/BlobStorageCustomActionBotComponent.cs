using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureStorageCustomAction
{
    public class BlobStorageCustomActionBotComponent : BotComponent
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DeclarativeType>(sp =>
                new DeclarativeType<BlobStorageCustomAction>(BlobStorageCustomAction.Kind));
        }
    }
}
