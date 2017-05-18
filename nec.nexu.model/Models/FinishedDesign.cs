using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
	public class FinishedDesign : EntityBase<int>
	{
		public virtual FinishedProduct Product { get; set; }
		public virtual DesignLocation Location { get; set; }
		public virtual DesignColorway DesignColorway { get; set; }
	}
}