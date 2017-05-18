using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nec.nexu.Models
{
    public class Inspiration : EntityBase<int>
    {
        public virtual string IpAddress { get; set; }
    }
}