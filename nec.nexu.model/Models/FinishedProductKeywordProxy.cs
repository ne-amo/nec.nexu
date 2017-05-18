using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class FinishedProductKeywordProxy
    {
        public virtual int Id { get; set; }
        public virtual int ProductId { get; set; }
        public virtual string Keyword { get; set; }
    }
}
