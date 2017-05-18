using System;
using System.Collections.Generic;
using nec.nexu.model.Models.ComplexTypes;

namespace nec.nexu.Models
{
    public class ConfiguratorDirectLink : EntityBase<int>
    {
        public ConfiguratorDirectLink()
        {
            this.Guid = Guid.NewGuid();
            this.DesignLocations = new PersistableIntCollection();
            this.ProductRegionGroups = new PersistableIntCollection();
            this.CreateDate = DateTime.Now;
            this.Materials = new List<Material>();
            this.Designs = new List<Design>();
        }

        public virtual Guid Guid { get; set; } // This should be how the links are refered in the application
        //public virtual ApplicationUser Creator { get; set; }

        public virtual DateTime CreateDate { get; set; }
        
        public virtual string DisplayName { get; set; }
        
        public virtual string Description { get; set; }
        
        public virtual ProductTemplate Template { get; set; }
        public virtual int? TemplateId { get; set; }
        
        public virtual FinishedProduct CompletedProduct { get; set; }
        public virtual int? CompletedProductId { get; set; }
        
        public virtual ColorList ColorList { get; set; }
        public virtual int? ColorListId { get; set; }
        
        public virtual PersistableIntCollection ProductRegionGroups { get; set; }
        
        public virtual List<Material> Materials { get; set; }

        public virtual List<Design> Designs { get; set; }
        
        public virtual Hierarchy DesignHierarchy { get; set; }
        public virtual int? DesignHierarchyId { get; set; }
        
        public virtual PersistableIntCollection  DesignLocations { get; set; }

        public void Build(ConfiguratorDirectLink cdl)
        {
            this.Guid = cdl.Guid;
            this.DisplayName = cdl.DisplayName;
            this.Description = cdl.Description;
            this.TemplateId = cdl.TemplateId;
            this.CompletedProductId = cdl.CompletedProductId;
            this.ColorListId = cdl.ColorListId;
            this.ProductRegionGroups = cdl.ProductRegionGroups;
            this.DesignHierarchyId = cdl.DesignHierarchyId;
            this.DesignLocations = cdl.DesignLocations;

        }
    }
}
