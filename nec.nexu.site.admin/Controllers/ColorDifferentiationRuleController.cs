using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;
using nec.nexu.Models.rules;
using nec.nexu.site.admin.Models;
using nec.nexu.ViewModels;

namespace nec.nexu.site.admin.Controllers
{
    public class ColorDifferentiationRuleController : Controller
    {
        // GET: ColorDifferentiationRule
        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage rsp = await client.GetAsync(ApiRoutes.COLOR_DIFF_RULE_API_URL);
                if (rsp.IsSuccessStatusCode)
                {
                    IEnumerable<ColorDifferentiationRule> list =
                        await rsp.Content.ReadAsAsync<IEnumerable<ColorDifferentiationRule>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            }
        }

        // GET: ColorDifferentiationRule/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                ColorDifferentiationRule ret = await client.Read<ColorDifferentiationRule>(ApiRoutes.COLOR_DIFF_RULE_API_URL + id.ToString());
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

        // GET: ColorDifferentiationRule/Create
        public async Task<ActionResult> Create()
        {
            using (ApiConnector client = new ApiConnector())
            {
                var designs = await client.Read<List<DesignResultMessage>>(ApiRoutes.DESIGNS_API_URL);
                ColorDifferentiationRule rule = new ColorDifferentiationRule();
                ColorDifferentiationRuleAdminViewModel vm = new ColorDifferentiationRuleAdminViewModel
                {
                    Rule = rule,
                    AllDesigns = designs
                };

                return View(vm);
            }
        }

        // POST: ColorDifferentiationRule/Create
        [HttpPost]
        public async Task<ActionResult> Create(ColorDifferentiationRuleAdminViewModel entity)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(entity);

                using (ApiConnector client = new ApiConnector())
                {
                    ColorDifferentiationRule rule = entity.Rule;
                    rule = await client.Create<ColorDifferentiationRule>(ApiRoutes.COLOR_DIFF_RULE_API_URL, rule);
                    if (rule != null)
                    {
                        return RedirectToAction("Details", new { id = rule.Id });
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

        // GET: ColorDifferentiationRule/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                var designs = await client.Read<List<DesignResultMessage>>(ApiRoutes.DESIGNS_API_URL);
                ColorDifferentiationRule rule =
                    await client.Read<ColorDifferentiationRule>(ApiRoutes.COLOR_DIFF_RULE_API_URL + id);
                ColorDifferentiationRuleAdminViewModel vm = new ColorDifferentiationRuleAdminViewModel
                {
                    Rule = rule,
                    AllDesigns = designs
                };

                return View(vm);
            }
        }

        // POST: ColorDifferentiationRule/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, ColorDifferentiationRuleAdminViewModel view)
        {
            try
            {
                using (ApiConnector client = new ApiConnector())
                {
                    var rule = view.Rule;
                    rule.Id = id;
                        await client.Update<ColorDifferentiationRule>(ApiRoutes.COLOR_DIFF_RULE_API_URL, id, rule);

                        return RedirectToAction("Index");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: ColorDifferentiationRule/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: ColorDifferentiationRule/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, FormCollection collection)
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
