using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using nec.nexu.model.Models;
using nec.nexu.Models;

namespace nec.nexu.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //public class ApplicationUser : IdentityUser
    //{
    //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
    //    {
    //        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //        var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
    //        // Add custom user claims here
    //        return userIdentity;
    //    }

    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Nickname { get; set; }
    //}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{

		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
            Database.SetInitializer<ApplicationDbContext>(null);
            //Database.SetInitializer<ApplicationDbContext>(new MigrateDatabaseToLatestVersion<ApplicationDbContext,???>();
            //Database.SetInitializer<ApplicationDbContext>(new CreateDatabaseIfNotExists<ApplicationDbContext>());
            //Database.SetInitializer<ApplicationDbContext>(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
            //Database.SetInitializer<ApplicationDbContext>(new DropCreateDatabaseAlways<ApplicationDbContext>());
		}
		
		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// Configure Code First to ignore PluralizingTableName convention 
			// If you keep this convention then the generated tables will have pluralized names. 
			//modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Entity<ApiAuthorizeKey>().HasKey(t => new { t.ApplicationName, t.IPAddress, t.Key });
			modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
			modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
			modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
            modelBuilder.Configurations.Add(new TemplateView.TemplateViewConfiguration());


		    modelBuilder.Entity<Hierarchy>().HasOptional(h => h.ParentHierarchy);

            modelBuilder.Entity<nec.nexu.Models.rules.ColorwayRule>()
                .HasMany<nec.nexu.Models.rules.ColorwayRuleValue>(x=>x.Values);

		    modelBuilder.Entity<ConfiguratorDirectLink>()
		        .HasMany(d => d.Designs)
		        .WithMany()
		        .Map(x =>
		        {
		            x.MapLeftKey("ConfiguratorDirectLinkId");
		            x.MapRightKey("DesignId");
		            x.ToTable("ConfiguratorDesignMapping");

		        });
            modelBuilder.Entity<ConfiguratorDirectLink>()
                .HasMany(m => m.Materials)
                .WithMany()
                .Map(x =>
                {
                    x.MapLeftKey("ConfiguratorDirectLinkId");
                    x.MapRightKey("MaterialId");
                    x.ToTable("ConfiguratorMaterialMapping");
                });
            modelBuilder.Entity<Material>()
                .HasMany(m => m.MatchingDesignColors)
                .WithMany()
                .Map(x =>
                {
                    x.MapLeftKey("MaterialId");
                    x.MapRightKey("DesignColorId");
                    x.ToTable("MaterialDesignColorMapping");
                });
           
		}

        private void DetectExisting<T>() where T : EntityBase<int>
        {
            var changes = ChangeTracker.Entries<T>();
            if (changes != null)
            {
                System.Collections.Generic.HashSet<int> ids = new System.Collections.Generic.HashSet<int>();
                foreach (var entry in changes)
                {
                    if (entry.Property(p => p.Id).CurrentValue > 0 && entry.State == EntityState.Added)
                    {
                        if (ids.Contains(entry.Property(p => p.Id).CurrentValue))
                        {
                            entry.State = EntityState.Detached;
                        }
                        else
                        {
                            entry.State = EntityState.Unchanged;
                            ids.Add(entry.Property(p => p.Id).CurrentValue);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// The SaveChanges method is overridden in nec.nexu.Models.ApplicationDbContext
        /// Entities of type EntityBase(int) that are added with an existing Id property will not be commited to database.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {

            //this.DetectExisting<Material>();
            //this.DetectExisting<FabricContentType>();

            /*
            var materials = ChangeTracker.Entries<Material>();
            if (materials != null)
            {
                System.Collections.Generic.HashSet<int> ids = new System.Collections.Generic.HashSet<int>();
                foreach (var entry in materials)
                {
                    if (entry.Property(p => p.Id).CurrentValue > 0 && entry.State == EntityState.Added)
                    {
                        if (ids.Contains(entry.Property(p => p.Id).CurrentValue))
                        {
                            entry.State = EntityState.Detached;
                        }
                        else
                        {
                            entry.State = EntityState.Unchanged;
                            ids.Add(entry.Property(p => p.Id).CurrentValue);
                        }
                    }
                }
            }
            
            var contents = ChangeTracker.Entries<FabricContentType>();
            if (contents != null)
            {
                foreach (var entry in contents)
                {

                }
            }

            */

            /*
            var context = ((IObjectContextAdapter)this).ObjectContext;
            if (changeSet != null)
            {
                foreach (var entry in changeSet)
                {
                    if (entry.Property(p => p.Id).CurrentValue > 0 && entry.State == EntityState.Added)
                    {
                        try
                        {
                            entity.State = EntityState.Unchanged;
                        }
                        catch { }
                    }
                }
            }
            */




            return base.SaveChanges();
        }

        /*
        public static void AttachToOrGet<T>(this ApplicationDbContext dbContext, string entitySetName, ref T entity) where T : EntityBase<int>
        {
            var context = ((IObjectContextAdapter)dbContext).ObjectContext;
            ObjectStateEntry entry;
            // Track whether we need to perform an attach
            bool attach = false;
            if (
                context.ObjectStateManager.TryGetObjectStateEntry
                    (
                        context.CreateEntityKey(entitySetName, entity),
                        out entry
                    )
                )
            {
                // Re-attach if necessary
                attach = entry.State == EntityState.Detached;
                // Get the discovered entity to the ref
                entity = (T)entry.Entity;
            }
            else
            {
                // Attach for the first time
                attach = true;
            }
            if (attach)
                context.AttachTo(entitySetName, entity);
        }
        */


        public DbSet<AppClient> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


		public DbSet<ColorList> ColorLists { get; set; }

		public DbSet<Design> Designs { get; set; }

		public DbSet<ConfiguratorDirectLink> ConfiguratorDirectLinks { get; set; }

		public DbSet<ProductTemplate> ProductTemplates { get; set; }

		public DbSet<FinishedProduct> FinishedProducts { get; set; }

		public DbSet<Hierarchy> Hierarchies { get; set; }

		public DbSet<TemplateView> TemplateViews { get; set; }

		public DbSet<ApiAuthorizeKey> ApiKeys { get; set; }

        public System.Data.Entity.DbSet<nec.nexu.Models.CustomBackground> CustomBackgrounds { get; set; }

        public System.Data.Entity.DbSet<nec.nexu.Models.FaceOff> FaceOffs { get; set; }

        public System.Data.Entity.DbSet<nec.nexu.Models.UserColorList> UserColorLists { get; set; }

        public DbSet<DesignColor> DesignColors { get; set; }
        public DbSet<FabricContentType> FabricContentTypes { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<ImageData> ImageDatas { get; set; }
        public DbSet<TemplateImage> TemplateImages { get; set; }
        public DbSet<DesignLayer> DesignLayers { get; set; }
        public DbSet<DesignLocation> DesignLocations { get; set; }
        public DbSet<DesignColorway> DesignColorways { get; set; }
        public DbSet<ColorContainer> ColorContainers { get; set; }
        public DbSet<ColorContainerSearchProxy> ColorContainerSearchProxies { get; set; }
        public DbSet<ProductRegionGroup> ProductRegionGroups { get; set; }
        public DbSet<Pattern> Patterns { get; set; }
        public DbSet<DesignRenderSetting> DesignRenderSettings { get; set; }
        public DbSet<DownloadFormat> DownloadFormats { get; set; }
        public DbSet<FinishedProductKeywordProxy> FinishedProductKeywordProxies { get; set; }
        //public DbSet<LocalizedTest> LocalizationTests { get; set; }

        public DbSet<nec.nexu.Models.rules.RequiredLogoRule> RulesRequiredLogos { get; set; }
        public DbSet<nec.nexu.Models.rules.DesignColorRule> RulesDesignColor { get; set; }
        public DbSet<nec.nexu.Models.rules.CountryDesignRule> RulesDesignCountryExclusion { get; set; }
        public DbSet<nec.nexu.Models.rules.ColorwayRule> RulesColorways { get; set; }
        public DbSet<nec.nexu.Models.rules.ColorDifferentiationRule> RulesColorDifferentiation { get; set; }
        public DbSet<nec.nexu.Models.rules.CountryProductExclusion> RulesCountryProductExclusions { get; set; }
        public DbSet<nec.nexu.Models.rules.CountryHierachyExclusion> CountryHierachyExclusions { get; set; }
        public DbSet<nec.nexu.data.security.CountryIp> CountryIps { get; set; }
        public DbSet<Country> Countrys { get; set; }

        public Country GetCountryByIp(string ip)
        {
            // INET_ATON Conversion
            int i;
            double netaddress = 0;
            if (ip.ToString() == "")
            {
                return null;
            }

            string[] dec = ip.ToString().Split('.');
            for (i = dec.Length - 1; i >= 0; i--)
            {
                netaddress += ((int.Parse(dec[i]) % 256) * System.Math.Pow(256, (3 - i)));
            }

            //Lookup
            var countryIp = this.CountryIps.Where(x => x.INET_ATON < netaddress).OrderByDescending(x => x.INET_ATON).First();
            if(countryIp == null)
                return null;
            return this.Countrys.Where(x => x.CountryCode == countryIp.CountryCode).FirstOrDefault();
        }
        public int GetCountryIdByIp(string ip)
        {
            Country c = this.GetCountryByIp(ip);
            return (c == null) ? 0 : c.Id;
        }

        public ImageDataCache GetDesignImageData(int imagedataid)
        {
            var par = new System.Data.SqlClient.SqlParameter("@ImageDataId", imagedataid);
            return this.Database.SqlQuery<ImageDataCache>("GetDesignImageData @ImageDataId", par).FirstOrDefault();
            //return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<ImageDataCache>("GetDesignImageData", par);
        }
        public ImageDataCache GetTemplateImageData(int imagedataid)
        {
            var par = new System.Data.SqlClient.SqlParameter("@ImageDataId", imagedataid);
            return this.Database.SqlQuery<ImageDataCache>("GetTemplateImageData @ImageDataId", par).FirstOrDefault();
            //var par = new ObjectParameter("ImageDataId", imagedataid);
            //return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<ImageDataCache>("GetTemplateImageData", par);
        }

        public int GetNewMaterialId()
        {
            return (int)this.Database.SqlQuery<decimal>("CreateMaterialId").FirstOrDefault();
        }

        public DbSet<nec.nexu.Models.roal.DesignColorToRoal>            Roal_DesignColorMap { get; set; }
        public DbSet<nec.nexu.Models.roal.DesignToRoal>                 Roal_DesignMap { get; set; }
        public DbSet<nec.nexu.Models.roal.HierarchyToRoal>              Roal_HierarchyMap { get; set; }
        public DbSet<nec.nexu.Models.roal.MaterialToRoal>               Roal_MaterialMap { get; set; }
        public DbSet<nec.nexu.Models.roal.ProductRegionToRoal>          Roal_ProductRegionMap { get; set; }
        public DbSet<nec.nexu.Models.roal.TemplateToRoal>               Roal_TemplateMap { get; set; }
        public DbSet<nec.nexu.Models.roal.DesignLocationToIllustration> Roal_DesignLocationMap { get; set; }

	}
}