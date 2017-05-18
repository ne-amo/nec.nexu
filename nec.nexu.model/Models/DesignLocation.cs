using System;
using System.Collections.Generic;
using nec.nexu.model.Models.ComplexTypes;

namespace nec.nexu.Models
{
    public class DesignLocation : LocalizeDisplayName
    {
        public DesignLocation()
        {
            this.Active = true;
            this.CanChoose = true;
            this.Removable = true;
            this.IsPrimary = false;
            this.Designs = new List<Design>();
            this.SortOrder = 1;
        }

        public virtual ProductTemplate Template { get; set; }
       
        public virtual int TemplateId { get; set; }

        public virtual int TemplateViewId { get; set; }

        public virtual int BackgroundLeftRegionId { get; set; }
        public virtual int BackgroundRightRegionId { get; set; }
        
        //public virtual ImageFile DefaultTemplateImage { get; set; }
        
        public virtual string Description { get; set; }
        
        public virtual string DisplayName { get; set; }
        
        public virtual string Code { get; set; }
        
        public virtual int? SortOrder { get; set; }

        /// <summary>
        /// Whether or not this location can be removed by the user.
        /// </summary>
        public virtual bool Removable { get; set; }
        /// <summary>
        /// Whether the user must pick assign a logo to this location.
        /// </summary>
        public virtual bool Required { get; set; }
        /// <summary>
        /// Whether the user can pick a logo for the section. This can be used to allow the flag to be preset on a cap.
        /// </summary>
        public virtual bool CanChoose { get; set; }

        /// <summary>
        /// If it's a primary location, it will determine the hierarchy of the cap
        /// </summary>
        public virtual bool IsPrimary { get; set; }

        /// <summary>
        /// This is used to link different design locations. IE. Front/Flawless.
        /// </summary>
        public virtual PersistableIntCollection SharedLocations { get; set; }

        
        public virtual List<Design> Designs { get; set; }

        public void Build(DesignLocation location)
        {
            this.Code = location.Code;
            this.DisplayName = location.DisplayName;
            this.Description = location.Description;
            this.TemplateId = location.TemplateId;
            this.SortOrder = location.SortOrder ?? 1;
            this.TemplateViewId = location.TemplateViewId;
        }
    }
}