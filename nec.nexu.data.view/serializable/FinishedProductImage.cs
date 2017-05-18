using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nec.nexu.JsonModels
{
    public class FinishedProductImage : nec.nexu.JsonModels.EntityBase
    {
        public virtual string DisplayName { get; set; }
        public virtual string FilePath { get; set; }
        public virtual ImageType ImageType { get; set; }
    }
}
