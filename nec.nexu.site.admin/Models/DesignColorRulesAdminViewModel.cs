
using System.Web.Mvc;
using nec.nexu.Models;
using nec.nexu.Models.rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nec.nexu.ViewModels;

namespace nec.nexu.site.admin.Models
{
    public class DesignColorRulesAdminViewModel
    {
        public DesignColorRulesAdminViewModel()
        {
            this.AllDesigns = new List<DesignResultMessage>();
        }
        public DesignColorRule DesignColorRule { get; set; }
        public String NewValues { get; set; }
        public List<DesignResultMessage> AllDesigns { get; set; }

        public IEnumerable<SelectListItem> DesignsListItems
        {
            get
            {
                return AllDesigns.Select(d => new SelectListItem() { Text = d.DisplayName, Value = d.Id.ToString() });
            }
        }
    }
}