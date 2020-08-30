using System;
using System.Collections.Generic;
using System.Text;

namespace Watermark.Dto
{
    public class WatermarkQueueDto
    {
        public List<string> Pictures { get; set; }
        public string City { get; set; }
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        public string Text { get; set; }
    }
}
