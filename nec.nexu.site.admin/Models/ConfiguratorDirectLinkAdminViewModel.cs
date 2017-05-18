using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;
using nec.nexu.ViewModels;

namespace nec.nexu.site.admin.Models
{
    public class ConfiguratorDirectLinkAdminViewModel
    {
        public ConfiguratorDirectLinkAdminViewModel()
        {
            this.SelectedDesignIds = new List<int>();
            this.SelectedProductRegionGroups = new List<int>();
            this.SelectedMaterialIds = new List<int>();
            this.SelectedDesignLocationIds = new List<int>();
        }

        public ConfiguratorDirectLink Configurator { get; set; }

        public List<ProductTemplate> AllProductTemplates { get; set; }

        public List<FinishedProductGalleryDetailView> AllFinishedProducts { get; set; }

        public List<ColorList> AllColorLists { get; set; }

        public List<Material> AllMaterials { get; set; }

        public List<DesignResultMessage> AllDesigns { get; set; }

        public List<Hierarchy> AllHierarchies { get; set; }

        public List<ProductRegionGroup> AllRegionGroups { get; set; }

        public List<DesignLocation> AllDesignLocations { get; set; }

        public List<int> SelectedDesignLocationIds { get; set; } 

        public List<int> SelectedProductRegionGroups { get; set; } 

        public List<int> SelectedMaterialIds { get; set; }

        public List<int> SelectedDesignIds { get; set; }

        public IEnumerable<SelectListItem> HierarchyListItems
        {
            get
            {
                return AllHierarchies.Select(h => new SelectListItem() { Text = h.DisplayName, Value = h.Id.ToString() });
            }
        }

        public IEnumerable<SelectListItem> FinishedProductsListItems
        {
            get
            {
                return AllFinishedProducts.Select(h => new SelectListItem() { Text = h.DisplayName, Value = h.Id.ToString() });
            }
        }

        public IEnumerable<SelectListItem> ProductTemplateListItems
        {
            get
            {
                return AllProductTemplates.Select(h => new SelectListItem() { Text = h.DisplayName, Value = h.Id.ToString() });
            }
        }

        public IEnumerable<SelectListItem> ColorListListItems
        {
            get
            {
                return AllColorLists.Select(h => new SelectListItem() { Text = h.DisplayName, Value = h.Id.ToString() });
            }
        } 

    }
}