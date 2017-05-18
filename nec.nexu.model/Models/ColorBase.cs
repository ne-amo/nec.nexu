using System;
//using nec.modules.color;

namespace nec.nexu.Models
{
    public class ColorBase :  LocalizeDisplayName//, IHsbColor
    {
        public virtual string Hex { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Pms { get; set; }
        public virtual double Hue { get; set; }
        public virtual double Saturation { get; set; }
        public virtual double Brightness { get; set; }
        public virtual double Opacity { get; set; }
        public virtual int? SortOrder { get; set; }
        public virtual BlendMode BlendMode { get; set; }
        /// <summary>
        /// A CSS valid color value to be applied to foreground text that is shown when the DesignColor is used as a background.
        /// </summary>
        public virtual string TextColor { get; set; }
        /*
        protected internal virtual HSB? _hsb { get; set; }
        public virtual HSB Hsb
        {
            get
            {
                if (!_hsb.HasValue)
                {
                    _hsb = new HSB(Hue, Saturation, Brightness);
                }
                return _hsb.Value;
            }
            set
            {
                _hsb = value;
            }
        }
        */ 
    }
}
