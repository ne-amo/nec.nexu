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
using nec.nexu.Models;
using AutoMapper;

namespace nec.nexu.Controllers
{
    /// <summary>
    /// Controller for Master Data CRUD
    /// </summary>
    [NexuCorsPoilcy]
    public class MasterdataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




        [HttpGet]
        [Route("api/masterdata/render/")]
        public IHttpActionResult Render(int viewId = 0, string regionCodes = null)
        {
            if (viewId == 0 || regionCodes == null)
            {
                return BadRequest();
            }
            FinishedProduct fp = new FinishedProduct();
            TemplateView view = db.TemplateViews.Find(viewId);
            ProductTemplate template = db.ProductTemplates.Where(x => x.Views.Select(s => s.Id).Contains(view.Id)).FirstOrDefault();
            fp.Template = template;
            fp.Regions = new List<FinishedRegion>();

            foreach (string s in regionCodes.Split(','))
            {
                string[] props = s.Split('|');
                string code = props[0];
                string hex = props[1];
                int opacity = int.Parse(props[2]);
                string _blend = props[3];
                BlendMode blend = BlendMode.MULTIPLY;
                if (_blend.ToLower() == "s")
                {
                    blend = BlendMode.SCREEN;
                }
                Material m = new Material();
                m.Hex = hex;
                m.Opacity = opacity;
                m.BlendMode = blend;
                fp.Regions.Add(new FinishedRegion
                {
                    Region = new ProductRegion { Code = code },
                    Material = m
                });
            }

            System.Drawing.Bitmap img = new FinishedProductsController().__renderProduct(fp, viewId, false, 0, 0);
            img.SetResolution(300, 300);
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








        #region DesignColor methods

        /// <summary>
        /// List Design Colors
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/masterdata/designcolors")]
        [ResponseType(typeof(List<DesignColor>))]
        public IHttpActionResult ListDesignColors(bool json = true)
        {
            if (json)
            {
                return Ok(AutoMapper.Mapper.Map<List<DesignColor>,List<nec.nexu.JsonModels.DesignColor>>(db.DesignColors.ToList()));
            }
            return Ok(db.DesignColors.ToList());
        }

        /// <summary>
        /// View Design Color
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/masterdata/designcolor/{id}")]
        [ResponseType(typeof(DesignColor))]
        public IHttpActionResult ViewDesignColor(bool json = true, int id = 0)
        {
            if(id == 0)
                return BadRequest();
            if (json)
            {
                return Ok(AutoMapper.Mapper.Map<DesignColor, nec.nexu.JsonModels.DesignColor>(db.DesignColors.Find(id)));
            }
            return Ok(db.DesignColors.Find(id));
        }

        /// <summary>
        /// Update Design Color
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/masterdata/designcolor/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateDesignColor(int id, DesignColor example)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != example.Id)
            {
                return BadRequest();
            }

            db.Entry(example).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Creates a new design color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/masterdata/designcolor")]
        [ResponseType(typeof(DesignColor))]
        public IHttpActionResult AddDesignColor(DesignColor color)
        {
            db.DesignColors.Add(color);
            db.SaveChanges();
            return Ok(color);
        }
        
        /// <summary>
        /// Creates multiple new design colors
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/masterdata/designcolors")]
        [ResponseType(typeof(List<DesignColor>))]
        public IHttpActionResult AddDesignColors(DesignColor[] colors)
        {
            foreach (DesignColor color in colors)
            {
                db.DesignColors.Add(color);
            }
            db.SaveChanges();
            return Ok(colors);
        }

        #endregion

        #region FabricContentType methods

        /// <summary>
        /// List FabricContentTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/masterdata/fabriccontenttypes")]
        [ResponseType(typeof(List<FabricContentType>))]
        public IHttpActionResult ListFabricContentTypes(bool json = true)
        {
            if(json)
                return Ok(Mapper.Map<List<FabricContentType>,List<nec.nexu.JsonModels.FabricContentType>>(db.FabricContentTypes.ToList()));
            
            return Ok(db.FabricContentTypes.ToList());
        }

        /// <summary>
        /// View FabricContentType
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/masterdata/fabriccontenttype/{id}")]
        [ResponseType(typeof(FabricContentType))]
        public IHttpActionResult ViewFabricContentType(int id)
        {
            //db.Materials.RemoveRange(db.Materials.ToList());
            //db.FabricContentTypes.RemoveRange(db.FabricContentTypes.ToList());
            //db.SaveChanges();
            return Ok(db.FabricContentTypes.Find(id));
        }

        /// <summary>
        /// Update FabricContentType
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/masterdata/fabriccontenttype/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateFabricContentType(int id, FabricContentType fct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != fct.Id)
            {
                return BadRequest();
            }

            db.Entry(fct).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Creates a new FabricContentType
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/masterdata/fabriccontenttype")]
        [ResponseType(typeof(FabricContentType))]
        public IHttpActionResult AddFabricContentType(FabricContentType entity)
        {
            db.FabricContentTypes.Add(entity);
            db.SaveChanges();
            return Ok(entity);
        }

        /// <summary>
        /// Creates multiple new FabricContentType
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/masterdata/fabriccontenttypes")]
        [ResponseType(typeof(List<FabricContentType>))]
        public IHttpActionResult AddFabricContentTypes(FabricContentType[] entities)
        {
            
            foreach (FabricContentType entity in entities)
            {
                db.FabricContentTypes.Add(entity);
            }
            db.SaveChanges();
            return Ok(entities);
        }

        #endregion

        #region Material methods

        /// <summary>
        /// List Materials
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/masterdata/materials")]
        [ResponseType(typeof(List<Material>))]
        public IHttpActionResult ListMaterials(bool json = true)
        {
            if(json)
                return Ok(AutoMapper.Mapper.Map < List<nec.nexu.Models.Material>, List<nec.nexu.JsonModels.Material>>(db.Materials.ToList()));

            return Ok(db.Materials.ToList());
        }

        /// <summary>
        /// View Material
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/masterdata/material/{id}")]
        [ResponseType(typeof(Material))]
        public IHttpActionResult ViewMaterial(int id)
        {

            return Ok(AutoMapper.Mapper.Map<nec.nexu.Models.Material, nec.nexu.JsonModels.Material>(db.Materials.Find(id)));
        }

        /// <summary>
        /// Update Material
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/masterdata/material/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMaterial(int id, Material m)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != m.Id)
            {
                return BadRequest();
            }


            Material material = db.Materials.Find(id);
            if (material != null)
            {
                m.Pattern = db.Patterns.Find(m.Pattern.Id);
                m.FabricContent = db.FabricContentTypes.Find(m.FabricContent.Id);
                material.Build(m);

                // designcolors
                if (m.MatchingDesignColors != null)
                {
                    material.MatchingDesignColors = new List<DesignColor>();
                    foreach (var dc in m.MatchingDesignColors)
                    {
                        var dbDc = db.DesignColors.Find(dc.Id);
                        if (dbDc != null)
                        {
                            material.MatchingDesignColors.Add(dbDc);
                        }
                    }
                }

                db.Entry(material).State = EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Creates a new Material
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/masterdata/material")]
        [ResponseType(typeof(Material))]
        public IHttpActionResult AddMaterial(Material entity)
        {
            db.Materials.Add(entity);
            db.SaveChanges();
            return Ok(entity);
        }

        /// <summary>
        /// Creates multiple new Materials
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/masterdata/materials")]
        [ResponseType(typeof(List<Material>))]
        public IHttpActionResult AddMaterials(Material[] entities)
        {
            foreach (Material entity in entities)
            {
                entity.FabricContent = db.FabricContentTypes.Find(entity.FabricContent.Id);
                db.Materials.Add(entity);
            }
            db.SaveChanges();
            return Ok(entities);
        }

        #endregion

        #region ImageData

        /// <summary>
        /// Return Imagedata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/masterdata/imagedata/{id}")]
        [ResponseType(typeof(ImageData))]
        public IHttpActionResult GetImageData(int id)
        {
            var ob = db.ImageDatas.Find(id);
            return Ok(ob);
        }
        /// <summary>
        /// Creates a new Imagedata
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/masterdata/imagedata")]
        [ResponseType(typeof(Material))]
        public IHttpActionResult AddImageData(ImageData entity)
        {
            db.ImageDatas.Add(entity);
            db.SaveChanges();
            return Ok(entity);
        }

        #endregion

        #region Pattern
        /// <summary>
        /// Return Patterns
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/masterdata/patterns")]
        [ResponseType(typeof(Pattern))]
        public IHttpActionResult GetPatterns(bool json = true)
        {
            if (json)
                return Ok(Mapper.Map<List<Pattern>,List<nec.nexu.JsonModels.Pattern>>(db.Patterns.ToList()));
            return Ok(db.Patterns.ToList());
        }
        #endregion

        #region ProductRegionGroup

      
        [HttpGet]
        [Route("api/masterdata/productregiongroups")]  
        [ResponseType(typeof(List<ProductRegionGroup>))]
        public IHttpActionResult ListProductRegionGroup(bool list = false, bool json = true)
        {
            if (list)
            {
                return Ok(
                db.ProductRegionGroups.Select(x => new nec.nexu.ViewModels.ListItem
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName
                }).ToList()
                );
            }
            if (json)
            {
                return Ok(AutoMapper.Mapper.Map<List<ProductRegionGroup>,List<nec.nexu.JsonModels.ProductRegionGroup>>(db.ProductRegionGroups.ToList()));
            }
            return Ok(db.ProductRegionGroups.ToList());
        }

        [HttpGet]
        [Route("api/masterdata/productregiongroups/translations/")]
        [ResponseType(typeof(List<ProductRegionGroup>))]
        public IHttpActionResult ListProductRegionGroupByTemplate(int id = 0)
        {
            var list = db.ProductTemplates.Find(id).RegionGroups.Select(x => new ProductRegionGroup
            {
                 Id = x.Id,
                 DisplayName = x.DisplayName,
                 LocalizedDisplayName = x.LocalizedDisplayName
            }).ToList();
            return Ok(list);
        }
        [HttpPost]
        [Route("api/masterdata/productregiongroups/translations/")]
        public IHttpActionResult EditProductRegionGroupTranslations(ProductTemplate data)
        {
            var list = db.ProductTemplates.Find(data.Id).RegionGroups;
            foreach (ProductRegionGroup group in list)
            {
                ProductRegionGroup edited = data.RegionGroups.First(x => x.Id == group.Id);
                group.LocalizedDisplayName = edited.LocalizedDisplayName;
                db.Entry(group).State = EntityState.Modified;
            }
            db.SaveChanges();
            return Ok();
        }


        [HttpGet]
        [Route("api/masterdata/productregiongroups/{id}")]
        [ResponseType(typeof(DesignColor))]
        public IHttpActionResult ViewProductRegionGroup(int id = 0)
        {

            return Ok(db.ProductRegionGroups.Find(id));
        }

        #endregion

        #region DesignLocation



        [HttpGet]
        [Route("api/masterdata/designlocations")]
        [ResponseType(typeof(List<DesignLocation>))]
        public IHttpActionResult ListDesignLocations()
        {
            return Ok(
                AutoMapper.Mapper.Map<List<nec.nexu.Models.DesignLocation>, List<nec.nexu.JsonModels.DesignLocation>>(
                db.DesignLocations.ToList())
                );
        }

        [HttpGet]
        [Route("api/masterdata/designlocations/{id}")]
        [ResponseType(typeof(DesignLocation))]
        public IHttpActionResult ViewDesignLocations(int id)
        {
            return Ok(db.DesignLocations.Find(id));
        }

        [HttpPost]
        [Route("api/masterdata/designlocations")]
        public IHttpActionResult PostDesignLocations(DesignLocation location)
        {
            if (!ModelState.IsValid)
            {
               return BadRequest(ModelState);
            }

            db.DesignLocations.Add(location);
            db.SaveChanges();

            return Ok(location);

        }

        [HttpPut]
        [Route("api/masterdata/designlocations/{id}")]
        public IHttpActionResult PutDesignLocations(int id, DesignLocation location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != location.Id)
            {
                return BadRequest();
            }

            DesignLocation  dblocation = db.DesignLocations.Find(id);
            if (dblocation != null)
            {
                dblocation.Build(location);
                db.Entry(dblocation).State = EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!DesignsLocationExists(id))
                {
                   
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        private bool DesignsLocationExists(int id)
        {
            return db.DesignLocations.Count(e => e.Id == id) > 0;
        }

        #endregion


        #region Translations

        [HttpGet]
        [Route("api/translations/designlocations/")]
        [ResponseType(typeof(List<DesignLocation>))]
        public IHttpActionResult Translation_Get_DesignLocations(int templateId)
        {
            var list = db.ProductTemplates.Find(templateId).DesignLocations.Select(x => new DesignLocation
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                LocalizedDisplayName = x.LocalizedDisplayName
            }).ToList();
            return Ok(list);
        }


        [HttpGet]
        [Route("api/translations/materials/")]
        [ResponseType(typeof(List<Material>))]
        public IHttpActionResult Translation_Get_Materials(int colorlistid)
        {
            var list = db.ColorLists.Find(colorlistid).Colors.Select(x => new Material
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                LocalizedDisplayName = x.LocalizedDisplayName
            }).ToList();
            return Ok(list);
        }
        [HttpPost]
        [Route("api/translations/materials/")]
        [ResponseType(typeof(List<Material>))]
        public IHttpActionResult Translation_Post_Materials(ColorList data)
        {

            var list = db.ColorLists.Find(data.Id).Colors;
            foreach (Material item in list)
            {
                Material edited = data.Colors.First(x => x.Id == item.Id);
                item.LocalizedDisplayName = edited.LocalizedDisplayName;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("api/translations/design/")]
        public IHttpActionResult Translation_Get_Design(string tapeNumber)
        {
            var d = db.Designs.First (x => x.TapeNumber == tapeNumber);

            Design ret = new Design
            {
                Id = d.Id,
                DisplayName = d.DisplayName,
                LocalizedDisplayName = d.LocalizedDisplayName
            };
            DesignColorway cw = new DesignColorway
            { 
                Layers = d.DefaultColorway.Layers.Where(layer => layer.Recolorable).Select(layer => new DesignLayer
                {
                    Id = layer.Id,
                    DisplayName = layer.DisplayName,
                    LocalizedDisplayName = layer.LocalizedDisplayName
                }).ToList()
            };

            ret.DefaultColorway = cw;


            return Ok(ret);
        }
        [HttpPost]
        [Route("api/translations/design/")]
        public IHttpActionResult Translation_Post_Design(Design data)
        {
            var d = db.Designs.Find(data.Id);
            d.LocalizedDisplayNameXml = data.LocalizedDisplayNameXml;
            db.Entry(d).State = EntityState.Modified;
            var list = d.DefaultColorway.Layers;
            foreach (DesignLayer item in list.Where(x=>x.Recolorable))
            {
                DesignLayer edited = data.DefaultColorway.Layers.First(x => x.Id == item.Id);
                item.LocalizedDisplayName = edited.LocalizedDisplayName;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();
            return Ok();
        }


        #endregion




        [HttpGet]
        [Route("api/masterdata/testcaps")]
        [ResponseType(typeof(ProductTemplate))]
        public IHttpActionResult GetProductTemplate()
        {


            db.FinishedProducts.Add(CreateTest("Dodgers Cap 1", 45, @"/renders/79105629_detail.png"));
            db.FinishedProducts.Add(CreateTest("Dodgers Cap 2", 45, @"/renders/79768612_detail.png"));
            db.FinishedProducts.Add(CreateTest("Dodgers Cap 3", 45, @"/renders/79768614_detail.png"));
            db.FinishedProducts.Add(CreateTest("Braves Cap 1", 26, @"/renders/79106205_detail.png"));
            db.FinishedProducts.Add(CreateTest("Braves Cap 2", 26, @"/renders/79269287_detail.png"));
            db.FinishedProducts.Add(CreateTest("New Era 1", 23, @"/renders/79768586_detail.png"));
            db.FinishedProducts.Add(CreateTest("New Era 2", 23, @"/renders/79768607_detail.png"));
            db.FinishedProducts.Add(CreateTest("Toronto Blue Jays", 68, @"/renders/79105862_detail.png"));
            db.FinishedProducts.Add(CreateTest("White Sox 2", 35, @"/renders/79768588_detail.png"));
            db.FinishedProducts.Add(CreateTest("Batman", 115, @"/renders/79768591_detail.png"));
            db.FinishedProducts.Add(CreateTest("Decepticons", 113, @"/renders/79768596_detail.png"));
            db.FinishedProducts.Add(CreateTest("Bulls HWC Cap", 96, @"/renders/79768600_detail.png"));
            db.FinishedProducts.Add(CreateTest("Superduperman", 114, @"/renders/79768608_detail.png"));
            db.FinishedProducts.Add(CreateTest("Hornets", 95, @"/renders/79768611_detail.png"));
            db.FinishedProducts.Add(CreateTest("Mariners", 61, @"/renders/79869294_detail.png"));

            //db.SaveChanges();
            return NotFound();
        }

        //CreateTest("Test Cap", 
        private FinishedProduct CreateTest(string capName, int hierarchyid, string imgPath)
        {
            try
            {
                ProductTemplate template = db.ProductTemplates.Find(1);
                Material material = db.Materials.Find(new Random().Next(0,db.Materials.Count()-1));
                Hierarchy hierarchy = db.Hierarchies.Find(hierarchyid);
                FinishedProduct ret = new FinishedProduct
                {
                    Active = true,
                    Featured = false,
                    Published = true,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Guid = Guid.NewGuid(),
                    DisplayName = capName,
                    Hierarchy = hierarchy,
                    Images = new List<FinishedProductImage>(),
                    Regions = new List<FinishedRegion>()
                };
                FinishedProductImage image = new FinishedProductImage();
                image.DisplayName = capName+" gallery image";
                image.Active = true;
                image.FilePath = imgPath;
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



    }
}