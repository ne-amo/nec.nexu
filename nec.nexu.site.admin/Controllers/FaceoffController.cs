using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using nec.nexu.Models;
using nec.nexu.site.admin.Models;
using nec.nexu.ViewModels;

namespace nec.nexu.site.admin.Controllers
{
    public class FaceoffController : Controller
    {
        // GET: Faceoff
        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage rsp = await client.GetAsync(ApiRoutes.FACEOFF_API_URL);
                if (rsp.IsSuccessStatusCode)
                {
                    IEnumerable<FaceOff> list = await rsp.Content.ReadAsAsync<IEnumerable<FaceOff>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            } 
        }

        // GET: Faceoff/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                FaceOff ret = await client.Read<FaceOff>(ApiRoutes.FACEOFF_API_URL + id.ToString());
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

        // GET: Faceoff/Create
        public async Task<ActionResult> Create()
        {
            using (ApiConnector client = new ApiConnector())
            {
                var fprsp = await client.Read<PagedResponse<FinishedProductGalleryDetailView>>(ApiRoutes.FINISHEDPRODUCT_API_URL);
                FaceoffAdminViewModel vm = new FaceoffAdminViewModel { AllFinishedProducts = fprsp.Items.ToList() };
                return View(vm);
            }
        }

        // POST: Faceoff/Create
        [HttpPost]
        public async Task<ActionResult>  Create(FaceoffAdminViewModel entity)
        {
            try
            {
               
                if (!ModelState.IsValid)
                    return View(entity);

                using(ApiConnector client = new ApiConnector())
                {
                    FaceOff fo = new FaceOff
                    {
                        DisplayName = entity.Faceoff.DisplayName,
                        Title = entity.Faceoff.Title,
                        Creator = entity.Faceoff.Creator,
                        Description = entity.Faceoff.Description,
                        EndDate = entity.Faceoff.EndDate,
                        FinishedProductAId = entity.Faceoff.FinishedProductAId,
                        FinishedProductBId = entity.Faceoff.FinishedProductBId,
                        StartDate = entity.Faceoff.StartDate,
                        Active = true
                    };

                    fo = await client.Create<FaceOff>(ApiRoutes.FACEOFF_API_URL, fo);
                    if (fo != null)
                    {
                        return RedirectToAction("Details", new {id = fo.Id});
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

        // GET: Faceoff/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                FaceOff fo = await client.Read<FaceOff>(ApiRoutes.FACEOFF_API_URL + id);
                var fprsp = await client.Read<PagedResponse<FinishedProductGalleryDetailView>>(ApiRoutes.FINISHEDPRODUCT_API_URL);
                FaceoffAdminViewModel vm = new FaceoffAdminViewModel
                {
                    Faceoff = fo,
                    AllFinishedProducts = fprsp.Items.ToList()
                };
                return View(vm);
            }
        }

        // POST: Faceoff/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, FaceoffAdminViewModel view)
        {
            try
            {
               
                view.Faceoff.Id = id;
                using (ApiConnector client = new ApiConnector())
                {
                    await client.Update<FaceOff>(ApiRoutes.FACEOFF_API_URL, id, view.Faceoff);

                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Edit", id);
            }
        }

        // GET: Faceoff/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Faceoff/Delete/5
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
