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
using nec.nexu.App_Start;
using nec.nexu.Extensions;
using nec.nexu.Models;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class CustomBackgroundsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: api/CustomBackgrounds
        [HttpGet]
        public IHttpActionResult GetCustomBackgrounds()
        {
            return Ok(db.CustomBackgrounds);
        }

        [HttpGet]
        [ResponseType(typeof(CustomBackground))]
        public IHttpActionResult GetCustomBackground(int id)
        {
            CustomBackground customBackground = db.CustomBackgrounds.Find(id);
            if (customBackground == null)
            {
                log.Error(string.Format("GET: api/CustomBackgrounds/:id - Resource: {0} - Not Found", id));
                return NotFound();
            }

            return Ok(customBackground);
        }

        [HttpGet]
        [Route("api/CustomBackgrounds/prodtemplateid/{productTemplateId}")]
        public IHttpActionResult GetCustomBackgroundByTemplateId(int productTemplateId)
        {
            IEnumerable<nec.nexu.Models.CustomBackground> ret = db.CustomBackgrounds
                .Where(cb => cb.AllowedTemplates.Select(t => t.Id).Contains(productTemplateId))
                .OrderBy(cb => cb.SortOrder);//cb.ProductTemplateId == productTemplateId);
            return Ok(AutoMapper.Mapper.Map<
                IEnumerable<nec.nexu.Models.CustomBackground>,
                IEnumerable<nec.nexu.JsonModels.CustomBackground>>(ret));
        }
        [HttpGet]
        [Route("api/CustomBackgrounds/general/")]
        public IHttpActionResult GetGeneralCustomBackground()
        {
            IEnumerable<nec.nexu.Models.CustomBackground> ret = db.CustomBackgrounds
                .Where(cb => cb.Active == true && cb.AllowedHierarchies.Count == 0 && cb.AllowedTemplates.Count == 0)
                .OrderBy(cb=>cb.SortOrder);
            return Ok(AutoMapper.Mapper.Map<
                IEnumerable<nec.nexu.Models.CustomBackground>,
                IEnumerable<nec.nexu.JsonModels.CustomBackground>>(ret));
        }

        [HttpGet]
        [Route("api/CustomBackgrounds/query/")]
        public IHttpActionResult GetQueriedBackgrounds(int templateId = 0, int hierarchyId = 0, int currentId = 0)
        {
            HashSet<CustomBackground> set = new HashSet<CustomBackground>();

            set.UnionWith(db.CustomBackgrounds
                .Where(cb => cb.Active == true &&
                        (cb.AllowedHierarchies.Count == 0 && cb.AllowedTemplates.Count == 0))
                        .ToList());

            if (templateId != 0)
            {
                set.UnionWith(db.ProductTemplates.Find(templateId).ExclusiveBackgrounds);
            }
            if (hierarchyId != 0)
            {
                Hierarchy hierarchy = db.Hierarchies.Find(hierarchyId);
                set.UnionWith(hierarchy.ExclusiveBackgrounds);

                if (hierarchy.ParentHierarchy != null)
                {
                    set.UnionWith(hierarchy.ParentHierarchy.ExclusiveBackgrounds);
                }
            }

            IEnumerable<nec.nexu.Models.CustomBackground> ret = set.OrderBy(cb => cb.SortOrder);
               
            return Ok(AutoMapper.Mapper.Map<
                IEnumerable<nec.nexu.Models.CustomBackground>,
                IEnumerable<nec.nexu.JsonModels.CustomBackground>>(ret));
        }


        [Route("api/CustomBackgrounds/active/{active}")]
        public IEnumerable<CustomBackground> GetActiveCustomBackground(bool active)
        {
            return db.CustomBackgrounds.Where(cb => cb.Active == active);
        }

        [HttpGet]
        [Route("api/CustomBackgrounds/heirarchy/{hid}")]
        public IHttpActionResult GetCustomBackgroundByHeirarchyId(int hid)
        {
            var ret = db.CustomBackgrounds
                .Where(cb => cb.AllowedHierarchies.Select(h => h.Id).Contains(hid))
                .OrderBy(cb => cb.SortOrder);//cb.ProductTemplateId == productTemplateId);
            return Ok(AutoMapper.Mapper.Map<
                IEnumerable<nec.nexu.Models.CustomBackground>,
                IEnumerable<nec.nexu.JsonModels.CustomBackground>>(ret));
        }



        [HttpGet]
        [Route("api/CustomBackgrounds/formats/")]
        public IHttpActionResult GetDownloadFormats()
        {
            List<DownloadFormat> ret = db.DownloadFormats
                .Where(x => x.Active.Value)
                .OrderBy(x => x.SortOrder)
                .ToList();
            return Ok(ret);
        }

        // PUT: api/CustomBackgrounds/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCustomBackground(int id, CustomBackground customBackground)
        {
            if (!ModelState.IsValid)
            {
                log.Error(string.Format("PUT: api/CustomBackgrounds/:id - Resource: {0} - Not Valid", id));
                return BadRequest(ModelState);
            }

            if (id != customBackground.Id)
            {
                log.Error(string.Format("PUT: api/CustomBackgrounds/:id - Resource: {0} - ID Mismatch", id));
                return BadRequest();
            }

            CustomBackground background = db.CustomBackgrounds.Find(id);
            if (background == null)
            {
                log.Error(string.Format("PUT: api/CustomBackgrounds/:id - Resource: {0} - Not Found", id));
                return NotFound();
            }
            else
            {
                background.Build(customBackground);
            }
            

            db.Entry(background).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CustomBackgroundExists(id))
                {
                    log.Error(string.Format("PUT: api/CustomBackgrounds/:id - Resource: {0}", id), ex);
                    return NotFound();
                }
                else
                {
                    log.Error(string.Format("PUT: api/CustomBackgrounds/:id - Resource: {0}", id), ex);
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/CustomBackgrounds
        [ResponseType(typeof(CustomBackground))]
        public IHttpActionResult PostCustomBackground(CustomBackground customBackground)
        {
            if (!ModelState.IsValid)
            {
                log.Error(string.Format("PUT: api/CustomBackgrounds/:id - Not Valid"));
                return BadRequest(ModelState);
            }

            db.CustomBackgrounds.Add(customBackground);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = customBackground.Id }, customBackground);
        }

        // DELETE: api/CustomBackgrounds/5
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteCustomBackground(int id)
        {
            CustomBackground customBackground = db.CustomBackgrounds.Find(id);
            if (customBackground == null)
            {
                log.Error(string.Format("PUT: api/CustomBackgrounds/:id - Resource: {0} - Not Found", id));
                return NotFound();
            }

            db.CustomBackgrounds.Remove(customBackground);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomBackgroundExists(int id)
        {
            return db.CustomBackgrounds.Count(e => e.Id == id) > 0;
        }
    }
}