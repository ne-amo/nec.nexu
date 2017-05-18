using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime;
using nec.nexu.Models;
using nec.nexu.Models.rules;
using nec.nexu.site.admin.Models;
using nec.nexu.ViewModels;

namespace nec.nexu.site.admin.Controllers
{
    public class ColorwayRuleController : Controller
    {
        // GET: ColorwayRule
        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage rsp = await client.GetAsync(ApiRoutes.COLORWAY_RULE_API_URL);
                if (rsp.IsSuccessStatusCode)
                {
                    IEnumerable<ColorwayRule> list =
                        await rsp.Content.ReadAsAsync<IEnumerable<ColorwayRule>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            }
        }

        // GET: ColorwayRule/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                ColorwayRule ret = await client.Read<ColorwayRule>(ApiRoutes.COLORWAY_RULE_API_URL + id.ToString());
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

        // GET: ColorwayRule/Create
        public async Task<ActionResult> Create()
        {
            using (ApiConnector client = new ApiConnector())
            {
                var designs = await client.Read<List<DesignResultMessage>>(ApiRoutes.DESIGNS_API_URL);
                var colors = await client.Read<List<DesignColor>>(ApiRoutes.DESIGN_COLORS_API_URL);
                ColorwayRule rule = new ColorwayRule();
                ColorwayRuleAdminViewModel vm = new ColorwayRuleAdminViewModel
                {
                    Rule = rule,
                    AllDesigns = designs,
                    AllDesignColors = colors
                };

                return View(vm);
            }
        }

        // POST: ColorwayRule/Create
        [HttpPost]
        public async Task<ActionResult> Create(ColorwayRuleAdminViewModel entity)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(entity);

                using (ApiConnector client = new ApiConnector())
                {
                    ColorwayRule rule = entity.Rule;

                    if (entity.SelectedColorId != null)
                    {
                        rule.Values = new List<ColorwayRuleValue>();
                        var newValue = new ColorwayRuleValue { DesignColorId = entity.SelectedColorId };
                        rule.Values.Add(newValue);
                    }
                    
                    rule = await client.Create<ColorwayRule>(ApiRoutes.COLORWAY_RULE_API_URL, rule);
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

        // GET: ColorwayRule/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                var designs = await client.Read<List<DesignResultMessage>>(ApiRoutes.DESIGNS_API_URL);
                var colors = await client.Read<List<DesignColor>>(ApiRoutes.DESIGN_COLORS_API_URL);
                ColorwayRule rule =
                    await client.Read<ColorwayRule>(ApiRoutes.COLORWAY_RULE_API_URL + id);
                ColorwayRuleAdminViewModel vm = new ColorwayRuleAdminViewModel
                {
                    Rule = rule,
                    AllDesigns = designs,
                    AllDesignColors = colors
                };

                return View(vm);
            }
        }

        // POST: ColorwayRule/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, ColorwayRuleAdminViewModel view)
        {
            try
            {
                using (ApiConnector client = new ApiConnector())
                {
                    ColorwayRule dbrule =
                    await client.Read<ColorwayRule>(ApiRoutes.COLORWAY_RULE_API_URL + id);
                    var rule = view.Rule;
                    rule.Id = id;
                    if (view.ColorwayRuleValueIds != null && view.ColorwayRuleValueIds.Count > 0)
                    {
                        rule.Values = dbrule.Values.Where(v => view.ColorwayRuleValueIds.Contains(v.Id)).ToList();
                    }

                    if (view.SelectedColorId != null)
                    {
                        var newValue = new ColorwayRuleValue {DesignColorId = view.SelectedColorId};
                        rule.Values.Add(newValue);
                    }
                    

                    await client.Update<ColorwayRule>(ApiRoutes.COLORWAY_RULE_API_URL, id, rule);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: ColorwayRule/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: ColorwayRule/Delete/5
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
