
using System.Xml.Linq;
using VMRunCommandCustomAction.AzureManagementAPI;
using VMRunCommandCustomAction.AzureManagementAPI.AzureVMRunCommands;
using VMRunCommandCustomAction.Extensions.AzureVMRunCommand;

namespace VMRunCommandCustomAction.Tests
{

    public class TestVMRunCommand
    {
        private readonly AzureAccountSettings settings;

        public TestVMRunCommand()
        {

            //TODO:Read Data From Environment Variables \KeyVault
            //TODO: Remove specific commit from GIT history
            settings = new AzureAccountSettings();
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestVMRunCommandCreateOrUpdate()
        {
            ResourceIdentifier vmResourceId = VirtualMachineResource.CreateResourceIdentifier(settings.SubscriptionId,
                                                                                              settings.ResourceGroupName,
                                                                                              settings.VMName);
            VirtualMachineResource vm = settings.ARMClient.GetVirtualMachineResource(vmResourceId);

            // get the collection of this VirtualMachineRunCommandResource
            VirtualMachineRunCommandCollection collection = vm.GetVirtualMachineRunCommands();

            string runCommandName = $"TestCmd-{Random.Shared.Next(1, 10)}-{DateTime.Now.Ticks}";

            await collection.DeleteRunCommandIfExistsAsync(runCommandName);
            VirtualMachineRunCommandData data = new VirtualMachineRunCommandData(new AzureLocation(settings.Location))
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
                Assert.That(runCmd.Location.DisplayName, Is.EqualTo(settings.Location));
            });
            await collection.DeleteRunCommandIfExistsAsync(runCommandName);

        }
        [Test]
        public async Task TestAddHostCmdCreateOrUpdateAsync()
        {

            AddHostRunCommand command = new AddHostRunCommand();
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
                Assert.That(runCmd.Location.DisplayName, Is.EqualTo(settings.Location));
            });
        }

        [Test]
        public async Task TestInvokeRunCmdShellToAddEntryInHostsAsync()
        {
            ResourceIdentifier virtualMachineResourceId = VirtualMachineResource.
                                                CreateResourceIdentifier(settings.SubscriptionId,
                                                                         settings.ResourceGroupName,
                                                                         settings.VMName);
            VirtualMachineResource virtualMachine = settings.ARMClient.GetVirtualMachineResource(virtualMachineResourceId);
            var ip = "10.300.186.001";
            var fqdn = "test4.1.com";
            // invoke the operation
            var input = new RunCommandInput("RunShellScript");
            input.Script.Add("echo $ip $fqdn | sudo tee -a /etc/hosts");
            input.Parameters.Add(new RunCommandInputParameter("ip", ip));
            input.Parameters.Add(new RunCommandInputParameter("fqdn", fqdn));

            ArmOperation<VirtualMachineRunCommandResult> lro = await virtualMachine.RunCommandAsync(WaitUntil.Completed, input);
            var result = lro.Value;
            Assert.That(result, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(result?.Value, Is.Not.Null);
                Assert.That(result?.Value.Count, Is.Not.Zero);
                Assert.That(result?.Value.Count == 1, Is.True);
                Assert.That(result?.Value[0].Code == "ProvisioningState/succeeded", Is.True);
                Assert.That(result?.Value[0].DisplayStatus == "Provisioning succeeded", Is.True);
                Assert.That(result?.Value[0].Message.Contains("succeeded"), Is.True);
                Assert.That(result?.Value[0].Message.Contains(ip), Is.True);
                Assert.That(result?.Value[0].Message.Contains(fqdn), Is.True);
            });
        }

        [Test]
        public async Task TestInvokeRunCmdShellToCreateShellScriptAsync()
        {
            ResourceIdentifier virtualMachineResourceId = VirtualMachineResource.
                                                CreateResourceIdentifier(settings.SubscriptionId,
                                                                         settings.ResourceGroupName,
                                                                         settings.VMName);
            VirtualMachineResource virtualMachine = settings.ARMClient.GetVirtualMachineResource(virtualMachineResourceId);
            // invoke the operation
            var input = new RunCommandInput("RunShellScript");
            input.Script.Add("echo 'echo $ip $fqdn | sudo tee -a /etc/hosts' >/home/azureuser/test-addhosts.sh");
            input.Script.Add("chmod 777 /home/azureuser/test-addhosts.sh");
            ArmOperation<VirtualMachineRunCommandResult> lro = await virtualMachine.RunCommandAsync(WaitUntil.Completed, input);
            var result = lro.Value;
            Assert.That(result, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(result?.Value, Is.Not.Null);
                Assert.That(result?.Value.Count, Is.Not.Zero);
                Assert.That(result?.Value.Count == 1, Is.True);
                Assert.That(result?.Value[0].Code == "ProvisioningState/succeeded", Is.True);
                Assert.That(result?.Value[0].DisplayStatus == "Provisioning succeeded", Is.True);
                Assert.That(result?.Value[0].Message.Contains("Enable succeeded"), Is.True);
            });
        }

            [Test]
            public async Task TestInvokeRunCmdShellToAddHostsAsync()
            {
                ResourceIdentifier virtualMachineResourceId = VirtualMachineResource.
                                                    CreateResourceIdentifier(settings.SubscriptionId,
                                                                             settings.ResourceGroupName,
                                                                             settings.VMName);
                VirtualMachineResource virtualMachine = settings.ARMClient.GetVirtualMachineResource(virtualMachineResourceId);
                var ip = "10.300.186.002";
                var fqdn = "test4.2.com";
                // invoke the operation
                var input = new RunCommandInput("RunShellScript");
                input.Script.Add("/home/azureuser/test-addhosts.sh $ip $fqdn");
                input.Parameters.Add(new RunCommandInputParameter("ip", ip));
                input.Parameters.Add(new RunCommandInputParameter("fqdn", fqdn));
                ArmOperation<VirtualMachineRunCommandResult> lro = await virtualMachine.RunCommandAsync(WaitUntil.Completed, input);
                var result = lro.Value;
                Assert.That(result, Is.Not.Null);

                Assert.Multiple(() =>
                {
                    Assert.That(result?.Value, Is.Not.Null);
                    Assert.That(result?.Value.Count, Is.Not.Zero);
                    Assert.That(result?.Value.Count == 1, Is.True);
                    Assert.That(result?.Value[0].Code == "ProvisioningState/succeeded", Is.True);
                    Assert.That(result?.Value[0].DisplayStatus == "Provisioning succeeded", Is.True);
                    Assert.That(result?.Value[0].Message.Contains("succeeded"), Is.True);
                    Assert.That(result?.Value[0].Message.Contains(ip), Is.True);
                    Assert.That(result?.Value[0].Message.Contains(fqdn), Is.True);
                    Assert.That(result?.Value[0].Message.Trim().EndsWith("[stderr]"), Is.True);
                });
            }
        [Test]
        public async Task TestInvokeShellCommandASync()
        {

            AddHostRunCommand command = new AddHostRunCommand();
            string ipAddress = "10.300.186.005";
            string fqdn = "test4.5.vm.com";
            var result= await command.InvokeShellCommandASync(ipAddress, fqdn);
            Assert.True(result);
        }

    }
}