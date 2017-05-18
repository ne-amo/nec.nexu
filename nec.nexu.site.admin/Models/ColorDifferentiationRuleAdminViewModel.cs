using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;
using nec.nexu.Models.rules;
using nec.nexu.ViewModels;

namespace nec.nexu.site.admin.Models
{
    public class ColorDifferentiationRuleAdminViewModel
    {
        public ColorDifferentiationRule Rule { get; set; }

        public List<DesignResultMessage> AllDesigns { get; set; }

        public IEnumerable<SelectListItem> DesignListItems
        {
            get
            {
                return AllDesigns.Select(d=> new SelectListItem() { Text = d.DisplayName, Value = d.Id.ToString() });
            }
        }
    }
}