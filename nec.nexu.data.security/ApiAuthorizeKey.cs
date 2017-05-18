using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace nec.nexu.Models
{
	public class ApiAuthorizeKey
	{
		[Key]
		public string IPAddress { get; set; }
		[Key]
		public string ApplicationName { get; set; }
		[Key]
		public string Key { get; set; }
	}
}