using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{

	public class ImageData : nec.nexu.JsonModels.EntityBase
	{
		/// <summary>
		/// Unmapped property. Used by service when creating template data collection.
		/// </summary>
		public virtual string Code { get; set; }
		public virtual byte[] Data { get; set; }
	}

}