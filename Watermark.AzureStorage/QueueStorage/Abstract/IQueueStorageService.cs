using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Watermark.AzureStorage.QueueStorage.Abstract
{
    public interface IQueueStorageService
    {
        Task SendMessageAsync(string message);
    }
}
