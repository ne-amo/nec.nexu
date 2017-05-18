using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;
using nec.nexu.site.admin.Models;
using nec.nexu.ViewModels;

namespace nec.nexu.site.admin.Controllers
{


    public class ColorContainerController : Controller
    {


        private string apiUrl = "api/ColorContainers/";
        private string apiMaterials = "api/masterdata/materials/";



        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl+"?list=true");
                if (response.IsSuccessStatusCode)
                {
                    List<ListItem> list = await response.Content.ReadAsAsync<List<ListItem>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            }
        }


        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl+id+"?view=true");
                if (response.IsSuccessStatusCode)
                {
                    nec.nexu.JsonModels.ColorContainer c = await response.Content.ReadAsAsync<nec.nexu.JsonModels.ColorContainer>();
                    List<nec.nexu.JsonModels.Material> ac = await client.Read<List<nec.nexu.JsonModels.Material>>(apiMaterials);
                    ColorContainerViewModel m = new ColorContainerViewModel
                    {
                         ColorContainer = c,
                         AllColors = ac,
                         TotalMaterialsUsing = c.Products.Count()
                    };
                    return View(m);
                }
                else
                {
                    return View();
                }
            }
        }

        //
        // GET: /ColorContainer/Create

        public async Task<ActionResult> Create()
        {
            ColorContainerViewModel cc = new ColorContainerViewModel();
            using (ApiConnector client = new ApiConnector())
            {
                cc.AllColors = await client.Read<List<nec.nexu.JsonModels.Material>>(apiMaterials);
                return View(cc);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(ColorContainerViewModel view)
        {
            try
            {
                nec.nexu.JsonModels.ColorContainer cc = view.ColorContainer;
                if (view.SelectedColors != null && view.SelectedColors.Count > 0)
                    cc.Colors = view.SelectedColors.Select(x => new nec.nexu.JsonModels.Material { Id = x }).ToList();
                using (ApiConnector client = new ApiConnector())
                {
                    await client.Create<nec.nexu.JsonModels.ColorContainer>(apiUrl, cc);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Create");
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl+id+"?view=true");
                if (response.IsSuccessStatusCode)
                {
                    nec.nexu.JsonModels.ColorContainer c = await response.Content.ReadAsAsync<nec.nexu.JsonModels.ColorContainer>();
                    List<nec.nexu.JsonModels.Material> ac = await client.Read<List<nec.nexu.JsonModels.Material>>(apiMaterials);
                    ColorContainerViewModel m = new ColorContainerViewModel
                    {
                         ColorContainer = c,
                         AllColors = ac,
                         TotalMaterialsUsing = c.Products.Count()
                    };
                    return View(m);
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, ColorContainerViewModel view)
        {
            try
            {
                nec.nexu.JsonModels.ColorContainer cc = view.ColorContainer;
                if (view.SelectedColors != null && view.SelectedColors.Count > 0)
                    cc.Colors = view.SelectedColors.Select(x => new nec.nexu.JsonModels.Material { Id = x }).ToList();
                using (ApiConnector client = new ApiConnector())
                {
                    await client.Update<nec.nexu.JsonModels.ColorContainer>(apiUrl, id, cc);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Edit", id);
            }
        }

        //
        // GET: /ColorContainer/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ColorContainer/Delete/5

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
