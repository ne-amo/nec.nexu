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

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class UserColorListsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: api/UserColorLists
        public IQueryable<UserColorList> GetUserColorLists()
        {
            return db.UserColorLists;
        }

        // GET: api/UserColorLists/{string}
        [ResponseType(typeof(nec.nexu.JsonModels.UserColorList))]
        public IHttpActionResult GetUserColorListByUser(string id)
        {
            List<UserColorList> userColorList = db.UserColorLists.Where(x=>x.CreatorId==id).ToList();
            if (userColorList == null)
            {
                log.Error(string.Format("POST: api/UserColorLists/:id - Resource: Not Found"));
                return NotFound();
            }

            return Ok(userColorList);
        }

        // PUT: api/UserColorLists/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUserColorList(int id, UserColorList userColorList)
        {
            if (!ModelState.IsValid)
            {
                log.Error(string.Format("PUT: api/UserColorLists/:id - Resource: Not Valid"));
                return BadRequest(ModelState);
            }

            if (id != userColorList.Id)
            {
                log.Error(string.Format("PUT: api/UserColorLists/:id - ID Mismatch"));
                return BadRequest();
            }

            UserColorList existingList = db.UserColorLists.Find(id);
            if (existingList != null)
            {
                //existingList.ColorList = userColorList.ColorList;
                //existingList.Creator = userColorList.Creator;
                existingList.CreatorId = userColorList.CreatorId;
                db.Entry(existingList).State = EntityState.Modified;
            }
            else
            {
                log.Error(string.Format("PUT: api/UserColorLists/:id - Not Found"));
                return NotFound();
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!UserColorListExists(id))
                {
                    log.Error(string.Format("PUT: api/UserColorLists/:id"), ex);
                    return NotFound();
                }
                else
                {
                    log.Error(string.Format("PUT: api/UserColorLists/:id"), ex);
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/UserColorLists
        [ResponseType(typeof(nec.nexu.JsonModels.UserColorList))]
        public IHttpActionResult PostUserColorList(nec.nexu.ViewModels.UserColorListViewModel userColorList)
        {
            if (!ModelState.IsValid)
            {
                log.Error(string.Format("POST: api/UserColorLists - Not Valid"));
                return BadRequest(ModelState);
            }
            UserColorList ucl = new UserColorList();
            ucl.CreatorId = userColorList.CreatorId;
            ucl.MaterialIds = new model.Models.ComplexTypes.PersistableIntCollection();
            ucl.DisplayName = userColorList.DisplayName;
            foreach (int id in userColorList.MaterialIds)
            {
                ucl.MaterialIds.Add(id);
            }

            db.UserColorLists.Add(ucl);
            db.SaveChanges();

            return Ok(ucl.Id);
        }

        // DELETE: api/UserColorLists/5
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteUserColorList(int id)
        {
            UserColorList userColorList = db.UserColorLists.Find(id);
            if (userColorList == null)
            {
                log.Error(string.Format("DELETE: api/UserColorLists/:id - Not Found"));
                return NotFound();
            }

            db.UserColorLists.Remove(userColorList);
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

        private bool UserColorListExists(int id)
        {
            return db.UserColorLists.Count(e => e.Id == id) > 0;
        }
    }
}