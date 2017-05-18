using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nec.nexu.Models;

namespace nec.nexu.ViewModels
{
	public class MaterialViewModel
	{
        public virtual int Id { get; set; }
		public virtual nec.nexu.JsonModels.FabricContentType FabricContent { get; set; }
        public virtual nec.nexu.JsonModels.ImageFile TrueView { get; set; }
        //[Newtonsoft.Json.JsonIgnore]
		public virtual nec.nexu.JsonModels.Pattern Pattern { get; set; }
        public virtual string Hex { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual double Opacity { get; set; }
        public virtual int? SortOrder { get; set; }
        public virtual BlendMode BlendMode { get; set; }
        public virtual string TextColor { get; set; }
        public virtual List<int> MatchingDesignColorIds { get; set; }
	}
}