using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class DesignColor : nec.nexu.JsonModels.ColorBase
    {
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<Material> MatchedToMaterials { get; set; }
    }
    public class DesignColorMatchMessage
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
    }
}
