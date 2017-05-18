using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class UserProductLike : nec.nexu.JsonModels.EntityBase
    {
        public virtual string IpAddress { get; set; }
        public virtual string UserId { get; set; }
    }
}