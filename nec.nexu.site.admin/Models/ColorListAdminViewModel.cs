using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;

namespace nec.nexu.site.admin.Models
{
    public class ColorListAdminViewModel
    {
        public ColorList ColorList { get; set; }
        public List<int> SelectedMaterialIds { get; set; }
        public List<Material> AllMaterials { get; set; }
    }
}