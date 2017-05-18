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
    public class RoalMaterialsController : Controller
    {
        private List<MaterialMessage> _GetRoalColors(string sessionkey)
        {
            string ph = sessionkey == STITCHES ? "0303N008": "0315";

            if (Session[sessionkey] == null)
            {
                List<MaterialMessage> roalColors = new List<MaterialMessage>();
                RoalServiceClient roal = new RoalServiceClient("ws");
                var r = roal.FindMaterialsByHierarchyCodes(
                    new MaterialListRequestMessage { SapHierarchyCodes = new string[] { ph } }).Materials;

                foreach (var arr in r.Values)
                {
                    foreach (MaterialMessage m in arr)
                    {
                        if (!roalColors.Any(x => x.Code == m.Code))
                            roalColors.Add(m);
                    }
                }
                roal.Close();
                Session[sessionkey] = roalColors;
            }
            var data = (List<MaterialMessage>)Session[sessionkey];
            return data;
        }
        private string MATERIALS = "roalmaterials";
        private string STITCHES = "roalstitches";

        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                List<MaterialToRoal> mappings = await client.Read<List<MaterialToRoal>>(ApiRoutes.ROAL_MATERIAL);
                if (mappings == null || mappings.Count == 0)
                {
                    return View(new List<RoalMapViewModel<MaterialToRoal>>());
                }

                List<Material> nexuColors = await client.Read<List<Material>>(ApiRoutes.MATERIALS_API_URL);

                List<MaterialMessage> roalColors = _GetRoalColors(MATERIALS);
                List<MaterialMessage> roalStitch = _GetRoalColors(STITCHES);

                List<RoalMapViewModel<MaterialToRoal>> data = new List<RoalMapViewModel<MaterialToRoal>>();

                foreach (MaterialToRoal map in mappings)
                {
                    MaterialMessage r = new MaterialMessage();
                    MaterialMessage s = new MaterialMessage();


                    if (!string.IsNullOrEmpty(map.FabricMaterialId))
                        r = roalColors.Where(x => x.Code == map.FabricMaterialId).FirstOrDefault();

                    if (!string.IsNullOrEmpty(map.StitchMaterialId))
                        s = roalStitch.Where(x => x.Code == map.StitchMaterialId).FirstOrDefault();

                    Material n = nexuColors.Where(x => x.Id == map.NexuMaterialId).FirstOrDefault();
                    data.Add(new RoalMapViewModel<MaterialToRoal>(map,
                        r.Description+" "+r.FccDescription,
                        n.DisplayName+" "+n.FabricContent.DisplayName,
                        s.Description));
                }
                
                return View(data);
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            return await getSingleItem(id);
        }

        [HttpGet]
        public JsonResult RoalColors()
        {
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = _GetRoalColors(MATERIALS) };
        }
        [HttpGet]
        public JsonResult RoalStitches()
        {
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = _GetRoalColors(STITCHES) };
        }

        public ActionResult Create()
        {
            return View();
        }

        public async Task<ActionResult> Edit(int id)
        {
            return await getSingleItem(id);
        }

        public async Task<ActionResult> Delete(int id)
        {
            return await getSingleItem(id);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id, FormCollection collection)
        {
            using (ApiConnector client = new ApiConnector())
            {
                await client.DeleteAsync(ApiRoutes.ROAL_DESIGNCOLOR +"?id=" + id);
            }
            return RedirectToAction("Index");
        }

        private async Task<ActionResult> getSingleItem(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                MaterialToRoal map = await client.Read<MaterialToRoal>(ApiRoutes.ROAL_MATERIAL + "?id=" + id);

                List<Material> nexuColors = await client.Read<List<Material>>(ApiRoutes.MATERIALS_API_URL);

                List<MaterialMessage> roalColors = _GetRoalColors(MATERIALS);
                List<MaterialMessage> roalStitch = _GetRoalColors(STITCHES);

                if (map != null)
                {
                    MaterialMessage r = new MaterialMessage();
                   MaterialMessage s = new MaterialMessage();


                    if(!string.IsNullOrEmpty(map.FabricMaterialId))
                        r = roalColors.Where(x => x.Code == map.FabricMaterialId).FirstOrDefault();

                    if (!string.IsNullOrEmpty(map.StitchMaterialId))
                        s = roalStitch.Where(x => x.Code == map.StitchMaterialId).FirstOrDefault();

                    Material n = nexuColors.Where(x => x.Id == map.NexuMaterialId).FirstOrDefault();

                    return View(
                        new RoalMapViewModel<MaterialToRoal>(map,
                            r.Description + " " + r.FccDescription,
                            n.DisplayName + " " + n.FabricContent.DisplayName,
                            s.Description)
                        );
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

    }
}