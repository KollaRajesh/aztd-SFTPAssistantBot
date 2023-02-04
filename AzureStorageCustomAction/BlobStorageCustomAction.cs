using AdaptiveExpressions.Properties;
using AzureStorageCustomAction.DataStorage;
using AzureStorageCustomAction.Extensions;
using AzureStorageCustomAction.Model;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageCustomAction
{
    //Custom Actions for Azure Storage operations
    public class BlobStorageCustomAction : Dialog
    {
        private readonly AzureStorage Store;
        public BlobStorageCustomAction([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) : base()
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);

            var settings = new StorageSettings();
            Store = new AzureStorage(settings.ConnectionString, settings.ContainerName);
        }

        [JsonProperty("$Kind")]
        public const string Kind = "BlobStorageCustomAction";

        //[JsonProperty("SourceServerDetails")]
        //public ObjectExpression<SourceServerInfo> SourceServerDetails { get; set; }

        //[JsonProperty("DestinationServerDetails")]
        //public ObjectExpression<SourceServerInfo> DestinationServerDetails { get; set; }

        [JsonProperty("PartnerName")]
        public StringExpression PartnerName { get; set; }

        [JsonProperty("SourceHostName")]
        public StringExpression SourceHostName { get; set; }

        [JsonProperty("SourcePortNumber")]
        public NumberExpression SourcePortNumber { get; set; }

        [JsonProperty("SourceUserName")]
        public StringExpression SourceUserName { get; set; }

        [JsonProperty("SourcePassword")]
        public StringExpression SourcePassword { get; set; }

        [JsonProperty("SourceSharedFolder")]
        public StringExpression SourceSharedFolder { get; set; }

        [JsonProperty("IsEncryptionRequiredForSource")]
        public BoolExpression IsEncryptionRequiredForSource { get; set; }


        [JsonProperty("PrivateKeyName")]
        public StringExpression PrivateKeyName { get; set; }

        [JsonProperty("PrivateKeyURL")]
        public StringExpression PrivateKeyURL { get; set; }


        [JsonProperty("DestinationHostName")]
        public StringExpression DestinationHostName { get; set; }

        [JsonProperty("DestinationPortNumber")]
        public NumberExpression DestinationPortNumber { get; set; }

        [JsonProperty("DestinationUserName")]
        public StringExpression DestinationUserName { get; set; }

        [JsonProperty("DestinationPassword")]
        public StringExpression DestinationPassword { get; set; }

        [JsonProperty("DestinationSharedFolder")]
        public StringExpression DestinationSharedFolder { get; set; }

        [JsonProperty("IsEncryptionRequiredForDestination")]
        public BoolExpression IsEncryptionRequiredForDestination { get; set; }

        [JsonProperty("PublicKeyName")]
        public StringExpression PublicKeyName { get; set; }

        [JsonProperty("PublicKeyURL")]
        public StringExpression PublicKeyURL { get; set; }

        [JsonProperty("resultProperty")]
        public StringExpression ResultProperty { get; set; }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var partnerName = PartnerName?.GetValue(dc.State);

            var blobJSON = new SFTPDelpoymentBlob(partnerName, GetSourceServerInfo(dc), GetDestinationServerInfo(dc)).Serialize();

            var blobName = $"{partnerName.GetUniqueDeploymentFileName()}.json";
            var blobURI= await Store.UploadContentAsync(blobJSON, blobName);

            var result = $"{blobName} has been uploaded successfully and file location: {blobURI}";

            if (ResultProperty != null)
            {
                dc.State.SetValue(this.ResultProperty.GetValue(dc.State), result);
            }

            return await dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
        }


        private  SourceServerInfo GetSourceServerInfo(DialogContext dc)
        {
            var hostName = SourceHostName?.GetValue(dc.State);
            var portNumber = Convert.ToInt32(SourcePortNumber?.GetValue(dc.State));
            var userName = SourceUserName?.GetValue(dc.State);
            var password = SourcePassword?.GetValue(dc.State);
            var sharedFolder = SourceSharedFolder?.GetValue(dc.State);
            var isFilesEncryptionRequired = Convert.ToBoolean(IsEncryptionRequiredForSource?.GetValue(dc.State));
            var keyName = PrivateKeyName?.GetValue(dc.State);
            var keyURL = PrivateKeyURL?.GetValue(dc.State);

            string blobURI = string.Empty;
            if (isFilesEncryptionRequired)
                blobURI = Store.UploadAsync(keyURL, keyName).Result;

            return  new SourceServerInfo(hostName, portNumber, userName, password, sharedFolder,
                                                  isFilesEncryptionRequired, keyName, blobURI);
        }

        private DestinationServerInfo GetDestinationServerInfo(DialogContext dc)
        {  
           var hostName = DestinationHostName?.GetValue(dc.State);
           var portNumber = Convert.ToInt32(DestinationPortNumber?.GetValue(dc.State));
           var userName = DestinationUserName?.GetValue(dc.State);
           var password = DestinationPassword?.GetValue(dc.State);
           var sharedFolder = DestinationSharedFolder?.GetValue(dc.State);
           var isFilesEncryptionRequired = Convert.ToBoolean(IsEncryptionRequiredForDestination?.GetValue(dc.State));
           var keyName = PublicKeyName?.GetValue(dc.State);
           var keyURL = PublicKeyURL?.GetValue(dc.State);

            string blobURI = string.Empty;
            if (isFilesEncryptionRequired)
                blobURI =  Store.UploadAsync(keyURL, keyName).Result;

            return new DestinationServerInfo(hostName, portNumber, userName, password, sharedFolder,
                                                  isFilesEncryptionRequired, keyName, blobURI);

        }
    }
}
