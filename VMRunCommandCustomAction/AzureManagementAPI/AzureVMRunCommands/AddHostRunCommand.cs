using Azure.Core;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager;
using Azure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VMRunCommandCustomAction.AzureManagementAPI.AzureVMRunCommands
{
    public class AddHostRunCommand : IRunCommand
    {
        private readonly AzureAccountSettings AzaccountSettings;

        private readonly string name;
        public AddHostRunCommand()
        {
            name = "AddHost";
            AzaccountSettings = new AzureAccountSettings();
        }
        public string Name { get { return name; } }
        public async Task<VirtualMachineRunCommandResource> CreateOrUpdateVMRunCommandAync(string ipAddress, string fqdn)
        {
            ResourceIdentifier vmResourceId = VirtualMachineResource.CreateResourceIdentifier(
                                                                        AzaccountSettings.SubscriptionId
                                                                        , AzaccountSettings.ResourceGroupName
                                                                        , AzaccountSettings.VMName);
            VirtualMachineResource virtualMachine = AzaccountSettings.ARMClient.GetVirtualMachineResource(vmResourceId);

            // get the collection of this VirtualMachineRunCommandResource
            VirtualMachineRunCommandCollection collection = virtualMachine.GetVirtualMachineRunCommands();
            ArmOperation< VirtualMachineRunCommandResource> lro= await collection.CreateOrUpdateAsync(WaitUntil.Completed, Name, BuildCommandData(ipAddress, fqdn));
            return lro.Value;
        }
        public async Task<VirtualMachineRunCommandResult> InvokeCommandASync(string ipAddress, string fqdn)
        {
            ResourceIdentifier vmResourceId = VirtualMachineResource.CreateResourceIdentifier(
                                                                        AzaccountSettings.SubscriptionId
                                                                        , AzaccountSettings.ResourceGroupName
                                                                        , AzaccountSettings.VMName);
            VirtualMachineResource virtualMachine = AzaccountSettings.ARMClient.GetVirtualMachineResource(vmResourceId);

            VirtualMachineRunCommandCollection collection = virtualMachine.GetVirtualMachineRunCommands();
            var CmdExists= await collection.ExistsAsync(name);
            VirtualMachineRunCommandResult result=null;
            if (CmdExists.Value)
            {
                // invoke the operation
                RunCommandInput input = new RunCommandInput(name)
                {
                    Parameters ={new RunCommandInputParameter("ip",ipAddress),
                                 new RunCommandInputParameter("fqdn",fqdn)}

                };
                ArmOperation<VirtualMachineRunCommandResult> lro = await virtualMachine.RunCommandAsync(WaitUntil.Completed, input);
                result= lro.Value;
            }
            return result;

         
        }
        /// <summary>
        /// Build VM Run Command Data 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="fqdn"></param>
        /// <returns></returns>
        public  VirtualMachineRunCommandData BuildCommandData(string ipAddress, string fqdn)
        {
            return new VirtualMachineRunCommandData(new AzureLocation(AzaccountSettings.Location))
            {
                Source = new VirtualMachineRunCommandScriptSource()
                {
                    Script = "echo $ip $fqdn | sudo tee -a /etc/hosts",
                },
                Parameters ={new RunCommandInputParameter("ip",ipAddress),
                                     new RunCommandInputParameter("fqdn",fqdn)
                              },
                AsyncExecution = false,
                //RunAsUser = "user1",
                //RunAsPassword = "<runAsPassword>",
                TimeoutInSeconds = 3600,
            };
        }
    }
}
