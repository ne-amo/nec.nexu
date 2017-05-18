using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class DesignLocation : nec.nexu.JsonModels.EntityBase
    {
        [Newtonsoft.Json.JsonIgnore]
        public virtual ProductTemplate Template { get; set; }
        public virtual int TemplateViewId { get; set; }
        public virtual string Description { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Code { get; set; }
        public virtual int? SortOrder { get; set; }
        public int BackgroundLeftRegionId { get; set; }
        public int BackgroundRightRegionId { get; set; }
        public bool Removable { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<Design> Designs { get; set; }

        public bool Required { get; set; }
        public bool CanChoose { get; set; }


        /// <summary>
        /// If it's a primary location, it will determine the hierarchy of the cap
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// This is used to link different design locations. IE. Front/Flawless.
        /// </summary>
        public int[] SharedLocations { get; set; }
    }
}
