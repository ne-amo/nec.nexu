using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class Design : nec.nexu.JsonModels.EntityBase
    {
	    public virtual string DisplayName { get; set; }
	    public virtual string Description { get; set; }
	    public virtual string TapeNumber { get; set; }
	    public virtual int HierarchyId { get; set; }
	    public virtual int SortOrder { get; set; }
	    public virtual int HeightPixel { get; set; }
	    public virtual int WidthPixel { get; set; }
	    //public virtual ImageFile BaseImage { get; set; }
	    public virtual ImageFile OfficialImage { get; set; }
	    public virtual DesignColorway DefaultColorway { get; set; }
        [Newtonsoft.Json.JsonIgnore]
	    public virtual DesignColorway OfficialColorway { get; set; }

        //[Newtonsoft.Json.JsonIgnore]
	    //public virtual List<DesignLocation> Locations { get; set; }

        public virtual int[] LocationIds { get; set; }
    }
}