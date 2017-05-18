using System.Web.Mvc;
using nec.nexu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nec.nexu.site.admin.Models
{
    public class RequiredLogoRuleAdminViewModel
    {
        public RequiredLogoRuleAdminViewModel()
        {
            this.AllDesigns = new List<Design>();
            this.AllDesignLocations = new List<DesignLocation>();
        }

        public int Id { get; set; }
        public bool Active { get; set; }
        public string DisplayName { get; set; }
        public string UserMessage { get; set; }
        public DesignRuleType Type = DesignRuleType.REQUIREDLOGO;
        public int SourceLocationId { get; set; }
        public int TargetDesignId { get; set; }
        public int TargetLocationId { get; set; }
        public int DesignId { get; set; }

        public List<Design> AllDesigns { get; set; }
        public List<DesignLocation> AllDesignLocations { get; set; }

        public IEnumerable<SelectListItem> DesignLocationsListItems
        {
            get
            {
                return AllDesignLocations.Select(fc => new SelectListItem() { Text = fc.DisplayName, Value = fc.Id.ToString() });
            }
        }

        public IEnumerable<SelectListItem> DesignsListItems
        {
            get
            {
                return AllDesigns.Select(p => new SelectListItem() { Text = p.DisplayName, Value = p.Id.ToString() });
            }
        }
    }

    public enum DesignRuleType
    {
        NONE,
        REQUIREDLOGO,
        COLORESTRICTION,
        COLORWAY,
        DIFFERENTIATION,
        COUNTRY
    }
}