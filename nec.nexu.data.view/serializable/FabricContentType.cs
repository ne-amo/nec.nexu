using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class FabricContentType : nec.nexu.JsonModels.EntityBase
    {
        public virtual string FccCode { get; set; }
        public virtual string DisplayName { get; set; }
    }
}
