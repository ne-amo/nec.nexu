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
using AutoMapper;


namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class HierarchiesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: api/Hierarchies
       //[HttpGet]
       //public IHttpActionResult GetHierarchies()
       //{
       //    return Ok(
       //        AutoMapper.Mapper.Map<List<nec.nexu.Models.Hierarchy>,List<nec.nexu.JsonModels.Hierarchy>>(db.Hierarchies.ToList())
       //        );
       //}

        /// <summary>
        /// Gets Hierarchies
        /// </summary>
        /// <param name="level">0 = All (default), 1 = License, 2 = Teams</param>
        /// <param name="list">Return JSON Object (false)(default) or Id/DisplayName object (true)</param>
        /// <param name="active">Active flag (default:true)</param>
        /// <returns></returns>
        public IHttpActionResult GetHierarchies(int level = 0, bool list = false, bool active = true)
        {
            var query = db.Hierarchies.Where(h => h.Active == active);
            switch (level)
            {
                case 1:
                    // Gets top level (license)
                    query = query.Where(x => x.ParentHierarchy == null);
                    break;
                case 2:
                    // Gets just children
                    query = query.Where(x => x.ParentHierarchy != null);
                    break;
                default:
                    // Get all
                    break;
            }
            query = query.OrderBy(x => x.DisplayName);
            if (list)
            {
                return Ok(Mapper.Map<List<nec.nexu.Models.Hierarchy>, List<nec.nexu.ViewModels.ListItem>>(query.ToList()));
            }
            else
            {
                return Ok(Mapper.Map<List<nec.nexu.Models.Hierarchy>, List<nec.nexu.JsonModels.Hierarchy>>(query.ToList()));
            }
        }
        [HttpGet]
        [Route("api/hierarchies/search/")]
        public IHttpActionResult GetHierarchiesByExample([FromUri]Hierarchy example, int templateId = 0, int locationId = 0)
        {
            //string ipAddress = string.Empty;
            //System.Net.IPAddress ip = null;
            //IPAddress.TryParse(System.Web.HttpContext.Current.Request.UserHostAddress, out ip);
            //
            //int userCountryId = db.GetCountryIdByIp(ipAddress);

            var hierarchies = db.Hierarchies.Where(x=> 1==1);

            if (example != null)
            {
                if (example.Active.HasValue)
                {
                    hierarchies = hierarchies.Where(x => x.Active == example.Active);
                }

                if (example.AllowedTemplates != null)
                {
                    foreach(var a in example.AllowedTemplates){
                        hierarchies = hierarchies.Where(x => x.AllowedTemplates.Contains(a));
                    }
                }
                if (example.ParentHierarchyId.HasValue)
                {
                    if (example.ParentHierarchyId.Value == 0)
                    {
                        hierarchies = hierarchies.Where(x => x.ParentHierarchyId == null);
                    }
                    else
                    {
                        hierarchies = hierarchies.Where(x => x.ParentHierarchyId == example.ParentHierarchyId.Value);
                    }
                }
                if(!string.IsNullOrEmpty(example.DisplayName))
                {
                    hierarchies = hierarchies.Where(x => x.DisplayName.Contains(example.DisplayName));
                }

                if (example.Id != 0)
                {
                    hierarchies = hierarchies.Where(x => x.Id == example.Id);
                    //if (example.ParentHierarchyId.Value == 0)
                    //{
                    //    hierarchies = hierarchies.Where(x => x.ParentHierarchyId == null);
                    //}
                    //else
                    //{
                    //    hierarchies = hierarchies.Where(x => x.ParentHierarchyId == example.ParentHierarchyId.Value);
                    //}
                }

            }

            var ret = hierarchies.OrderBy(x => x.DisplayName).OrderBy(x => x.SortOrder).ToList();
            return Ok(AutoMapper.Mapper.Map<List<nec.nexu.Models.Hierarchy>, List<nec.nexu.JsonModels.Hierarchy>>(ret));
        }
        private  List<int> getHierarchyIdsByDesignLocation(int locationId){
            return db.Designs
                .Where(design => design.Locations.Select(loc => loc.Id).Contains(locationId))
                .Select(design => design.HierarchyId)
                .Distinct()
                .ToList();
        }

        public IHttpActionResult GetHierarchies(int parentId, int templateId, int locationId)
        {

            string ipAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
            int userCountryId = 0;
            try
            {
                userCountryId = db.GetCountryIdByIp(ipAddress);
            }
            catch{

            }
                

            var designIdsQuery = db.Designs.Where(x => true);

            if (userCountryId != 0)
            {
                designIdsQuery = designIdsQuery.Where(design => design.Locations.Select(loc => loc.Id).Contains(locationId));
            }

            List<int> designIds = designIdsQuery
                .Select(design => design.HierarchyId)
                .Distinct()
                .ToList();

            //List<nec.nexu.Models.Hierarchy> hierarchies;
            // List<int> designIds = db.Designs
            //     .Where(design => design.Locations.Select(loc => loc.Id).Contains(locationId))
            //     .Select(design => design.HierarchyId)
            //     .Distinct()
            //     .ToList();
            

            var hierarchies = db.Hierarchies.Where(h => h.Active == true);
            hierarchies = hierarchies
                .Where(h => !db.CountryHierachyExclusions.Where(x => x.CountryId == userCountryId && x.TemplateId == templateId)
                    .Select(c => c.HierarchyId).Contains(h.Id));
            
            if (parentId != 0)
            {
                hierarchies = hierarchies.Where(h => h.ParentHierarchy.Id == parentId);
                hierarchies = hierarchies.Where(h => designIds.Contains(h.Id));
            }
            else
            {
                hierarchies = hierarchies.Where(h => h.ParentHierarchy == null);


                hierarchies = hierarchies.Where(h => db.Hierarchies.Where(sub => sub.Active == true &&
                            db.Designs
                                .Where(design => design.Locations.Select(loc => loc.Id).Contains(locationId))
                                .Select(design => design.HierarchyId)
                                .Distinct()
                                .Contains(sub.Id)
                        ).Select(x => x.ParentHierarchyId).Distinct().Contains(h.Id)
                );
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            db.Database.Log = s => sb.AppendLine(s);
            var ret = hierarchies.OrderBy(x => x.SortOrder).ToList();
            string sql = sb.ToString();
            return Ok(AutoMapper.Mapper.Map<List<nec.nexu.Models.Hierarchy>, List<nec.nexu.JsonModels.Hierarchy>>(ret));
        }
        /*
        public IHttpActionResult GetHierarchies(int parentId, int templateId, int locationId)
        {

            string ipAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
            int userCountryId = db.GetCountryIdByIp("12.162.119.194");
            //List<nec.nexu.Models.Hierarchy> hierarchies;
            List<int> designIds = db.Designs
                .Where(design => design.Locations.Select(loc => loc.Id).Contains(locationId))
                .Select(design => design.HierarchyId)
                .Distinct()
                .ToList();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (parentId != 0)
            {
                var hierarchies =
                db.Hierarchies
                .Where(h =>
                    // Matches Parent Id
                    h.ParentHierarchy.Id == parentId &&
                        // Is Active
                    h.Active == true &&
                        // Allowed on template for the user's country
                    !db.CountryHierachyExclusions.Where(x => x.CountryId == userCountryId && x.TemplateId == templateId).Select(c => c.HierarchyId).Contains(h.Id) &&
                        // Belongs to a design that is allowed at the specified location
                    designIds.Contains(h.Id)
                )
                .ToList();
                return Ok(AutoMapper.Mapper.Map<List<nec.nexu.Models.Hierarchy>, List<nec.nexu.JsonModels.Hierarchy>>(hierarchies.ToList()));
            }
            else
            {
                var hierarchies =
                    db.Hierarchies
                    .Where(h =>
                        (
                        // Does not have parent Id. This would indicate a top-level hierarchy (license)
                        h.ParentHierarchy == null &&
                            // Is Active
                        h.Active == true &&
                            // Allowed on template for the user's country
                        !db.CountryHierachyExclusions.Where(x => x.CountryId == userCountryId && x.TemplateId == templateId).Select(c => c.HierarchyId).Contains(h.Id)
                            // Belongs to a design that is allowed at the specified location
                        )
                        ||
                        (
                        db.Hierarchies.Where(sub => sub.Active == true  &&
                            db.Designs
                                .Where(design => design.Locations.Select(loc => loc.Id).Contains(locationId))
                                .Select(design => design.HierarchyId)
                                .Distinct()
                                .Contains(sub.Id)
                        ).Select(x=>x.ParentHierarchyId).Distinct().Contains(h.Id)
                        )
                        //designIds.Contains(h.Id)
                    );

                //db.Database.Log = s => sb.AppendLine(s);
                var ret = hierarchies.ToList();
                //string sql = sb.ToString();
                return Ok(AutoMapper.Mapper.Map<List<nec.nexu.Models.Hierarchy>, List<nec.nexu.JsonModels.Hierarchy>>(ret));
            }


        }
        */
        // GET: api/Hierarchies/5
        [ResponseType(typeof(Hierarchy))]
        public IHttpActionResult GetHierarchy(int id)
        {
            Hierarchy hierarchy = db.Hierarchies.Find(id);
            if (hierarchy == null)
            {
                log.Error(string.Format("GET: api/Hierarchies/:id - Resource: {0} - Not Found", id.ToString()));
                return NotFound();
            }

            return Ok(hierarchy);
        }

        // PUT: api/Hierarchies/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHierarchy(int id, Hierarchy hierarchy)
        {
            if (id != hierarchy.Id)
            {
                return BadRequest();
            }

            Hierarchy h = db.Hierarchies.Find(id);
            if (h != null)
            {
                h.Build(hierarchy);
                db.Entry(h).State = EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!HierarchyExists(id))
                {
                    log.Error(string.Format("PUT: api/Hierarchies/:id - Resource: {0} - ", id), ex);
                    return NotFound();
                }
                else
                {
                    log.Error(string.Format("PUT: api/Hierarchies/:id - Resource: {0} - ", id), ex);
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // POST: api/Hierarchies
        [ResponseType(typeof(Hierarchy))]
        public IHttpActionResult PostHierarchy(Hierarchy hierarchy)
        {

            Hierarchy h = new Hierarchy();
            h.Build(hierarchy);
            //if (hierarchy.ParentHierarchy != null)
            //{
            //    h.ParentHierarchy = db.Hierarchies.Find(hierarchy.ParentHierarchy.Id);
            //}
            db.Hierarchies.Add(h);
            db.SaveChanges();
            return Ok(h);
            //return CreatedAtRoute("DefaultApi", new { id = hierarchy.Id }, hierarchy);
        }

        // DELETE: api/Hierarchies/5
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteHierarchy(int id)
        {
            Hierarchy hierarchy = db.Hierarchies.Find(id);
            if (hierarchy == null)
            {
                return NotFound();
            }

            db.Hierarchies.Remove(hierarchy);
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

        private bool HierarchyExists(int id)
        {
            return db.Hierarchies.Count(e => e.Id == id) > 0;
        }
    }
}