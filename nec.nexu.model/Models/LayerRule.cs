using System;

namespace nec.nexu.Models
{
    public class LayerRule : EntityBase<int>
    {
        public virtual string Field { get; set; }
        public virtual string Operator { get; set; }
        public virtual string Value { get; set; }
    }
}
