using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class ColorList : LocalizeDisplayName
	{
		public string DisplayName { get; set; }
		public virtual List<Material> Colors { get; set; }
	}
}
