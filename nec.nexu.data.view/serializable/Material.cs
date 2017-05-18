using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class Material : nec.nexu.JsonModels.ColorBase
	{
		public virtual FabricContentType FabricContent { get; set; }
		public virtual List<int> MatchingDesignColorIds { get; set; }
		//public virtual Stitch MatchingEyelet { get; set; }
		//public virtual Stitch MatchingStitch { get; set; }
		public virtual ImageFile TrueView { get; set; }
		public virtual Pattern Pattern { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<ColorList> ColorLists { get; set; }

	}
}
