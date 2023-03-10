using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Resources;
using VMRunCommandCustomAction.AzureManagementAPI.AzureVMRunCommands;

namespace VMRunCommandCustomAction
{
    //Invoke VM Run Command Custom Action 
    public class VMRunCommandCustomAction : Dialog
    {
        public VMRunCommandCustomAction([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) : base()
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$Kind")]
        public const string Kind = "VMRunCommandCustomAction";
        //"VMName": {
        //  "$ref": "schema:#/definitions/stringExpression",
        //  "title": "Virtual Machine Name",
        //  "description": "ip address to add in hosts"
        //},
        //"ResourceGroupName": {
        //  "$ref": "schema:#/definitions/stringExpression",
        //  "title": "Resouce Group Name",
        //  "description": "Resource group name for virtual machine"
        //},

        //[JsonProperty("VMName")]
        //public StringExpression VMName { get; set; }

        //[JsonProperty("ResourceGroupName")]
        //public StringExpression ResourceGroupName { get; set; }

        [JsonProperty("IpAddress")]
        public StringExpression IpAddress { get; set; }

        [JsonProperty("FQDN")]
        public StringExpression FQDN { get; set; }

        [JsonProperty("resultProperty")]
        public StringExpression ResultProperty { get; set; }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            //var vmName = VMName?.GetValue(dc.State);

            //var resourceGroupName = ResourceGroupName?.GetValue(dc.State);

            var ipAddress = IpAddress?.GetValue(dc.State);

            var fqdn = FQDN?.GetValue(dc.State);

            var result = string.Empty;

            AddHostRunCommand command = new AddHostRunCommand();
            VirtualMachineRunCommandResource vmRunCmdResource =  command.CreateOrUpdateVMRunCommandAync(ipAddress, fqdn).Result;
            
            if (vmRunCmdResource.HasData)
            {
                VirtualMachineRunCommandData runCmd = vmRunCmdResource.Data;
                result = $"Adding IpAddress {ipAddress} and FQDN\\HostName {fqdn} to Host file is {runCmd.ProvisioningState}.";
            }

            if (ResultProperty != null)
            {
                dc.State.SetValue(this.ResultProperty.GetValue(dc.State), result);
            }

            return dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
        }
    }
}
