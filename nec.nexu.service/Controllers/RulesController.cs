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
using nec.nexu.Models.rules;
using Sitecore.Form.Core.Attributes;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class RulesController : ApiController 
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region RequiredLogoRules
        /// <summary>
        /// List requiredlogorules
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/rule/requiredlogorules")]
        [ResponseType(typeof(List<RequiredLogoRule>))]
        public IHttpActionResult ListRequiredLogoRules()
        {
            return Ok(db.RulesRequiredLogos.ToList());
        }

        /// <summary>
        /// View requiredlogorule
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/rule/requiredlogorules/{id}")]
        [ResponseType(typeof(RequiredLogoRule))]
        public IHttpActionResult ViewRequiredLogoRule(int id)
        {

            return Ok(db.RulesRequiredLogos.Find(id));
        }

        /// <summary>
        /// Update requiredlogorule
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/rule/requiredlogorules/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateRequiredLogoRule(int id, RequiredLogoRule rlr)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rlr.Id)
            {
                return BadRequest();
            }

            RequiredLogoRule requiredLogo = db.RulesRequiredLogos.Find(id);
            if (requiredLogo != null)
            {
                requiredLogo.Active = rlr.Active;
                requiredLogo.DesignId = rlr.DesignId;
                requiredLogo.DisplayName = rlr.DisplayName;
                requiredLogo.SourceLocationId = rlr.SourceLocationId;
                requiredLogo.TargetDesignId = rlr.TargetDesignId;
                requiredLogo.TargetLocationId = rlr.TargetLocationId;
                requiredLogo.Type = rlr.Type;
                requiredLogo.UserMessage = rlr.UserMessage;

                db.Entry(requiredLogo).State = EntityState.Modified;
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
        /// Creates a new requiredlogorule
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/rule/requiredlogorules")]
        [ResponseType(typeof(RequiredLogoRule))]
        public IHttpActionResult AddRequiredLogoRule(RequiredLogoRule rlr)
        {
            db.RulesRequiredLogos.Add(rlr);
            db.SaveChanges();
            return Ok(rlr);
        }

        #endregion

        #region DesignColorRules
        /// <summary>
        /// List designcolorrules
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/rule/designcolorrules")]
        [ResponseType(typeof(List<DesignColorRule>))]
        public IHttpActionResult ListDesignColorRules()
        {
            return Ok(db.RulesDesignColor.ToList());
        }

        /// <summary>
        /// View designcolorrule
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/rule/designcolorrules/{id}")]
        [ResponseType(typeof(DesignColorRule))]
        public IHttpActionResult ViewDesignColorRule(int id)
        {

            return Ok(db.RulesDesignColor.Find(id));
        }

        /// <summary>
        /// Update designcolorrule
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/rule/designcolorrules/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateDesignColorRule(int id, DesignColorRule dcr)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dcr.Id)
            {
                return BadRequest();
            }

            DesignColorRule designColor = db.RulesDesignColor.Find(id);
            if (designColor != null)
            {
                designColor.Active = dcr.Active;
                designColor.DisplayName = dcr.DisplayName;
                designColor.UserMessage = dcr.UserMessage;
                designColor.DesignId = dcr.DesignId;
                designColor.Combination = dcr.Combination;
                designColor.ImageDataId = dcr.ImageDataId;

                designColor.Values.Clear();
                designColor.Values = dcr.Values;

                //designColor.Values = designColor.Values.Where(v => dcr.Values.Select(i => i.Id).Contains(v.Id)).ToList();

                //designColor.Values.Clear();
                //designColor.Values.AddRange(dcr.Values);

                db.Entry(designColor).State = EntityState.Modified;
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
        /// Creates a new designcolorrule
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/rule/designcolorrules")]
        [ResponseType(typeof(DesignColorRule))]
        public IHttpActionResult AddDesignColorRule(DesignColorRule dcr) 
        {
            db.RulesDesignColor.Add(dcr);
            db.SaveChanges();
            return Ok(dcr);
        }
        #endregion

        #region Color Differentiation RUle
        /// <summary>
        /// List ColorDifferentiationRule
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/rule/colordifferentiation")]
        [ResponseType(typeof(List<ColorDifferentiationRule>))]
        public IHttpActionResult ListColorDifferentiationRules()
        {
            return Ok(db.RulesColorDifferentiation.ToList());
        }

        /// <summary>
        /// View ColorDifferentiationRule
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/rule/colordifferentiation/{id}")]
        [ResponseType(typeof(ColorDifferentiationRule))]
        public IHttpActionResult ViewColorDifferentiationRule(int id)
        {

            return Ok(db.RulesColorDifferentiation.Find(id));
        }

        [HttpPost]
        [Route("api/rule/colordifferentiation")]
        [ResponseType(typeof(ColorDifferentiationRule))]
        public IHttpActionResult AddColorDifferentiationRule(ColorDifferentiationRule dcr)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RulesColorDifferentiation.Add(dcr);
            db.SaveChanges();
            return Ok(dcr);
        }

        [HttpPut]
        [Route("api/rule/colordifferentiation/{id}")]
        [ResponseType(typeof (void))]
        public IHttpActionResult UpdateColorDifferentiationRule(int id, ColorDifferentiationRule dcr)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dcr.Id)
            {
                return BadRequest();
            }

            ColorDifferentiationRule rule = db.RulesColorDifferentiation.Find(id);
            if (rule != null)
            {
                rule.DesignId = dcr.DesignId;
                rule.DisplayName = dcr.DisplayName;
                rule.RequiredColors = dcr.RequiredColors;
                rule.UserMessage = dcr.UserMessage;
            }

            db.Entry(rule).State = EntityState.Modified;
            

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }


        #endregion

        #region Colorway Rule
        /// <summary>
        /// List Colorway
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/rule/Colorway")]
        [ResponseType(typeof(List<ColorwayRule>))]
        public IHttpActionResult ListColorwayRules()
        {
            return Ok(db.RulesColorways.ToList());
        }

        /// <summary>
        /// View RulesColorways
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/rule/Colorway/{id}")]
        [ResponseType(typeof(ColorwayRule))]
        public IHttpActionResult ViewColorwayRule(int id)
        {

            return Ok(db.RulesColorways.Find(id));
        }

        [HttpPost]
        [Route("api/rule/Colorway")]
        [ResponseType(typeof(ColorwayRule))]
        public IHttpActionResult AddColorwayRule(ColorwayRule dcr)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var design = db.Designs.Find(dcr.DesignId);
            foreach (var v in dcr.Values)
            {
                v.ImageDataId = design.OfficialImage != null ? design.OfficialImage.Id : 0;
            } 

            db.RulesColorways.Add(dcr);
            db.SaveChanges();
            return Ok(dcr);
        }

        [HttpPut]
        [Route("api/rule/Colorway/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateColorwayRule(int id, ColorwayRule dcr)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dcr.Id)
            {
                return BadRequest();
            }

            ColorwayRule rule = db.RulesColorways.Find(id);
            if (rule != null)
            {
                var design = db.Designs.Find(dcr.DesignId);
                rule.DesignId = dcr.DesignId;
                rule.DisplayName = dcr.DisplayName;
                rule.UserMessage = dcr.UserMessage;

                rule.Values = rule.Values.Where(v => dcr.Values.Select(i => i.Id).Contains(v.Id)).ToList();
                rule.Values.AddRange(dcr.Values.Where(v => v.Id == null || v.Id == 0));

                foreach (var v in rule.Values)
                {
                    v.ImageDataId = design.OfficialImage != null ? design.OfficialImage.Id : 0;
                } 
            }

            db.Entry(rule).State = EntityState.Modified;


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }
        #endregion
    }
}