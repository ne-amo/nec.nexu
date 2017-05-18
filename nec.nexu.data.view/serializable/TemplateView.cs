using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class TemplateView : nec.nexu.JsonModels.EntityBase
    {
        /// <summary>
        /// The name of this object that will displayed on the UI.
        /// </summary>
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// The order in which to sort this view in the collection.
        /// </summary>
        public virtual int SortOrder { get; set; }

        /// <summary>
        /// Unmapped property. Converts or retrieves Base64 SVG content to byte array for storage.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        public virtual string SvgContent
        {
            get
            {
                if (_Base64EncodedSvg == null)
                    return null;
                return Convert.ToBase64String(_Base64EncodedSvg);
            }
            set
            {
                if(!string.IsNullOrEmpty(value))
                    _Base64EncodedSvg = Convert.FromBase64String(value);
            }
        }

        [System.ComponentModel.DataAnnotations.Schema.Column]
        private byte[] _Base64EncodedSvg { get; set; }

        public string ThumbnailPath { get; set; }

        public virtual bool IsDefault { get; set; }

        /// <summary>
        /// Should this be included in the 360 view
        /// </summary>
        public virtual bool Include360 { get; set; }
        /// <summary>
        /// Should this view appear in the viewer carousel
        /// </summary>
        public virtual bool IncludeUI { get; set; }

        /// <summary>
        /// The individual TemplateImage entities that are used to make up the raster object of the view.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<TemplateImage> Images { get; set; }


        public class TemplateViewConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<TemplateView>
        {
            public TemplateViewConfiguration()
            {
                Property(x => x._Base64EncodedSvg);
            }
        }
    }
}
