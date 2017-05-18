using System;
using System.Collections.Generic;
using System.Linq;
using nec.nexu.Models;

namespace nec.nexu.Models.roal
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
            public Dictionary<int, int> Settings = new Dictionary<int, int>();
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




    [System.ComponentModel.DataAnnotations.Schema.Table("ROAL_MaterialToRoal")]
    public class MaterialToRoal
    {
        public int Id { get; set; }
        public int NexuMaterialId { get; set; }
        public string FabricMaterialId { get; set; }
        public string StitchMaterialId { get; set; }
        public string DCStitchMaterialId { get; set; }
        public string BartackLeft { get; set; }
        public string BartackRight { get; set; }
    }
    [System.ComponentModel.DataAnnotations.Schema.Table("ROAL_DesignColorToRoal")]
    public class DesignColorToRoal
    {
        public int Id { get; set; }
        public int NexuDesignColorId { get; set; }
        public int RoalDesignColorId { get; set; }
    }
    [System.ComponentModel.DataAnnotations.Schema.Table("ROAL_DesignToRoal")]
    public class DesignToRoal
    {

        private char _splitChar = '|';
        private char _relationChar = '&';

        public int Id { get; set; }
        public int NexuDesignId { get; set; }
        public int RoalDesignId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column]
        public string _SerializedDictionary { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public Dictionary<int, int> RoalSequenceOrder_to_NexuLayerId
        {
            get
            {
                if (string.IsNullOrEmpty(this._SerializedDictionary))
                    return new Dictionary<int, int>();
                else
                {
                    try
                    {
                        Dictionary<int, int> d = new Dictionary<int, int>();
                        foreach (string s in this._SerializedDictionary.Split(_splitChar))
                        {
                            d.Add(int.Parse(s.Split(_relationChar)[0]), int.Parse(s.Split(_relationChar)[1]));
                        }
                        return d;
                    }
                    catch
                    {
                        return new Dictionary<int, int>();
                    }
                }
            }
            set
            {
                this._SerializedDictionary = String.Join(_splitChar.ToString(), value.Select(x => x.Key.ToString() + _relationChar + x.Value.ToString()).ToArray());
            }
        }
    }
    [System.ComponentModel.DataAnnotations.Schema.Table("ROAL_DesignLocationToIllustration")]
    public class DesignLocationToIllustration
    {
        public int Id { get; set; }
        public int NexuLocationId { get; set; }
        public int RoalIllustrationId { get; set; }
    }
    [System.ComponentModel.DataAnnotations.Schema.Table("ROAL_HierarchyToRoal")]
    public class HierarchyToRoal
    {
        public int Id { get; set; }
        public int NexuHierarchyId { get; set; }
        public string RoalBuCode { get; set; }
        public string RoalSportCode { get; set; }
        public string RoalLicenseCode { get; set; }
        public string RoalTeamCode { get; set; }
        public string RoalClassCode { get; set; }
    }
    [System.ComponentModel.DataAnnotations.Schema.Table("ROAL_TemplateToRoal")]
    public class TemplateToRoal
    {
        public int Id { get; set; }
        public int NexuTemplateId { get; set; }
        public int RoalConfigurationId { get; set; }
        public string SilhouetteCode { get; set; }
        public string GenderId { get; set; }
        public string SeasonId { get; set; }
        public string CollectionId { get; set; }
    }

    public enum ROALRegionType
    {
        REGION = 0,
        STITCH = 1,
        COMPONENT = 2,
        BARTACK_LEFT = 3,
        BARTACK_RIGHT = 4,
        VISOR_STITCH = 5
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("ROAL_ProductRegionToRoal")]
    public class ProductRegionToRoal
    {
        public int Id { get; set; }
        public int NexuProductRegionId { get; set; }
        public int RoalProductRegionId { get; set; }
        public ROALRegionType RoalType { get; set; }
    }
}
