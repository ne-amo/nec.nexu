using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class ImageDataCache
    {
        public int Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        public byte[] Data { get; set; }
    }
}
