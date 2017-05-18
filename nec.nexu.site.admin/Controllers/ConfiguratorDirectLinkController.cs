using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using nec.nexu.model.Models.ComplexTypes;
using nec.nexu.Models;
using nec.nexu.site.admin.Models;
using nec.nexu.ViewModels;

namespace nec.nexu.site.admin.Controllers
{
    public class ConfiguratorDirectLinkController : Controller
    {
        // GET: ConfiguratorDirectLink
        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage rsp = await client.GetAsync(ApiRoutes.CONFIGURATOR_LINK_API_URL+"?grid=true");
                if (rsp.IsSuccessStatusCode)
                {
                    IEnumerable<ConfiguratorDirectLink> list = await rsp.Content.ReadAsAsync<IEnumerable<ConfiguratorDirectLink>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            } 
        }

        // GET: ConfiguratorDirectLink/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                ConfiguratorDirectLink ret = await client.Read<ConfiguratorDirectLink>(ApiRoutes.CONFIGURATOR_LINK_API_URL + id.ToString());
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

        // GET: ConfiguratorDirectLink/Create
        public async Task<ActionResult> Create()
        {
            /*
            using (ApiConnector client = new ApiConnector())
            {
                ConfiguratorDirectLink link = new ConfiguratorDirectLink();
                var templates = await client.Read<List<ProductTemplate>>(ApiRoutes.PRODUCT_TEMPLATES_API_URL );
                var prods = await client.Read<PagedResponse<FinishedProductGalleryDetailView>>(ApiRoutes.FINISHEDPRODUCT_API_URL);
                var parHie = await client.Read<List<Hierarchy>>(ApiRoutes.HIERARCHY_API_URL);
                var colors = await client.Read<List<ColorList>>(ApiRoutes.COLORLIST_API_URL);
                var materials = await client.Read<List<Material>>(ApiRoutes.MATERIALS_API_URL);
                var designs = await client.Read<List<DesignResultMessage>>(ApiRoutes.DESIGNS_API_URL);
                var prodRegions = await client.Read<List<ProductRegionGroup>>(ApiRoutes.PRODUCT_REGION_GROUPS_API_URL);
                var designLocations = await client.Read<List<DesignLocation>>(ApiRoutes.DESIGN_LOCATION_API_URL);


                ConfiguratorDirectLinkAdminViewModel vm = new ConfiguratorDirectLinkAdminViewModel
                {
                    Configurator = link,
                    AllProductTemplates = templates,
                    AllFinishedProducts = prods.Items.ToList(),
                    AllHierarchies = parHie,
                    AllColorLists = colors,
                    AllMaterials = materials,
                    AllDesigns = designs,
                    AllRegionGroups = prodRegions,
                    AllDesignLocations = designLocations
                };

                return View(vm);

            }
            */
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ConfiguratorDirectLink entity)
        {
            using (ApiConnector client = new ApiConnector())
            {
                ConfiguratorDirectLink cdl = await client.Create<ConfiguratorDirectLink>(ApiRoutes.CONFIGURATOR_LINK_API_URL, entity);
                if (cdl != null)
                {
                    return new JsonResult {
                         Data = cdl.Id,
                         JsonRequestBehavior =  JsonRequestBehavior.AllowGet 
                    };
                }
                else
                {
                    return RedirectToAction("Create");
                }
            }
        }

        //// POST: ConfiguratorDirectLink/Create
        //[HttpPost]
        //public async Task<ActionResult> Create(ConfiguratorDirectLinkAdminViewModel entity)
        //{


        //    using (ApiConnector client = new ApiConnector())
        //    {
        //        ConfiguratorDirectLink cdl = await client.Create<ConfiguratorDirectLink>(ApiRoutes.CONFIGURATOR_LINK_API_URL, entity);
        //        if (cdl != null)
        //        {
        //            return RedirectToAction("Details", new { id = cdl.Id });
        //        }
        //        else
        //        {
        //            return RedirectToAction("Create");
        //        }
        //    }


        //    /*
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return View(entity);

        //        using (ApiConnector client = new ApiConnector())
        //        {
        //            ConfiguratorDirectLink cdl = new ConfiguratorDirectLink();  
        //            cdl.Build(entity.Configurator);
        //            cdl.Active = true;
        //            cdl.ProductRegionGroups = new PersistableIntCollection();
        //            foreach (var i in entity.SelectedProductRegionGroups)
        //            {
        //                cdl.ProductRegionGroups.Add(i);
        //            }

        //            cdl.DesignLocations = new PersistableIntCollection();
        //            foreach (var i in entity.SelectedDesignLocationIds)
        //            {
        //                cdl.DesignLocations.Add(i);
        //            }

        //            var newMats = new List<Material>();
        //            var newDes = new List<Design>();
        //            if (entity.SelectedMaterialIds != null)
        //            {
        //                newMats = entity.SelectedMaterialIds.Select(m => new Material { Id = m }).ToList();
        //            }
        //            if (entity.SelectedDesignIds != null)
        //            {
        //                newDes = entity.SelectedDesignIds.Select(m => new Design { Id = m }).ToList();
        //            }
        //            cdl.Materials = newMats;
        //            cdl.Designs = newDes;

        //            cdl = await client.Create<ConfiguratorDirectLink>(ApiRoutes.CONFIGURATOR_LINK_API_URL, cdl);
        //            if (cdl != null)
        //            {
        //                return RedirectToAction("Details", new {id = cdl.Id });
        //            }
        //            else
        //            {
        //                return RedirectToAction("Create");
        //            }
        //        }

                
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //     */
        //}

        // GET: ConfiguratorDirectLink/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                ConfiguratorDirectLink link =
                    await client.Read<ConfiguratorDirectLink>(ApiRoutes.CONFIGURATOR_LINK_API_URL + id);
                var templates = await client.Read<List<ProductTemplate>>(ApiRoutes.PRODUCT_TEMPLATES_API_URL);
                var prods = await client.Read<PagedResponse<FinishedProductGalleryDetailView>>(ApiRoutes.FINISHEDPRODUCT_API_URL);
                var parHie = await client.Read<List<Hierarchy>>(ApiRoutes.HIERARCHY_API_URL);
                var colors = await client.Read<List<ColorList>>(ApiRoutes.COLORLIST_API_URL);
                var materials = await client.Read<List<Material>>(ApiRoutes.MATERIALS_API_URL);
                var designs = await client.Read<List<DesignResultMessage>>(ApiRoutes.DESIGNS_API_URL);
                var prodRegions = await client.Read<List<ProductRegionGroup>>(ApiRoutes.PRODUCT_REGION_GROUPS_API_URL);
                var designLocations = await client.Read<List<DesignLocation>>(ApiRoutes.DESIGN_LOCATION_API_URL);


                ConfiguratorDirectLinkAdminViewModel vm = new ConfiguratorDirectLinkAdminViewModel
                {
                    Configurator = link,
                    AllProductTemplates = templates,
                    AllFinishedProducts = prods.Items.ToList(),
                    AllHierarchies = parHie,
                    AllColorLists = colors,
                    AllMaterials = materials,
                    AllDesigns = designs,
                    AllRegionGroups = prodRegions,
                    AllDesignLocations = designLocations
                };

                return View(vm);

            }
        }

        // POST: ConfiguratorDirectLink/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, ConfiguratorDirectLinkAdminViewModel view)
        {
            try
            {
                
                var newMats = new List<Material>();
                var newDes = new List<Design>();
              
                if(view.SelectedMaterialIds != null)
                {
                    newMats = view.SelectedMaterialIds.Select(m => new Material {Id = m}).ToList();
                }
                if (view.SelectedDesignIds != null)
                {
                    newDes = view.SelectedDesignIds.Select(m => new Design {Id = m}).ToList();
                }


                view.Configurator.Id = id;
                view.Configurator.Materials = newMats;
                view.Configurator.Designs = newDes;
                view.Configurator.ProductRegionGroups = new PersistableIntCollection();
                view.Configurator.DesignLocations = new PersistableIntCollection();
                foreach (var i in view.SelectedProductRegionGroups)
                {
                    view.Configurator.ProductRegionGroups.Add(i);
                }
                foreach (var i in view.SelectedDesignLocationIds)
                {
                    view.Configurator.DesignLocations.Add(i);
                }

                using (ApiConnector client = new ApiConnector())
                {
                    await client.Update<ConfiguratorDirectLink>(ApiRoutes.CONFIGURATOR_LINK_API_URL, id, view.Configurator);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: ConfiguratorDirectLink/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: ConfiguratorDirectLink/Delete/5
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
