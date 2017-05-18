using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nec.nexu.JsonModels
{
    public class ColorContainer : nec.nexu.JsonModels.EntityBase
    {
        public virtual string ContainerName { get; set; }
        public virtual List<Material> Colors { get; set; }
        public virtual List<int> Products { get; set; }
    }
}
