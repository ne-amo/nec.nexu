using System;
using System.Collections.Generic;
using nec.nexu.data.security.ComplexTypes;
using System.Linq;

namespace nec.nexu.Models
{
    public class FinishedProduct : EntityBase<int>
    {
        public FinishedProduct()
        {
            //this.DateCreated = DateTime.Now;
            this.DateModified = DateTime.Now;
            this.LastInteractedWith = DateTime.Now;
            this.Keywords = new PersistableStringCollection();
        }

        public virtual string DisplayName { get; set; }

        public virtual int? MaterialId { get; set; }

        public virtual Guid Guid { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateModified { get; set; }

        public virtual ProductTemplate Template { get; set; }

        public virtual Hierarchy Hierarchy { get; set; }

        public virtual List<FinishedRegion> Regions { get; set; }

        public virtual List<FinishedDesign> Designs { get; set; }

        public virtual CustomBackground BackgroundImage { get; set; }

        public virtual int? ImageBucket { get; set; }

        public virtual List<FinishedProductImage> Images { get; set; }

        public virtual List<UserProductView> UserViews { get; set; }

        public virtual List<UserProductLike> UserLikes { get; set; }

        public virtual List<Inspiration> Inspirations { get; set; }

        public virtual PersistableStringCollection Keywords { get; set; }

        public virtual bool Published { get; set; }

        public virtual bool Public { get; set; }

        public virtual bool Featured { get; set; }

        public virtual int Purchases { get; set; }

        // traversal object unneeded currently
        //public virtual FinishedProduct InspiredByFinishedProduct { get; set; }

        public virtual int? InspiredByFinishedProductId { get; set; }

        public virtual DateTime LastInteractedWith { get; set; }


        public virtual string CountryCode { get; set; }

        public void Build(FinishedProduct finishedProduct)
        {
            this.BackgroundImage = finishedProduct.BackgroundImage;
            this.Creator = finishedProduct.Creator;
            //this.Designs = finishedProduct.Designs;
            this.DisplayName = finishedProduct.DisplayName;
            this.Featured = finishedProduct.Featured;
            //this.Hierarchy = finishedProduct.Hierarchy;
            this.Images = finishedProduct.Images;
            this.InspiredByFinishedProductId = finishedProduct.InspiredByFinishedProductId;
            this.Keywords = finishedProduct.Keywords;
            this.MaterialId = finishedProduct.MaterialId;
            this.Public = finishedProduct.Public;
            this.Published = finishedProduct.Published;
            //this.Regions = finishedProduct.Regions;
            //this.Template = finishedProduct.Template;
            if (this.Regions != null && finishedProduct.Regions != null)
            {
                if (this.Regions == null)
                {
                    this.Regions = new List<FinishedRegion>();
                }
                foreach (FinishedRegion a in this.Regions)
                {
                    FinishedRegion update = finishedProduct.Regions.FirstOrDefault(x=>x.Region.Id == a.Region.Id);
                    if (update != null)
                    {
                        a.Material = update.Material;
                    }
                }
            }
            if (this.Designs != null && finishedProduct.Designs != null)
            {
                if (this.Designs == null)
                {
                    this.Designs = new List<FinishedDesign>();
                }
                // Update existing Logo positions
                foreach (FinishedDesign a in this.Designs)
                {
                    FinishedDesign update = finishedProduct.Designs.FirstOrDefault(x => x.Location.Id == a.Location.Id);
                    if (update != null)
                    {
                        a.DesignColorway = update.DesignColorway;
                    }
                }
                foreach (FinishedDesign a in finishedProduct.Designs.Where(x => !this.Designs.Select(e => e.Location.Id).Contains(x.Location.Id)))
                {
                    this.Designs.Add(a);
                }
                // Remove designs that were deleted
                foreach (FinishedDesign a in this.Designs.Where(x => !finishedProduct.Designs.Select(e => e.Location.Id).Contains(x.Location.Id)))
                {
                    this.Designs.Remove(a);
                }
            }



        }

    }
}