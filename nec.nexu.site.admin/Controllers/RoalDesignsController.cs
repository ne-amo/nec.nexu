using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;
using nec.nexu.site.admin.Models;
using nec.nexu.ViewModels;
using nec.nexu.Models.roal;
using Nec.Infrastructure.Messages;

namespace nec.nexu.site.admin.Controllers
{
    public class RoalDesignsController : Controller
    {

        private List<ColorListMessage> _GetRoalColors()
        {
            if (Session["RoalDesignColors"] == null)
            {
                RoalServiceClient roal = new RoalServiceClient("ws");
                List<ColorListMessage> roalColors = roal.FindColorListByExample(new ColorListMessage { ColorGroup = 2 }).ToList();
                roal.Close();
                Session["RoalDesignColors"] = roalColors;
            }
            var data = (List<ColorListMessage>)Session["RoalDesignColors"];
            return data;
        }

        private ColorwayMessage _GetColorway(int illustrationId)
        {
            RoalServiceClient roal = new RoalServiceClient("ws");
            List<ColorwayMessage> roalColors = roal.FindColorwayByExample(new ColorwayMessage { IllustrationId = illustrationId, IsDefault = true }).ToList();
            roal.Close();
            return roalColors.FirstOrDefault();
        }

        private IllustrationMessage _GetIllustration(int id)
        {
            RoalServiceClient roal = new RoalServiceClient("ws");
            IllustrationMessage roalColors = roal.FindIllustration(id);
            roal.Close();
            return roalColors;
        }
        private List<IllustrationMessage> _GetIllustration(string number)
        {
            RoalServiceClient roal = new RoalServiceClient("ws");
            IllustrationMessage[] roalColors = roal.FindIllustrationByExample(new IllustrationMessage { ManualIllustrationNumber = number });
            roal.Close();
            return roalColors.ToList();
        }

        [HttpGet]
        public JsonResult RoalColorway(int illustrationId)
        {
            return new JsonResult { Data = _GetColorway(illustrationId), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        //
        // GET: /RoalDesigns/

        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                List<DesignToRoal> mappings = await client.Read<List<DesignToRoal>>(ApiRoutes.ROAL_DESIGN);

                if (mappings == null || mappings.Count == 0)
                {
                    return View(new List<DesignColorToRoal>());
                }

                List<RoalMapViewModel<DesignToRoal>> data = new List<RoalMapViewModel<DesignToRoal>>();
                foreach (DesignToRoal d in mappings)
                {

                    Design design = await client.Read<Design>(ApiRoutes.DESIGNS_API_URL+"/"+d.NexuDesignId);
                    data.Add(new RoalMapViewModel<DesignToRoal>(d, design.TapeNumber, design.TapeNumber));
                }

                return View(data);
            }
        }

        //
        // GET: /RoalDesigns/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /RoalDesigns/Create

        public ActionResult Create()
        {
            return View();
        }

        private async Task<bool> isAlreadyMapped(string tapenumber)
        {

            using (ApiConnector client = new ApiConnector())
            {
                var nexudesigns = await client.Read<List<Design>>("api/designs/search/?tape=" + tapenumber);
                if (nexudesigns.Any())
                {
                    var map = await client.Read<DesignToRoal>("api/roal/design/" + nexudesigns.First().Id);
                    if (map == null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(FormCollection form)
        {
            if (form["command"] == "search")
            {
                string tape = form["tapenumber"];
                if (string.IsNullOrEmpty(tape))
                {
                    ModelState.AddModelError("error", "You must enter a tape number");
                }
                else
                {
                    
                    bool AlreadyMapped = await isAlreadyMapped(tape);
                    if (AlreadyMapped)
                    {
                        ModelState.AddModelError("error", "Already Mapped. Use Edit.");
                        return View();
                    }

                    RoalServiceClient roal = new RoalServiceClient("ws");
                    IllustrationMessage[] roaldesigns = roal.FindIllustrationByExample(new IllustrationMessage { ManualIllustrationNumber = tape });
                    if (roaldesigns.Length == 0)
                    {
                        ModelState.AddModelError("error", "ROAL Illustration Not Found");
                        return View();
                    }
                    ColorwayMessage roalColorway = roal.FindColorwayByExample(new ColorwayMessage { IllustrationId = (int)roaldesigns.First().Id, IsDefault = true }).First();
                    roal.Close();

                    Design nexudesign = null;
                    DesignColorway nexuColorway = null;
                    using (ApiConnector client = new ApiConnector())
                    {
                        var nexudesigns = await client.Read<List<Design>>("api/designs/search/?tape=" + tape);
                        if (nexudesigns.Count == 0)
                        {
                            ModelState.AddModelError("error", "NEXU Design Not Found");
                            return View();
                        }
                        nexudesign = nexudesigns.First();
                        nexuColorway = await client.Read<DesignColorway>(string.Format("api/designs/defaultcolorway/?designId={0}",nexudesign.Id));
                    }
                    ViewBag.TapeNumber = tape;
                    ViewBag.RoalDesign = roaldesigns.First();
                    ViewBag.RoalColorway = roalColorway;
                    ViewBag.NexuDesign = nexudesign;
                    ViewBag.NexuColorway = nexuColorway;
                    ViewBag.SearchPostBack = true;
                }
            }
            else if (form["command"] == "create")
            {
                var roalId = int.Parse(form["roalId"]);
                var nexuId = int.Parse(form["nexuId"]);
                var mapping = form["mapping"];

                DesignToRoal msg = new DesignToRoal { _SerializedDictionary = mapping, NexuDesignId = nexuId, RoalDesignId = roalId };
                using (ApiConnector client = new ApiConnector())
                {
                    var response = await client.PostAsJsonAsync("api/roal/design/", msg);
                    if (!response.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError("error", "Failed submitting to NEXU");
                        return View();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(form["REFRESH"]))
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            return View();
                        }
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetRoalByTape(string tape)
        {
            if(string.IsNullOrEmpty(tape))
            {
                throw new Exception("Invalid request");
            }
            RoalServiceClient roal = new RoalServiceClient("ws");
            IllustrationMessage[] i = roal.FindIllustrationByExample(new IllustrationMessage { ManualIllustrationNumber = tape });
            roal.Close();
            return new JsonResult { Data = i, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpGet]
        public async Task<JsonResult> GetNexuByTape(string tape)
        {
            if (string.IsNullOrEmpty(tape))
            {
                throw new Exception("Invalid request");
            }

            using (ApiConnector client = new ApiConnector())
            {
                List<Design> i = await client.Read<List<Design>>("api/designs/search/?tape=" + tape);
                return new JsonResult { Data = i, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //

        //
        // GET: /RoalDesigns/Edit/5

        public async Task<ActionResult> Edit(int id)
        {
            DesignToRoal map = null;
            Design nexudesign = null;
            DesignColorway nexuColorway = null;


            using (ApiConnector client = new ApiConnector())
            {
                map = await client.Read<DesignToRoal>("api/roal/design/" + id);
                nexudesign = await client.Read<Design>("api/designs/" + map.NexuDesignId);
                nexuColorway = await client.Read<DesignColorway>(string.Format("api/designs/defaultcolorway/?designId={0}", nexudesign.Id));
            }


            RoalServiceClient roal = new RoalServiceClient("ws");
            IllustrationMessage roaldesign = roal.FindIllustration(map.RoalDesignId);
            ColorwayMessage roalColorway = roal.FindColorwayByExample(new ColorwayMessage { IllustrationId = (int)roaldesign.Id, IsDefault = true }).First();
            roal.Close();

            Dictionary<int,string> mapping = new Dictionary<int,string>();
            foreach (var nexuLayer in map.RoalSequenceOrder_to_NexuLayerId.GroupBy(x=>x.Value))
            {
                mapping.Add(nexuLayer.Key, String.Join(",",nexuLayer.ToArray()));
            }

            ViewBag.Original = map;
            ViewBag.TapeNumber = roaldesign.ManualIllustrationNumber;
            ViewBag.RoalDesign = roaldesign;
            ViewBag.RoalColorway = roalColorway;
            ViewBag.NexuDesign = nexudesign;
            ViewBag.NexuColorway = nexuColorway;
            ViewBag.SearchPostBack = true;
            ViewBag.Mapping = mapping;

            return View();
        }

        //
        // POST: /RoalDesigns/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /RoalDesigns/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /RoalDesigns/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
