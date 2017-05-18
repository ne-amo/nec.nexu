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
    public class ProductTemplateController : Controller
    {

        private string apiUrl = "api/ProductTemplates/";

        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    List<ProductTemplate> list = await response.Content.ReadAsAsync<List<ProductTemplate>>();
                    return View(list.Select(x=> new ProductTemplate { Id = x.Id, DisplayName = x.DisplayName }));
                }
                else
                {
                    return View();
                }
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductTemplate data)
        {
            try
            {
                using (ApiConnector client = new ApiConnector())
                {
                    ProductTemplate t = new ProductTemplate();
                    data.DefaultMaterial = new Material { Id = 57 };
                    var template = await client.Create<ProductTemplate>(apiUrl, data);
                    return new JsonResult { Data = template.Id, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch(Exception ex)
            {
                return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id, FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
