using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AzureStorageCustomAction.Model
{
    public class ServerBase
    {

        public ServerBase() { }

        public ServerBase(string hostName, int portNumber, string userName, string password, 
                          string sharedFolder, bool isEncryptionRequired)
        {
            HostName = hostName;
            PortNumber = portNumber;
            UserName = userName;
            Password = password;
            SharedFolder = sharedFolder;
            IsEncryptionRequired = isEncryptionRequired;
        }

        public string HostName { get; set; }
        public int PortNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SharedFolder { get; set; }
        public bool IsEncryptionRequired { get; set; }
    }
}
