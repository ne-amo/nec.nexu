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

namespace nec.nexu.site.admin.Controllers
{
    public class HierarchyController : Controller
    {
        //
        // GET: /Hierarchy/

        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage rsp = await client.GetAsync("api/hierarchies/search");
                if (rsp.IsSuccessStatusCode)
                {
                    IEnumerable<Hierarchy> list = await rsp.Content.ReadAsAsync<IEnumerable<Hierarchy>>();
                    foreach (Hierarchy h in list)
                    {
                        if (h.ParentHierarchyId.HasValue)
                        {
                            h.ParentHierarchy = list.First(x => x.Id == h.ParentHierarchyId.Value);
                        }
                        else
                        {
                            h.ParentHierarchy = new Hierarchy();
                        }
                    }

                    return View(list);
                }
                else 
                {
                    return View();
                }
            }
        }

        //
        // GET: /Hierarchy/Details/5

        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                Hierarchy ret = await client.Read<Hierarchy>(ApiRoutes.HIERARCHY_API_URL + id.ToString());
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

        //
        // GET: /Hierarchy/Create

        public async Task<ActionResult> Create()
        {
            using (ApiConnector client = new ApiConnector())
            {
                var parHie = await client.Read<List<Hierarchy>>("api/hierarchies/search");
                var templates = await client.Read<List<ProductTemplate>>("api/ProductTemplates?list=true");
                ViewBag.ProductTemplate = templates;
                HierarchyAdminViewModel vm = new HierarchyAdminViewModel {
                    AllHierarchies = parHie
                    ,Hierarchy = new Hierarchy()
                };
                return View(vm);
            }
        }

        //
        // POST: /Hierarchy/Create

        [HttpPost]
        public async Task<ActionResult> Create(HierarchyAdminViewModel entity)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(entity);

                using (ApiConnector client = new ApiConnector())
                {
                    Hierarchy h = new Hierarchy
                    {
                        DisplayName = entity.Hierarchy.DisplayName,
                        SortOrder = entity.Hierarchy.SortOrder ?? 0,
                        Active = false,
                        ParentHierarchyId = entity.Hierarchy.ParentHierarchyId
                    };

                    h = await client.Create<Hierarchy>(ApiRoutes.HIERARCHY_API_URL, h);
                    if (h != null)
                    {
                        return RedirectToAction("Details", new {id = h.Id});
                    }
                    else
                    {
                        return RedirectToAction("Create");
                    }
                }

            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Hierarchy/Edit/5

        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                Hierarchy hierarchy = await client.Read<Hierarchy>(ApiRoutes.HIERARCHY_API_URL + id);
                var allHierarchyResponse = await client.Read<List<Hierarchy>>(ApiRoutes.HIERARCHY_API_URL);
                HierarchyAdminViewModel vm = new HierarchyAdminViewModel
                {
                    Hierarchy = hierarchy,
                    AllHierarchies = allHierarchyResponse
                };

                return View(vm);

            }
        }

        //
        // POST: /Hierarchy/Edit/5

        [HttpPost]
        public async Task<ActionResult>  Edit(int id, HierarchyAdminViewModel view)
        {
            try
            {
                view.Hierarchy.Id = id;
                using (ApiConnector client = new ApiConnector())
                {
                    await client.Update<Hierarchy>(ApiRoutes.HIERARCHY_API_URL, id, view.Hierarchy);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Edit", id);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Reorder()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage rsp = await client.GetAsync(ApiRoutes.HIERARCHY_API_URL+"?level=0");
                if (rsp.IsSuccessStatusCode)
                {
                    IEnumerable<Hierarchy> list = await rsp.Content.ReadAsAsync<IEnumerable<Hierarchy>>();
                    Session["reorder_hierarchies"] = list;
                    ViewBag.Hierarchies = list.Where(x=>x.ParentHierarchyId == null);
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Reorder(FormCollection form, IEnumerable<Hierarchy> models)
        {
            string command = form["command"];

            if (command == null)
                return View();

            command = command.ToUpper();

            if (command == "SEARCH")
            {

            }
            else if (command == "SUBMIT")
            {

            }

            return View();
        }

        //
        // GET: /Hierarchy/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Hierarchy/Delete/5

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
