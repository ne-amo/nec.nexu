using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;

namespace nec.nexu.site.admin.Models
{
    public class DesignLocationAdminViewModel
    {
        public DesignLocation DesignLocation { get; set; }

        public List<ProductTemplate> AllProductTemplates { get; set; }

        public List<TemplateView> AvailableViews { get; set; } 

        public IEnumerable<SelectListItem> ProductTemplateListItems
        {
            get
            {
                return AllProductTemplates.Select(h => new SelectListItem() { Text = h.DisplayName, Value = h.Id.ToString() });
            }
        }
    }
}