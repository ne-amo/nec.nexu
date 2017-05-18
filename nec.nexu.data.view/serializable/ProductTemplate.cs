using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{

    //public class ProductTemplate : nec.nexu.JsonModels.EntityBase
    //{
    //    public virtual string DisplayName { get; set; }
    //    public virtual int SortOrderLogo { get; set; }
    //    public virtual IList<DesignLocation> DesignLocations { get; set; }
    //    //public virtual IList<ProductRegion> Regions { get; set; }
    //    public virtual IList<ProductRegionGroup> RegionGroups { get; set; }
    //    public virtual Material DefaultMaterial { get; set; }
    //    public virtual IList<ImageFile> Images { get; set; }
    //}
    /// <summary>
    /// This is the data object used to build a template in the configurator.
    /// </summary>
    public class ProductTemplate : nec.nexu.JsonModels.EntityBase
    {
        public virtual Guid Guid { get; set; }
        /// <summary>
        /// Display name for Admin
        /// </summary>
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// Where the "Logos" should be sorted in the UI.
        /// </summary>
        public virtual int SortOrderLogo { get; set; }
        /// <summary>
        /// Description of price.
        /// </summary>
        public virtual string PriceDescription { get; set; }


        /// <summary>
        /// Style code for placing orders
        /// </summary>
        public virtual string StyleCode { get; set; }

        /// <summary>
        /// Locations for logo placement.
        /// </summary>
        public virtual ICollection<DesignLocation> DesignLocations { get; set; }
        /// <summary>
        /// Groupings of the different regions that go into the template. This is used toi sort the UI between Panels/Eyelets etc...
        /// </summary>
        public virtual ICollection<ProductRegionGroup> RegionGroups { get; set; }
        /// <summary>
        /// Default material to drop in for Regions.
        /// </summary>
        public virtual Material DefaultMaterial { get; set; }
        /// <summary>
        /// Views to be used for graphical representation.
        /// </summary>
        public virtual ICollection<TemplateView> Views { get; set; }
        /// <summary>
        /// Reference to the Renders used on the server side. Should probably be commented out or unmapped.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<ImageFile> Renders { get; set; }
    }

}