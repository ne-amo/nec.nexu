using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
public class DesignLayer : LocalizeDisplayName //EntityBase<int>
{
	public virtual string DisplayName { get; set; }
	public virtual string Code { get; set; }
	public virtual int SortOrder { get; set; }
	public virtual bool Recolorable { get; set; }
	public virtual DesignColor Color { get; set; }
	public virtual ImageFile LayerFile { get; set; }
	/// <summary>
	/// The ID of the image data entity. We don't want the full byte array return in the JSON object
	/// </summary>
	public virtual int ImageDataId { get; set; }
	public virtual string Rule { get; set; }
	public virtual List<LayerRule> Rules { get; set; }
}
}