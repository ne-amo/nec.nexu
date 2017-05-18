using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
	public class FinishedDesign : nec.nexu.JsonModels.EntityBase
	{
        [Newtonsoft.Json.JsonIgnore]
		public virtual FinishedProduct Product { get; set; }
		public virtual DesignLocation Location { get; set; }
		public virtual DesignColorway DesignColorway { get; set; }
	}
}