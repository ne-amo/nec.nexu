using System;
using System.Collections.Generic;
using System.Drawing;

namespace nec.nexu.Models
{
    public class ImageFile : EntityBase<int>
    {
        public virtual string DisplayName { get; set; }
	    public virtual string FilePath { get; set; }
        public virtual string ThumbnailPath { get; set; }
        public virtual int HeightPx { get; set; }
        public virtual int WidthPx { get; set; }
        public virtual ImageType ImageType { get; set; }
        /*
        private virtual byte[] _data { get; set; }
        private virtual Bitmap _image { get; set; }
        public virtual Bitmap Image
        {
            get
            {
                if(_image == null && _data != null)
                    _image = new Bitmap(Bitmap.FromStream(new System.IO.MemoryStream(this._data)));
                return _image;
            }
            set
            {
                _image = value;
            }
        }
        */
    }
}