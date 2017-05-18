using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using nec.nexu.model.Models.ComplexTypes;
using nec.nexu.Models;
using System;

namespace nec.nexu.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser
	{
	    public ApplicationUser()
	    {
	        UserPurchases = new PersistableIntCollection();
	    }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
			// Add custom user claims here
			return userIdentity;
		}

		public string FirstName { get; set; }
		
        public string LastName { get; set; }

        public string Nickname { get; set; }

        public string ProfileImageFileName { get; set; }

        public List<int> PurchasedProducts { get; set; }

        public virtual PersistableIntCollection UserPurchases { get; set; } 
        
        public List<int> LikedProducts { get; set; }

        public Country UserCountry { get; set; }

        public bool ReceiveEmail { get; set; }

        public decimal DesignScore { get; set; }

        public bool Active { get; set; }
	}

}