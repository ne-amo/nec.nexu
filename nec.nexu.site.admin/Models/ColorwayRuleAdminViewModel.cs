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
    public class ColorwayRuleAdminViewModel
    {
        public ColorwayRule Rule { get; set; }

        public List<DesignResultMessage> AllDesigns { get; set; }

        public List<DesignColor> AllDesignColors { get; set; } 

        public List<int> ColorwayRuleValueIds { get; set; }

        public int SelectedColorId { get; set; }

        public List<int> ImageId { get; set; } 

        public IEnumerable<SelectListItem> DesignListItems
        {
            get
            {
                return AllDesigns.Select(d => new SelectListItem() { Text = d.DisplayName, Value = d.Id.ToString() });
            }
        }

        public IEnumerable<SelectListItem> DesignColorListItems
        {
            get
            {
                return AllDesignColors.Select(d => new SelectListItem() { Text = d.DisplayName, Value = d.Id.ToString() });
            }
        }
    }
}