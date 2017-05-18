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
using nec.nexu.Models.roal;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class RoalController : ApiController
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
        [Route("api/roal/{materialId}")]
        public IHttpActionResult GetRoalMessage(int materialId)
        {
            FinishedProduct fp = db.FinishedProducts.FirstOrDefault(x => x.MaterialId == materialId);
            if (fp == null)
                return BadRequest("Material Id "+materialId+" Not Found");
            try
            {
                NexuToRoalMessage message = new NexuToRoalMessage();
                message.CountryCode = fp.CountryCode;
                HierarchyToRoal hierarchy = db.Roal_HierarchyMap.First(x => x.NexuHierarchyId == fp.Hierarchy.Id);
                TemplateToRoal template = db.Roal_TemplateMap.First(x => x.NexuTemplateId == fp.Template.Id);
                message = _Build(message, hierarchy, template, fp.Regions, fp.Designs);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #region Design to ROAL

        [HttpGet]
        [Route("api/roal/design/")]
        public IHttpActionResult GetDesignToRoal(int id = 0, int nexuId = 0, int roalId = 0)
        {
            List<DesignToRoal> data = new List<DesignToRoal>();
            if (nexuId == 0 && roalId == 0 && id == 0)
            {
                data = db.Roal_DesignMap.ToList();
            }
            if (id != 0)
            {
                return Ok(db.Roal_DesignMap.Find(id));
            }
            if (nexuId != 0 && roalId == 0)
            {
                data = db.Roal_DesignMap.Where(x => x.NexuDesignId == nexuId).ToList();
            }
            if (nexuId == 0 && roalId != 0)
            {
                data = db.Roal_DesignMap.Where(x => x.RoalDesignId == roalId).ToList();
            }
            if (nexuId != 0 && roalId != 0)
            {
                data = db.Roal_DesignMap.Where(x => x.NexuDesignId == nexuId && x.RoalDesignId == roalId).ToList();
            }
            return Ok(data);
        }
        [HttpPost]
        [Route("api/roal/design/")]
        public IHttpActionResult PostDesignToRoal(DesignToRoal msg)
        {
            if (msg.NexuDesignId == 0 && msg.RoalDesignId == 0 )
            {
                return BadRequest("Invalid parameters");
            }
            if (db.Roal_DesignMap.Any(x => x.NexuDesignId == msg.NexuDesignId))
            {
                return BadRequest("NExU material is already mapped");
            }

            db.Roal_DesignMap.Add(msg);
            db.SaveChanges();

            return Ok(msg);
        }
        [HttpPut]
        [Route("api/roal/design/{id}")]
        public IHttpActionResult PutDesignToRoal(int id, DesignToRoal msg)
        {

            if (id == 0)
            {
                return BadRequest("Invalid parameters");
            }
            if (msg.NexuDesignId == 0 && msg.RoalDesignId == 0)
            {
                return BadRequest("Invalid parameters");
            }

            DesignToRoal toUpdate = db.Roal_DesignMap.Find(id);
            if (toUpdate == null)
                return BadRequest("Record does not exist");

            toUpdate.NexuDesignId = msg.NexuDesignId;
            toUpdate.RoalDesignId = msg.RoalDesignId;
            toUpdate._SerializedDictionary = msg._SerializedDictionary;

            db.Entry(toUpdate).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(toUpdate);
        }
        [HttpDelete]
        [Route("api/roal/design/")]
        public IHttpActionResult DeleteDesignToRoal(int id)
        {

            if (id == 0)
            {
                return BadRequest("Invalid parameters");
            }

            DesignToRoal toDelete = db.Roal_DesignMap.Find(id);
            if (toDelete == null)
                return BadRequest("Entity does not exist");


            db.Roal_DesignMap.Remove(toDelete);
            db.SaveChanges();

            return Ok();
        }

        #endregion // Design to ROAL



        #region Material to ROAL

        [HttpGet]
        [Route("api/roal/material/")]
        public IHttpActionResult GetMaterialToRoal(int id = 0, int nexuId = 0, string roalId = null)
        {
            List<MaterialToRoal> data = new List<MaterialToRoal>();
            if (nexuId == 0 && string.IsNullOrEmpty(roalId) && id == 0)
            {
                data = db.Roal_MaterialMap.ToList();
            }
            if (id != 0)
            {
                return Ok(db.Roal_MaterialMap.Find(id));
            }
            if (nexuId != 0 && string.IsNullOrEmpty(roalId))
            {
                data = db.Roal_MaterialMap.Where(x => x.NexuMaterialId == nexuId).ToList();
            }
            if (nexuId == 0 && !string.IsNullOrEmpty(roalId))
            {
                data = db.Roal_MaterialMap.Where(x => x.FabricMaterialId == roalId).ToList();
            }
            if (nexuId != 0 && !string.IsNullOrEmpty(roalId))
            {
                data = db.Roal_MaterialMap.Where(x => x.NexuMaterialId == nexuId && x.FabricMaterialId == roalId).ToList();
            }
            return Ok(data);
        }
        [HttpPost]
        [Route("api/roal/material/")]
        public IHttpActionResult PostMaterialToRoal(MaterialToRoal msg)
        {
            if (msg.NexuMaterialId == 0 && string.IsNullOrEmpty(msg.FabricMaterialId) && string.IsNullOrEmpty(msg.StitchMaterialId))
            {
                return BadRequest("Invalid parameters");
            }
            if (db.Roal_MaterialMap.Any(x => x.NexuMaterialId == msg.NexuMaterialId))
            {
                return BadRequest("NExU material is already mapped");
            }

            db.Roal_MaterialMap.Add(msg);
            db.SaveChanges();

            return Ok(msg);
        }
        [HttpPut]
        [Route("api/roal/material/{id}")]
        public IHttpActionResult PutMaterialToRoal(int id, MaterialToRoal msg)
        {

            if (id == 0)
            {
                return BadRequest("Invalid parameters");
            }
            if (msg.NexuMaterialId == 0 && string.IsNullOrEmpty(msg.FabricMaterialId)  && string.IsNullOrEmpty(msg.StitchMaterialId))
            {
                return BadRequest("Invalid parameters");
            }

            MaterialToRoal toUpdate = db.Roal_MaterialMap.Find(id);
            if (toUpdate == null)
                return BadRequest("Record does not exist");

            toUpdate.NexuMaterialId = msg.NexuMaterialId;
            toUpdate.FabricMaterialId = msg.FabricMaterialId;
            toUpdate.StitchMaterialId = msg.StitchMaterialId;

            db.Entry(toUpdate).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(toUpdate);
        }
        [HttpDelete]
        [Route("api/roal/material/")]
        public IHttpActionResult DeleteMaterialToRoal(int id)
        {

            if (id == 0)
            {
                return BadRequest("Invalid parameters");
            }

            MaterialToRoal toDelete = db.Roal_MaterialMap.Find(id);
            if (toDelete == null)
                return BadRequest("Entity does not exist");


            db.Roal_MaterialMap.Remove(toDelete);
            db.SaveChanges();

            return Ok();
        }

        #endregion // Material to ROAL


        #region DesignColor to ROAL

        [HttpGet]
        [Route("api/roal/designcolor/")]
        public IHttpActionResult GetDesignColorToRoal(int id = 0, int nexuId = 0, int roalId = 0)
        {
            List<DesignColorToRoal> data = new List<DesignColorToRoal>();
            if (nexuId == 0 && roalId == 0 && id == 0)
            {
                data = db.Roal_DesignColorMap.ToList();
            }
            if (id != 0)
            {
                return Ok(db.Roal_DesignColorMap.Find(id));
            }
            if (nexuId != 0 && roalId == 0)
            {
                data = db.Roal_DesignColorMap.Where(x=>x.NexuDesignColorId == nexuId).ToList();
            }
            if (nexuId == 0 && roalId != 0)
            {
                data = db.Roal_DesignColorMap.Where(x => x.RoalDesignColorId == roalId).ToList();
            }
            if (nexuId != 0 && roalId != 0)
            {
                data = db.Roal_DesignColorMap.Where(x => x.NexuDesignColorId == nexuId && x.RoalDesignColorId == roalId).ToList();
            }
            return Ok(data);
        }
        [HttpPost]
        [Route("api/roal/designcolor/")]
        public IHttpActionResult PostDesignColorToRoal(DesignColorToRoal msg)
        {
            if (msg.NexuDesignColorId == 0 && msg.RoalDesignColorId == 0)
            {
                return BadRequest("Invalid parameters");
            }
            if(db.Roal_DesignColorMap.Any(x => x.NexuDesignColorId == msg.NexuDesignColorId))
            {
                return BadRequest("NExU color is already mapped");
            }

            db.Roal_DesignColorMap.Add(msg);
            db.SaveChanges();

            return Ok(msg);
        }
        [HttpPut]
        [Route("api/roal/designcolor/{id}")]
        public IHttpActionResult PutDesignColorToRoal(int id, DesignColorToRoal msg)
        {

            if (id == 0)
            {
                return BadRequest("Invalid parameters");
            }
            if (msg.NexuDesignColorId == 0 && msg.RoalDesignColorId == 0)
            {
                return BadRequest("Invalid parameters");
            }

            DesignColorToRoal toUpdate = db.Roal_DesignColorMap.Find(id);
            if (toUpdate == null)
                return BadRequest("Record does not exist");

            toUpdate.NexuDesignColorId = msg.NexuDesignColorId;
            toUpdate.RoalDesignColorId = msg.RoalDesignColorId;

            db.Entry(toUpdate).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(toUpdate);
        }
        [HttpDelete]
        [Route("api/roal/designcolor/")]
        public IHttpActionResult DeleteDesignColorToRoal(int id)
        {

            if (id == 0)
            {
                return BadRequest("Invalid parameters");
            }

            DesignColorToRoal toDelete = db.Roal_DesignColorMap.Find(id);
            if (toDelete == null)
                return BadRequest("Entity does not exist");


            db.Roal_DesignColorMap.Remove(toDelete);
            db.SaveChanges();

            return Ok();
        }

        #endregion // DesignColor to ROAL


        #region Private Build methods

        private NexuToRoalMessage _Build(NexuToRoalMessage message,
            HierarchyToRoal Hmap,
            TemplateToRoal Tmap,
            List<FinishedRegion> Rmap,
            List<FinishedDesign> Dmap)
        {
            message = _Build(message, Hmap);
            message = _Build(message, Tmap);
            message = _Build(message, Rmap);
            message = _BuildStitches(message, Rmap);
            message = _BuildVisorStitch(message, Rmap);
            message = _BuildBartack(message, Rmap);
            message = _Build(message, Dmap);
            return message;
        }

        private NexuToRoalMessage _Build(NexuToRoalMessage message, HierarchyToRoal map)
        {
            message.BuCode = map.RoalBuCode;
            message.SportCode = map.RoalSportCode;
            message.LicenseCode = map.RoalLicenseCode;
            message.TeamCode = map.RoalTeamCode;
            message.ClassCode = map.RoalClassCode;
            return message;
        }
        private NexuToRoalMessage _Build(NexuToRoalMessage message, TemplateToRoal map)
        {
            message.ConfigurationId = map.RoalConfigurationId;
            message.SilhouetteCode = map.SilhouetteCode;
            message.GenderId = map.GenderId;
            message.SeasonId = map.SeasonId;
            message.CollectionId = map.CollectionId;
            return message;
        }
        private NexuToRoalMessage _Build(NexuToRoalMessage message, List<FinishedRegion> map)
        {
            foreach (FinishedRegion region in map)
            {
                ProductRegionToRoal r = db.Roal_ProductRegionMap.FirstOrDefault(x => x.NexuProductRegionId == region.Region.Id && x.RoalType == ROALRegionType.REGION);
                if (r == null)
                    continue;
                MaterialToRoal m = db.Roal_MaterialMap.First(x => x.NexuMaterialId == region.Material.Id);
                message.RegionMaterials.Add(r.RoalProductRegionId,m.FabricMaterialId);/*
                    new Tuple<string, string>(m.FabricMaterialId, m.StitchMaterialId)
                    );*/
            }
            return message;
        }
        private NexuToRoalMessage _BuildStitches(NexuToRoalMessage message, List<FinishedRegion> map)
        {

            var mappings = db.Roal_ProductRegionMap.Where(x => x.RoalType == ROALRegionType.STITCH).ToList();

            // This is driven by the mapping data rather than the regions inb NEXU. ROAL can have more data that NEXU. Might have to change again, or expand to other build methods - amo 1-2017
            foreach (var mapping in mappings)
            {
                try
                {
                    var mappedRegion = map.FirstOrDefault(x => x.Region.Id == mapping.NexuProductRegionId);
                    if (mappedRegion == null)
                        continue;
                    MaterialToRoal m = db.Roal_MaterialMap.First(x => x.NexuMaterialId == mappedRegion.Material.Id);
                    message.StitchMaterials.Add(mapping.RoalProductRegionId, m.StitchMaterialId);
                }
                catch (Exception ex)
                {
                    string h = "h";
                }
            }

            //foreach (FinishedRegion region in map)
            //{
            //    var items = db.Roal_ProductRegionMap.Where(x => x.NexuProductRegionId == region.Region.Id && x.RoalType == ROALRegionType.STITCH).ToList();
            //    foreach (var r in items)
            //    {
            //        MaterialToRoal m = db.Roal_MaterialMap.First(x => x.NexuMaterialId == region.Material.Id);
            //         message.StitchMaterials.Add(r.RoalProductRegionId,m.StitchMaterialId);
            //    }
            //
/*          //
            //    ProductRegionToRoal r = db.Roal_ProductRegionMap.FirstOrDefault(x => x.NexuProductRegionId == region.Region.Id && x.RoalType == ROALRegionType.STITCH);
            //    if (r == null)
            //        continue;
            //
            //        new Tuple<string, string>(m.FabricMaterialId, m.StitchMaterialId)
            //        );*/
            //}
            return message;
        }
        private NexuToRoalMessage _BuildVisorStitch(NexuToRoalMessage message, List<FinishedRegion> map)
        {
            foreach (FinishedRegion region in map)
            {
                ProductRegionToRoal r = db.Roal_ProductRegionMap.FirstOrDefault(x => x.NexuProductRegionId == region.Region.Id && x.RoalType == ROALRegionType.VISOR_STITCH);
                if (r == null)
                    continue;
                MaterialToRoal m = db.Roal_MaterialMap.First(x => x.NexuMaterialId == region.Material.Id);
                message.StitchMaterials.Add(r.RoalProductRegionId, m.DCStitchMaterialId);/*
                    new Tuple<string, string>(m.FabricMaterialId, m.StitchMaterialId)
                    );*/
            }
            return message;
        }
        private NexuToRoalMessage _BuildBartack(NexuToRoalMessage message, List<FinishedRegion> map)
        {
            foreach (FinishedRegion region in map)
            {
                ProductRegionToRoal r = db.Roal_ProductRegionMap.FirstOrDefault(x => x.NexuProductRegionId == region.Region.Id && x.RoalType == ROALRegionType.BARTACK_LEFT);
                if (r == null)
                {
                    ProductRegionToRoal r2 = db.Roal_ProductRegionMap.FirstOrDefault(x => x.NexuProductRegionId == region.Region.Id && x.RoalType == ROALRegionType.BARTACK_RIGHT);
                    if (r2 == null)
                    {
                        continue;
                    }
                    else
                    {
                        MaterialToRoal m = db.Roal_MaterialMap.First(x => x.NexuMaterialId == region.Material.Id);
                        message.StitchMaterials.Add(r2.RoalProductRegionId, m.BartackRight);
                    }
                }
                else
                {
                    MaterialToRoal m = db.Roal_MaterialMap.First(x => x.NexuMaterialId == region.Material.Id);
                    message.StitchMaterials.Add(r.RoalProductRegionId, m.BartackLeft);
                }
            }
            return message;
        }
        private NexuToRoalMessage _Build(NexuToRoalMessage message, List<FinishedDesign> map)
        {
            foreach (FinishedDesign finishedDesign in map)
            {
                DesignLocationToIllustration location = db.Roal_DesignLocationMap.FirstOrDefault(x => x.NexuLocationId == finishedDesign.Location.Id);
                DesignToRoal design = db.Roal_DesignMap.First(x => x.NexuDesignId == finishedDesign.DesignColorway.Design.Id);

                NexuToRoalMessage.DesignSetting setting = new NexuToRoalMessage.DesignSetting();
                setting.DesignId = design.RoalDesignId;
                setting.LocationId = location.RoalIllustrationId;

                foreach (KeyValuePair<int, int> roalsequence in design.RoalSequenceOrder_to_NexuLayerId)
                {
                    DesignLayer layer = finishedDesign.DesignColorway.Layers.First(x => x.ImageDataId == roalsequence.Value);
                    DesignColorToRoal color = db.Roal_DesignColorMap.First(x => x.NexuDesignColorId == layer.Color.Id);
                    setting.Settings.Add(roalsequence.Key, color.RoalDesignColorId);

                    //message.Designs.Add(new Tuple<int, int, int, int>(

                    //    location.RoalIllustrationId,
                    //    design.RoalDesignId,
                    //    roalsequence.Key,
                    //    color.RoalDesignColorId)
                    //);
                }

                message.Designs.Add(setting);
            }
            return message;
        }
        private List<ProductRegion> GetRegions(ICollection<ProductRegionGroup> x)
        {
            List<ProductRegion> regions = new List<ProductRegion>();
            foreach (ProductRegionGroup group in x)
            {
                regions.AddRange(group.Regions);
            }
            return regions;
        }

        #endregion // Private builds


        [HttpGet]
        [Route("api/roal/fix/")]
        public IHttpActionResult Fix(int id)
        {
            //foreach (var entry in db.Roal_DesignMap.Where(x=>x.Id == id).ToList())
            //{
            //    try
            //    {
            //        List<string> newValues = new List<string>();
            //        foreach (var combo in entry.RoalSequenceOrder_to_NexuLayerId)
            //        {
            //            newValues.Add(string.Format("{0}&{1}", combo.Key, db.DesignLayers.Find(combo.Value).ImageDataId));
            //        }
            //        entry._SerializedDictionary = String.Join("|", newValues.ToArray());
            //        db.Entry(entry).State = EntityState.Modified;
            //    }
            //    catch (Exception ex)
            //    {
            //        return BadRequest(ex.Message);
            //    }
            //}
            //db.SaveChanges();
            return Ok();
        }


    }
}