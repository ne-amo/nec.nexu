using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using nec.nexu.Models;
using nec.nexu.ViewModels;
using nec.nexu.site.admin.Models;

namespace nec.nexu.site.admin.Controllers
{
    public class ColorListController : Controller
    {

        private string apiUrl = "api/colorlists/";
        private string apiMaterials = "api/masterdata/materials";

        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    List<ColorListViewModel> list = await response.Content.ReadAsAsync<List<ColorListViewModel>>();
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
                ColorList ret = await client.Read<ColorList>(apiUrl + id.ToString());
                if (ret != null)
                {
                    return View(ret);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        public async Task<ActionResult> Create()
        {
            using (ApiConnector client = new ApiConnector())
            {
                List<Material> materials = await client.Read<List<Material>>(apiMaterials);
                ColorListAdminViewModel ret = new ColorListAdminViewModel { AllMaterials = materials };
                return View(ret);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(ColorListAdminViewModel entity)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(entity);
                
                using (ApiConnector client = new ApiConnector())
                {
                    ColorList list = new ColorList
                    {
                         DisplayName = entity.ColorList.DisplayName,
                         Active = true
                    };
                    //List<Material> materials = await client.Read<List<Material>>(apiMaterials);
                    list.Colors = entity.SelectedMaterialIds.Select(x => new Material { Id = x }).ToList();// materials.Where(x => entity.SelectedMaterialIds.Contains(x.Id)).ToList();
                    list = await client.Create<ColorList>(apiUrl, list);

                    if (list != null)
                        return RedirectToAction("Details", new { id = list.Id });
                    else
                        return RedirectToAction("Create");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {

                ColorList list = await client.Read<ColorList>(apiUrl + id.ToString());
                List<Material> materials = await client.Read<List<Material>>(apiMaterials);
                if (list != null && materials != null)
                {
                    ColorListAdminViewModel ret = new ColorListAdminViewModel();
                    ret.ColorList = list;
                    ret.AllMaterials = materials;
                    return View(ret);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, ColorListAdminViewModel view)
        {
            try
            {
                if(view.SelectedMaterialIds != null && view.SelectedMaterialIds.Count > 0)
                    view.ColorList.Colors = view.SelectedMaterialIds.Select(x => new Material { Id = x }).ToList();
                using (ApiConnector client = new ApiConnector())
                {
                    await client.Update<ColorList>(apiUrl, id, view.ColorList);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Edit", id);
            }
        }


        public ActionResult Delete(int id)
        {
            return View();
        }

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
