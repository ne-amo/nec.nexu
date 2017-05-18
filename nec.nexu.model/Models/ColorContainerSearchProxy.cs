using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nec.nexu.Models;

namespace nec.nexu.model.Models
{
    public class ColorContainerSearchProxy : EntityBase<int>
    {
        public virtual int ProductId { get; set; }
        public virtual int ContainerId { get; set; }
        public virtual int Count { get; set; }
    }
}
