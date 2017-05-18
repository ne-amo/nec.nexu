using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class Hierarchy : nec.nexu.JsonModels.EntityBase
    {
        public virtual string DisplayName { get; set; }

        public int? ParentHierarchyId { get; set; }

        public virtual Hierarchy ParentHierarchy { get; set; } 
    }
}