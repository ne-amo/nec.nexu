using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
	public class Material : ColorBase
	{
	    public Material()
	    {
	        this.MatchingDesignColors = new List<DesignColor>();
            //this.FabricContent = new FabricContentType();
            //this.Pattern = new Pattern();
	    }
		public virtual FabricContentType FabricContent { get; set; }
		public virtual List<DesignColor> MatchingDesignColors { get; set; }
		//public virtual Stitch MatchingEyelet { get; set; }
		//public virtual Stitch MatchingStitch { get; set; }
		public virtual ImageFile TrueView { get; set; }
		public virtual Pattern Pattern { get; set; }

        public virtual List<ColorList> ColorLists { get; set; }
        public virtual List<ColorContainer> ColorContainers { get; set; }

	    public void Build(Material material)
	    {
	        FabricContent = material.FabricContent;
	        MatchingDesignColors = material.MatchingDesignColors;
	        TrueView = material.TrueView;
	        Pattern = material.Pattern;
	        ColorLists = material.ColorLists;
	        ColorContainers = material.ColorContainers;
	        Hue = material.Hue;
	        Brightness = material.Brightness;
	        Saturation = material.Saturation;
	        Hex = material.Hex;
	        DisplayName = material.DisplayName;
	        Opacity = material.Opacity;
	        Pms = material.Pms;
	        TextColor = material.TextColor;
	        SortOrder = material.SortOrder;
	        BlendMode = material.BlendMode;
	    }
	}
}
