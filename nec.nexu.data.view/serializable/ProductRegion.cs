using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
public class ProductRegion : nec.nexu.JsonModels.EntityBase
{
    public virtual string DisplayName { get; set; }
    /// <summary>
    /// The string to identify this region in the SVG
    /// </summary>
    public virtual string Code { get; set; }
    public virtual bool EnableClick { get; set; }

    /// <summary>
    /// If "matching" is enabled, this is the ID of the region from which the matching color is pulled.
    /// </summary>
    public virtual int? MatchingRegion { get; set; }
    /// <summary>
    /// Whether or not the Fabric Content description should be shown when showing a summary of this material.
    /// </summary>
    public virtual bool? ShowFabricContent { get; set; }

    /// <summary>
    /// This flag indicates to the configurator whether or not it should be rendered as SVG vector graphic.
    /// </summary>
    public virtual bool CanHavePattern { get; set; }
    /// <summary>
    /// The region to pull material from when "match" is chosen 
    /// </summary>
    //public virtual ProductRegion MatchingRegion { get; set; }
	//public virtual ImageFile LayerFile { get; set; }
}

}