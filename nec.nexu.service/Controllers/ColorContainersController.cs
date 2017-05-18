using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper.QueryableExtensions;
using log4net;
using nec.nexu.Models;
using nec.nexu.ViewModels;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AutoMapper;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class ColorContainersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: api/ColorContainers
        /// <summary>
        /// Returns colorcontainers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/colorcontainers/")]
        public IHttpActionResult GetColorContainers(bool active = true, bool list = false, string lang = "en")
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);

            IQueryable<ColorContainer> query = db.ColorContainers;
            if (active)
                query = query.Where(x => x.Active == true);


            query = query.OrderBy(x => x.ContainerName);

            if (list)
            {
                return Ok(Mapper.Map<List<nec.nexu.Models.ColorContainer>, List<nec.nexu.ViewModels.ListItem>>(query.ToList()));
            }
            else
            {
                return Ok(Mapper.Map<List<nec.nexu.Models.ColorContainer>, List<nec.nexu.JsonModels.ColorContainer>>(query.ToList()));
            }
        }

        [HttpPost]
        [Route("api/colorcontainer/{id}/translations")]
        public IHttpActionResult PostTranslations(int id, string[] lang)
        {
            ColorContainer cc = db.ColorContainers.Find(id);
            foreach (string entry in lang)
            {
                cc.LocalizedContainerName[entry.Split(',')[0]] = entry.Split(',')[1];
            }
            db.Entry(cc).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("api/colorcontainers/{id}")]
        public IHttpActionResult GetColorContainerById(int id = 0, bool view = false)
        {
            if (id == 0)
                return BadRequest();
            ColorContainer container = db.ColorContainers.Find(id);

            if (container == null)
            {
                log.Error(string.Format("GET: api/colorcontainers/:id - Not Found"));
                return NotFound();
            }

            if (view)
                return Ok(Mapper.Map<ColorContainer, nec.nexu.JsonModels.ColorContainer>(container));
            return Ok(container);
        }

        [HttpPost]
        [Route("api/colorcontainers")]
        public IHttpActionResult PostColorContainer(ColorContainer container)
        {

            if (!ModelState.IsValid)
            {
                log.Error(string.Format("POST: api/colorcontainer - Resource: Not Valid"));
                return BadRequest(ModelState);
            }

            if (container.Colors == null)
            {
                log.Error(string.Format("POST: api/colorcontainer - Resource: No Colors Present"));
                return BadRequest();
            }

            var ids = container.Colors.Select(c => c.Id).ToList();
            ColorContainer newContainer = new ColorContainer();
            newContainer.Active = true;
            newContainer.ContainerName = container.ContainerName;
            newContainer.Colors = db.Materials.Where(x => ids.Contains(x.Id)).ToList<Material>();
            db.ColorContainers.Add(newContainer);
            db.SaveChanges();

            return Ok(newContainer);
        }

        // PUT: api/colorcontainers/5
        [HttpPut]
        [ResponseType(typeof(void))]
        [Route("api/colorcontainers/{id}")]
        public IHttpActionResult PutColorContainer(int id, ColorContainer container)
        {
            if (!ModelState.IsValid)
            {
                log.Error(string.Format("PUT: api/colorcontainers/:id - Resource: Not Valid"));
                return BadRequest(ModelState);
            }

            if (id != container.Id)
            {
                log.Error(string.Format("PUT: api/colorcontainers/:id - ID Mismatch"));
                return BadRequest();
            }

            if (!ColorContainerExists(id))
            {
                log.Error(string.Format("PUT: api/colorcontainers/:id - Resource: Not Found"));
                return NotFound();
            }

            ColorContainer existingContainer = db.ColorContainers.Find(id);

            existingContainer.ContainerName = container.ContainerName;
            existingContainer.Active = container.Active;
            db.Entry(existingContainer).Collection(x => x.Colors).Load();
            if (container.Colors == null || container.Colors.Count == 0)
            {
                existingContainer.Colors = null;
            }
            else
            {
                List<int> ids = container.Colors.Select(c => c.Id).ToList();
                existingContainer.Colors = db.Materials.Where(x => ids.Contains(x.Id)).ToList<Material>();
            }

            db.Entry(existingContainer).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ColorContainerExists(id))
                {
                    log.Error(string.Format("PUT: api/colorcontainers/:id"), ex);
                    return NotFound();
                }
                else
                {
                    log.Error(string.Format("PUT: api/colorcontainers/:id"), ex);
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/colorcontainers/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        [Route("api/colorcontainers/{id}")]
        public IHttpActionResult DeleteColorContainer(int id)
        {
            ColorContainer container = db.ColorContainers.Find(id);
            if (container == null)
            {
                log.Error(string.Format("DELETE: api/colorcontainers - Resource: Not Found"));
                return NotFound();
            }

            db.ColorContainers.Remove(container);
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

        private bool ColorContainerExists(int id)
        {
            return db.ColorContainers.Count(e => e.Id == id) > 0;
        }
    }
}
