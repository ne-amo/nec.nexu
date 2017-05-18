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
using AutoMapper.QueryableExtensions;
using log4net;
using nec.nexu.Models;
using nec.nexu.ViewModels;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
	public class ColorListsController : ApiController
	{
		private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		
		// GET: api/ColorLists
        /// <summary>
        /// Returns active colorlists
        /// </summary>
        /// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetColorLists(bool active = true, bool list = true)
		{
            var query = db.ColorLists.Where(x => x.Active == active);

            if (list)
            {
                return Ok(query.Select(x => new ListItem { Id = x.Id, DisplayName = x.DisplayName }).ToList());
            }

			return Ok(
                AutoMapper.Mapper.Map<List<ColorList>,List<ColorListViewModel>>(
                    db.ColorLists.Where(x=>x.Active==true).ToList()
                ));
		}
        /// <summary>
        /// Returns active and deactivated color lists
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ColorLists/all")]
        public IQueryable<ColorListViewModel> GetAllColorLists()
        {
            return db.ColorLists.Project().To<ColorListViewModel>();
        }

		// GET: api/ColorLists/5
		[ResponseType(typeof(nec.nexu.JsonModels.ColorList))]
		public IHttpActionResult GetColorList(int id)
		{
            if (id == 0)
                return BadRequest();
			ColorList colorList = db.ColorLists.Find(id);
			if (colorList == null)
			{
                log.Error(string.Format("GET: api/ColorLists/:id - Resource: Not Found"));
				return NotFound();
			}
			return Ok(AutoMapper.Mapper.Map<ColorList,ColorListViewModel>(colorList));
		}

        [HttpGet]
        [Route("api/ColorLists/allColors/")]
        public IHttpActionResult GetAllColors()
        {
            List<Material> materials = new List<Material>();
            foreach(ColorList list in db.ColorLists.Where(x=>x.Active == true).ToList())
            {
                foreach (Material color in list.Colors.ToList())
                {
                    if (!materials.Select(x => x.Id).Contains(color.Id))
                    {
                        materials.Add(color);
                    }
                }
            }
            return Ok(AutoMapper.Mapper.Map<List<nec.nexu.Models.Material>, List<nec.nexu.JsonModels.Material>>(materials));
        }

		// PUT: api/ColorLists/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutColorList(int id, ColorList colorList)
		{
			if (!ModelState.IsValid)
			{
                log.Error(string.Format("PUT: api/ColorLists/:id - Resource: Not Valid"));
				return BadRequest(ModelState);
			}

			if (id != colorList.Id)
			{
                log.Error(string.Format("PUT: api/ColorLists/:id - ID Mismatch"));
				return BadRequest();
			}
            if (!ColorListExists(id))
            {
                log.Error(string.Format("PUT: api/ColorLists/:id - Resource: Not Found"));
                return NotFound();
            }
            
            ColorList existing = db.ColorLists.Find(id);
            
            existing.DisplayName = colorList.DisplayName;
            existing.Active = colorList.Active;
            db.Entry(existing).Collection(x => x.Colors).Load();
            if (colorList.Colors == null || colorList.Colors.Count == 0)
            {
                existing.Colors = null;
            }
            else
            {
                List<int> ids = colorList.Colors.Select(c => c.Id).ToList();
                existing.Colors = db.Materials.Where(x => ids.Contains(x.Id)).ToList<Material>();
            }

            db.Entry(existing).State = EntityState.Modified;
			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				if (!ColorListExists(id))
				{
                    log.Error(string.Format("PUT: api/ColorLists/:id"),ex);
					return NotFound();
				}
				else
				{
                    log.Error(string.Format("PUT: api/ColorLists/:id"),ex);
					throw;
				}
			}

			return StatusCode(HttpStatusCode.NoContent);
		}

		// POST: api/ColorLists
        [ResponseType(typeof(nec.nexu.JsonModels.ColorList))]
		public IHttpActionResult PostColorList(ColorList colorList)
		{
			if (!ModelState.IsValid)
			{
                log.Error(string.Format("POST: api/ColorLists - Resource: Not Valid"));
				return BadRequest(ModelState);
			}

            if (colorList.Colors == null)
            {
                log.Error(string.Format("POST: api/ColorLists - Resource: No Colors Present"));
                return BadRequest();
            }

            var ids = colorList.Colors.Select(c => c.Id).ToList();
            ColorList entity = new ColorList();
            entity.Active = true;
            entity.DisplayName = colorList.DisplayName;
            entity.Colors = db.Materials.Where(x => ids.Contains(x.Id)).ToList<Material>();
			db.ColorLists.Add(entity);
			db.SaveChanges();

            return Ok(entity);
		}

		// DELETE: api/ColorLists/5
        [ResponseType(typeof(void))]
		public IHttpActionResult DeleteColorList(int id)
		{
			ColorList colorList = db.ColorLists.Find(id);
			if (colorList == null)
			{
                log.Error(string.Format("DELETE: api/ColorLists - Resource: Not Found"));
				return NotFound();
			}

			db.ColorLists.Remove(colorList);
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

		private bool ColorListExists(int id)
		{
			return db.ColorLists.Count(e => e.Id == id) > 0;
		}
	}
}