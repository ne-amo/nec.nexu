using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nec.nexu.Models;

namespace nec.nexu.site.admin.Models
{
    public class CustomBackgroundAdminViewModel
    {
        public CustomBackground CustomBackground { get; set; }
        public List<ProductTemplate> AllProductTemplates { get; set; }
        public List<Hierarchy> AllHierarchies { get; set; }
    }
}