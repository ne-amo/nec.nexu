using System;
using System.Collections.Generic;
using System.Linq;
using nec.nexu.model.Models.ComplexTypes;

namespace nec.nexu.Models
{
    public class Hierarchy : LocalizeDisplayName
    {
        public Hierarchy()
        {
            this.ExclusiveBackgrounds = new List<CustomBackground>();
        }

        public virtual string DisplayName { get; set; }

        public int? ParentHierarchyId { get; set; }

        public virtual Hierarchy ParentHierarchy { get; set; }

        public int? SortOrder { get; set; }

        public PersistableIntCollection AllowedTemplates { get; set; }

        public virtual List<CustomBackground> ExclusiveBackgrounds { get; set; }

        public void Build(Hierarchy hierarchy)
        {
            this.DisplayName = hierarchy.DisplayName;
            this.ParentHierarchyId = hierarchy.ParentHierarchyId;
            this.SortOrder = hierarchy.SortOrder;
        }
    }
}