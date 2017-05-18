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
    public class CustomBackgroundsController : Controller
    {
        private string apiUrl = "api/custombackgrounds/";
        private string apiProductTemplate = "api/producttemplates/";
        private string apiHierarchy = "api/hierarchies";

        private string fileSavePath = "~/Images/Backgrounds/";
        private string thumbnailSavePath = "~/Images/Backgrounds/Thumbnails/";

        // GET: CustomBackgrounds
        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    List<CustomBackground> list = await response.Content.ReadAsAsync<List<CustomBackground>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            }
        }

        // GET: CustomBackgrounds/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                CustomBackground ret = await client.Read<CustomBackground>(apiUrl + id.ToString());
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

        // GET: CustomBackgrounds/Create
        public async Task<ActionResult> Create()
        {
            using (ApiConnector client = new ApiConnector())
            {
                List<ProductTemplate> prodTemplates = await client.Read<List<ProductTemplate>>(apiProductTemplate);
                List<Hierarchy> hierarchies = await client.Read<List<Hierarchy>>(apiHierarchy);
                CustomBackgroundAdminViewModel ret = new CustomBackgroundAdminViewModel(){ 
                    AllHierarchies = hierarchies, 
                    AllProductTemplates = prodTemplates,
                    CustomBackground = new CustomBackground()
                };
                return View(ret);
            }
        }

        // POST: CustomBackgrounds/Create
        [HttpPost]
        public async Task<ActionResult> Create(CustomBackgroundAdminViewModel model, HttpPostedFileBase ThumbnailPath, HttpPostedFileBase FilePath)
        {

            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                using (ApiConnector client = new ApiConnector())
                {

                    string assetRoot = System.Configuration.ConfigurationManager.AppSettings["assetPath"].ToString();
                    string extension = Path.GetExtension(Request.Files["FilePath"].FileName);
                    string fileName = Path.GetRandomFileName().Replace(".", string.Empty);
                    string FileFullPath = assetRoot + @"nexu_bg\" + fileName + extension;
                    string ThumbnailFullPath = assetRoot + @"nexu_bg\" + fileName +"_tn"+ extension;

                    //string ThumbnailFullPath = Path.Combine(Server.MapPath(this.thumbnailSavePath), background.ThumbnailPath);
                    //string FileFullPath = Path.Combine(Server.MapPath(this.fileSavePath), background.FilePath);

                    if (Request.Files["ThumbnailPath"] != null )//&& Path.GetDirectoryName(thumbnailSavePath) != null)
                    {
                        Request.Files["ThumbnailPath"].SaveAs(ThumbnailFullPath);
                    }

                    if (Request.Files["FilePath"] != null )//&& Path.GetDirectoryName(fileSavePath) != null)
                    {
                        Request.Files["FilePath"].SaveAs(FileFullPath);
                    }

                    CustomBackground newBackground = new CustomBackground()
                    {
                        FilePath = @"/nexu_bg/" + fileName + extension,
                        ThumbnailPath = @"/nexu_bg/" + fileName + "_tn" + extension,
                        Active = true,
                        DisplayName = model.CustomBackground.DisplayName,
                        SortOrder = model.CustomBackground.SortOrder
                        //HierarchyId = background.HierarchyId,
                        //ProductTemplateId = background.ProductTemplateId
                    };

                    newBackground = await client.Create(apiUrl, newBackground);

                    if (newBackground != null)
                        return RedirectToAction("Details", new { id = newBackground.Id });
                    else
                        return RedirectToAction("Create");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: CustomBackgrounds/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                CustomBackground background = await client.Read<CustomBackground>(apiUrl + id.ToString());
                List<Hierarchy> allHierarchies = await client.Read<List<Hierarchy>>(apiHierarchy);
                List<ProductTemplate> allProductTemplates = await client.Read<List<ProductTemplate>>(apiProductTemplate);
                if (background != null)
                {
                    CustomBackgroundAdminViewModel cbView = new CustomBackgroundAdminViewModel()
                    {
                        AllHierarchies = allHierarchies,
                        AllProductTemplates = allProductTemplates,
                        CustomBackground = background
                    };
                    return View(cbView);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        // POST: CustomBackgrounds/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, CustomBackground background, HttpPostedFileBase ThumbnailPath, HttpPostedFileBase FilePath)
        {
            try
            {
                using (ApiConnector client = new ApiConnector())
                {
                    string ThumbnailFullPath = Path.Combine(Server.MapPath(this.thumbnailSavePath), background.ThumbnailPath);
                    string FileFullPath = Path.Combine(Server.MapPath(this.fileSavePath), background.FilePath);

                    if (Request.Files["ThumbnailPath"] != null && Path.GetDirectoryName(thumbnailSavePath) != null)
                    {
                        Request.Files["ThumbnailPath"].SaveAs(ThumbnailFullPath);
                    }

                    if (Request.Files["FilePath"] != null && Path.GetDirectoryName(fileSavePath) != null)
                    {
                        Request.Files["FilePath"].SaveAs(FileFullPath);
                    }

                    background.FilePath = FileFullPath;
                    background.ThumbnailPath = ThumbnailFullPath;

                    await client.Update(apiUrl, id, background);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Edit", id);
            }
        }

        // GET: CustomBackgrounds/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            
            try
            {
                using (ApiConnector client = new ApiConnector())
                {
                    CustomBackground ret = await client.Read<CustomBackground>(apiUrl + id.ToString());
                    return View(ret);
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // POST: CustomBackgrounds/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                using (ApiConnector client = new ApiConnector())
                {
                    var responseMessage = await client.DeleteAsync(apiUrl + id.ToString());
                }


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
