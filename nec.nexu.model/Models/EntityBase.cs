using System;

namespace nec.nexu.Models
{
    public class EntityBase<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }
        public virtual bool? Active { get; set; }
    }
}
