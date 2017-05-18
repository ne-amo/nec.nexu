using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class FabricContentType : LocalizeDisplayName
    {
        public virtual string FccCode { get; set; }
        public virtual string DisplayName { get; set; }
    }
}
