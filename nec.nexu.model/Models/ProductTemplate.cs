using System;
using System.Collections.Generic;
using System.Linq;

namespace nec.nexu.Models
{

    //public class ProductTemplate : EntityBase<int>
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
    public class ProductTemplate : LocalizeDisplayName
    {

        public ProductTemplate()
        {
            this.ExclusiveBackgrounds = new List<CustomBackground>();
        }

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
        public virtual ICollection<ImageFile> Renders { get; set; }

        public virtual List<CustomBackground> ExclusiveBackgrounds { get; set; }


        public virtual string LocalizedPriceDescriptionXml
        {
            get { return LocalizedPriceDescription.Value; }
            set { LocalizedPriceDescription.Value = value; }
        }
        private MultilingualString _LocalizedPriceDescription;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        [TranslationFor("PriceDescription")]
        public virtual MultilingualString LocalizedPriceDescription
        {
            get { return _LocalizedPriceDescription ?? (_LocalizedPriceDescription = new MultilingualString()); }
            set { _LocalizedPriceDescription = value; }
        }


        public void Build(ProductTemplate productTemplate)
        {
            //this.DefaultMaterial = productTemplate.DefaultMaterial;
            //this.DesignLocations = productTemplate.DesignLocations;
            //this.DisplayName = productTemplate.DisplayName;
            //this.RegionGroups = productTemplate.RegionGroups;
            //this.Renders = productTemplate.Renders;
            //this.SortOrderLogo = productTemplate.SortOrderLogo;
            this.DefaultMaterial = productTemplate.DefaultMaterial;
            this.DisplayName = productTemplate.DisplayName;
            this.SortOrderLogo = productTemplate.SortOrderLogo;

            if (this.DesignLocations != null && productTemplate.DesignLocations != null)
            {
                if (this.DesignLocations == null)
                {
                    this.DesignLocations = new List<DesignLocation>();
                }
                foreach (DesignLocation a in productTemplate.DesignLocations.Where(x=> ! this.DesignLocations.Select(e=>e.Id).Contains(x.Id)))
                {
                    this.DesignLocations.Add(a);
                }
                foreach (DesignLocation a in this.DesignLocations.Where(x => !productTemplate.DesignLocations.Select(e => e.Id).Contains(x.Id)))
                {
                    this.DesignLocations.Remove(a);
                }
            }


            if (this.RegionGroups != null && productTemplate.RegionGroups != null)
            {
                if (this.RegionGroups == null)
                {
                    this.RegionGroups = new List<ProductRegionGroup>();
                }
                foreach (ProductRegionGroup a in productTemplate.RegionGroups.Where(x => !this.RegionGroups.Select(e => e.Id).Contains(x.Id)))
                {
                    this.RegionGroups.Add(a);
                }
                foreach (ProductRegionGroup a in this.RegionGroups.Where(x => !productTemplate.RegionGroups.Select(e => e.Id).Contains(x.Id)))
                {
                    this.RegionGroups.Remove(a);
                }
            }

            if (this.Renders != null && productTemplate.Renders != null)
            {
                if (this.Renders == null)
                {
                    this.Renders = new List<ImageFile>();
                }
                foreach (ImageFile a in productTemplate.Renders.Where(x => !this.Renders.Select(e => e.Id).Contains(x.Id)))
                {
                    this.Renders.Add(a);
                }
                foreach (ImageFile a in this.Renders.Where(x => !productTemplate.Renders.Select(e => e.Id).Contains(x.Id)))
                {
                    this.Renders.Remove(a);
                }
            }


            if (this.Views != null && productTemplate.Views != null)
            {
                if (this.Views == null)
                {
                    this.Views = new List<TemplateView>();
                }
                foreach (TemplateView a in productTemplate.Views.Where(x => !this.Views.Select(e => e.Id).Contains(x.Id)))
                {
                    this.Views.Add(a);
                }
                foreach (TemplateView a in this.Views.Where(x => !productTemplate.Views.Select(e => e.Id).Contains(x.Id)))
                {
                    this.Views.Remove(a);
                }
            }


            if (this.ExclusiveBackgrounds != null && productTemplate.ExclusiveBackgrounds != null)
            {
                if (this.ExclusiveBackgrounds == null)
                {
                    this.ExclusiveBackgrounds = new List<CustomBackground>();
                }
                foreach (CustomBackground a in productTemplate.ExclusiveBackgrounds.Where(x => !this.ExclusiveBackgrounds.Select(e => e.Id).Contains(x.Id)))
                {
                    this.ExclusiveBackgrounds.Add(a);
                }
                foreach (CustomBackground a in this.ExclusiveBackgrounds.Where(x => !productTemplate.ExclusiveBackgrounds.Select(e => e.Id).Contains(x.Id)))
                {
                    this.ExclusiveBackgrounds.Remove(a);
                }
            }


        }
    }

}