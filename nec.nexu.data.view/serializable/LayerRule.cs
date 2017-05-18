using System;

namespace nec.nexu.JsonModels
{
    public class LayerRule : nec.nexu.JsonModels.EntityBase
    {
        public virtual string Field { get; set; }
        public virtual string Operator { get; set; }
        public virtual string Value { get; set; }
    }
}
