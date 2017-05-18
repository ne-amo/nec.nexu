using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;

namespace nec.nexu.site.admin.Controllers
{
    public class FabricContentTypesController : Controller
    {
        private string apiFCTs = "api/masterdata/fabriccontenttypes";
        private string apiFCT = "api/masterdata/fabriccontenttype/";


        // GET: FabricContentTypes
        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiFCTs);
                if (response.IsSuccessStatusCode)
                {
                    List<FabricContentType> list = await response.Content.ReadAsAsync<List<FabricContentType>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            }
        }

        // GET: FabricContentTypes/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiFCT + id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    FabricContentType ret = await response.Content.ReadAsAsync<FabricContentType>();
                    return View(ret);
                }
                else
                {
                    return View();
                }
            }
        }

        // GET: FabricContentTypes/Create
        public ActionResult Create()
        {
            return View(new FabricContentType());
        }

        // POST: FabricContentTypes/Create
        [HttpPost]
        public async Task<ActionResult> Create(FabricContentType fct)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(fct);

                using (ApiConnector client = new ApiConnector())
                {
                    FabricContentType newFCT = new FabricContentType()
                    {
                        Active = fct.Active,
                        DisplayName = fct.DisplayName,
                        FccCode = fct.FccCode
                    };
                    FabricContentType ret = await client.Create<FabricContentType>(apiFCT, newFCT);
                    if (ret != null)
                    {
                        return RedirectToAction("Details", new { id = ret.Id });
                    }
                    else
                    {
                        return View(fct);
                    }
                }
            }
            catch
            {
                return View(fct);
            }
        }

        // GET: FabricContentTypes/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiFCT + id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    FabricContentType ret = await response.Content.ReadAsAsync<FabricContentType>();
                    return View(ret);
                }
                else
                {
                    return View();
                }
            }
        }

        // POST: FabricContentTypes/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, FabricContentType fct)
        {
            try
            {
                if(!ModelState.IsValid)
                    return RedirectToAction("Edit", new { id = id });

                using (ApiConnector client = new ApiConnector())
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync(apiFCT + id, fct);
                    return RedirectToAction("Details", new {id = id});
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: FabricContentTypes/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FabricContentTypes/Delete/5
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
