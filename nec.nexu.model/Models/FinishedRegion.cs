using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
public class FinishedRegion : EntityBase<int>
{
	public virtual FinishedProduct Product { get; set; }
	public virtual ProductRegion Region { get; set; }
	public virtual Material Material { get; set; }
}
}