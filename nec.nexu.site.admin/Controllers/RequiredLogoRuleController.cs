using System.Runtime.InteropServices;
using nec.nexu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using nec.nexu.site.admin.Models;

namespace nec.nexu.site.admin.Controllers
{
    public class RequiredLogoRuleController : Controller
    {
        private const string apiRLR = "api/rule/requiredlogorules/";
        private const string apiDesign = "api/designs/";
        private const string apiDesignLocation = "api/rule/requiredlogorules/";

        // GET: RequiredLogoRule
        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiRLR);
                if (response.IsSuccessStatusCode)
                {
                    List<RequiredLogoRuleAdminViewModel> list = await response.Content.ReadAsAsync<List<RequiredLogoRuleAdminViewModel>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            }
        }

        // GET: RequiredLogoRule/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                RequiredLogoRuleAdminViewModel ret = await client.Read<RequiredLogoRuleAdminViewModel>(apiRLR + id);
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

        // GET: RequiredLogoRule/Create
        public async Task<ActionResult> Create()
        {
            using (ApiConnector client = new ApiConnector())
            {

                List<Design> designs = await client.Read<List<Design>>(ApiRoutes.DESIGNS_API_URL);
                List<DesignLocation> designLocations = await client.Read<List<DesignLocation>>(ApiRoutes.DESIGN_LOCATION_API_URL);
                RequiredLogoRuleAdminViewModel ret = new RequiredLogoRuleAdminViewModel();
                ret.AllDesignLocations = designLocations;
                ret.AllDesigns = designs;
                return View(ret);
            }
        }

        // POST: RequiredLogoRule/Create
        [HttpPost]
        public async Task<ActionResult> Create(RequiredLogoRuleAdminViewModel rlr)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(rlr);

                using (ApiConnector client = new ApiConnector())
                {
                    RequiredLogoRuleAdminViewModel newRLR = new RequiredLogoRuleAdminViewModel()
                    {
                        Active = rlr.Active,
                        DisplayName = rlr.DisplayName,
                        DesignId = rlr.DesignId,
                        SourceLocationId = rlr.SourceLocationId,
                        TargetLocationId = rlr.TargetLocationId,
                        TargetDesignId = rlr.TargetDesignId,
                        Type = rlr.Type,
                        UserMessage = rlr.UserMessage
                    };
                    RequiredLogoRuleAdminViewModel ret = await client.Create<RequiredLogoRuleAdminViewModel>(apiRLR, newRLR);
                    if (ret != null)
                    {
                        return RedirectToAction("Details", new { id = ret.Id });
                    }
                    else
                    {
                        return View(rlr);
                    }
                }
            }
            catch
            {
                return View(rlr);
            }
        }

        // GET: RequiredLogoRule/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                RequiredLogoRuleAdminViewModel rule = await client.Read<RequiredLogoRuleAdminViewModel>(apiRLR + id);
                List<Design> designs = await client.Read<List<Design>>(ApiRoutes.DESIGNS_API_URL);
                List<DesignLocation> designLocations = await client.Read<List<DesignLocation>>(ApiRoutes.DESIGN_LOCATION_API_URL);
                if (rule != null)
                {
                    RequiredLogoRuleAdminViewModel ret = new RequiredLogoRuleAdminViewModel()
                    {
                        AllDesignLocations = designLocations,
                        AllDesigns = designs,
                        Active = rule.Active,
                        DisplayName = rule.DisplayName,
                        UserMessage = rule.UserMessage,
                        Id = rule.Id,
                        DesignId = rule.DesignId,
                        SourceLocationId = rule.SourceLocationId,
                        TargetDesignId = rule.TargetDesignId,
                        TargetLocationId = rule.TargetLocationId,
                        Type = rule.Type
                    };
                    return View(ret);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        // POST: RequiredLogoRule/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, RequiredLogoRuleAdminViewModel rlr)
        {
            try
            {
                if (!ModelState.IsValid)
                    return RedirectToAction("Edit", new { id = id });

                using (ApiConnector client = new ApiConnector())
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync(apiRLR + id, rlr);
                    return RedirectToAction("Details", new { id = id });
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: RequiredLogoRule/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RequiredLogoRule/Delete/5
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
