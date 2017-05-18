using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using nec.nexu.Extensions;
using nec.nexu.model.Models;
using nec.nexu.Models;
using nec.nexu.ViewModels;
using nec.nexu.Models.rules;
using log4net;
using log4net.Config;
using nec.nexu.model.Models.ComplexTypes;
using ImageBlurFilter;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class FinishedProductsController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Product Retrieval and Search Methods

        // GET: api/FinishedProducts
        [HttpGet]
        public IHttpActionResult GetFinishedProducts(int pageNumber = 1, int pageSize = 25)
        {
            var products = (pageNumber > 1)
                  ? db.FinishedProducts
                  .Where(fp => fp.Active == true)
                      .OrderByDescending(fp => fp.DateCreated)
                      .Skip(pageSize * (pageNumber - 1))
                      .Take(pageSize).ToList()
                      :    db.FinishedProducts
                        .Where(fp => fp.Active == true)
                      .OrderByDescending(fp => fp.DateCreated)
                          .Take(pageSize).ToList();


            return Ok(new PagedResponse<FinishedProductGalleryDetailView>
            {
                Items = AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(
                    products),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = db.FinishedProducts.Count(fp => fp.Active == true)
            });
        }

        // GET: api/FinishedProducts/5
        [ResponseType(typeof(nec.nexu.JsonModels.FinishedProduct))]
        public IHttpActionResult GetFinishedProduct(int id)
        {
            FinishedProduct finishedProduct = db.FinishedProducts.Find(id);
            if (finishedProduct == null)
            {
                log.Error(string.Format("GET: api/FinishedProducts/:id - Resource: {0} - Not Found", id.ToString()));
                return NotFound();
            }

            return Ok(finishedProduct);
        }

        [HttpGet]
        [Route("api/finishedproducts/configurator/{id}")]
        [ResponseType(typeof(nec.nexu.JsonModels.FinishedProduct))]
        public IHttpActionResult GetFinishedProductForConfigurator(int id)
        {
            FinishedProduct finishedProduct = db.FinishedProducts.Find(id);
            List<FinishedRegion> regs = finishedProduct.Regions;
            if (finishedProduct == null)
            {
                log.Error(string.Format("GET: api/FinishedProducts/:id - Resource: {0} - Not Found", id.ToString()));
                return NotFound();
            }
            try
            {
                return Ok(AutoMapper.Mapper.Map<nec.nexu.Models.FinishedProduct, nec.nexu.JsonModels.FinishedProduct>(finishedProduct));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Total Number of products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/finishedproducts/total")]
        public IHttpActionResult GetTotalProducts()
        {
            int total = db.FinishedProducts.Count(x=>x.Active == true);
            return Ok(total);
        }




        //GET: products by user
        /// <summary>
        /// Get Finished Products by User Id
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/finishedproducts/user/{uid}")]
        public IHttpActionResult GetProductsByUser(string uid, int pageNumber = 1, int pageSize = 25)
        {
            var userProducts = (pageNumber > 1)
                 ? db.FinishedProducts
                     .Where(fp => fp.Creator.Id == uid)
                     .OrderByDescending(fp => fp.DateCreated)
                     .Skip(pageSize * (pageNumber - 1))
                     .Take(pageSize).ToList()
                     : db.FinishedProducts.Where(fp => fp.Creator.Id == uid)
                         .OrderByDescending(fp => fp.DateCreated)
                         .Take(pageSize).ToList();


            return Ok(new PagedResponse<FinishedProductGalleryDetailView>
            {
                Items = AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(
                    userProducts),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = db.FinishedProducts.Count(fp => fp.Creator.Id == uid)
            });
        }

        [HttpGet]
        [Route("api/finishedproducts/hierarchy/{hid}")]
        public IHttpActionResult GetProductsByHierarchy(int hid, int pageNumber = 1, int pageSize = 25)
        {
            var userProducts = (pageNumber > 1)
                 ? db.FinishedProducts
                     .Where(fp => fp.Hierarchy.Id == hid)
                     .OrderByDescending(fp => fp.DateCreated)
                     .Skip(pageSize * (pageNumber - 1))
                     .Take(pageSize).ToList()
                     : db.FinishedProducts.Where(fp => fp.Hierarchy.Id == hid)
                         .OrderByDescending(fp => fp.DateCreated)
                         .Take(pageSize).ToList();


            return Ok(new PagedResponse<FinishedProductGalleryDetailView>
            {
                Items = AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(
                    userProducts),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = db.FinishedProducts.Count(fp => fp.Hierarchy.Id == hid)
            });
        }

        [HttpGet]
        [Route("api/finishedproducts/user/{uid}/getpublished")]
        public IHttpActionResult GetPublishedProductsByUser(string uid, int pageNumber = 1, int pageSize = 25)
        {
            var userProducts = (pageNumber > 1)
                ? db.FinishedProducts
                    .Where(fp => fp.Creator.Id == uid && fp.Published)
                    .OrderByDescending(fp => fp.DateCreated)
                    .Skip(pageSize*(pageNumber - 1))
                    .Take(pageSize).ToList()
                    : db.FinishedProducts.Where(fp => fp.Creator.Id == uid && fp.Published)
                        .OrderByDescending(fp => fp.DateCreated)
                        .Take(pageSize).ToList();


            return Ok(new PagedResponse<FinishedProductGalleryDetailView>
            {
                Items = AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(
                    userProducts),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = db.FinishedProducts.Count(fp => fp.Creator.Id == uid)
            });
        }

        [HttpGet]
        [Route("api/finishedproducts/trending")]
        public IHttpActionResult GetTrendingProducts(DateTime interval)
        {
            var trendingProducts = db.FinishedProducts.Select(fp => fp)
                .Where(fp => fp.LastInteractedWith >= interval)
                .OrderByDescending(p => (p.UserViews.Count + (p.UserLikes.Count*20)));

            return Ok(new PagedResponse<FinishedProductGalleryDetailView>
            {
                Items = AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(
                    trendingProducts.ToList())
            });
        }

        [Authorize]
        [Route("api/finishedproducts/userdata/{uid}")]
        [ResponseType(typeof(UserProductDataViewModel))]
        public IHttpActionResult GetUserProductData(string uid)
        {
            var prods = db.FinishedProducts.Where(fp => fp.Creator.Id == uid);
            int like = Enumerable.Sum(prods, p => p.UserLikes.Count);
            int views = Enumerable.Sum(prods, p => p.UserViews.Count);
            return Ok(new UserProductDataViewModel {LikeCount = like, ViewCount = views});
        }

        [Route("api/finishedproducts/detail/{id}")]
        [ResponseType(typeof(FinishedProductGalleryDetailView))]
        public IHttpActionResult GetFinishedProductDetail(int id)
        {
            try
            {
                FinishedProduct model = db.FinishedProducts.Find(id);
                var r = AutoMapper.Mapper.Map<FinishedProduct, FinishedProductGalleryDetailView>(model);
                return Ok(r);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("GET: api/finishedproducts/detail/{id}"), ex);
                throw ex;
            }
        }

        [Route("api/finishedproducts/gallery")]
        [ResponseType(typeof(List<FinishedProductGalleryDetailView>))]
        public IHttpActionResult GetGalleryItems()
        {
            try
            {
                List<FinishedProduct> models = db.FinishedProducts.ToList();
                var r = AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(models);
                return Ok(r);
            }
            catch(Exception ex)
            {
                log.Error(string.Format("GET: api/finishedproducts/gallery"),ex);
                throw ex;
            }
        }


        /// <summary>
        /// Returns a paged response of product gallery messages, based on the supplied color container.
        /// </summary>
        /// <param name="ccid">Id of color container entity</param>
        /// <param name="pageNumber">Offset of list to return.</param>
        /// <param name="pageSize">Number of items to return</param>
        /// <returns>PagedResponse of FinishedProductGalleryDetailView</returns>
        [Route("api/finishedproducts/colorcontainer/{ccid}")]
        public IHttpActionResult GetFinishedProductsByColorContainer(int ccid, int pageNumber = 1, int pageSize = 25)
        {
            // Get the chosen color container
            ColorContainer container = db.ColorContainers.Find(ccid);
            // Get the proxy objects to search for product ids
            List<ColorContainerSearchProxy> proxies = db.ColorContainerSearchProxies
                .Where(ccsp => ccsp.ContainerId == ccid)
                .GroupBy(ccsp => ccsp.ProductId).Select(ccsp => ccsp.FirstOrDefault())
                .ToList();
            List<int> prodIds = proxies.Select(p => p.ProductId).ToList();
            // Get active products that use color container
            List<FinishedProduct> products = db.FinishedProducts
                    .Where(fp => fp.Active == true && prodIds.Contains(fp.Id))
                    .ToList();
            // Total for paging response
            int Total = products.Count;
            // Create a sorted list by the strength (count) of container proxy object
            IEnumerable<FinishedProduct> sorted = products.ToDictionary(x => x, x => proxies.FirstOrDefault(y => y.ProductId == x.Id))
                .OrderByDescending(x => x.Value.Count).Select(x => x.Key);
            // Paging
            if (pageNumber > 1)
                sorted = sorted.Skip(pageSize * (pageNumber - 1));
            if (!products.Any())
            {
                NotFound();
            }
            // return response
            return Ok(new PagedResponse<FinishedProductGalleryDetailView>
            {
                Items = AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(sorted.Take(pageSize).ToList()),
                PageSize = pageSize,
                Total = Total
            });
        }

        [Route("api/finishedproducts/likedproducts/{userId}")]
        [ResponseType(typeof(IEnumerable<nec.nexu.ViewModels.FinishedProductViewViewModel>))]
        public IHttpActionResult GetLikedProducts(string userId)
        {
            IEnumerable<FinishedProduct> likedProducts =
                db.FinishedProducts.Where(fp => fp.UserLikes.Count(ul => ul.UserId == userId) > 0);

            if (likedProducts.Any())
            {
                log.Error(string.Format("GET: api/finishedproducts/likedproducts/:userid - Not Found"));
                return NotFound();
            }

            return Ok(AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(likedProducts.ToList()));
        }

        // GET: get featured products
        [HttpGet]
        [Route("api/finishedproducts/featured")]
        [ResponseType(typeof(IEnumerable<nec.nexu.ViewModels.FinishedProductGalleryDetailView>))]
        public IHttpActionResult GetFeatured(int pageNumber = 1, int pageSize = 25)
        {
            List<FinishedProduct> featuredProducts = db.FinishedProducts
                .Where(x => x.Active == true)
                .Where(fp => fp.Featured)
                .OrderBy(p => p.DateCreated)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize).ToList();

            PagedResponse<FinishedProductGalleryDetailView> retResponse = new PagedResponse
                <FinishedProductGalleryDetailView>
            {
                Items =
                    AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(
                        featuredProducts),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = db.FinishedProducts.Count(fp => fp.Active == true && fp.Featured)
            };

            return Ok(retResponse);
        }

        // GET: get active/inactive products
        [Route("api/finishedproducts/active/{status}")]
        [ResponseType(typeof(IEnumerable<nec.nexu.ViewModels.FinishedProductGalleryDetailView>))]
        public IEnumerable<FinishedProduct> GetActive(bool activeStatus)
        {
            List<FinishedProduct> activeProducts = db.FinishedProducts.Where(fp => fp.Active == activeStatus).ToList();

            return activeProducts;
        }

        // GET: get public/private products
        [Route("api/finishedproducts/public/{status}")]
        [ResponseType(typeof(IEnumerable<nec.nexu.ViewModels.FinishedProductGalleryDetailView>))]
        public IHttpActionResult GetPublic(bool status)
        {
            List<FinishedProduct> products = db.FinishedProducts
                .Where(x => x.Active == true)
                .Where(fp => fp.Published == status).ToList();

            return Ok(AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(products));
        }

        // GET: get most viewed product
        [Route("api/finishedproducts/mostviewed")]
        [ResponseType(typeof(PagedResponse<FinishedProductGalleryDetailView>))]
        public IHttpActionResult GetMostViewed(int pageSize = 1, int pageNumber = 25)
        {
            IQueryable<FinishedProduct> query = null;
            query = (pageNumber > 1) ?
                db.FinishedProducts
                .Where(x => x.Active == true)
                .OrderByDescending(product => product.UserViews.Count)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                : db.FinishedProducts
                .Where(x => x.Active == true)
                .OrderByDescending(product => product.UserViews.Count)
                .Take(pageSize);
            List<FinishedProduct> viewedProducts = viewedProducts = query.ToList();
            if (viewedProducts == null)
            {
                log.Error(string.Format("GET: api/finishedproducts/mostviewed - Not Found"));
                return NotFound();
            }
            PagedResponse<FinishedProductGalleryDetailView> ret = new PagedResponse<FinishedProductGalleryDetailView>
            {
                Items = AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(viewedProducts),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = db.FinishedProducts.Where(fp => fp.Active == true).Count()
            };
            return Ok(ret);

        }


        // GET: get products purchased by user
        [Authorize]
        [Route("api/finishedproducts/userpurchased/{uid}")]
        public IHttpActionResult GetUserPurchased(string uid)
        {
            List<int> prodIds = db.Users.Find(uid).UserPurchases.ToList();

            if (prodIds.Any())
            {
                log.Error(string.Format("GET: api/finishedproducts/userpurchased/:uid - Not Found"));
                NotFound();
            }

            List<FinishedProduct> purchasedProducts = db.FinishedProducts.Where(fp => prodIds.Contains(fp.Id)).ToList();

            return Ok(purchasedProducts);
        }

        // GET: get most liked product
        [Route("api/finishedproducts/mostliked")]
        [ResponseType(typeof(FinishedProduct))]
        public IHttpActionResult GetMostLiked()
        {
            FinishedProduct likedproduct = db.FinishedProducts
                .Where(x => x.Active == true)
                .OrderByDescending(product => product.UserLikes.Count).FirstOrDefault();

            if (likedproduct == null)
            {
                log.Error(string.Format("GET: api/finishedproducts/mostliked - Not Found"));
                return NotFound();
            }

            return Ok(likedproduct);
        }

        // GET: get the inspiration count for a product
        [Route("api/finishedproducts/{productId}/inspirationcount")]
        public IHttpActionResult GetInspirationCount(int productId)
        {
            FinishedProduct product = db.FinishedProducts.Find(productId);
            if (product == null)
            {
                log.Error(string.Format("GET: api/finishedproducts/:productId/inspirationcount - Not Found"));
                return NotFound();
            }

            return Ok(product.Inspirations.Count);
        }

        // GET: get products by keyword
        [Route("api/finishedproducts/keyword")]
        [HttpGet]
        public IHttpActionResult SearchByKeyword(string keyword, int count = 25)
        {
            List<string> keywordsAry = keyword.Split(',').ToList();
            //var prods = (from fp in db.FinishedProducts where fp.Keywords != null && keywords.Any(k => k == keywordsAry.Any(ky => ky)) select fp).Take(count);

            //var prods =
            //    (
            //        from fp in db.FinishedProducts        
            //        from k in keywordsAry   
            //        where fp.Keywords.Contains(k)
            //        select fp
            //    ).Take(count).ToList();

            var prods = new List<FinishedProduct>();
            int _c = 0;

            // linq does not like the scalar string collection, need to foreach it
            foreach (var prod in db.FinishedProducts)
            {
                if (_c >= count) break;

                if (keywordsAry.Where(key => prod.Keywords != null).Where(key => prod.Keywords.Contains(key)).Any(key => !prods.Contains(prod)))
                {
                    prods.Add(prod);
                    _c += 1;
                }
            }


            return Ok(new PagedResponse
                <FinishedProductGalleryDetailView>
            {
                Items =
                    AutoMapper.Mapper.Map<List<FinishedProduct>, List<FinishedProductGalleryDetailView>>(
                        prods.ToList()),

                PageSize = count,
                Total = db.FinishedProducts.Count(fp => fp.Active == true && fp.Featured)
            });
        }

        [Route("api/finishedproducts/{productId}/rank")]
        [HttpGet]
        public IHttpActionResult CapScore(int productId)
        {
            FinishedProduct fp = db.FinishedProducts.Find(productId);
            int total = db.FinishedProducts.Where(x => x.Active == true).Count();
            int designScore = 0;
            int likes = fp.UserLikes.Count;
            int views = fp.UserViews.Count;
            int inspirations = fp.Inspirations.Count;

            designScore = (likes * 10) + (inspirations * 100) + ((views / total) / 2);

            return Ok(designScore);
        }

        // GET: api/finishedproducts/{userId}/getscore
        [Authorize]
        [Route("api/finishedproducts/{userId}/getscore")]
        [ResponseType(typeof(int))]
        public IHttpActionResult DesignScore(string userId)
        {
            int designScore = 0;
            int likes = 0;
            int views = 0;
            int inspirations = 0;

            IEnumerable<FinishedProduct> userProducts = db.FinishedProducts.Where(fp => fp.Creator.Id == userId);

            foreach (FinishedProduct prod in userProducts)
            {
                //total likes
                likes += prod.UserLikes.Count;

                //total views
                views += prod.UserViews.Count;

                //inspirations
                inspirations += prod.Inspirations.Count;

            }

            designScore = (likes * 10) + (inspirations * 100) + ((views / userProducts.Count()) / 2);

            return Ok(designScore);
        }

        [HttpGet]
        [Route("api/finishedproducts/inspiredby/{pid}")]
        public IHttpActionResult GetProductsByInspiredProduct(int pid)
        {
            return Ok(db.FinishedProducts.Where(fp => (fp.InspiredByFinishedProductId.HasValue && fp.InspiredByFinishedProductId == pid)).ToList());
        }


        [HttpGet]
        [Route("api/finishedproducts/search/")]
        public IHttpActionResult SearchProductsByCriteria(
            string k = null,
            string h = null,
            string t = null,
            string c = null,
            int count = 50,
            int page = 1,
            string orderBy = "RECENT")
        {
            FinishedProductSearchCriteria criteria = new FinishedProductSearchCriteria();
            if (k != null)
                criteria.Keywords = k.Split(',').ToArray();
            if (h != null)
                criteria.HiearchyIds = h.Split(',').Select(x=>int.Parse(x)).ToArray();
            if (t != null)
                criteria.TemplateIds = t.Split(',').Select(x => int.Parse(x)).ToArray();
            if (c != null)
                criteria.ColorContainers = c.Split(',').Select(x => int.Parse(x)).ToArray();

            criteria.PageSize = count;
            criteria.PageNumber = page;
            criteria.OrderBy = orderBy;

            return Ok(this.searchFinishedProducts(criteria));
        }



        #endregion

        #region Product Modification Methods

        // PUT: api/FinishedProducts/5
        [ResponseType(typeof(FinishedProduct))]
        public IHttpActionResult PutFinishedProduct(int id, FinishedProduct finishedProduct)
        {
            if (!ModelState.IsValid)
            {
                log.Error(string.Format("PUT: api/finishedproducts/:id - Resource: {0} - Not Valid", id.ToString()));
                return BadRequest(ModelState);
            }

            if (id != finishedProduct.Id)
            {
                log.Error(string.Format("PUT: api/finishedproducts/:id"));
                return BadRequest();
            }

            FinishedProduct fp = db.FinishedProducts.Find(id);
            db.Entry(fp).Collection(x => x.Regions).Load();
            db.Entry(fp).Collection(x => x.Designs).Load();
            IEnumerable<int> designIds = finishedProduct.Designs.Select(d => d.DesignColorway.Design.Id);
            if (fp != null)
            {
                finishedProduct.Template = fp.Template;
                finishedProduct.BackgroundImage = db.CustomBackgrounds.Find(finishedProduct.BackgroundImage.Id);
                finishedProduct.Regions = this.FormatFinishedRegions(finishedProduct);
                finishedProduct.Designs = this.FormatFinishedDesigns(finishedProduct);
                fp.Build(finishedProduct);
                fp.Hierarchy = db.Hierarchies.Find(finishedProduct.Hierarchy.Id);
                fp.Keywords = finishedProduct.Keywords;
                fp.DateModified = DateTime.Now;
            }

            // Lookup any country exclusions based on design or hierarchy rules. These need to be copied over to the finished product to prevent them from showing in the gallery.
            List<CountryDesignRule> designRules = db.RulesDesignCountryExclusion
                .Where(r => designIds.Contains(r.DesignId)).ToList();
            List<CountryHierachyExclusion> hierarchyRules = db.CountryHierachyExclusions
                .Where(r => fp.Hierarchy.Id == r.HierarchyId).ToList();


            // Render
            int Viewid = fp.Template.Views.Where(x => x.IsDefault).First().Id; ;
            string root = System.Configuration.ConfigurationManager.AppSettings["renderLocation"].ToString();
            string path = root + fp.ImageBucket + @"\" + fp.Guid + "_" + Viewid + ".png";
            this.saveRender(fp, Viewid, path);
            this.saveCheckoutImages(path, fp.MaterialId.Value);
            /*
            db.Entry(fp).Collection(x => x.Images).Load();
            fp.Images.Clear();
            fp.Images.Add(new FinishedProductImage
            {
                Active = true,
                DisplayName = fp.DisplayName,
                FilePath = @"/renders/" + fp.ImageBucket + @"/" + fp.Guid + "_" + Viewid + ".png"
            });
            */
            // Create the containers
            this.GenerateColorContainer(fp);
            this.GenerateKeywordContainers(fp);
            // Now that we have the FinishedProduct Id in memory, we can process the exclusion rules.
            List<CountryProductExclusion> excludedCountries = this.ProcessCountryExclusions(fp.Id, hierarchyRules, designRules);
            db.RulesCountryProductExclusions.RemoveRange(db.RulesCountryProductExclusions.Where(x => x.FinishedProductId == fp.Id));
            if (excludedCountries.Any())
            {
                db.RulesCountryProductExclusions.AddRange(excludedCountries);

            }
            
            using (var transaction = db.Database.BeginTransaction())
            {

                try
                {
                    db.Entry(fp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!FinishedProductExists(id))
                    {
                        log.Error(string.Format("PUT: api/FinishedProducts/:id - Resource: {0} - Not Found", id.ToString()), ex);
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }


            return Ok(AutoMapper.Mapper.Map<nec.nexu.Models.FinishedProduct, nec.nexu.JsonModels.FinishedProduct>(fp)); 

            //return StatusCode(HttpStatusCode.NoContent);
        }

        private void saveCheckoutImages(string sourcePath, int materialId)
        {
            string root = System.Configuration.ConfigurationManager.AppSettings["checkoutRenders"];
            nec.nexu.rendering.NexuRender render = new nec.nexu.rendering.NexuRender();
            using (Bitmap detail = new Bitmap(sourcePath))
            {
                render.ScaleImage(detail, 329, 329).Save(System.IO.Path.Combine(root, string.Format("{0}_detail.png", materialId)));
                render.ScaleImage(detail, 111, 111).Save(System.IO.Path.Combine(root, string.Format("{0}_cart.png", materialId)));
            }
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("api/FinishedProducts/{id}/keywords")]
        public IHttpActionResult PostFinishedProductKeywords(int id, string[] values)
        {
            if (values == null)
            {
                return BadRequest();
            }
            FinishedProduct fp = db.FinishedProducts.Find(id);
            if (fp == null)
            {
                return BadRequest();
            }
            fp.Keywords.Clear();
            foreach (string s in values)
            {
                fp.Keywords.Add(s);
            }
            this.GenerateKeywordContainers(fp);
            try
            {
                db.Entry(fp).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {

            }
            return Ok();
        }

        // POST: Add User View
        [Route("api/finishedproducts/{productId}/view")]
        [ResponseType(typeof(FinishedProductViewViewModel))]
        public IHttpActionResult AddUserView(int productId)
        {
            string ipAddress = HttpContext.Current.Request.UserHostAddress;

            if (!ModelState.IsValid)
            {
                log.Error(string.Format("POST: api/finishedproducts/:id/view - Resource: Not Valid"));
                return BadRequest(ModelState);
            }

            FinishedProduct finishedProduct = db.FinishedProducts.Find(productId);
            if (finishedProduct == null)
            {
                log.Error(string.Format("POST: api/finishedproducts/:id/view - Resource: {0} - Not Found", productId.ToString()));
                return NotFound();
            }

            if (finishedProduct.UserViews == null)
            {
                finishedProduct.UserViews = new List<UserProductView>();
            }

            if (finishedProduct.UserViews
                    .FirstOrDefault(u => u.IpAddress == ipAddress && (u.Timestamp.HasValue && u.Timestamp.Value > DateTime.Now.AddDays(-1))) == null)
            {
                finishedProduct.UserViews.Add(new UserProductView { IpAddress = ipAddress, Timestamp = DateTime.Now });
                db.Entry(finishedProduct).State = EntityState.Modified;
                if (finishedProduct.Creator != null)
                {
                    var userProds = db.FinishedProducts.Where(fp => (fp.Creator != null && fp.Creator.Id == finishedProduct.Creator.Id)).ToList();
                    var user = db.Users.Find(finishedProduct.Creator.Id); //grab new context
                    this.CalculateDesignerScore(user, userProds);
                    db.Entry(user).State = EntityState.Modified;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!FinishedProductExists(productId))
                    {
                        log.Error(string.Format("POST: api/finishedproducts/:id/view"), ex);
                        return NotFound();
                    }
                    else
                    {
                        log.Error(string.Format("POST: api/finishedproducts/:id/view"), ex);
                        throw;
                    }
                }
            }

            return StatusCode(HttpStatusCode.OK);

        }

        // POST: Add User Like
        [Route("api/finishedproducts/{productId}/like")]
        public IHttpActionResult AddUserLike(int productId, string userId = null)
        {
            string user_ip = HttpContext.Current.Request.UserHostAddress;
            string user_id = userId;

            if (!ModelState.IsValid)
            {
                log.Error(string.Format("POST: api/finishedproducts/:productid/like - Invalid"));
                return BadRequest(ModelState);
            }

            FinishedProduct finishedProduct = db.FinishedProducts.Find(productId);
            if (finishedProduct == null)
            {
                log.Error(string.Format("POST: api/finishedproducts/:productid/like - Not Found"));
                return NotFound();
            }

            if (finishedProduct.UserLikes == null)
            {
                finishedProduct.UserLikes = new List<UserProductLike>();
            }

            if (finishedProduct.UserLikes
                        .FirstOrDefault(ul => ul.IpAddress == user_ip && (ul.Timestamp.HasValue && ul.Timestamp.Value > DateTime.Now.AddDays(-1))) == null)
            {
                finishedProduct.UserLikes.Add(new UserProductLike { IpAddress = user_ip, UserId = user_id, Timestamp = DateTime.Now });
                db.Entry(finishedProduct).State = EntityState.Modified;

                if (finishedProduct.Creator != null)
                {
                    var userProds = db.FinishedProducts.Where(fp => (fp.Creator != null && fp.Creator.Id == finishedProduct.Creator.Id)).ToList();
                    var user = db.Users.Find(finishedProduct.Creator.Id); //grab new context
                    this.CalculateDesignerScore(user, userProds);
                    db.Entry(user).State = EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!FinishedProductExists(productId))
                    {
                        log.Error(string.Format("POST: api/finishedproducts/:productid/like"), ex);
                        return NotFound();
                    }
                    else
                    {
                        log.Error(string.Format("POST: api/finishedproducts/:productid/like"), ex);
                        throw;
                    }
                }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // GET: Add User Purchase
        [HttpGet]
        [Route("api/finishedproducts/{productId}/purchased/{userId}")]
        public IHttpActionResult AddUserPurchase(int productId, string userId)
        {

            if (!ModelState.IsValid)
            {
                log.Error(string.Format("POST: api/finishedproducts/:productid/purchased/:userid - Resource: Not Valid"));
                return BadRequest(ModelState);
            }

            FinishedProduct finishedProduct = db.FinishedProducts.Find(productId);
            if (finishedProduct == null)
            {
                log.Error(string.Format("POST: api/finishedproducts/:productid/purchased/:userid - Resource: {0} - Not Found", productId.ToString()));
                return NotFound();
            }

            //get user
            ApplicationUser user = db.Users.Find(userId);
            if (user == null)
            {
                log.Error(string.Format("POST: api/finishedproducts/:productid/purchased/:userid - Resource: {0} - Not Found", userId.ToString()));
                return NotFound();
            }

            user.UserPurchases.Add(finishedProduct.Id);
            db.Entry(user).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (db.Users.Count(u => u.Id == user.Id) < 0)
                {
                    log.Error(string.Format("POST: api/finishedproducts/:id/view"), ex);
                    return NotFound();
                }
                else
                {
                    log.Error(string.Format("POST: api/finishedproducts/:id/view"), ex);
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.OK);

        }

        // POST: Add an Inspiration to a product
        [Route("api/finishedproducts/{productId}/inspiration")]
        public IHttpActionResult AddInspiration(int productId)
        {
            string user_ip = HttpContext.Current.Request.UserHostAddress;

            if (!ModelState.IsValid)
            {
                log.Error(string.Format("POST: api/finishedproducts/:productid/inspiration - Invalid"));
                return BadRequest(ModelState);
            }

            FinishedProduct finishedProduct = db.FinishedProducts.Find(productId);
            if (finishedProduct == null)
            {
                log.Error(string.Format("POST: api/finishedproducts/:productid/inspiration - Not Found"));
                return NotFound();
            }

            if (finishedProduct.Inspirations == null)
            {
                finishedProduct.Inspirations = new List<Inspiration>();
            }

            if (finishedProduct.Inspirations.FirstOrDefault(ul => ul.IpAddress == user_ip) == null)
            {
                finishedProduct.Inspirations.Add(new Inspiration { IpAddress = user_ip });
                db.Entry(finishedProduct).State = EntityState.Modified;
                if (finishedProduct.Creator != null)
                {
                    var userProds = db.FinishedProducts.Where(fp => (fp.Creator != null && fp.Creator.Id == finishedProduct.Creator.Id)).ToList();
                    var user = db.Users.Find(finishedProduct.Creator.Id); //grab new context
                    this.CalculateDesignerScore(user, userProds);
                    db.Entry(user).State = EntityState.Modified;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!FinishedProductExists(productId))
                    {
                        log.Error(string.Format("POST: api/finishedproducts/:productid/inspiration"), ex);
                        return NotFound();
                    }
                    else
                    {
                        log.Error(string.Format("POST: api/finishedproducts/:productid/inspiration"), ex);
                        throw;
                    }
                }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // GET: set publish flag
        [Route("api/finishedproducts/{productId}/publishstatus/{publishFlag}")]
        public IHttpActionResult SetPublishedStatus(int productId, bool publishFlag)
        {
            FinishedProduct product = db.FinishedProducts.Find(productId);
            if (product == null)
            {
                log.Error(string.Format("GET: api/finishedproducts/:productId/publishstatus/:publishFlag - Not Found"));
                return NotFound();
            }

            product.Published = publishFlag;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: set active status
        [Route("api/finishedproducts/{productId}/activestatus/{activeStatus}")]
        public IHttpActionResult SetActiveStatus(int productId, bool activeStatus)
        {
            FinishedProduct product = db.FinishedProducts.Find(productId);
            if (product == null)
            {
                log.Error(string.Format("GET: api/finishedproducts/:productId/activestatus/:activeStatus - Not Found"));
                return NotFound();
            }

            product.Active = activeStatus;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: set public status
        [Route("api/finishedproducts/{productId}/publicstatus/{publicStatus}")]
        public IHttpActionResult SetPublicStatus(int productId, bool publicStatus)
        {
            FinishedProduct product = db.FinishedProducts.Find(productId);
            if (product == null)
            {
                log.Error(string.Format("GET: api/finishedproducts/:productId/publicstatus/:publicstatus - Not Found"));
                return NotFound();
            }

            product.Public = publicStatus;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion

        #region Product Create Methods

        // POST: api/FinishedProducts
        [HttpPost]
        [Authorize]
        [AllowAnonymous]
        [ResponseType(typeof(nec.nexu.JsonModels.FinishedProduct))]
        public IHttpActionResult PostFinishedProduct(FinishedProduct model)
        {
            FinishedProduct finishedProduct = model;
            List<CountryDesignRule> designRules = null;
            List<CountryHierachyExclusion> hierarchyRules = null;
            if (finishedProduct == null)
            {
                log.Error(string.Format("POST: api/finishedproducts/ - Resource: Not Valid"));
                return BadRequest(ModelState);
            }
            if (finishedProduct.Id  != 0)
            {
                log.Error("Existing");
                return BadRequest();
            }
            try
            {


                IEnumerable<int> designIds = finishedProduct.Designs.Select(d => d.DesignColorway.Design.Id);
                finishedProduct.Guid = Guid.NewGuid();
                finishedProduct.Active = true;
                finishedProduct.DateCreated = DateTime.Now;
                // Set References
                finishedProduct.Creator = db.Users.Find(finishedProduct.Creator.Id); //this.getCurrentUser()
                finishedProduct.Template = db.ProductTemplates.Find(finishedProduct.Template.Id);
                finishedProduct.Hierarchy = db.Hierarchies.Find(finishedProduct.Hierarchy.Id);
                finishedProduct.BackgroundImage = db.CustomBackgrounds.Find(finishedProduct.BackgroundImage.Id);
                //Set Collections
                finishedProduct.Regions = this.FormatFinishedRegions(finishedProduct);
                finishedProduct.Designs = this.FormatFinishedDesigns(finishedProduct);
                finishedProduct.Images = new List<FinishedProductImage>();
                finishedProduct.Keywords = model.Keywords;

                // Lookup any country exclusions based on design or hierarchy rules. These need to be copied over to the finished product to prevent them from showing in the gallery.
                designRules = db.RulesDesignCountryExclusion
                    .Where(r => designIds.Contains(r.DesignId)).ToList();
                hierarchyRules = db.CountryHierachyExclusions
                    .Where(r => finishedProduct.Hierarchy.Id == r.HierarchyId).ToList();
            }
            catch(Exception ex)
            {
                log.Error("error building product", ex);
            }

            
            // Begin transaction for (potential) multiple save commit to DB.
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.FinishedProducts.Add(finishedProduct);
                    db.SaveChanges();

                    // Create the containers
                    this.GenerateColorContainer(finishedProduct);
                    this.GenerateKeywordContainers(finishedProduct);

                    // Now that we have the FinishedProduct Id in memory, we can process the exclusion rules.
                    List<CountryProductExclusion> excludedCountries = this.ProcessCountryExclusions(finishedProduct.Id, hierarchyRules, designRules);
                    if (excludedCountries.Any())
                    {
                        db.RulesCountryProductExclusions.AddRange(excludedCountries);
                        db.SaveChanges();
                    }

                    log.Info("Rendering image");
                    int Viewid = finishedProduct.Template.Views.Where(x=>x.IsDefault).First().Id;
                    string root = System.Configuration.ConfigurationManager.AppSettings["renderLocation"].ToString();
                    int imageBucket = (finishedProduct.ImageBucket.HasValue) ? finishedProduct.ImageBucket.Value : this.GetImageBucket(finishedProduct.Id, root);
                    string path = root + imageBucket + @"\" + finishedProduct.Guid + "_" + Viewid + ".png";
                    this.saveRender(finishedProduct, Viewid, path);
                    this.saveCheckoutImages(path, finishedProduct.MaterialId.Value);

                    // Modify domain entity
                    finishedProduct.ImageBucket = imageBucket;
                    db.Entry(finishedProduct).Collection(x => x.Images).Load();
                    finishedProduct.Images.Add(new FinishedProductImage
                    {
                        Active = true,
                        DisplayName = finishedProduct.DisplayName,
                        FilePath = @"/renders/" + imageBucket + @"/" + finishedProduct.Guid + "_" + Viewid + ".png"
                    });
                    db.Entry(finishedProduct).State = EntityState.Modified;
                    db.SaveChanges();


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    log.Error("error saving product",ex);
                    transaction.Rollback();
                }
            }
            return Ok(AutoMapper.Mapper.Map<nec.nexu.Models.FinishedProduct, nec.nexu.JsonModels.FinishedProduct>(finishedProduct)); 

        }

        [HttpGet]
        [Route("api/MaterialId/")]
        public IHttpActionResult GetMaterialId()
        {
            int materialid = db.GetNewMaterialId();
            return Ok(materialid);
        }

        [HttpGet]
        [Route("api/TestProduct/")]
        public IHttpActionResult Test(string code = null, int templateid = 0, bool n = false, int w = 0, int h = 0)
        {
            if (templateid == 0)
                return BadRequest("You must specify a valid template id");
            FinishedProduct fp = new FinishedProduct();
            ProductTemplate template = db.ProductTemplates.Find(templateid);
            fp.Template = template;
            fp.Regions = new List<FinishedRegion>();


            ProductRegionGroup group = template.RegionGroups.First();
            ProductRegion primaryRegion = group.Regions.First(x => x.Code == "PRIMARY");
            Material primaryMaterial = db.Materials.First(x => x.TextColor == "PRIMARY" && x.DisplayName == code);
            fp.Regions.Add(new FinishedRegion { Material = primaryMaterial, Region = primaryRegion });

            ProductRegion secondaryRegion = group.Regions.FirstOrDefault(x => x.Code == "SECONDARY");
            if (secondaryRegion != null)
            {
                Material secondaryMaterial = db.Materials.First(x => x.TextColor == "SECONDARY" && x.DisplayName == code);
                fp.Regions.Add(new FinishedRegion { Material = secondaryMaterial, Region = secondaryRegion });
            }

            fp.Designs = new List<FinishedDesign>();
            Design design = db.Designs.First(x => x.DisplayName == code && x.Active == true);
            DesignLocation loc = template.DesignLocations.First();
            FinishedDesign front = new FinishedDesign();
            front.DesignColorway = design.DefaultColorway;
            front.Location = loc;
            fp.Designs.Add(front);


            Bitmap img = this.renderProduct(fp, template.Views.First().Id, n, w, h);
            byte[] byteArray = new byte[0];
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();
                byteArray = stream.ToArray();
            }
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(byteArray);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            return ResponseMessage(response);
        }

        #endregion

        #region Image Creation Service Methods

        [HttpGet]
        [Route("api/finishedproducts/{finishedProductId}/render/{views}")]
        public IHttpActionResult SaveFinishedProductView(int finishedProductId, string views)
        {
            try
            {
                // set up path and save image file
                FinishedProduct finishedProduct = db.FinishedProducts.Find(finishedProductId);
                int[] ids = views.Split(',').Select(x=>int.Parse(x)).ToArray();
                List<TemplateView> viewEntities = finishedProduct.Template.Views.ToList();
                foreach (int viewId in ids)
                {
                    int Viewid = viewId;
                    string viewName = viewEntities.FirstOrDefault(x => x.Id == Viewid).DisplayName;
                    string root = System.Configuration.ConfigurationManager.AppSettings["renderLocation"].ToString();
                    int imageBucket = (finishedProduct.ImageBucket.HasValue) ? finishedProduct.ImageBucket.Value : this.GetImageBucket(finishedProductId, root);
                    string path = root + imageBucket + @"\\" + finishedProduct.Guid + "_" + Viewid + ".png";
                    this.saveRender(finishedProduct, Viewid, path);

                    // Modify domain entity
                    finishedProduct.ImageBucket = imageBucket;
                    db.Entry(finishedProduct).Collection(x => x.Images).Load();
                    finishedProduct.Images.Add(new FinishedProductImage
                    {
                        Active = true,
                        DisplayName = finishedProduct.DisplayName+" "+viewName,
                        FilePath = @"/renders/" + imageBucket + @"/" + finishedProduct.Guid + "_" + Viewid + ".png"
                    });
                }

                db.Entry(finishedProduct).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                this.log.Error(ex);
            }
            return Ok();
        }
        
        [HttpGet]
        [Route("api/finishedproducts/view/{viewId}")]
        public IHttpActionResult GetView(int viewId, string p, bool n = false, int w = 0, int h = 0)
        {
            try
            {
                //log.Info("Rendering image");
                string json = FromBase64(p);
                FinishedProduct product = Newtonsoft.Json.JsonConvert.DeserializeObject<FinishedProduct>(json);
                int Viewid = viewId;
                Bitmap img = this.renderProduct(product, Viewid, n, w, h);
                byte[] byteArray = new byte[0];
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Close();
                    byteArray = stream.ToArray();
                }
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(byteArray);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                this.log.Error(ex);
                return BadRequest();
            }

        }
        
        [HttpGet]
        [Route("api/finishedproducts/{finishedProductId}/view/{viewId}")]
        public IHttpActionResult GetView(int finishedProductId, int viewId, bool n = false, int w = 0, int h = 0)
        {
            try
            {
                //log.Info("Rendering image");
                int Viewid = viewId;
                FinishedProduct product = db.FinishedProducts.Find(finishedProductId);


                Bitmap img = this.renderProduct(product, Viewid, n, w, h);

                byte[] byteArray = new byte[0];
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Close();
                    byteArray = stream.ToArray();
                }
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(byteArray);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                this.log.Error(ex);
                return BadRequest();
            }

        }

        [HttpGet]
        [Route("api/finishedproducts/download/{viewId}")]
        public IHttpActionResult Download(int viewId, string p, int bg = 0, int format = 0)
        {
            try
            {
                BackgroundDownloadImage background = db.CustomBackgrounds.Find(bg).DownloadImages.FirstOrDefault(x => x.FormatId == format);
                if (background == null)
                {
                    return BadRequest();
                }
                string json = FromBase64(p);
                FinishedProduct product = Newtonsoft.Json.JsonConvert.DeserializeObject<FinishedProduct>(json);
                int Viewid = viewId;
                Bitmap img = this.renderProduct(product, Viewid, false, 0, 0);
                Bitmap ret = this.placeOnBackground(img, background);
                byte[] byteArray = new byte[0];
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    ret.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Close();
                    byteArray = stream.ToArray();
                }
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(byteArray);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                this.log.Error(ex);
                return BadRequest();
            }

        }
        
        [HttpGet]
        [Route("api/finishedproducts/{finishedProductId}/download/{viewId}")]
        public IHttpActionResult Download(int finishedProductId, int viewId=0, int bg = 0, int format = 1)
        {
            try
            {
                int ViewId = viewId;
                int BG = bg;
                int Format = format;
                FinishedProduct product = db.FinishedProducts.Find(finishedProductId);
                if (product == null)
                {
                    return BadRequest();
                }
                if (BG == 0)
                {
                    BG = product.BackgroundImage.Id;
                }
                BackgroundDownloadImage background = db.CustomBackgrounds.Find(BG).DownloadImages.FirstOrDefault(x => x.FormatId == format);
                if (background == null)
                {
                    return BadRequest();
                }
                if (ViewId == 0)
                {
                    ViewId = product.Template.Views.FirstOrDefault(x => x.IsDefault).Id;
                }
                Bitmap img = this.renderProduct(product, ViewId, false, 0, 0);
                Bitmap ret = this.placeOnBackground(img, background);
                byte[] byteArray = new byte[0];
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    ret.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Close();
                    byteArray = stream.ToArray();
                }
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(byteArray);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                this.log.Error(ex);
                return BadRequest();
            }

        }

        #endregion


        // DELETE: api/FinishedProducts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteFinishedProduct(int id)
        {
            FinishedProduct finishedProduct = db.FinishedProducts.Find(id);
            if (finishedProduct == null)
            {
                log.Error(string.Format("DELETE: api/FinishedProducts/:id - Resource: {0} - Not Found", id.ToString()));
                return NotFound();
            }

            db.FinishedProducts.Remove(finishedProduct);
            db.SaveChanges();

            return Ok();
        }


        #region Private members

        private PagedResponse<FinishedProductGalleryDetailView> searchFinishedProducts(FinishedProductSearchCriteria criteria)
        {
            PagedResponse<FinishedProductGalleryDetailView> response = new PagedResponse<FinishedProductGalleryDetailView>();

            // Log SQL
            //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            int total = this.buildSearch(criteria).Select(x=>x.Id).Count();

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //db.Database.Log = s => sb.Append(s);
            //string sql = sb.ToString();

            IQueryable<FinishedProduct> collection = this.buildSearch(criteria);
            if (criteria.PageNumber > 1)
            {
                collection = collection.Skip(criteria.PageSize * (criteria.PageNumber - 1));
            }
            collection = collection.Take(criteria.PageSize);
            response.Items = AutoMapper.Mapper.Map<List<FinishedProduct>,List<FinishedProductGalleryDetailView>>(collection.ToList());
            response.PageNumber = criteria.PageNumber;
            response.PageSize = criteria.PageSize;
            response.Total = total;
            return response;
        }
        private IQueryable<FinishedProduct> buildSearch(FinishedProductSearchCriteria criteria)
        {
            var collection = db.FinishedProducts.Where(x => x.Active == true);


            if (criteria.Keywords != null && criteria.Keywords.Count() > 0)
            {
                foreach (string word in criteria.Keywords)
                {
                    collection = collection.Where(x => db.FinishedProductKeywordProxies.Where(w=>w.Keyword.Contains(word)).Select(w=>w.ProductId).Contains(x.Id));
                }
            }
            if (criteria.HiearchyIds != null && criteria.HiearchyIds.Count() > 0)
            {
                collection = collection.Where(x => criteria.HiearchyIds.Contains(x.Hierarchy.Id));
            }
            if (criteria.TemplateIds != null && criteria.TemplateIds.Count() > 0)
            {
                collection = collection.Where(x => criteria.TemplateIds.Contains(x.Template.Id));
            }
            if (criteria.ColorContainers != null && criteria.ColorContainers.Count() > 0)
            {
                collection = collection.Where(x =>
                    db.ColorContainerSearchProxies
                    .Where(c => criteria.ColorContainers.Contains(c.ContainerId))
                    .Select(c => c.ProductId)
                    .Contains(x.Id)
                );
            }

            if (criteria.OrderBy == "VISITED")
            {
                //if (criteria.ColorContainers != null && criteria.ColorContainers.Count() > 0)
                //{
                //    collection = collection.OrderByDescending(x => x.UserViews.Count())
                //                            .ThenBy(x => db.ColorContainerSearchProxies.Where(c => c.ProductId == x.Id).Max(c => c.Count));
                //}
                //else
                //{
                collection = collection.OrderByDescending(x => x.UserViews.Count());
                //}
            }
            else if (criteria.OrderBy == "POPULAR")
            {
                //if (criteria.ColorContainers != null && criteria.ColorContainers.Count() > 0)
                //{
                //    collection = collection.OrderByDescending(x => x.UserLikes.Count())
                //        .ThenBy(x => db.ColorContainerSearchProxies.Where(c => c.ProductId == x.Id).Max(c => c.Count));
                //}
                //else
                //{
                collection = collection.OrderByDescending(x => x.UserLikes.Count());
                //}
            }
            else if (criteria.OrderBy == "COLOR")
            {
                if (criteria.ColorContainers != null && criteria.ColorContainers.Count() > 0)
                {
                    collection = collection.OrderByDescending(x =>
                        db.ColorContainerSearchProxies.Where(c => c.ProductId == x.Id && criteria.ColorContainers.Contains(c.ContainerId)).Max(c => c.Count));
                }
                else
                {
                    collection = collection.OrderByDescending(x => x.DateCreated);
                }
            }
            else
            {
                //if (criteria.ColorContainers != null && criteria.ColorContainers.Count() > 0)
                //{
                //    collection = collection.OrderByDescending(x => x.DateCreated)
                //                    .ThenBy(x => db.ColorContainerSearchProxies.Where(c => c.ProductId == x.Id).Max(c => c.Count));
                //}
                //else
                //{
                collection = collection.OrderByDescending(x => x.DateCreated);
                //}
            }
            //if (criteria.PageNumber > 1)
            //{
            //    collection = collection.Skip(criteria.PageSize * (criteria.PageNumber - 1));
            //}

            return collection;
        }


        private void saveRender(FinishedProduct product, int ViewId, string Path)
        {
            this.renderProduct(product, ViewId).Save(Path);
        }

        internal Bitmap __renderProduct(FinishedProduct product, int ViewId,
            bool normalizeSize = false,
            int width = 0,
            int height = 0
            )
        {
            return this.renderProduct(product, ViewId, normalizeSize, width, height);
        }

        private Bitmap renderProduct(FinishedProduct product, int ViewId,
            bool normalizeSize = false,
            int width = 0,
            int height = 0
            )
        {
            TemplateView view = db.TemplateViews.Find(ViewId);
            nec.nexu.rendering.NexuRender render = new rendering.NexuRender();
            TemplateImage rootImage = view.Images.First(x => x.Code.ToUpper() == "ROOT");
            int dpi = 96;
            Bitmap rootBitmap = new Bitmap(System.Drawing.Image.FromStream(new System.IO.MemoryStream(db.ImageDatas.Find(rootImage.ImageDataId).Data)));
            rootBitmap.SetResolution(dpi,dpi);
            Graphics rootGraphics = Graphics.FromImage(rootBitmap);
            string patternRoot = System.Configuration.ConfigurationManager.AppSettings["patternRoot"].ToString();
            foreach (TemplateImage image in view.Images.Where(x => x.Code.ToUpper() != "ROOT"))
            {
                ImageData viewImageData = db.ImageDatas.Find(image.ImageDataId);
                FinishedRegion region = product.Regions.First(x => x.Region.Code.ToUpper() == image.Code.ToUpper());
                Material color = region.Material;
                Pattern pattern = null;
                if (color.Pattern != null)
                {
                    pattern = db.Patterns.Find(color.Pattern.Id);
                }
                if (pattern != null)
                {
                    /* render Pattern */
                    using (
                        Bitmap patternBitmap = 
                            new Bitmap(
                                System.Drawing.Image.FromFile(patternRoot + pattern.ViewImages.FirstOrDefault(x => x.TemplateViewId == view.Id).ImagePath),
                                new Size { Width = rootBitmap.Width, Height = rootBitmap.Height }
                                )
                        )
                    {
                        using (Bitmap layerBitmap = new Bitmap(System.Drawing.Image.FromStream(new System.IO.MemoryStream(viewImageData.Data))))
                        {
                            render.DrawPatternLayer(
                                ref rootGraphics,
                                patternBitmap,
                                dpi,
                                (int)rootBitmap.Width,
                                (int)rootBitmap.Height,
                                (int)image.xPos,
                                (int)image.yPos,
                                layerBitmap
                                );
                        }
                    }

                }
                else
                {
                    /* Can't process pattern, render as flat color */
                    using (Bitmap layerBitmap = new Bitmap(System.Drawing.Image.FromStream(new System.IO.MemoryStream(viewImageData.Data))))
                    {
                        render.DrawRegionLayer(
                            ref rootGraphics,
                            rootBitmap,
                            color.Hex,
                            color.BlendMode.ToString(),
                            (int)color.Opacity,
                            dpi,
                            (int)rootBitmap.Width,
                            (int)rootBitmap.Height,
                            (int)image.xPos,
                            (int)image.yPos,
                            layerBitmap
                            );
                    }

                }


            }
            if(product.Designs !=null)
            {
                foreach (FinishedDesign design in product.Designs)
                {

                    DesignRenderSetting rs = db.DesignRenderSettings.FirstOrDefault(x=>x.TemplateViewId == ViewId && x.DesignLocationId == design.Location.Id);
                    if (rs == null)
                        continue;
                    DesignLayer baseLayer = design.DesignColorway.Layers.First(x=>x.Code.ToUpper() == "GRAY");
                    Bitmap baseImage = new Bitmap(System.Drawing.Image.FromStream(new System.IO.MemoryStream(db.ImageDatas.Find(baseLayer.ImageDataId).Data)));

                    using (Bitmap clipMask = new Bitmap(System.Drawing.Image.FromStream(new System.IO.MemoryStream(rs.ClippingMask))))
                    {

                    render.DrawLogoLayer(
                                    ref rootGraphics,
                                    baseImage,
                                    dpi,
                                    design.DesignColorway.Design.WidthPixel,
                                    design.DesignColorway.Design.HeightPixel,
                                    rootBitmap.Width,
                                    rootBitmap.Height,
                                    0,
                                    0,
                                    new System.Drawing.Drawing2D.Matrix(rs.M1, rs.M2, rs.M3, rs.M4, rs.M5, rs.M6),
                                    clipMask
                                    );

                    foreach (DesignLayer layer in design.DesignColorway.Layers.Where(x => x.Code.ToUpper() != "GRAY"))
                    {
                        using (Bitmap layerImage = new Bitmap(System.Drawing.Image.FromStream(new System.IO.MemoryStream(db.ImageDatas.Find(layer.ImageDataId).Data))))
                        {

                                render.DrawLogoLayer(
                                    ref rootGraphics,
                                    baseImage,
                                    layer.Color.Hex,
                                    layer.Color.BlendMode.ToString(),
                                    (int)layer.Color.Opacity,
                                    dpi,
                                    design.DesignColorway.Design.WidthPixel,
                                    design.DesignColorway.Design.HeightPixel,
                                    rootBitmap.Width,
                                    rootBitmap.Height,
                                    0,
                                    0,
                                    new System.Drawing.Drawing2D.Matrix(rs.M1, rs.M2, rs.M3, rs.M4, rs.M5, rs.M6),
                                    layerImage,
                                    clipMask
                                    );
                            }
                        }
                    }
                }
            }
            if (normalizeSize && (width != 0 && height != 0))
            {
                return render.PlaceImage(
                    render.ScaleImage(rootBitmap,width,height),
                    width, height);
            }
            return rootBitmap;
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FinishedProductExists(int id)
        {
            return db.FinishedProducts.Count(e => e.Id == id) > 0;
        }

        private string FromBase64(string base64)
        {
            var data = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(data);
        }
        private string ToBase64(string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }
        private int GetImageBucket(int seed, string root)
        {
            int buckets = int.Parse(System.Configuration.ConfigurationManager.AppSettings["imageBuckets"].ToString());
            ImageSort.KeyDistributor<int> kd = new ImageSort.KeyDistributor<int>(buckets);
            int bucket = kd.CalculateBucketIndex(seed);
            if (!System.IO.Directory.Exists(root + bucket))
                System.IO.Directory.CreateDirectory(root + bucket);
            return bucket;
        }

        private List<ProductRegion> GetRegions(ICollection<ProductRegionGroup> x)
        {
            List<ProductRegion> regions = new List<ProductRegion>();
            foreach (ProductRegionGroup group in x)
            {
                regions.AddRange(group.Regions);
            }
            return regions;
        }
        private List<FinishedRegion> FormatFinishedRegions(FinishedProduct fp)
        {
            List<ProductRegion> regions = this.GetRegions(fp.Template.RegionGroups);
            foreach(FinishedRegion fr in fp.Regions)
            {
                fr.Region = regions.FirstOrDefault(x=>x.Id == fr.Region.Id);
                fr.Material = db.Materials.Find(fr.Material.Id);
                fr.Active = true;
            }
            return fp.Regions;
        }
        private DesignColorway FormatDesignColorway(DesignColorway example)
        {
            DesignColorway ret;
            /* // Test Colorway
            if(colorway exists)
             * load from db and return existing entity
             * */
            //else
            ret = new DesignColorway();
            ret.Active = true;
            ret.Design = db.Designs.Find(example.Design.Id);
            ret.Layers = new List<DesignLayer>();
            foreach (DesignLayer defaultLayer in ret.Design.DefaultColorway.Layers)
            {
                ret.Layers.Add(this.FormatDesignLayer(example.Layers.FirstOrDefault(x=>x.ImageDataId == defaultLayer.ImageDataId),
                    defaultLayer));
            }
            return ret;
        }
        private DesignLayer FormatDesignLayer(DesignLayer example, DesignLayer defaultLayer)
        {
            DesignLayer ret = new DesignLayer();
            ret.Active = true;
            ret.Code = defaultLayer.Code;
            if(example.Color != null)
                ret.Color = db.DesignColors.Find(example.Color.Id);
            ret.DisplayName = defaultLayer.DisplayName;
            ret.ImageDataId = defaultLayer.ImageDataId;
            ret.Recolorable = defaultLayer.Recolorable;
            ret.SortOrder = defaultLayer.SortOrder;
            return ret;
        }

        private List<FinishedDesign> FormatFinishedDesigns(FinishedProduct fp)
        {
            List<FinishedDesign> ret = new List<FinishedDesign>();
            foreach (FinishedDesign example in fp.Designs)
            {
                FinishedDesign nFinishedDesign = new FinishedDesign();
                nFinishedDesign.Location = fp.Template.DesignLocations.FirstOrDefault(x => x.Id == example.Location.Id);
                nFinishedDesign.Active = true;
                nFinishedDesign.DesignColorway = this.FormatDesignColorway(example.DesignColorway);
                ret.Add(nFinishedDesign);
            }
            return ret;
        }

        private List<CountryProductExclusion> ProcessCountryExclusions(
            int finishedProductId,
            IEnumerable<CountryHierachyExclusion> hierarchy,
            IEnumerable<CountryDesignRule> designs)
        {
            List<CountryProductExclusion> excludedCountries = new List<CountryProductExclusion>();
            foreach (CountryHierachyExclusion rule in hierarchy)
            {
                if (!excludedCountries.Any(x => x.CountryId == rule.CountryId))
                {
                    CountryProductExclusion cpe = new CountryProductExclusion
                    {
                        CountryId = rule.CountryId,
                        FinishedProductId = finishedProductId
                    };
                    excludedCountries.Add(cpe);
                }
            }
            foreach (CountryDesignRule rule in designs)
            {
                if (!excludedCountries.Any(x => x.CountryId == rule.CountryId))
                {
                    CountryProductExclusion cpe = new CountryProductExclusion
                    {
                        CountryId = rule.CountryId,
                        FinishedProductId = finishedProductId
                    };
                    excludedCountries.Add(cpe);
                }
            }
            return excludedCountries;
        }

        private void CalculateDesignerScore(ApplicationUser user, List<FinishedProduct> products)
        {
            int likes = 0;
            int views = 0;
            int insp = 0;

            foreach (FinishedProduct prod in products)
            {
                if (prod.UserLikes != null)
                {
                    likes += prod.UserLikes.Count;
                }
                if (prod.UserViews != null)
                {
                    views += prod.UserViews.Count;
                }
                if (prod.Inspirations != null)
                {
                    insp += prod.Inspirations.Count;
                }  
            }

            user.DesignScore = ((likes*10) + (insp*100) + ((views/products.Count)/2));
        }

        private void GenerateKeywordContainers(FinishedProduct fp)
        {
            db.FinishedProductKeywordProxies.RemoveRange(db.FinishedProductKeywordProxies.Where(x => x.ProductId == fp.Id));
            db.FinishedProductKeywordProxies.AddRange(fp.Keywords.Select(x => new FinishedProductKeywordProxy { ProductId = fp.Id, Keyword = x }));
            db.SaveChanges();
        }

        private void GenerateColorContainer(FinishedProduct fp)
        {
            //color container
            List<FinishedRegion> regions = fp.Regions;
            Dictionary<ColorContainer, int> containerCounts = new Dictionary<ColorContainer, int>();
            foreach (FinishedRegion region in regions)
            {
                IEnumerable<ColorContainer> found = db.ColorContainers.Where(cc => cc.Colors.Any(j => j.Id == region.Material.Id));
                foreach (ColorContainer container in found)
                {
                    if (containerCounts.ContainsKey(container))
                        containerCounts[container]++;
                    else
                        containerCounts.Add(container, 1);
                }
            }
            db.ColorContainerSearchProxies.RemoveRange(
                db.ColorContainerSearchProxies.Where(x => x.ProductId == fp.Id)
                );

            foreach (KeyValuePair<ColorContainer, int> entry in containerCounts)
            {
                ColorContainerSearchProxy prox = new ColorContainerSearchProxy
                {
                    Count = entry.Value,
                    ContainerId = entry.Key.Id,
                    ProductId = fp.Id,
                    Active = true
                };
                db.ColorContainerSearchProxies.Add(prox);
                db.SaveChanges();
            }

            /* Verta-code

            List<Material> colors = new List<Material>();
            List<ColorContainer> containers = new List<ColorContainer>();

            foreach (FinishedRegion region in regions)
            {

                if (colors.FirstOrDefault(c => c.Id == region.Material.Id) == null)
                {
                    colors.Add(region.Material);
                }

                foreach (Material col in colors)
                {
                    IEnumerable<ColorContainer> ccs = db.ColorContainers.Where(cc => cc.Colors.Any(j => j.Id == col.Id));
                    containers.AddRange(ccs);
                }
            }

            foreach (Material col in colors)
            {
                int count = 0;
                foreach (FinishedRegion region in fp.Regions)
                {
                    if (region.Material.Id == col.Id)
                    {
                        count += 1;
                    }
                }


                // get color containers that match each color

                var contains = containers.Where(cc => cc.Colors.Any(c => c.Id == col.Id));

                // removew and/or update old records
                foreach (var colorContainer in contains)
                {
                    var oldContainers =
                        db.ColorContainerSearchProxies.Where(
                            ccsp => ccsp.ContainerId == colorContainer.Id && ccsp.ProductId == fp.Id);

                    db.ColorContainerSearchProxies.RemoveRange(oldContainers);
                    //db.SaveChanges();
                }

                // insert

                foreach (ColorContainer ctain in contains)
                {
                    var prox = new ColorContainerSearchProxy
                    {
                        Count = count,
                        ContainerId = ctain.Id,
                        ProductId = fp.Id,
                        Active = true
                    };

                    db.ColorContainerSearchProxies.Add(prox);
                    db.SaveChanges();
                }
            }
            */
        }


        private Bitmap placeOnBackground(Bitmap product, BackgroundDownloadImage background)
        {
            Bitmap backgroundImage = new Bitmap(System.Configuration.ConfigurationManager.AppSettings["backgroundLocation"].ToString() + background.FilePath);
            Bitmap returnBitmap = new Bitmap(backgroundImage.Width, backgroundImage.Height);
            backgroundImage.SetResolution(96, 96);
            returnBitmap.SetResolution(96, 96);
            returnBitmap.MakeTransparent();
            Graphics graphics = Graphics.FromImage(returnBitmap);
            // Place background
            graphics.DrawImage(backgroundImage, new Point { X = 0, Y = 0 });
            // Place product
            Bitmap productImage = product;
            productImage.SetResolution(96, 96);
            double resolutionDiff = ((double)96 / (double)72);
            int w = (int)Math.Floor((double)(productImage.Width * resolutionDiff));
            int h = (int)Math.Floor((double)(productImage.Height * resolutionDiff));
            if (background.ImageScale != 100)
            {
                var scale = ((double)background.ImageScale / 100);
                w = (int)Math.Floor((double)(w * scale));
                h = (int)Math.Floor((double)(h * scale));
                productImage = new nec.nexu.rendering.NexuRender().ResizeImage(productImage, w, h);
            }
            float x = background.CenterX - (w / 2);
            float y = background.CenterY - (h / 2);
            graphics.DrawImage(productImage, x, y);
            return returnBitmap;
        }

        private ApplicationUser getCurrentUser()
        {
            string userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                userId = "1";
            }
            return db.Users.Find(userId);
        }



        #endregion


    }
}