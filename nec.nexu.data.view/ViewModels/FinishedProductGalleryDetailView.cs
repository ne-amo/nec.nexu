using System;
using System.Collections.Generic;
using nec.nexu.data.security.ComplexTypes;
using nec.nexu.Models;

namespace nec.nexu.ViewModels
{
    public class FinishedProductGalleryDetailView
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string DisplayName { get; set; }
        public string CreatorName { get; set; }
        public string CreatorCountry { get; set; }
        public string CreatorProfileImagePath { get; set; }
        public int TemplateId { get; set; }
        public int HierarchyId { get; set; }
        public int TotalLikes { get; set; }
        public int TotalViews { get; set; }
        public int TotalInspirations { get; set; }
        public int CapRank { get; set; }
        public bool Featured { get; set; }
        public bool LimitedEdition { get; set; }
        public DateTime CreationDate { get; set; }
        public string ThumbnailPath { get; set; }
        public string DetailPath { get; set; }
        public int Purchases { get; set; }
        public string PriceDescription { get; set; }
        public PersistableStringCollection Keywords { get; set; }
        public List<GalleryMaterialView> Materials { get; set; }
        public List<FinishedProductImage> AllImages { get; set; }
    }
}
