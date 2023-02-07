using AzureStorageCustomAction.Model;
using AzureStorageCustomAction.Extensions;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;
using AzureStorageCustomAction.DataStorage;

namespace AzureStorageCustomAction.Tests
{
    public class Tests
    {

        private  AzureStorage Store;
        [SetUp]
        public void Setup()
        {
            var settings = new StorageSettings();
            Store = new AzureStorage(settings.ConnectionString, settings.ContainerName);
        }

        [Test]
        public void TestSFTPDelpoymentBlobSerialize()
        {
            var partnerName = "TestCompany";

            //Arrange
            var sourceServerInfo = GetSourceServerInfo;
            var destinationServerInfo = GetDestinationServerInfo;
            //act
            var jsonString = new SFTPDelpoymentBlob(partnerName, sourceServerInfo, destinationServerInfo).Serialize();

            //Assert
           var sftpDelpoymentBlob = JsonSerializer.Deserialize<SFTPDelpoymentBlob>(jsonString);
            Assert.IsNotNull(sftpDelpoymentBlob);   
            Assert.That(partnerName, Is.EqualTo(sftpDelpoymentBlob.PartnerName));
            Assert.That(sourceServerInfo.HostName, Is.EqualTo(sftpDelpoymentBlob.SourceServerInfo.HostName));
            Assert.That(sourceServerInfo.UserName, Is.EqualTo(sftpDelpoymentBlob.SourceServerInfo.UserName));
            Assert.That(sourceServerInfo.Password, Is.EqualTo(sftpDelpoymentBlob.SourceServerInfo.Password));
            Assert.That(sourceServerInfo.PortNumber, Is.EqualTo(sftpDelpoymentBlob.SourceServerInfo.PortNumber));
            Assert.That(sourceServerInfo.SharedFolder, Is.EqualTo(sftpDelpoymentBlob.SourceServerInfo.SharedFolder));

            Assert.That(destinationServerInfo.HostName, Is.EqualTo(sftpDelpoymentBlob.DestinationServerInfo.HostName));
            Assert.That(destinationServerInfo.UserName,     Is.EqualTo(sftpDelpoymentBlob.DestinationServerInfo.UserName));
            Assert.That(destinationServerInfo.Password,     Is.EqualTo(sftpDelpoymentBlob.DestinationServerInfo.Password));
            Assert.That(destinationServerInfo.PortNumber,   Is.EqualTo(sftpDelpoymentBlob.DestinationServerInfo.PortNumber));
            Assert.That(destinationServerInfo.SharedFolder, Is.EqualTo(sftpDelpoymentBlob.DestinationServerInfo.SharedFolder));
            //Assert.Pass();
        }

        [Test]
        public void TestSFTPDelpoymentBlobUpload()
        {
            var partnerName = "TestCompany";

            //Arrange
            var sourceServerInfo = GetSourceServerInfo;
            var destinationServerInfo = GetDestinationServerInfo;
            //act
            var blobJSON = new SFTPDelpoymentBlob(partnerName, sourceServerInfo, destinationServerInfo).Serialize();

            var blobName = partnerName.GetUniqueDeploymentFileName();
            var blobURI =  Store.UploadContentAsync(blobJSON, blobName).Result;

            //Assert
            Assert.IsNotEmpty(blobJSON);
            Assert.IsNotEmpty(blobName);
            Assert.IsNotEmpty(blobURI);
            //Assert.Pass();
        }

        private static DestinationServerInfo GetDestinationServerInfo=> new("LARDesti1", 8051, "pDesti", "$13$!@$", @"\\Source\images\",
                                                  true, "Sourcecert.pfx", @"https:\\asdf\SourceCert.pfx");

        private static SourceServerInfo GetSourceServerInfo =>  new("LARSource1", 8050, "psource", "$13$!@$", @"\\Source\images\",
                                                  true, "Destinationcert.pfx", @"https:\\asdf\Destcert.pfx");
    }
}