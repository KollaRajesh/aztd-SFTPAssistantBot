using Azure.Storage.Blobs;
using Azure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace AzureStorageCustomAction.DataStorage
{
    public  class AzureStorage
    {
        readonly BlobServiceClient blobServiceClient;
        BlobContainerClient blobContainerClient;
        public AzureStorage(string connectionString, string containerName)
        {
            blobServiceClient = new BlobServiceClient(connectionString);

            CreateContainer(containerName);

            if (blobContainerClient == null)
                throw new Exception("Create Container is failed");
        }

        private async void CreateContainer(string containerName)
        {
            try
            {
                blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                if (blobContainerClient == null) return;

                bool isExits = await blobContainerClient.ExistsAsync();
                if (isExits)
                    return;

                await blobContainerClient.CreateIfNotExistsAsync();
            }
            catch (RequestFailedException)
            {
            }
        }

        public async Task<string> UploadAsync(string fileUrl, string fileName)
        {

            WebClient wc = new WebClient();
            MemoryStream stream = new MemoryStream(wc.DownloadData(fileUrl));

            return await UploadAsync(fileName, stream);
        }

        public async Task<string> UploadContentAsync(string content, string fileName)
        {

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return await UploadAsync(fileName, stream);
        }

        private async Task<string> UploadAsync(string fileName, MemoryStream stream)
        {
            var blob = blobContainerClient.GetBlobClient(fileName);

            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

            var result = await blob.UploadAsync(stream);

            return blob.Uri.AbsoluteUri;
        }

    }
}
