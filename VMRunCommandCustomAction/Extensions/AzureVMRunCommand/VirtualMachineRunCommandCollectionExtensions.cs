using Azure;
using Azure.ResourceManager.Compute;
using System.Net;
using System.Threading.Tasks;

namespace VMRunCommandCustomAction.Extensions.AzureVMRunCommand
{
    public static class VirtualMachineRunCommandCollectionExtensions
    {
        public static async Task<bool> ExistsRunCommandAsync(this VirtualMachineRunCommandCollection collection,
                                                                        string runCommandName)
        {
            var result = false;
            try
            {
                var runCMDResource = await collection.GetAsync(runCommandName);
                result = runCMDResource.Value.Data.Name == runCommandName;
            }
            catch (RequestFailedException ex)
            {

                if ((HttpStatusCode)ex.Status == HttpStatusCode.NotFound && ex.ErrorCode == "ResourceNotFound")
                {
                    return result;

                }
            }
            return result;
        }

        public static async Task<bool> DeleteRunCommandIfExistsAsync(this VirtualMachineRunCommandCollection collection,
                                                                       string runCommandName)
        {
            var result = false;
            try
            {
                var runCMDResource = await collection.GetAsync(runCommandName);

                if (runCMDResource.Value.Data.Name == runCommandName)
                {
                    var armOperation = await runCMDResource.Value.DeleteAsync(WaitUntil.Completed);
                    result = armOperation.HasCompleted;
                }
            }
            catch (RequestFailedException ex)
            {

                if ((HttpStatusCode)ex.Status == HttpStatusCode.NotFound && ex.ErrorCode == "ResourceNotFound")
                {
                    return result;

                }
            }
            return result;
        }
    }
}

