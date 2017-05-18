using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using log4net;
using nec.nexu.Models;
using AutoMapper;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class ProductTemplatesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /*
        private FinishedProduct CreateTest()
        {
            try
            {
                ProductTemplate template = db.ProductTemplates.Find(1);
                Material material = db.Materials.FirstOrDefault();
                Hierarchy hierarchy = db.Hierarchies.Find(3);
                FinishedProduct ret = new FinishedProduct
                {
                    Active = true,
                    Featured = true,
                    Published = true,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Guid = Guid.NewGuid(),
                    DisplayName = "Test Cap 2",
                    Hierarchy = hierarchy,
                    Images = new List<FinishedProductImage>(),
                    Regions = new List<FinishedRegion>()
                };
                FinishedProductImage image = new FinishedProductImage();
                image.DisplayName = "Test cap gallery image";
                image.Active = true;
                image.FilePath = @"/renders/testcap1.png";
                ret.Images.Add(image);

                foreach (ProductRegionGroup group in template.RegionGroups)
                {
                    foreach (ProductRegion region in group.Regions)
                    {
                        ret.Regions.Add(new FinishedRegion { Active = true, Region = region, Material = material });
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        */ 

        // GET: api/ProductTemplates
        [HttpGet]
        public IHttpActionResult GetProductTemplates(bool active = true, bool list = false)
        {
            IQueryable<ProductTemplate> query = db.ProductTemplates;
            if (active)
                query = query.Where(x => x.Active == true);


            query = query.OrderBy(x => x.DisplayName);

            if (list)
            {
                return Ok(Mapper.Map<List<nec.nexu.Models.ProductTemplate>, List<nec.nexu.ViewModels.ListItem>>(query.ToList()));
            }
            else
            {
                return Ok(Mapper.Map<List<nec.nexu.Models.ProductTemplate>, List<nec.nexu.JsonModels.ProductTemplate>>(query.ToList()));
            }

        }

        // GET: api/ProductTemplates
        [HttpGet]
        [Route("api/ProductTemplates/list")]
        public IHttpActionResult ListProductTemplates()
        {

            var list = db.ProductTemplates.Where(x=>x.Active == true).ToList().Select(x => new nec.nexu.JsonModels.ProductTemplate
            { 
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    DesignLocations = AutoMapper.Mapper.Map <
                        ICollection<nec.nexu.Models.DesignLocation>,
                     ICollection<nec.nexu.JsonModels.DesignLocation>
                    >(x.DesignLocations)
            }).ToList();
            return Ok(list);
        }

        // GET: api/ProductTemplates/5
        [ResponseType(typeof(ProductTemplate))]
        public IHttpActionResult GetProductTemplate(int id)
        {
            //if (id == 99)
            //{
            //    db.FinishedProducts.Add(CreateTest());
            //    db.SaveChanges();
            //    return NotFound();
            //}
            //else
            //{
                ProductTemplate productTemplate = db.ProductTemplates.Find(id);
                if (productTemplate == null)
                {
                    log.Error(string.Format("GET: api/ProductTemplates/:id - Not Found"));
                    return NotFound();
                }

                return Ok(productTemplate);
            //}
        }

        // PUT: api/ProductTemplates/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProductTemplate(int id, ProductTemplate productTemplate)
        {
            if (!ModelState.IsValid)
            {
                log.Error(string.Format("PUT: api/ProductTemplates/:id - Not Valid"));
                return BadRequest(ModelState);
            }

            if (id != productTemplate.Id)
            {
                log.Error(string.Format("PUT: api/ProductTemplates/:id ID Mismatch"));
                return BadRequest();
            }

            ProductTemplate template = db.ProductTemplates.Find(id);
            if (template != null)
            {
                template.Build(productTemplate);
                db.Entry(template).State = EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProductTemplateExists(id))
                {
                    log.Error(string.Format("PUT: api/ProductTemplates/:id ID Mismatch"), ex);
                    return NotFound();
                }
                else
                {
                    log.Error(string.Format("PUT: api/ProductTemplates/:id ID Mismatch"), ex);
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        [Route("api/producttemplate/{templateId}/addview")]
        public IHttpActionResult AddTemplateView(int templateId, TemplateView view)
        {
            ProductTemplate template = db.ProductTemplates.Find(templateId);
            if (template != null)
            {
                db.Entry(template).Collection(x => x.Views).Load();
                template.Views.Add(view);
                db.Entry(template).State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }
            return Ok(view.Id);
        }

        [HttpDelete]
        [ResponseType(typeof(void))]
        [Route("api/producttemplate/{templateId}/removeview/{templateView}")]
        public IHttpActionResult RemoveTemplateView(int templateId =0, int templateView=0)
        {
            if (templateId == 0 || templateView == 0)
                return BadRequest();
            ProductTemplate template = db.ProductTemplates.Find(templateId);
            if (template != null)
            {
                db.Entry(template).Collection(x => x.Views).Load();
                TemplateView view = template.Views.FirstOrDefault(x=>x.Id == templateView);
                if (view != null)
                {
                    template.Views.Remove(view);
                    foreach (TemplateImage image in view.Images)
                    {
                        db.ImageDatas.Remove(db.ImageDatas.Find(image.ImageDataId));
                    }
                }
                db.TemplateViews.Remove(view);
                db.DesignRenderSettings.RemoveRange(db.DesignRenderSettings.Where(x => x.TemplateViewId == view.Id));
                db.Entry(template).State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }
            return Ok();
        }


        // POST: api/ProductTemplates
        [ResponseType(typeof(ProductTemplate))]
        public IHttpActionResult PostProductTemplate(ProductTemplate productTemplate)
        {
           //if (!ModelState.IsValid)
           //{
           //    log.Error(string.Format("POST: api/ProductTemplates - Not Valid"));
           //    return BadRequest(ModelState);
           //}
            ProductTemplate newTemplate = new ProductTemplate();
            newTemplate.Guid = Guid.NewGuid();
            newTemplate.Active = true;
            newTemplate.SortOrderLogo = productTemplate.SortOrderLogo;
            newTemplate.PriceDescription = productTemplate.PriceDescription;
            newTemplate.DisplayName = productTemplate.DisplayName;
            Material defaultMaterial = db.Materials.Find(productTemplate.DefaultMaterial.Id);
            if (defaultMaterial != null)
                newTemplate.DefaultMaterial = defaultMaterial;


            newTemplate.DesignLocations = new List<DesignLocation>();
            foreach (DesignLocation loc in productTemplate.DesignLocations)
            {
                if (loc.SharedLocations == null)
                {
                    loc.SharedLocations = new model.Models.ComplexTypes.PersistableIntCollection();
                }
                newTemplate.DesignLocations.Add(loc);
            }

            newTemplate.RegionGroups = productTemplate.RegionGroups;

            db.ProductTemplates.Add(newTemplate);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = productTemplate.Id }, productTemplate);
        }

        // DELETE: api/ProductTemplates/5
        [ResponseType(typeof(ProductTemplate))]
        public IHttpActionResult DeleteProductTemplate(int id)
        {
            ProductTemplate productTemplate = db.ProductTemplates.Find(id);
            if (productTemplate == null)
            {
                log.Error(string.Format("DELETE: api/ProductTemplates/:id - Not Found"));
                return NotFound();
            }

            db.ProductTemplates.Remove(productTemplate);
            db.SaveChanges();

            return Ok(productTemplate);
        }

        
        [HttpGet]
        [Route("api/masterdata/producttemplate/{id}/views")]
        [ResponseType(typeof (List<TemplateView>))]
        public IHttpActionResult GetTemplateViewsByProductTemplate(int id)
        {
            var prodTemplate = db.ProductTemplates.FirstOrDefault(pt => pt.Id == id);
            if (prodTemplate != null && prodTemplate.Views != null)
            {
                return Ok(prodTemplate.Views.ToList());
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        [Route("api/producttemplate/{id}/createbase")]
        [ResponseType(typeof(int))]
        public IHttpActionResult CreateBaseCapForTemplate(int id)
        {
            var prodTemplate = db.ProductTemplates.FirstOrDefault(pt => pt.Id == id);
            if (prodTemplate != null)
            {

                ProductTemplate template = db.ProductTemplates.Find(id);
                List<FinishedRegion> regions = new List<FinishedRegion>();
                foreach (ProductRegion region in this.GetRegions(template.RegionGroups))
                {
                    regions.Add(new FinishedRegion
                    {
                        Material = template.DefaultMaterial,
                        Region = region
                    });
                }

                FinishedProduct newBlank = new FinishedProduct();
                newBlank.Guid = Guid.NewGuid();
                newBlank.Active = true;
                newBlank.DateCreated = DateTime.Now;
                // Set References
                newBlank.Creator = db.Users.Find("1"); //@TODO change
                newBlank.Template = template;
                //Set Collections
                newBlank.Regions = regions;

                db.FinishedProducts.Add(newBlank);
                db.SaveChanges();

                return Ok(newBlank.Id);
            }
            else
            {
                return NotFound();
            }
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductTemplateExists(int id)
        {
            return db.ProductTemplates.Count(e => e.Id == id) > 0;
        }
    }
}