using System;
using System.Collections.Generic;
using nec.nexu.model.Models.ComplexTypes;

namespace nec.nexu.JsonModels
{
    public class ConfiguratorDirectLink : nec.nexu.JsonModels.EntityBase
    {
        public virtual Guid Guid { get; set; } // This should be how the links are refered in the application
        //public virtual ApplicationUser Creator { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        //public virtual ProductTemplate Template { get; set; }
        public virtual int? TemplateId { get; set; }
        //public virtual FinishedProduct CompletedProduct { get; set; }
        public virtual int? CompletedProductId { get; set; }
        //public virtual ColorList ColorList { get; set; }
        public virtual int? ColorListId { get; set; }
        public virtual int[] ProductRegionGroups { get; set; }
        public virtual List<Material> Materials { get; set; }
        public virtual List<Design> Designs { get; set; }
        //public virtual Hierarchy DesignHierarchy { get; set; }
        public virtual int? DesignHierarchyId { get; set; }
        public virtual int[] DesignLocations { get; set; }
    }
}
