using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
	// Models returned by AccountController actions.

	public class ExternalLoginViewModel
	{
		public string Name { get; set; }

		public string Url { get; set; }

		public string State { get; set; }
	}

	public class ManageInfoViewModel
	{
		public string LocalLoginProvider { get; set; }

		public string Email { get; set; }

		public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

		public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
	}

	public class UserInfoViewModel
	{
        public string Id { get; set; }

		public string Email { get; set; }

		public bool HasRegistered { get; set; }

		public string LoginProvider { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Nickname { get; set; }

        public string CountryName { get; set; }

		public List<FinishedProduct> FinishedProducts { get; set; }
		public int FinishedProductsCount { get; set; }

        public string ProfileImageUrl { get; set; }

        public bool ReceiveEmail { get; set; }

        //public List<string> FavoriteTeams { get; set; } 
	}

	public class UserLoginInfoViewModel
	{
		public string LoginProvider { get; set; }

		public string ProviderKey { get; set; }
	}
}
