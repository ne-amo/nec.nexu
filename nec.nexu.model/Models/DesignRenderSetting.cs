using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class DesignRenderSetting
    {
        public int Id { get; set; }
        public int TemplateViewId { get; set; }
        public int DesignLocationId { get; set; }
        public float M1 { get; set; }
        public float M2 { get; set; }
        public float M3 { get; set; }
        public float M4 { get; set; }
        public float M5 { get; set; }
        public float M6 { get; set; }
        public byte[] ClippingMask { get; set; }
    }
}
