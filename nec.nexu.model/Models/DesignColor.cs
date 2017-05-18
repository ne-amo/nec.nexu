using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class DesignColor : ColorBase
    {
        public virtual List<Material> MatchedToMaterials { get; set; }
    }
}
