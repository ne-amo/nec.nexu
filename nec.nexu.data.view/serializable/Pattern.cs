using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class Pattern : nec.nexu.JsonModels.EntityBase
    {
        public virtual string DisplayName { get; set; }
        public virtual ImageFile Thumbnail { get; set; }
        public virtual List<PatternViewImage> ViewImages { get; set; }
    }
}
