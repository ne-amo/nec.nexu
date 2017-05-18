using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class FaceoffVote : EntityBase<int>
    {
        public FaceoffVote()
        {
            this.Active = true;
        }
        public virtual string IpAddress { get; set; }
    }
}