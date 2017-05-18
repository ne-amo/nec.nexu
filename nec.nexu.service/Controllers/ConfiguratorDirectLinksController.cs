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
using log4net;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class ConfiguratorDirectLinksController : ApiController
    {
		private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: api/ConfiguratorDirectLinks
        public IHttpActionResult GetConfiguratorDirectLinks(bool list = false, bool grid = false)
        {
            var query = db.ConfiguratorDirectLinks;
            if (list)
            {
                return Ok(query.Select(x => new nec.nexu.ViewModels.ListItem { Id = x.Id, DisplayName = x.DisplayName }));
            }
            if (grid)
            {
                List<nec.nexu.JsonModels.ConfiguratorDirectLink> ret = query.Select(x => new nec.nexu.JsonModels.ConfiguratorDirectLink
                {
                    Guid = x.Guid,
                    DisplayName = x.DisplayName,
                    Description = x.Description,
                    Id = x.Id,
                    CreateDate = x.CreateDate,
                    Active = x.Active.Value
                }).ToList();
                return Ok(ret);
            }

            var json = AutoMapper.Mapper.Map<List<nec.nexu.Models.ConfiguratorDirectLink>, List<nec.nexu.JsonModels.ConfiguratorDirectLink>>(query.ToList());
            return Ok(json);

        }

        // GET: api/ConfiguratorDirectLinks/5
        [ResponseType(typeof(nec.nexu.JsonModels.ConfiguratorDirectLink))]
        public IHttpActionResult GetConfiguratorDirectLink(int id)
        {
            ConfiguratorDirectLink configuratorDirectLink = db.ConfiguratorDirectLinks.Find(id);
                
            if (configuratorDirectLink == null)
            {
                log.Error(string.Format("GET: api/ConfiguratorDirectLink/:id - Resource: {0} - Not Found", id));
                return NotFound();
            }
            try
            {
                return Ok(
                    AutoMapper.Mapper.Map<ConfiguratorDirectLink, nec.nexu.JsonModels.ConfiguratorDirectLink>(configuratorDirectLink)
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }


        [Route("api/ConfiguratorDirectLinks/uid/{guid}")]
        [ResponseType(typeof(nec.nexu.JsonModels.ConfiguratorDirectLink))]
        public IHttpActionResult GetConfiguratorDirectLink(Guid guid)
        {
            ConfiguratorDirectLink configuratorDirectLink = db.ConfiguratorDirectLinks.FirstOrDefault(x=>x.Active == true && x.Guid == guid);
            if (configuratorDirectLink == null)
            {
                log.Error(string.Format("GET: api/ConfiguratorDirectLink/:id - Resource: {0} - Not Found", guid));
                return NotFound();
            }

            return Ok(
                AutoMapper.Mapper.Map < ConfiguratorDirectLink,nec.nexu.JsonModels.ConfiguratorDirectLink>(configuratorDirectLink)
            );
        }

        // PUT: api/ConfiguratorDirectLinks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutConfiguratorDirectLink(int id, ConfiguratorDirectLink configuratorDirectLink)
        {
            if (!ModelState.IsValid)
            {
                log.Error(string.Format("PUT: api/ConfiguratorDirectLink/:id - Resource: {0} - Not Valid", id));
                return BadRequest(ModelState);
            }

            if (id != configuratorDirectLink.Id)
            {
                log.Error(string.Format("PUT: api/ConfiguratorDirectLink/:id - Resource: {0} - ID Mismatch", id));
                return BadRequest();
            }

            ConfiguratorDirectLink dbConfigurator = db.ConfiguratorDirectLinks.Find(id);
            if (dbConfigurator != null)
            {
                dbConfigurator.Build(configuratorDirectLink);

                // mats
                if (configuratorDirectLink.Materials != null)
                {
                    dbConfigurator.Materials = new List<Material>();
                    foreach (var mat in configuratorDirectLink.Materials)
                    {
                        var dbMat = db.Materials.Find(mat.Id);
                        if (dbMat != null)
                        {
                            dbConfigurator.Materials.Add(dbMat);
                        }
                    }
                }

                //designs 
                if (configuratorDirectLink.Designs != null)
                {
                    dbConfigurator.Designs = new List<Design>();
                    foreach (var des in configuratorDirectLink.Designs)
                    {
                        var dbDes = db.Designs.Find(des.Id);
                        if (dbDes != null)
                        {
                            dbConfigurator.Designs.Add(dbDes);
                        }
                    }
                }

                db.Entry(dbConfigurator).State = EntityState.Modified;
            }

            

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ConfiguratorDirectLinkExists(id))
                {
                    log.Error(string.Format("PUT: api/ConfiguratorDirectLink/:id - Resource: {0} ", id), ex);
                    return NotFound();
                }
                else
                {
                    log.Error(string.Format("PUT: api/ConfiguratorDirectLink/:id - Resource: {0} ", id), ex);
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ConfiguratorDirectLinks
        [ResponseType(typeof(nec.nexu.JsonModels.ConfiguratorDirectLink))]
        public IHttpActionResult PostConfiguratorDirectLink(ConfiguratorDirectLink configuratorDirectLink)
        {

            ConfiguratorDirectLink dbConfigurator = new ConfiguratorDirectLink();
            if (dbConfigurator != null)
            {
                dbConfigurator.Build(configuratorDirectLink);

                // mats
                if (configuratorDirectLink.Materials != null)
                {
                    foreach (var mat in configuratorDirectLink.Materials)
                    {
                        var dbMat = db.Materials.Find(mat.Id);
                        if (dbMat != null)
                        {
                            dbConfigurator.Materials.Add(dbMat);
                        }
                    }
                }

                //designs 
                if (configuratorDirectLink.Designs != null)
                {
                    foreach (var des in configuratorDirectLink.Designs)
                    {
                        var dbDes = db.Designs.Find(des.Id);
                        if (dbDes != null)
                        {
                            dbConfigurator.Designs.Add(dbDes);
                        }
                    }
                }

               // db.Entry(dbConfigurator).State = EntityState.Modified;
            }

            db.ConfiguratorDirectLinks.Add(dbConfigurator);
            db.SaveChanges();

            return Ok(AutoMapper.Mapper.Map<ConfiguratorDirectLink, nec.nexu.JsonModels.ConfiguratorDirectLink>(dbConfigurator));
        }

        // DELETE: api/ConfiguratorDirectLinks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteConfiguratorDirectLink(int id)
        {
            ConfiguratorDirectLink configuratorDirectLink = db.ConfiguratorDirectLinks.Find(id);
            if (configuratorDirectLink == null)
            {
                log.Error(string.Format("PUT: api/ConfiguratorDirectLink/:id - Resource: {0} - Not Found", id));
                return NotFound();
            }

            db.ConfiguratorDirectLinks.Remove(configuratorDirectLink);
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

        private bool ConfiguratorDirectLinkExists(int id)
        {
            return db.ConfiguratorDirectLinks.Count(e => e.Id == id) > 0;
        }

        private ConfiguratorDirectLink Update(ConfiguratorDirectLink example)
        {
            ConfiguratorDirectLink ret = db.ConfiguratorDirectLinks.Find(example.Id);
            AutoMapper.Mapper.Map<ConfiguratorDirectLink, ConfiguratorDirectLink>(example, ret);
            ret.Id = example.Id;
            return ret;
        }
    }
}