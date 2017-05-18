using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
	public class ColorList : nec.nexu.JsonModels.EntityBase
	{
		public string DisplayName { get; set; }
		public virtual List<Material> Colors { get; set; }
	}
}
