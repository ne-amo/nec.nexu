using System;
using System.Collections.Generic;

namespace nec.nexu.bridge
{
    public class NexuToRoalMessage
    {
        public class DesignSetting
        {
            public int DesignId { get; set; }
            public int LocationId { get; set; }
            /// <summary>
            /// Roal Sequence, Design Color Id
            /// </summary>
            public Dictionary<int, int> Settings { get; set; }
        }
        public int ConfigurationId { get; set; }
        public string SilhouetteCode { get; set; }
        public string GenderId { get; set; }
        public string SeasonId { get; set; }
        public string CollectionId { get; set; }
        public string BuCode { get; set; }
        public string SportCode { get; set; }
        public string LicenseCode { get; set; }
        public string TeamCode { get; set; }
        public string ClassCode { get; set; }
        public string CountryCode { get; set; }
        /// <summary>
        /// Key: Id of ROAL Region  Config
        /// Value: Raw Material Id pair. Value 1: Fabric Material Id(0315), Stitch Material Id(PC Thread) 
        /// </summary>
        public Dictionary<int, string> RegionMaterials = new Dictionary<int, string>();
        //public Dictionary<int, Tuple<string, string>> RegionMaterials = new Dictionary<int, Tuple<string, string>>();

        /// <summary>
        /// Key: Id of Stitch Config 
        /// Value: Raw Material Id pair. Value 1: Fabric Material Id(0315), Stitch Material Id(PC Thread) 
        /// </summary>
        public Dictionary<int, string> StitchMaterials = new Dictionary<int, string>();
        //public Dictionary<int, Tuple<string, string>> StitchMaterials = new Dictionary<int, Tuple<string, string>>();

        /// <summary>
        /// Key: Id of Component Config 
        /// Value: Raw Material Id pair. Value 1: Fabric Material Id(0315)
        /// </summary>
        public Dictionary<int, string> ComponentMaterials = new Dictionary<int, string>();

        /// <summary>
        /// Value 1: Illustration / Location Id
        /// Value 2: Design Id
        /// Value 3: ROAL Colorsequence Order Id
        /// Value 4: ROAL Color Id
        /// </summary>
        //public List<Tuple<int, int, int, int>> Designs = new List<Tuple<int, int, int, int>>();
        public List<DesignSetting> Designs = new List<DesignSetting>();
    }
}
