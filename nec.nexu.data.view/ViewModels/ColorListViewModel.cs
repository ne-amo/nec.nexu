using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace nec.nexu.ViewModels
{
	public class ColorListViewModel
	{
		public int Id { get; set; }

        public bool Active { get; set; }

		public string DisplayName { get; set; }

		public int MaterialCount { get; set; }

		public List<MaterialViewModel> Colors { get; set; }
	}
}