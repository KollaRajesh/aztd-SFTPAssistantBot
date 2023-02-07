// See https://aka.ms/new-console-template for more information
using Azure.Storage.Blobs;
using System.Text;

var test ="sample.zml".Remove(10 - 4, 3);
Console.WriteLine("Hello, World!");
var date=DateTime.Now.ToString("MMddyyyy HHmmss");
var name= GetDeploymentFileName("Capgemini");
Console.WriteLine(name);
Console.ReadLine();
//run();

void run()
{

    var connectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1;";

    string containerName = "sftpdeploymentsfiles";
    BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

    container.CreateIfNotExists();

    BlobClient blob = container.GetBlobClient("myString2");

    var myStr = "Hello!";
    var content = Encoding.UTF8.GetBytes(myStr);
    using var ms = new MemoryStream(content);
    container.UploadBlob("myString", ms);

    using var ms1 = new MemoryStream(content);
    blob.Upload(ms1);
}

static string GetDeploymentFileName (string PartnerName)
{
    var hashcode = Guid.NewGuid().GetHashCode();
    hashcode = hashcode > 0 ? hashcode : hashcode * -1;
    var datetime = DateTime.Now.ToString("MMddyyyyHHmmss");
    var result = $" {PartnerName}-Deployment-{datetime}-{hashcode}";
    return result;
}