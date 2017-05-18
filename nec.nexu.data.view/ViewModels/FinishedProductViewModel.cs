using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nec.nexu.ViewModels
{
    public class FinishedProductViewModel
    {
        public virtual string DisplayName { get; set; }

        public virtual int? MaterialId { get; set; }

        public virtual Guid Guid { get; set; }

        public int Views { get; set; }

        public int Likes { get; set; }

        public int Inspirations { get; set; }
    }
}