using System;
using System.Drawing;

namespace nec.modules.bitmap
{
    public class RecolorOptions
    {
        public Color Color { get; set; }
        /// <summary>
        /// Alpha value 0-255
        /// </summary>
        public int Alpha { get; set; }
        /// <summary>
        /// String indicating blend mode. Options are "MULTIPLY", "SCREEN", "OVERLAY"
        /// </summary>
        public string BlendMode { get; set;}
        /// <summary>
        /// Whether or not to trim the input bitmap prior to recoloring. Default true.
        /// </summary>
        public bool Trim = true;
    }
}
