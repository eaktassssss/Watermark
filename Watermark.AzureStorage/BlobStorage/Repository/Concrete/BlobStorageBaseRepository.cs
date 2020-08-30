namespace Watermark.AzureStorage.BlobStorage.Repository.Concrete
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Specialized;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Watermark.AzureStorage.BlobStorage.Repository.Abstract;
    using Watermark.AzureStorage.ClientConnetion;
    using Watermark.Common.Enums;

    public class BlobStorageBaseRepository :IBlobStorageRepository
    {
        public string BlobPath { get; } = "https://eastorageaccountservice.blob.core.windows.net";

        internal BlobServiceClient _blobServiceClient;

        public BlobStorageBaseRepository()
        {
            _blobServiceClient = new BlobServiceClient(StorageConnection.ConnectionString);
        }

        public async Task DeleteAsync(string fileName, ContainerType containerType)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerType.ToString());
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.DeleteAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<Stream> DownloadAsync(string fileName, ContainerType containerType)
        {

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerType.ToString());
                var blobClient = containerClient.GetBlobClient(fileName);
                var contentInfo = await blobClient.DownloadAsync();
                return contentInfo.Value.Content;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<List<string>> GetLogAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerType.log.ToString());
                await containerClient.CreateIfNotExistsAsync();
                var appendBlobClient = containerClient.GetAppendBlobClient(fileName);
                await appendBlobClient.CreateIfNotExistsAsync();
                List<string> logs = new List<string>();
                var contentInfo = await appendBlobClient.DownloadAsync();
                using (var read = new StreamReader(contentInfo.Value.Content))
                {
                    string line = string.Empty;
                    while ((line = read.ReadLine()) != null)
                    {
                        logs.Add(read.ReadLine());
                    }
                }
                return logs;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<string> GetNames(ContainerType containerType)
        {
            try
            {
                var blobNames = new List<string>();
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerType.ToString());
                var blobs = containerClient.GetBlobs();
                blobs.ToList().ForEach(x =>
                {
                    blobNames.Add(x.Name);
                });
                return blobNames;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task SetLogAsync(string text, string blobName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerType.log.ToString());
                var appendBlobClient = containerClient.GetAppendBlobClient(blobName);
                await appendBlobClient.CreateIfNotExistsAsync();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                    {
                        streamWriter.Write($"CreateTime{DateTime.Now}:{text}\n");
                        streamWriter.Flush();
                        memoryStream.Position = 0;
                        await appendBlobClient.AppendBlockAsync(memoryStream);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task UploadAsync(Stream stream, string fileName, ContainerType containerType)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerType.ToString());
                await containerClient.CreateIfNotExistsAsync();
                await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                var blobClient = containerClient.GetBlobClient(fileName);
                var contentInfo = await blobClient.UploadAsync(stream);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
