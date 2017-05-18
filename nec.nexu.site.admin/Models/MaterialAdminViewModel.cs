using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;

namespace nec.nexu.site.admin.Models
{
    public class MaterialAdminViewModel
    {
        public MaterialAdminViewModel()
        {
            this.SelectedDesignColorIds = new List<int>();
        }

        public Material Material { get; set; }
        public List<FabricContentType> AllFabricContentTypes { get; set; }
        public List<Pattern> AllPatterns { get; set; }
        public List<DesignColor> AllDesignColors { get; set; }
        public BlendMode BlendMode { get; set; }
        public List<int> SelectedDesignColorIds { get; set; }

        public IEnumerable<SelectListItem> FabricContentListItems
        {
            get
            {
                return AllFabricContentTypes.Select(fc => new SelectListItem() { Text = fc.DisplayName, Value = fc.Id.ToString() });
            }
        }

        public IEnumerable<SelectListItem> PatternListItems
        {
            get
            {
                return AllPatterns.Select(p => new SelectListItem() { Text = p.Id.ToString(), Value = p.Id.ToString() });
            }
        }
    }
}