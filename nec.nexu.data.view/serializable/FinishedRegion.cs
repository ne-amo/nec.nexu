using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
public class FinishedRegion : nec.nexu.JsonModels.EntityBase
{
    [Newtonsoft.Json.JsonIgnore]
	public virtual FinishedProduct Product { get; set; }

	public virtual ProductRegion Region { get; set; }
	public virtual Material Material { get; set; }
}
}