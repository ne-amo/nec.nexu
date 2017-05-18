using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nec.nexu.Models
{
    public abstract class LocalizeDisplayName : EntityBase<int> 
    {

        public LocalizeDisplayName()
        {
            this.LocalizedDisplayName = new MultilingualString();
        }

        public virtual string LocalizedDisplayNameXml
        {
            get { return LocalizedDisplayName.Value; }
            set { LocalizedDisplayName.Value = value; }
        }
        private MultilingualString _LocalizedDisplayName;
        [NotMapped]
        [TranslationFor("DisplayName")]
        public virtual MultilingualString LocalizedDisplayName
        {
            get { return _LocalizedDisplayName ?? (_LocalizedDisplayName = new MultilingualString()); }
            set { _LocalizedDisplayName = value; }
        }
    }
}
