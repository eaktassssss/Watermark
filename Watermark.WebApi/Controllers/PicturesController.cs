using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Watermark.Common.Hubs;

namespace Watermark.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PicturesController :ControllerBase
    {

        private readonly IHubContext<NotificationHub> _hubContext;
        public PicturesController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("{connectionId}")]
        public async Task<ActionResult> CompletedNotification(string connectionId)
        {
            try
            {
                if (String.IsNullOrEmpty(connectionId))
                    throw new ArgumentNullException("Connection bilgisi belirtilmemiş");
                else
                {
                   await _hubContext.Clients.Client(connectionId).SendAsync("CompletedNotification");
                    return Ok();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}