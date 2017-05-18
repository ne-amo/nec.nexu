using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nec.nexu.Models
{
    public class UserProductView : EntityBase<int>
    { 
        public virtual string IpAddress { get; set; }
        public virtual DateTime? Timestamp { get; set; }
    }
}