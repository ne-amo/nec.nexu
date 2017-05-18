using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class FinishedProduct : nec.nexu.JsonModels.EntityBase
    {
	    public virtual string DisplayName { get; set; }
        
        public virtual int? MaterialId { get; set; }
        
        public virtual Guid Guid { get; set; }

        public virtual SerializedUser Creator { get; set; }
	    
        public virtual DateTime DateCreated { get; set; }
	    
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Description of price.
        /// </summary>
        public virtual string PriceDescription { get; set; }
	    
       
        public virtual ProductTemplate Template { get; set; }

        public virtual Hierarchy Hierarchy { get; set; }
	    
        public virtual List<FinishedRegion> Regions { get; set; }
	    
        public virtual List<FinishedDesign> Designs { get; set; }

        public virtual CustomBackground BackgroundImage { get; set; }
        
        public virtual List<FinishedProductImage> Images { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<UserProductView> UserViews { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<UserProductLike> UserLikes { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<Inspiration> Inspirations { get; set; }

        public virtual List<String> Keywords { get; set; } 

        public virtual bool Published { get; set; }

        public virtual bool Public { get; set; }

        public virtual bool Featured { get; set; }

        public virtual string CountryCode { get; set; }
    }
}