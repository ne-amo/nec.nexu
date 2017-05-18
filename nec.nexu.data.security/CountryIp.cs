using System;

namespace nec.nexu.data.security
{
    public class CountryIp
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long INET_ATON { get; set; }
        public string CountryCode { get; set; }
    }
}
