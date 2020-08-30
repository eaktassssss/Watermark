namespace Watermark.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Watermark.AzureStorage.BlobStorage.Repository.Abstract;
    using Watermark.AzureStorage.QueueStorage.Abstract;
    using Watermark.AzureStorage.TableStorage.Repository.Abstract;
    using Watermark.Common.Enums;
    using Watermark.Dto;
    using Watermark.Entities;

    public class PictureController :Controller
    {
        internal ITableStorageRepository<UserPicture> _tableStorageRepository;

        internal IBlobStorageRepository _blobStorageRepository;

        internal IQueueStorageService _queueStorageSevice;

        public PictureController(ITableStorageRepository<UserPicture> tableStorageRepository, IBlobStorageRepository blobStorageRepository, IQueueStorageService queueStorageSevice)
        {
            _tableStorageRepository = tableStorageRepository;
            _blobStorageRepository = blobStorageRepository;
            _queueStorageSevice = queueStorageSevice;
        }
        public string UserId { get; set; } = "19031903";
        public string City { get; set; } = "istanbul";
        public async Task<ActionResult> Index()
        {
            var paths = new List<FileBlobDto>();
            var user = await _tableStorageRepository.GetAsync(City, UserId);
            if (user != null)
                if (user.RawPaths != null)
                    user.Paths = JsonConvert.DeserializeObject<List<string>>(user.RawPaths);
            ViewBag.UserId = UserId;
            ViewBag.City = City;
            if (user != null)
            {
                user.Paths.ForEach(x =>
                {
                    paths.Add(new FileBlobDto { Name = x, Url = $"{_blobStorageRepository.BlobPath}/{ContainerType.picture}/{x}" });
                });
            }
            return View(paths);
        }

        [HttpPost]
        public async Task<ActionResult> Index(IEnumerable<IFormFile> contents)
        {
            var pictureNames = new List<string>();
            foreach (var item in contents)
            {
                var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(item.FileName)}";
                await _blobStorageRepository.UploadAsync(item.OpenReadStream(), fileName, ContainerType.picture);
                pictureNames.Add(fileName);
            }

            var user = await _tableStorageRepository.GetAsync(City, UserId);
            if (user != null)
            {
                var picture = JsonConvert.DeserializeObject<List<string>>(user.RawPaths);

                pictureNames.AddRange(picture);
                user.RawPaths = JsonConvert.SerializeObject(pictureNames);
                await _tableStorageRepository.AddAsync(user);
            }
            else
            {
                var newUser = new UserPicture
                {
                    RowKey = UserId,
                    PartitionKey = City,
                    RawPaths = JsonConvert.SerializeObject(pictureNames)
                };
                await _tableStorageRepository.AddAsync(newUser);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> AddWatermark(WatermarkQueueDto dto)
        {
            if (dto.Pictures.Count == 0)
                return Json(new { message = "Döküman seçimi yapılmadı!", success = false });
            else
            {
                var serielizeObject = JsonConvert.SerializeObject(dto);
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(serielizeObject));
                await _queueStorageSevice.SendMessageAsync(base64);
                return Json(new { message = "Yazı ekleme işlemi başarılı", success = true });
            }
        }

        public async Task<IActionResult> WatermarkPictureList()
        {
            var files = new List<FileBlobDto>();
            UserPicture pictures = await _tableStorageRepository.GetAsync(City, UserId);
            pictures.WatermarkPaths = JsonConvert.DeserializeObject<List<string>>(pictures.WatermarkRawPaths);
            pictures.WatermarkPaths.ForEach(x =>
            {
                files.Add(new FileBlobDto { Name = x, Url = $"{_blobStorageRepository.BlobPath}/{ContainerType.watermarkpicture}/{x}" });
            });
            return View(files);
        }
    }
}
