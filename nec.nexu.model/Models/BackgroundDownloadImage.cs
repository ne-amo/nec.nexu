using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class BackgroundDownloadImage : EntityBase<int>
    {
        /// <summary>
        /// Id of the Download Format
        /// </summary>
        public int   FormatId { get; set; }
        /// <summary>
        /// File path to image
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// X Position to center the product image on
        /// </summary>
        public float CenterX { get; set; }
        /// <summary>
        /// Y Position to center the product image on
        /// </summary>
        public float CenterY { get; set; }
        /// <summary>
        /// Scale of the product image. 100 would be normal size.
        /// </summary>
        public int ImageScale { get; set; }
        /// <summary>
        /// Bottom left X Position of the text
        /// </summary>
        public float TextX { get; set; }
        /// <summary>
        /// Bottom left Y Position of the text
        /// </summary>
        public float TextY { get; set; }
        /// <summary>
        /// Font Size
        /// </summary>
        public int FontSize { get; set; }
    }
}
