using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class Pattern : LocalizeDisplayName
    {
        public virtual string DisplayName { get; set; }
        public virtual ImageFile Thumbnail { get; set; }
        public virtual List<PatternViewImage> ViewImages { get; set; }
    }
}
