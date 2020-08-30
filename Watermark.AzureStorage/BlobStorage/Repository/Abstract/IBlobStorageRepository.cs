using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Watermark.Common.Enums;

namespace Watermark.AzureStorage.BlobStorage.Repository.Abstract
{
    public interface IBlobStorageRepository
    {
        string BlobPath { get;}
        Task UploadAsync(Stream stream, string fileName, ContainerType containerType);
        Task<Stream> DownloadAsync(string fileName, ContainerType containerType);
        Task DeleteAsync(string fileName, ContainerType containerType);
        Task SetLogAsync(string text, string blobName);
        Task<List<string>> GetLogAsync(string fileName);
        List<string> GetNames(ContainerType containerType);
    }
}
