using Azure.Identity;
using Azure.ResourceManager;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace VMRunCommandCustomAction.AzureManagementAPI
{
    public  class AzureAccountSettings
    {
        //TODO:Integrate with Bot
        //TODO:Read Data From Environment Variables \KeyVault
        //TODO:Need to check why Azure is not showing commands in Run Commands windows of VM
        //TODO:Need to check why DefaultAzurecredentials is not able to get token when we deploy app in Azure app service under managed identity.
        readonly string subscriptionId;
        readonly string resourceGroupName;
        readonly string vmName;
        readonly string location;
        readonly string tenantId;
        readonly ArmClient armClient;
        readonly IConfiguration Config;


        public AzureAccountSettings():this(null)
        {
            
        }
        public AzureAccountSettings(IConfiguration configuration)
        {
            Config = configuration;
            subscriptionId = @"<<Before run\testPlease, Please update Subscription Id here before run \test>";
            resourceGroupName = @"<<Before run\testPlease, Please update Resource group name where VM resides  >>";
            vmName = @"<<Before run\testPlease, Please update linux VM name here>>";
            location = @"<<Before run\testPlease, Please update Display location here>>";
            tenantId = @"<<Before run\testPlease, Please update tenant location here>>";

            

            // authenticate your client
            armClient = new ArmClient(new DefaultAzureCredential());

        }

        public string SubscriptionId { get { return subscriptionId; } }
        public string ResourceGroupName { get { return resourceGroupName; } }
        public string VMName { get { return vmName; } }

        public string Location { get { return location; } }         
        public string TenantId { get { return tenantId;} }

        public ArmClient ARMClient { get { return armClient; } }
              
    }
}
