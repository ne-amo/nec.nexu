using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class Design : LocalizeDisplayName
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
	public virtual DesignColorway OfficialColorway { get; set; }
	public virtual List<DesignLocation> Locations { get; set; }

    //public virtual ICollection<DesignColorway> AllColorways { get; set; }
}
}