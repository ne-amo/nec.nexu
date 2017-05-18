using System;
using System.Collections.Generic;
using System.Drawing;

namespace nec.nexu.JsonModels
{

	public class TemplateImage : nec.nexu.JsonModels.EntityBase
	{
        public virtual string Code { get; set; }
        public virtual float xPos { get; set; }
        public virtual float yPos { get; set; }
        public virtual float Width { get; set; }
        public virtual float Height { get; set; }
        /// <summary>
        ///  Unmapped property. Used only for communication between the parser and service when setting up a template.
        /// </summary>
        public virtual string FilePath { get; set; }
        /// <summary>
        /// The ID of the image data entity. We don't want the full byte array return in the JSON object
        /// </summary>
		public virtual int ImageDataId { get; set; }
	}

}