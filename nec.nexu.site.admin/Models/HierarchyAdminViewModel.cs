using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;

namespace nec.nexu.site.admin.Models
{
    public class HierarchyAdminViewModel
    {
        public Hierarchy Hierarchy { get; set; }
        public List<Hierarchy> AllHierarchies { get; set; }

        public IEnumerable<SelectListItem> HierarchyListItems
        {
            get
            {
                return AllHierarchies.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = string.Format("{0}{1}{2}",
                        x.DisplayName,
                        x.ParentHierarchy == null ? " (No Parent)" : " (" + x.ParentHierarchy.DisplayName + ")",
                        x.Active.Value ? string.Empty : " (Inactive)")
                }).OrderBy(x=>x.Text);
            }
        } 
    }
}