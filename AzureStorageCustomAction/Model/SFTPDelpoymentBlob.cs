using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageCustomAction.Model
{
    public class SFTPDelpoymentBlob
    {
      public  SFTPDelpoymentBlob(string partnerName, SourceServerInfo sourceServerInfo, DestinationServerInfo destinationServerInfo)
        {
            PartnerName = partnerName;
            SourceServerInfo = sourceServerInfo;
            DestinationServerInfo = destinationServerInfo;
        }

        public  string PartnerName { get; set; }
        public SourceServerInfo SourceServerInfo { get; set; }
        public DestinationServerInfo DestinationServerInfo { get; set; }
    }
}
