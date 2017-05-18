using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nec.nexu.site.admin.Models
{
    public class ColorContainerViewModel
    {
        public nec.nexu.JsonModels.ColorContainer ColorContainer { get; set; }
        public List<nec.nexu.JsonModels.Material> AllColors { get; set; }
        public List<int> SelectedColors { get; set; }
        public int TotalMaterialsUsing { get; set; }
    }
}