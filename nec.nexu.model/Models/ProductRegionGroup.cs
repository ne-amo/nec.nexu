using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class ProductRegionGroup : LocalizeDisplayName
    {
        public virtual string DisplayName { get; set; }
        public virtual string Label { get; set; }
        public virtual int SortOrder { get; set; }
        public virtual bool ForceGroup { get; set; }
        public virtual bool AllowMatching { get; set; }
        public virtual bool AllowRecolorAll { get; set; }
        public virtual int TemplateViewId { get; set; }
        public virtual List<ProductRegion> Regions { get; set; }
    }
}
