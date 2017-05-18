using System;
using System.Collections.Generic;


namespace nec.nexu.JsonModels
{
    public class PatternViewImage : nec.nexu.JsonModels.EntityBase
    {
        //public virtual Pattern Pattern { get; set; }
        //public virtual int PatternId { get; set; }
        public virtual string ImagePath { get; set; }
        public virtual int TemplateViewId { get; set; }
    }
}
