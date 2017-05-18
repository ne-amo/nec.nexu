using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class ColorContainer : EntityBase<int>
    {
        public virtual string ContainerName { get; set; }
        public virtual List<Material> Colors { get; set; }
        public virtual List<FinishedProduct> Products { get; set; }

        #region Localization
        public virtual string LocalizedContainerNameXml
        {
            get { return LocalizedContainerName.Value; }
            set { LocalizedContainerName.Value = value; }
        }
        private MultilingualString _LocalizedContainerName;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        [TranslationFor("ContainerName")]
        public virtual MultilingualString LocalizedContainerName
        {
            get { return _LocalizedContainerName ?? (_LocalizedContainerName = new MultilingualString()); }
            set { _LocalizedContainerName = value; }
        }
        #endregion
    }
}
