using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;
using nec.nexu.site.admin.Models;

namespace nec.nexu.site.admin.Controllers
{
    public class DesignLocationsController : Controller
    {
        // GET: DesignLocations
        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage rsp = await client.GetAsync(ApiRoutes.DESIGN_LOCATION_API_URL);
                if (rsp.IsSuccessStatusCode)
                {
                    IEnumerable<DesignLocation> list = await rsp.Content.ReadAsAsync<IEnumerable<DesignLocation>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            }
        }

        // GET: DesignLocations/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                DesignLocation ret = await client.Read<DesignLocation>(ApiRoutes.DESIGN_LOCATION_API_URL + id);
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

        // GET: DesignLocations/Create
        public async Task<ActionResult> Create()
        {
            using (ApiConnector client = new ApiConnector())
            {
                DesignLocation location = new DesignLocation();
                var templates = await client.Read<List<ProductTemplate>>(ApiRoutes.PRODUCT_TEMPLATES_API_URL);

                DesignLocationAdminViewModel vm = new DesignLocationAdminViewModel
                {
                    DesignLocation = location,
                    AllProductTemplates = templates
                };

                return View(vm);
            }
        }

        // POST: DesignLocations/Create
        [HttpPost]
        public async Task<ActionResult> Create(DesignLocationAdminViewModel entity)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(entity);

                using (ApiConnector client = new ApiConnector())
                {
                    DesignLocation dl = new DesignLocation();
                    dl.Build(entity.DesignLocation);

                    dl = await client.Create<DesignLocation>(ApiRoutes.DESIGN_LOCATION_API_URL, dl);
                    if (dl != null)
                    {
                        return RedirectToAction("Details", new {id = dl.Id});
                    }
                    else
                    {
                        return RedirectToAction("Create");
                    }

                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: DesignLocations/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                DesignLocation location = await client.Read<DesignLocation>(ApiRoutes.DESIGN_LOCATION_API_URL + id);
                var templates = await client.Read<List<ProductTemplate>>(ApiRoutes.PRODUCT_TEMPLATES_API_URL);
                DesignLocationAdminViewModel nm = new DesignLocationAdminViewModel
                {
                    DesignLocation = location,
                    AllProductTemplates = templates
                };

                return View(nm);
            }
        }

        // POST: DesignLocations/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, DesignLocationAdminViewModel view)
        {
            try
            {
                using (ApiConnector client = new ApiConnector())
                {
                    view.DesignLocation.Id = id;
                    await client.Update<DesignLocation>(ApiRoutes.DESIGN_LOCATION_API_URL, id, view.DesignLocation);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Edit", id);
            }
        }

        // GET: DesignLocations/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DesignLocations/Delete/5
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
