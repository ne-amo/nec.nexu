using System;
using System.Collections.Generic;
using nec.nexu.Models.roal;

namespace nec.nexu.site.admin.Models
{

    public class RoalMapViewModel<T>
    {
        public T Map { get; set; }
        public string RoalDisplayName { get; set; }
        public string NexuDisplayName { get; set; }
        public string RoalDisplayName2 { get; set; }

        public RoalMapViewModel() { }

        public RoalMapViewModel(T map, string roal, string nexu)
        {
            this.Map = map;
            this.RoalDisplayName = roal;
            this.NexuDisplayName = nexu;
        }

        public RoalMapViewModel(T map, string roal, string nexu, string roal2)
        {
            this.Map = map;
            this.RoalDisplayName = roal;
            this.NexuDisplayName = nexu;
            this.RoalDisplayName2 = roal2;
        }
    }

}