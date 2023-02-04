using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageCustomAction.Model
{
    public class DestinationServerInfo :ServerBase
    {
        public DestinationServerInfo(string hostName, int portNumber,   string userName, string password,
                               string sharedFolder, bool isEncryptionRequired = false, string publicKeyName="", string publicKeyURL = "")
            : base(hostName, portNumber, userName, password, sharedFolder, isEncryptionRequired)
        {
            if (isEncryptionRequired)
            {
                PublicKeyName = publicKeyName;
                PublicKeyURL = publicKeyURL;
            }
        }
        public string PublicKeyName { get; set; }
        public string PublicKeyURL { get; set; }

    }
}
