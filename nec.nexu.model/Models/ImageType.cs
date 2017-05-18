using System;

namespace nec.nexu.Models
{
    public class ImageType : EntityBase<int>
    {
        public virtual string DisplayName { get; set; }
        public virtual string Suffix { get; set; }
        public virtual bool Default { get; set; }
        public virtual int? SortOrder { get; set; }
        public virtual bool ConfiguratorTemplate { get; set; }
    }
}
