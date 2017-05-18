using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{

	public class ImageData : EntityBase<int>
	{
		/// <summary>
		/// Unmapped property. Used by service when creating template data collection.
		/// </summary>
		public virtual string Code { get; set; }
		public virtual byte[] Data { get; set; }
	}

}