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
    public class RoalDesignColorsController : Controller
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


        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                List<DesignColorToRoal> mappings = await client.Read<List<DesignColorToRoal>>(ApiRoutes.ROAL_DESIGNCOLOR);
                if (mappings == null || mappings.Count == 0)
                {
                    return View(new List<DesignColorToRoal>());
                }

                List<DesignColor> nexuColors = await client.Read<List<DesignColor>>(ApiRoutes.DESIGN_COLORS_API_URL);

                List<ColorListMessage> roalColors = _GetRoalColors();

                List<RoalMapViewModel<DesignColorToRoal>> data = new List<RoalMapViewModel<DesignColorToRoal>>();

                foreach(DesignColorToRoal map in mappings)
                {
                    ColorListMessage r = roalColors.Where(x=>x.Id == map.RoalDesignColorId).FirstOrDefault();
                    DesignColor n = nexuColors.Where(x=>x.Id == map.NexuDesignColorId).FirstOrDefault();
                    data.Add(new RoalMapViewModel<DesignColorToRoal>(map, r.Description, n.DisplayName));
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
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = _GetRoalColors() };
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
                DesignColorToRoal map = await client.Read<DesignColorToRoal>(ApiRoutes.ROAL_DESIGNCOLOR + "?id=" + id);
                List<DesignColor> nexuColors = await client.Read<List<DesignColor>>(ApiRoutes.DESIGN_COLORS_API_URL);
                List<ColorListMessage> roalColors = _GetRoalColors();
                if (map != null)
                {
                    ColorListMessage r = roalColors.Where(x => x.Id == map.RoalDesignColorId).FirstOrDefault();
                    DesignColor n = nexuColors.Where(x => x.Id == map.NexuDesignColorId).FirstOrDefault();

                    return View(new RoalMapViewModel<DesignColorToRoal>(map, r.Description, n.DisplayName));
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

    }
}