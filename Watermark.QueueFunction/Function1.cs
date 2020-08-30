namespace Watermark.QueueFunction
{
    using Microsoft.Azure.WebJobs;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Watermark.AzureStorage.BlobStorage.Repository.Concrete;
    using Watermark.AzureStorage.ClientConnetion;
    using Watermark.AzureStorage.TableStorage.Repository.Concrete;
    using Watermark.Common.Enums;
    using Watermark.Dto;
    using Watermark.Entities;

    public static class Function1
    {
        [FunctionName("Function1")]
        public async static Task Run([QueueTrigger("watermarkqueue")]WatermarkQueueDto myQueueItem)
        {
            try
            {
                StorageConnection.ConnectionString = "****";
                var blobStorage = new BlobStorageBaseRepository();
                var tableStorage = new TableStorageBaseRepository<UserPicture>();
                foreach (var item in myQueueItem.Pictures)
                {
                    var stream = await blobStorage.DownloadAsync(item, ContainerType.picture);
                    var response = await AddWatermark(myQueueItem.Text, stream);
                    await blobStorage.UploadAsync(response, item, ContainerType.watermarkpicture);
                }
                var userPicture = await tableStorage.GetAsync(myQueueItem.City, myQueueItem.UserId).ConfigureAwait(false);
                if (userPicture.WatermarkRawPaths != null)
                {
                    var watermark = JsonConvert.DeserializeObject<List<string>>(userPicture.WatermarkRawPaths);
                    myQueueItem.Pictures.AddRange(watermark);
                }
                userPicture.WatermarkRawPaths = JsonConvert.SerializeObject(myQueueItem.Pictures);
                await tableStorage.AddAsync(userPicture);
                var client = new HttpClient();
                var request = await client.GetAsync("https://localhost:44308/api/Pictures/CompletedNotification/" + myQueueItem.ConnectionId);

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static Task<MemoryStream> AddWatermark(string watermarkText, Stream PictureStream)
        {
            MemoryStream ms = new MemoryStream();

            using (Image image = Bitmap.FromStream(PictureStream))
            {
                using (Bitmap tempBitmap = new Bitmap(image.Width, image.Height))
                {
                    using (Graphics gph = Graphics.FromImage(tempBitmap))
                    {
                        gph.DrawImage(image, 0, 0);
                        var font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold);
                        var color = Color.FromArgb(255, 0, 0);
                        var brush = new SolidBrush(color);
                        var point = new Point(20, image.Height - 50);
                        gph.DrawString(watermarkText, font, brush, point);
                        tempBitmap.Save(ms, ImageFormat.Png);
                    }
                }
            }
            ms.Position = 0;
            return Task.FromResult(ms);
        }
    }
}
