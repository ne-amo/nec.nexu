using System;
using System.Collections.Generic;
using System.Linq;

namespace nec.nexu.Models
{
    public class CustomBackground : LocalizeDisplayName
    {
        public virtual string DisplayName { get; set; }

        public virtual string FilePath { get; set; }

        public virtual string ThumbnailPath { get; set; }

        public virtual int? SortOrder { get; set; }

        //public virtual int ProductTemplateId { get; set; }

        //public virtual int HierarchyId { get; set; }

        public virtual List<ProductTemplate> AllowedTemplates { get; set; }

        public virtual List<Hierarchy> AllowedHierarchies { get; set; }


        public virtual List<BackgroundDownloadImage> DownloadImages { get; set; }

        //public virtual Hierarchy Hierarchy { get; set; }

        public void Build(CustomBackground customBackground)
        {
            this.Active = true;
            this.DisplayName = customBackground.DisplayName;
            this.FilePath = customBackground.FilePath;
            //this.HierarchyId = customBackground.HierarchyId;
            //this.ProductTemplateId = customBackground.ProductTemplateId;
            this.ThumbnailPath = customBackground.ThumbnailPath;

            if (this.DownloadImages != null && customBackground.DownloadImages != null)
            {
                if (this.DownloadImages == null)
                {
                    this.DownloadImages = new List<BackgroundDownloadImage>();
                }
                foreach (BackgroundDownloadImage a in customBackground.DownloadImages.Where(x => !this.DownloadImages.Select(e => e.Id).Contains(x.Id)))
                {
                    this.DownloadImages.Add(a);
                }
                foreach (BackgroundDownloadImage a in this.DownloadImages.Where(x => !customBackground.DownloadImages.Select(e => e.Id).Contains(x.Id)))
                {
                    this.DownloadImages.Remove(a);
                }
            }


            if (this.AllowedHierarchies != null && customBackground.AllowedHierarchies != null)
            {
                if (this.AllowedHierarchies == null)
                {
                    this.AllowedHierarchies = new List<Hierarchy>();
                }
                foreach (Hierarchy a in customBackground.AllowedHierarchies.Where(x => !this.AllowedHierarchies.Select(e => e.Id).Contains(x.Id)))
                {
                    this.AllowedHierarchies.Add(a);
                }
                foreach (Hierarchy a in this.AllowedHierarchies.Where(x => !customBackground.AllowedHierarchies.Select(e => e.Id).Contains(x.Id)))
                {
                    this.AllowedHierarchies.Remove(a);
                }
            }



            if (this.AllowedTemplates != null && customBackground.AllowedTemplates != null)
            {
                if (this.AllowedTemplates == null)
                {
                    this.AllowedTemplates = new List<ProductTemplate>();
                }
                foreach (ProductTemplate a in customBackground.AllowedTemplates.Where(x => !this.AllowedTemplates.Select(e => e.Id).Contains(x.Id)))
                {
                    this.AllowedTemplates.Add(a);
                }
                foreach (ProductTemplate a in this.AllowedTemplates.Where(x => !customBackground.AllowedTemplates.Select(e => e.Id).Contains(x.Id)))
                {
                    this.AllowedTemplates.Remove(a);
                }
            }




        }
    }
}