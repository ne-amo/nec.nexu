using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using log4net;
using log4net.Config;
using nec.nexu.App_Start;
using nec.nexu.Models;
using nec.nexu.ViewModels;
using log4net;
using System.Drawing;
using System.IO;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class DesignsController : ApiController
    {
		private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        public IHttpActionResult GetDesigns(int hierarchyId = 0, int locationId = 0)
        {
            string ipAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
            int userCountryId = db.GetCountryIdByIp("12.162.119.194");
            Hierarchy h = db.Hierarchies.Find(hierarchyId);
            var designs = db.Designs.Where(d => d.Active == true);
            designs = designs.Where(d => !db.RulesDesignCountryExclusion.Where(x => x.CountryId == userCountryId).Select(c => c.DesignId).Contains(d.Id));
            designs = designs.Where(d => (d.HierarchyId == h.Id || d.HierarchyId == h.ParentHierarchy.Id));
            if (locationId != 0)
            {
                designs = designs.Where(d => d.Locations.Select(x => x.Id).Contains(locationId));
            }
            

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            db.Database.Log = s => sb.AppendLine(s);
            var ret = designs.OrderBy(x=>x.SortOrder).ToList();
            string sql = sb.ToString();

            return Ok(AutoMapper.Mapper.Map<List<Design>, List<DesignResultMessage>>(ret));
        }

        [HttpGet]
        [Route("api/designs/search/")]
        public IHttpActionResult SearchDesigns
            (
                bool active = true,
                string name = null,
                bool message = false,
                string tape = null
            )
        {
            var query = db.Designs.Where(x => x.Active == active);
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.DisplayName.Contains(name));
            }
            if (!string.IsNullOrEmpty(tape))
            {
                query = query.Where(x => x.TapeNumber.Contains(tape));
            }
            if (message)
            {
                return Ok(AutoMapper.Mapper.Map<List<Design>, List<nec.nexu.ViewModels.DesignResultMessage>>(query.ToList()));
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            db.Database.Log = s => sb.AppendLine(s);
            string sql = sb.ToString();
            var g = query.ToList();

            return Ok(AutoMapper.Mapper.Map<List<Design>,List<nec.nexu.JsonModels.Design>>(g));

        }


        // GET: api/Designs/5
        [HttpGet]
        [ResponseType(typeof(nec.nexu.JsonModels.Design))]
        public IHttpActionResult GetDesign(int id)
        {
            Design design = db.Designs.Find(id);
            if (design == null)
            {
                log.Error(string.Format("GET: api/Designs/:id - Not Found"));
                return NotFound();
            }
            var json = AutoMapper.Mapper.Map<nec.nexu.Models.Design, nec.nexu.JsonModels.Design>(design);
            return Ok(json);
        }
        //[HttpGet]
        //[Route("api/designs/clear/")]
        //public IHttpActionResult Clear(int designId = 0)
        //{
        //    //db.DesignColorways.RemoveRange(db.DesignColorways.ToList());
        //    //foreach (var d in db.Designs.ToList())
        //    //{
        //    //    d.DefaultColorway.Design = null;
        //    //    db.Entry(d.DefaultColorway).State = EntityState.Modified;
        //    //    db.Designs.Remove(d);
        //    //}
        //    //db.SaveChanges();
        //    //return Ok();
        //}


        /// <summary>
        /// Retreives a Json Default Colorway for submitted design
        /// </summary>
        /// <param name="designId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/designs/defaultcolorway/")]
        [ResponseType(typeof(DesignColorway))]
        public IHttpActionResult GetDefaultColorway(int designId)
        {

            Design d = db.Designs.Find(designId);
            return Ok(AutoMapper.Mapper.Map<nec.nexu.Models.DesignColorway, nec.nexu.JsonModels.DesignColorway>(d.DefaultColorway));
        }
        /// <summary>
        /// Retreives a Json Default Colorway for submitted design
        /// </summary>
        /// <param name="designId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/designs/rules/{designId}")]
        [ResponseType(typeof(nec.nexu.JsonModels.DesignRulesCollection))]
        public IHttpActionResult GetDesignRules(int designId = 0)
        {
            nec.nexu.JsonModels.DesignRulesCollection rules = new JsonModels.DesignRulesCollection { DesignId = designId };
            List<nec.nexu.Models.rules.ColorDifferentiationRule> cdr = db.RulesColorDifferentiation.Where(x => x.Active == true && x.DesignId == designId).ToList();
            if (cdr.Any())
                rules.ColorDifferentiationRules.AddRange(
                    AutoMapper.Mapper.Map<
                    List<nec.nexu.Models.rules.ColorDifferentiationRule>,
                    List<nec.nexu.JsonModels.ColorDifferentiationRule>>(cdr)
                );

            List<nec.nexu.Models.rules.DesignColorRule> rdc = db.RulesDesignColor.Where(x => x.Active == true && x.DesignId == designId).ToList();
            if (rdc.Any())
                rules.DesignColorRules.AddRange(
                    AutoMapper.Mapper.Map<
                    List<nec.nexu.Models.rules.DesignColorRule>,
                    List<nec.nexu.JsonModels.DesignColorRule>>(rdc)
                );

            List<nec.nexu.Models.rules.ColorwayRule> cwr = db.RulesColorways.Where(x => x.Active == true && x.DesignId == designId).ToList();
            if (cwr.Any())
                rules.ColorwayRules.AddRange(
                    AutoMapper.Mapper.Map<
                    List<nec.nexu.Models.rules.ColorwayRule>,
                    List<nec.nexu.JsonModels.ColorwayRule>>(cwr)
                );
            return Ok(rules);
        }


        /// <summary>
        /// Checks supplied colorway against any design rules.
        /// </summary>
        /// <param name="designId"></param>
        /// <param name="colorway"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/designs/checkcolorway/{designId}")]
        [ResponseType(typeof(nec.nexu.Models.rules.ColorwayRule))]
        public IHttpActionResult CheckColorwayRule(int designId, DesignColorway colorway)
        {
            var rules = db.RulesColorways.Where(r => r.DesignId == designId).ToList();
            if (!rules.Any())
                return Ok(false);
            bool allow = true;
            nec.nexu.Models.rules.ColorwayRule matchedRule = null;
            foreach (nec.nexu.Models.rules.ColorwayRule rule in rules)
            {
                bool matchesRule = true;
                foreach (nec.nexu.Models.rules.ColorwayRuleValue value in rule.Values)
                {
                    DesignLayer layer = colorway.Layers.FirstOrDefault(x => x.ImageDataId == value.ImageDataId);
                    if (layer == null)
                    {
                        matchesRule = false;
                        break;
                    }
                    if (layer.Color.Id != value.DesignColorId)
                    {
                        matchesRule = false;
                        break;
                    }
                }
                if (matchesRule)
                {
                    allow = false;
                    matchedRule = rule;
                    break;
                }
                else
                    continue;
            }
            if (matchedRule == null)
            {
                return Ok(false);
            }
            return Ok(matchedRule);
        }

        /// <summary>
        /// Return Design by Layer Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/designs/bylayerimagedata/{id}")]
        [ResponseType(typeof(ImageDataCache))]
        public IHttpActionResult GetDesignByLayer(int id)
        {
            //Design ob = db.Designs.Where(x =>
            //    x.DefaultColorway.Layers.Select(y => y.ImageDataId).Contains(id)).FirstOrDefault();
            var ob = db.GetDesignImageData(id);
            return Ok(ob);
        }

        // PUT: api/Designs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDesign(int id, Design design)
        {

            if (id != design.Id)
            {
                log.Error(string.Format("PUT: api/Designs/:id - Not Valid"));
                return BadRequest();
            }
            // Update basic fields
            Design original = db.Designs.Find(id);
            original.DisplayName = design.DisplayName;
            original.Description = design.Description;
            original.TapeNumber = design.TapeNumber;
            original.SortOrder = design.SortOrder;

            // Update Locations
            int[] locs =  design.Locations.Select(y => y.Id).ToArray();
            if (original.Locations == null)
            {
                original.Locations = new List<DesignLocation>();
            }
            else
            {
                original.Locations.Clear();
            }
            original.Locations.AddRange(db.DesignLocations.Where(x => locs.Contains(x.Id)));

            // Update layers
            db.Entry(original.DefaultColorway).Collection(x => x.Layers).Load();
            foreach (DesignLayer layer in original.DefaultColorway.Layers)
            {
                DesignLayer dl = design.DefaultColorway.Layers.First(x => x.Id == layer.Id);
                layer.DisplayName = dl.DisplayName;
                layer.Code = dl.Code;
                layer.SortOrder = dl.SortOrder;
                layer.Recolorable = dl.Recolorable;
                if (layer.Color != null)
                    layer.Color = db.DesignColors.Find(dl.Color.Id);

            }

            // Update Official Image

            db.Entry(original).State = EntityState.Modified;

            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Designs
        [ResponseType(typeof(Design))]
        public IHttpActionResult PostDesign(Design design)
        {
            //if (!ModelState.IsValid)
            //{
            //    log.Error(string.Format("POST: api/Designs/:id - Not Valid"));
            //    return BadRequest(ModelState);
            //}

            //DesignLocation loc = db.DesignLocations.Find(design.Locations.FirstOrDefault().Id);
            //design.Locations = new List<DesignLocation>();
            //design.Locations.Add(loc);

            //List<DesignLocation> locations = new List<DesignLocation>();
            int[] locIds = design.Locations.Select(y=>y.Id).ToArray();
            design.Locations = db.DesignLocations.Where(x => locIds.Contains(x.Id) ).ToList();

            foreach (DesignLayer layer in design.DefaultColorway.Layers)
            {
                if(layer.Color !=null)
                    layer.Color = db.DesignColors.Find(layer.Color.Id);
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                db.Designs.Add(design);
                db.SaveChanges();
                design.DefaultColorway.Design = design;
                db.Entry(design.DefaultColorway).State = EntityState.Modified;
                db.SaveChanges();
                transaction.Commit();
            }
            return Ok(AutoMapper.Mapper.Map<nec.nexu.Models.Design, nec.nexu.JsonModels.Design>(design));
                //CreatedAtRoute("DefaultApi", new { id = design.Id }, design);
        }

        // DELETE: api/Designs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteDesign(int id)
        {
            Design design = db.Designs.Find(id);
            if (design == null)
            {
                log.Error(string.Format("DELETE: api/Designs/:id - Not Found"));
                return NotFound();
            }
            //db.Entry(design).Collection(x => x.Locations).Load();
            //while(design.Locations.Any())
            //{
            //    design.Locations.Remove(design.Locations.ElementAt(0));
            //}
            //design.DefaultColorway = null;
            //design.OfficialImage = null;
            //db.Designs.Remove(design);

            design.Active = false;
            db.Entry(design).State = EntityState.Modified;
            db.SaveChanges();


            return Ok();
        }

        [HttpGet]
        [Route("api/designs/preview/")]
        [ResponseType(typeof(Bitmap))]
        public HttpResponseMessage RenderDesign(string cw, int colorwayid = 0, int h = 0, int w = 0)
        {
            DesignColorway colorway = null;

            if (colorwayid != 0)
            {
                colorway = db.DesignColorways.Find(colorwayid);
            }
            else if (!string.IsNullOrEmpty(cw))
            {
                string json = FromBase64(cw);
                colorway = Newtonsoft.Json.JsonConvert.DeserializeObject<DesignColorway>(json);
            }

            if (colorway == null)
            {
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }


            nec.modules.bitmap.BitmapManager bm = new nec.modules.bitmap.BitmapManager();
            nec.modules.bitmap.RecolorOptions options = new nec.modules.bitmap.RecolorOptions();

            var baseLayer = colorway.Layers.First(x => !x.Recolorable);
            var baseData = db.ImageDatas.Find(baseLayer.ImageDataId);

            int resolution = 72;

            Bitmap returnImage = new Bitmap(Image.FromStream(new MemoryStream(baseData.Data)));
            Graphics returnGraphics = Graphics.FromImage(returnImage);

            foreach (var layer in colorway.Layers.Where(x => x.Recolorable))
            {
                DesignColor color = layer.Color;

                var layerData = db.GetDesignImageData(layer.ImageDataId);

                int red = int.Parse(color.Hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                int green = int.Parse(color.Hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                int blue = int.Parse(color.Hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

                options.Color = System.Drawing.Color.FromArgb(red, green, blue);
                options.BlendMode = color.BlendMode.ToString();
                options.Alpha = (int)Math.Floor((double)(255 * (color.Opacity / 100)));
                options.Trim = false;

                using (Bitmap rootImage = new Bitmap(Image.FromStream(new MemoryStream(baseData.Data))))
                {
                    using (Bitmap newLayer = new Bitmap(rootImage.Width,rootImage.Height))
                    {
                        newLayer.MakeTransparent();
                        Graphics g = Graphics.FromImage(newLayer);
                        Bitmap _layer = new Bitmap(Image.FromStream(new MemoryStream(layerData.Data)));
                        g.DrawImage(_layer, layerData.X, layerData.Y);
                        using (var toDraw = new Bitmap(Image.FromStream(new MemoryStream(bm.GetRecoloredArray(rootImage, newLayer, options)))))
                        {
                            returnGraphics.DrawImage(toDraw, layerData.X, layerData.X);
                        }
                    }
                }
               
            }

            int width = returnImage.Width;
            int height = returnImage.Height;

            if (h != 0 || w != 0)
            {
                width = w;
                height = h;

                if (h != 0)
                {
                    double ratio = (double)height / (double)returnImage.Height;
                    width = Convert.ToInt32(returnImage.Width * ratio);
                }
                else if (w != 0)
                {
                    double ratio = (double)width / (double)returnImage.Width;
                    height = Convert.ToInt32(returnImage.Height * ratio);
                }
            }


            nec.nexu.rendering.NexuRender render = new nec.nexu.rendering.NexuRender();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                returnImage.SetResolution(resolution, resolution);
                render.ScaleImage(returnImage, width, height)
                    .Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                var response = new HttpResponseMessage();
                response.Content = new ByteArrayContent(ms.ToArray());
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                return response;
            }
        }
        private string FromBase64(string base64)
        {
            var data = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(data);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DesignExists(int id)
        {
            return db.Designs.Count(e => e.Id == id) > 0;
        }

        private class RequiredRuleResponseMessage
        {
            public nec.nexu.JsonModels.DesignColorway DesignColorway { get; set; }
            public int LocationId { get; set; }
            public string UserMessage { get; set; }
        }

        [HttpGet]
        [Route("api/designs/requiredrule/")]
        public IHttpActionResult CheckRequiredLogoRule(int locationId = 0, int designId = 0)
        {
            if (locationId == 0 || designId == 0)
            {
                return BadRequest();
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                
                db.Database.Log = s => sb.AppendLine(s);
                var query = db.RulesRequiredLogos.Where(x => x.SourceLocationId == locationId && x.DesignId == designId && x.Active);
                
                
                if (query.Count() >0)
                {
                    var rule = query.OrderByDescending(x => x.Id).First();
                    var resultColorway = db.Designs.Find(rule.TargetDesignId).DefaultColorway;
                    var resultLocation = db.DesignLocations.Find(rule.TargetLocationId);
                    var result = new RequiredRuleResponseMessage
                    {
                        DesignColorway = AutoMapper.Mapper.Map<nec.nexu.Models.DesignColorway, nec.nexu.JsonModels.DesignColorway>(resultColorway),
                        LocationId = resultLocation.Id,
                        UserMessage = rule.UserMessage
                    };
                    string sql = sb.ToString();
                    return Ok(result);
                }
                else
                {
                    string sql = sb.ToString();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                string sql = sb.ToString();
                return InternalServerError(ex);
            }
        }
    }
}