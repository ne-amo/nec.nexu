using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class UserProductLike : EntityBase<int>
    {
        public virtual string IpAddress { get; set; }
        public virtual string UserId { get; set; }
        public virtual DateTime? Timestamp { get; set; }
    }
}