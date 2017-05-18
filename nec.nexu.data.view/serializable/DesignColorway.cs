using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
public class DesignColorway : nec.nexu.JsonModels.EntityBase
{
	public virtual Design Design { get; set; }
	public virtual ICollection<DesignLayer> Layers { get; set; }
}
}