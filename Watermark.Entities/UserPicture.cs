using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watermark.Entities
{
    public class UserPicture :TableEntity
    {
        public string WatermarkRawPaths { get; set; }
        public string RawPaths { get; set; }
        [IgnoreProperty]
        public List<string> Paths { get; set; }
        [IgnoreProperty]
        public List<string> WatermarkPaths { get; set; }
    }
}
