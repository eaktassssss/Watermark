using Azure.Storage.Queues;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watermark.AzureStorage.ClientConnetion;
using Watermark.AzureStorage.QueueStorage.Abstract;

namespace Watermark.AzureStorage.QueueStorage.Concrete
{
    public class QueueStorageService :IQueueStorageService
    {
        QueueClient _queueClient;
        public QueueStorageService()
        {
            _queueClient = new QueueClient(StorageConnection.ConnectionString, "watermarkqueue");
            _queueClient.CreateIfNotExistsAsync();
        }
        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (String.IsNullOrEmpty(message))
                    throw new ArgumentNullException("Gönderilen parametre boş olamaz");
                else
                    await _queueClient.SendMessageAsync(message);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message); ;
            }


        }
    }
}
