using Azure.Core;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager;
using Azure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.ContentModel;

namespace VMRunCommandCustomAction.AzureManagementAPI.AzureVMRunCommands
{
    public class AddHostRunCommand : IRunCommand
    {
        private readonly AzureAccountSettings settings;

        private readonly string name;
        public AddHostRunCommand()
        {
            name = "AddHost";
            settings = new AzureAccountSettings();
        }
        public string Name { get { return name; } }
        public async Task<VirtualMachineRunCommandResource> CreateOrUpdateVMRunCommandAync(string ipAddress, string fqdn)
        {
            ResourceIdentifier vmResourceId = VirtualMachineResource.CreateResourceIdentifier(
                                                                        settings.SubscriptionId
                                                                        , settings.ResourceGroupName
                                                                        , settings.VMName);
            VirtualMachineResource virtualMachine = settings.ARMClient.GetVirtualMachineResource(vmResourceId);

            // get the collection of this VirtualMachineRunCommandResource
            VirtualMachineRunCommandCollection collection = virtualMachine.GetVirtualMachineRunCommands();
            ArmOperation< VirtualMachineRunCommandResource> lro= await collection.CreateOrUpdateAsync(WaitUntil.Completed, Name, BuildCommandData(ipAddress, fqdn));
            return lro.Value;
        }
        [Obsolete("This function is throwing error -The entity was not found in this Azure location.")]
        public async Task<VirtualMachineRunCommandResult> InvokeCommandASync(string ipAddress, string fqdn)
        {
            ResourceIdentifier vmResourceId = VirtualMachineResource.CreateResourceIdentifier(
                                                                        settings.SubscriptionId
                                                                        , settings.ResourceGroupName
                                                                        , settings.VMName);
            VirtualMachineResource virtualMachine = settings.ARMClient.GetVirtualMachineResource(vmResourceId);

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
        /// Add Host entry in linux vm using run shell command 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="fqdn"></param>
        /// <returns></returns>
        public async Task<bool> InvokeShellCommandASync(string ipAddress, string fqdn)
        {
            var result = false;
            ResourceIdentifier virtualMachineResourceId = VirtualMachineResource.
                                                CreateResourceIdentifier(settings.SubscriptionId,
                                                                         settings.ResourceGroupName,
                                                                         settings.VMName);
            VirtualMachineResource virtualMachine = settings.ARMClient.GetVirtualMachineResource(virtualMachineResourceId);
            // invoke the operation
            var input = new RunCommandInput("RunShellScript");
            input.Script.Add("echo $ip $fqdn | sudo tee -a /etc/hosts");
            input.Parameters.Add(new RunCommandInputParameter("ip", ipAddress));
            input.Parameters.Add(new RunCommandInputParameter("fqdn", fqdn)); 

            ArmOperation<VirtualMachineRunCommandResult> lro = await virtualMachine.RunCommandAsync(WaitUntil.Completed, input);
            var cmdResult = lro.Value;

            if (cmdResult != null && cmdResult.Value != null && cmdResult.Value.Count == 1)
            {
                var instanceView = cmdResult.Value[0];
                if (instanceView.Code.Equals("ProvisioningState/succeeded") &&
                    instanceView.Message.Trim().EndsWith("[stderr]"))
                        result =true;
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
            return new VirtualMachineRunCommandData(new AzureLocation(settings.Location))
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
