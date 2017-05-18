using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class DesignColorway : EntityBase<int>
    {
	    public virtual Design Design { get; set; }
	    public virtual ICollection<DesignLayer> Layers { get; set; }
    }
}