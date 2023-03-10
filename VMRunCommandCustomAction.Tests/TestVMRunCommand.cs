
using System.Xml.Linq;
using VMRunCommandCustomAction.AzureManagementAPI;
using VMRunCommandCustomAction.AzureManagementAPI.AzureVMRunCommands;
using VMRunCommandCustomAction.Extensions.AzureVMRunCommand;

namespace VMRunCommandCustomAction.Tests
{

    public class TestVMRunCommand
    {
        private readonly AzureAccountSettings AzaccountSettings;

        public TestVMRunCommand()
        {
            //TODO:Write custom class for azure settings 
            //TODO:Write function to create \update runcommand in custom action project
            //TODO:Build Custom action metod for adding hosts
            //TODO:Integrate with Bot
            //TODO:Read Data From Environment Variables \KeyVault
            AzaccountSettings=new AzureAccountSettings();
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestVMRunCommandCreateOrUpdate()
        {
            ResourceIdentifier vmResourceId = VirtualMachineResource.CreateResourceIdentifier(AzaccountSettings.SubscriptionId,
                                                                                                AzaccountSettings.ResourceGroupName,
                                                                                                AzaccountSettings.VMName);
            VirtualMachineResource vm = AzaccountSettings.ARMClient.GetVirtualMachineResource(vmResourceId);

            // get the collection of this VirtualMachineRunCommandResource
            VirtualMachineRunCommandCollection collection = vm.GetVirtualMachineRunCommands();

            string runCommandName = $"TestCmd-{Random.Shared.Next(1, 10)}-{DateTime.Now.Ticks}";

            await collection.DeleteRunCommandIfExistsAsync(runCommandName);
            VirtualMachineRunCommandData data = new VirtualMachineRunCommandData(new AzureLocation(AzaccountSettings.Location))
            {
                Source = new VirtualMachineRunCommandScriptSource()
                {
                    Script = "echo Hello World!",
                },
                Parameters ={
                    new RunCommandInputParameter("param1","value1"),new RunCommandInputParameter("param2","value2")
                },
                AsyncExecution = false,
                RunAsUser = "user1",
                RunAsPassword = "<runAsPassword>",
                TimeoutInSeconds = 3600,
            };

            ArmOperation<VirtualMachineRunCommandResource> lro = await collection.CreateOrUpdateAsync(WaitUntil.Completed, runCommandName, data);
            VirtualMachineRunCommandResource result = lro.Value;

            VirtualMachineRunCommandData runCmd = result.Data;
            Assert.That(runCmd, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(runCmd.Name, Is.Not.Null);
                Assert.That(runCmd.Name, Is.EqualTo(runCommandName));
            });

            Assert.Multiple(() =>
            {
                Assert.That(runCmd.Parameters, Is.Not.Null);
                Assert.That(runCmd.Parameters.Count, Is.EqualTo(data.Parameters.Count));
            });

            Assert.Multiple(() =>
            {
                Assert.That(runCmd.Location.DisplayName, Is.EqualTo(AzaccountSettings.Location));
            });
            await collection.DeleteRunCommandIfExistsAsync(runCommandName);

        }
        [Test]
        public async Task TestAddHostCmdCreateOrUpdateAsync()
        {

            AddHostRunCommand command =new AddHostRunCommand();
            string ipAddress = "20.002.123.146";
            string fqdn = "test2.vm.com";

            VirtualMachineRunCommandResource vmRunCmdResource = await command.CreateOrUpdateVMRunCommandAync(ipAddress, fqdn);
            
            VirtualMachineRunCommandData runCmd = vmRunCmdResource.Data;

            Assert.That(runCmd, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(runCmd.Name, Is.Not.Null);
                Assert.That(runCmd.Name, Is.EqualTo(command.Name));
            });

            Assert.Multiple(() =>
            {
                Assert.That(runCmd.Parameters, Is.Not.Null);
                Assert.That(runCmd.Parameters.Count, Is.EqualTo(2));
            });

            Assert.Multiple(() =>
            {
                Assert.That(runCmd.Location.DisplayName, Is.EqualTo(AzaccountSettings.Location));
            });
        }
               
        
    }
}