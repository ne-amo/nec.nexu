using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class CustomBackground : nec.nexu.JsonModels.EntityBase
    {
        public virtual string DisplayName { get; set; }

        public virtual string FilePath { get; set; }

        public virtual string ThumbnailPath { get; set; }

        public virtual int[] HierarchyIds { get; set; }

        public virtual int[] TemplateIds { get; set; }
    }
}