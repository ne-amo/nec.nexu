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
    public class TemplateViewsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/<controller>
        public IHttpActionResult Get()
        {
            return Ok(db.TemplateViews.ToList());
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            TemplateView templateView = db.TemplateViews.Find(id);
            if (templateView == null)
            {
                log.Error(string.Format("GET: api/templateviews/:id - Resource: Not Found"));
                return NotFound();
            }
            return Ok(templateView);
        }

        public IHttpActionResult Create(TemplateView entity)
        {
            if (entity.Id != 0)
            {
                return BadRequest("Entity already exists. Please use update method");
            }
            db.TemplateViews.Add(entity);
            db.SaveChanges();
            return Ok(entity);
        }

        [HttpGet]
        [Route("api/templateview/clip/{id}")]
        public IHttpActionResult ViewClipMask(int id)
        {
            DesignRenderSetting setting = db.DesignRenderSettings.Find(id);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(setting.ClippingMask);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            return ResponseMessage(response);
        }

        [HttpPost]
        [Route("api/templateview/rendersetting/create")]
        public IHttpActionResult Create(DesignRenderSetting entity)
        {
            if (entity.Id != 0)
            {
                return BadRequest("Entity already exists. Please use update method");
            }
            db.DesignRenderSettings.Add(entity);
            db.SaveChanges();
            return Ok(entity);
        }

        [HttpPut]
        [Route("api/templateview/updatesvg/{viewId}")]
        public IHttpActionResult UpdateSVG(int viewId, TemplateView view)
        {
            TemplateView exsiting = db.TemplateViews.Find(viewId);
            if (exsiting != null && view !=null)
            {
                exsiting.SvgContent = view.SvgContent;
                db.Entry(exsiting).State = EntityState.Modified;
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


        /// <summary>
        /// Return TemplateImage by ImageDataId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/templateimage/byimagedata/{id}")]
        [ResponseType(typeof(ImageDataCache))]
        public IHttpActionResult GetImageByImageData(int id)
        {
            var ob = db.GetTemplateImageData(id);
            return Ok(ob);
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var view = db.TemplateViews.Find(id);
            var imageDataIds = view.Images.Select(y => y.Id).ToArray();
            db.ImageDatas.RemoveRange(db.ImageDatas.Where(x => imageDataIds.Contains(x.Id)));
            db.TemplateImages.RemoveRange(view.Images);
            db.TemplateViews.Remove(view);
            db.SaveChanges();
            return Ok();
        }


        /*
        public IHttpActionResult Get(ProductTemplate productTemplate)
        {
            ProductTemplate template = db.ProductTemplates.Find(productTemplate.Id);
            if (template == null || template.Views == null)
            {
                return NotFound();
            }
            return Ok(template.Views);
        }
        */

    }
}