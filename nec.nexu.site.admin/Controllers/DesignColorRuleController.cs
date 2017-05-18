using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models.rules;
using nec.nexu.site.admin.Models;
using System.Threading.Tasks;
using nec.nexu.Models;
using Newtonsoft.Json;
using nec.nexu.ViewModels;

namespace nec.nexu.site.admin.Controllers
{
    public class DesignColorRuleController : Controller
    {
        private const string apiDCR = "api/rule/designcolorrules/";
        private const string apiDesign = "api/rule/requiredlogorules/";

        // GET: DesignColorRule
        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiDCR);
                if (response.IsSuccessStatusCode)
                {
                    List<DesignColorRule> list = await response.Content.ReadAsAsync<List<DesignColorRule>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            }
        }

        // GET: DesignColorRule/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                DesignColorRule ret = await client.Read<DesignColorRule>(apiDCR + id);
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

        // GET: DesignColorRule/Create
        public async Task<ActionResult> Create()
        {
            using (ApiConnector client = new ApiConnector())
            {
                var designs = await client.Read<List<DesignResultMessage>>(ApiRoutes.DESIGNS_API_URL);
                DesignColorRulesAdminViewModel ret = new DesignColorRulesAdminViewModel();
                ret.AllDesigns = designs;
                return View(ret);
            }
        }

        // POST: DesignColorRule/Create
        [HttpPost]
        public async Task<ActionResult> Create(DesignColorRulesAdminViewModel dcr)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dcr);

                using (ApiConnector client = new ApiConnector())
                {
                    DesignColorRule newDCR = new DesignColorRule()
                    {
                        Active = dcr.DesignColorRule.Active,
                        DisplayName = dcr.DesignColorRule.DisplayName,
                        DesignId = dcr.DesignColorRule.DesignId,
                        Combination = dcr.DesignColorRule.Combination,
                        ImageDataId = dcr.DesignColorRule.ImageDataId,
                        UserMessage = dcr.DesignColorRule.UserMessage
                    };
                    
                    newDCR.Values = new List<DesignColorRuleValue>();
                    if (dcr.NewValues != null)
                    {
                        dynamic valuesObj = JsonConvert.DeserializeObject(dcr.NewValues);
                        foreach (var item in valuesObj)
                        {
                            DesignColorRuleValue newVal = new DesignColorRuleValue()
                            {
                                DesignColorId = item.DesignColorId,
                                Operator = item.Operator
                            };
                            newDCR.Values.Add(newVal);
                        }
                    }

                    DesignColorRule ret = await client.Create<DesignColorRule>(apiDCR, newDCR);
                    if (ret != null)
                    {
                        return RedirectToAction("Details", new { id = ret.Id });
                    }
                    else
                    {
                        return View(dcr);
                    }
                }
            }
            catch
            {
                return View(dcr);
            }
        }

        // GET: DesignColorRule/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                DesignColorRule rule = await client.Read<DesignColorRule>(apiDCR + id);
                var designs = await client.Read<List<DesignResultMessage>>(ApiRoutes.DESIGNS_API_URL);
                String values = "";
                if (rule.Values != null)
                {
                    values = JsonConvert.SerializeObject(rule.Values.ToArray());
                }
                
                if (rule != null)
                {
                    DesignColorRulesAdminViewModel ret = new DesignColorRulesAdminViewModel()
                    {
                        DesignColorRule = rule,
                        AllDesigns = designs,
                        NewValues = values
                    };
                    return View(ret);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        // POST: DesignColorRule/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, DesignColorRulesAdminViewModel dcr)
        {
            try
            {
                if (!ModelState.IsValid)
                    return RedirectToAction("Edit", new { id = id });

                using (ApiConnector client = new ApiConnector())
                {
                    dcr.DesignColorRule.Id = id;
                   
                    dcr.DesignColorRule.Values = new List<DesignColorRuleValue>();
                    if (dcr.NewValues != null)
                    {

                        //DesignColorRule rule = await client.Read<DesignColorRule>(apiDCR + id);
                        //if (dcr.NewValues != null && !String.IsNullOrEmpty( dcr.NewValues))
                        //{
                        //    rule.Values = rule.Values.Where(v => dcr.NewValues.Contains(v)).ToList();
                        //}

                        dynamic valuesObj = JsonConvert.DeserializeObject(dcr.NewValues);
                        foreach (var item in valuesObj)
                        {
                            DesignColorRuleValue newVal = new DesignColorRuleValue()
                            {
                                DesignColorId = item.DesignColorId,
                                Operator = item.Operator
                            };
                            dcr.DesignColorRule.Values.Add(newVal);
                        }
                    }
                    HttpResponseMessage response = await client.PutAsJsonAsync(apiDCR + id, dcr.DesignColorRule);
                    return RedirectToAction("Details", new { id = id });
                }
            }
            catch
            {
                return View(dcr);
            }
        }

        public async Task<ActionResult> cascadingDropDown(string designId)
        {
            using (ApiConnector client = new ApiConnector())
            {
                List<Design> designs = await client.Read<List<Design>>(ApiRoutes.DESIGNS_API_URL);
                return View(designs);
            }
        }

        // GET: DesignColorRule/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: DesignColorRule/Delete/5
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
