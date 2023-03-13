using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using System.Threading.Tasks;

namespace VMRunCommandCustomAction.AzureManagementAPI.AzureVMRunCommands
{
    public interface IRunCommand
    {
        string Name { get; }
        Task<VirtualMachineRunCommandResource> CreateOrUpdateVMRunCommandAync(string ipAddress, string fqdn);

        Task<bool> InvokeShellCommandASync(string ipAddress, string fqdn);
    }
}