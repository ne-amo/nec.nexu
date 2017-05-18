using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;
using System.Threading.Tasks;
using nec.nexu.site.admin.Models;
using System.IO;

namespace nec.nexu.site.admin.Controllers
{
    public class MaterialsController : Controller
    {
        private string apiMaterialsIndex = "api/masterdata/materials";
        private string apiMaterial = "api/masterdata/material/";
        private string apiDesignColors = "api/masterdata/designcolors/";
        private string apiFabricContentTypes = "api/masterdata/fabriccontenttypes/";
        private string apiPatterns = "api/masterdata/patterns/";

        private string trueViewFileDir = "~/Images/TrueView/";

        // GET: Materials
        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiMaterialsIndex);
                if (response.IsSuccessStatusCode)
                {
                    List<Material> list = await response.Content.ReadAsAsync<List<Material>>();
                    return View(list);
                }
                else
                {
                    return View();
                }
            }
        }

        // GET: Materials/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                Material ret = await client.Read<Material>(apiMaterial + id);
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

        // GET: Materials/Create
        public async Task<ActionResult> Create()
        {
            using (ApiConnector client = new ApiConnector())
            {
                Material material = new Material();
                List<DesignColor> allDesignColors = await client.Read<List<DesignColor>>(apiDesignColors);
                List<FabricContentType> allFabricContentTypes = await client.Read<List<FabricContentType>>(apiFabricContentTypes);
                List<Pattern> allPatterns = await client.Read<List<Pattern>>(apiPatterns);
                MaterialAdminViewModel returnModel = new MaterialAdminViewModel()
                {
                    Material = material,
                    AllDesignColors = allDesignColors,
                    AllFabricContentTypes = allFabricContentTypes,
                    AllPatterns = allPatterns
                };
                return View(returnModel);
            }
        }

        // POST: Materials/Create
        [HttpPost]
        public async Task<ActionResult> Create(Material material)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(material);

                using (ApiConnector client = new ApiConnector())
                {
                    var color = ColorTranslator.FromHtml(material.Hex);
                    double hue, saturation, brightness;
                    hue = color.GetHue();
                    saturation = color.GetSaturation();
                    brightness = color.GetBrightness();
                    Material newMaterial = new Material()
                    {
                        BlendMode = material.BlendMode,
                        ColorContainers = material.ColorContainers,
                        Active = true,
                        Brightness = brightness,
                        ColorLists = material.ColorLists,
                        DisplayName = material.DisplayName,
                        FabricContent = material.FabricContent,
                        Hex = material.Hex,
                        Hue = hue,
                        MatchingDesignColors = material.MatchingDesignColors,
                        Opacity = material.Opacity,
                        Pattern = material.Pattern,
                        Pms = material.Pms,
                        Saturation = saturation,
                        SortOrder = material.SortOrder,
                        TextColor = material.TextColor,
                        TrueView = material.TrueView
                    };

                    newMaterial = await client.Create(apiMaterial, newMaterial);

                    if (newMaterial != null)
                        return RedirectToAction("Details", new { id = newMaterial.Id });
                    else
                        return RedirectToAction("Create");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Materials/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                Material material = await client.Read<Material>(apiMaterial + id);
                List<DesignColor> allDesignColors = await client.Read<List<DesignColor>>(apiDesignColors);
                List<FabricContentType> allFabricContentTypes = await client.Read<List<FabricContentType>>(apiFabricContentTypes);
                List<Pattern> allPatterns = await client.Read<List<Pattern>>(apiPatterns);
                try
                {
                    ViewBag.imgPath =
                        material.TrueView.FilePath.Replace(Request.ServerVariables["APPL_PHYSICAL_PATH"], "~/")
                            .Replace(@"\", "/");
                }
                catch
                {
                }
                MaterialAdminViewModel returnModel = new MaterialAdminViewModel()
                {
                    Material = material,
                    AllDesignColors = allDesignColors,
                    AllFabricContentTypes = allFabricContentTypes,
                    AllPatterns = allPatterns
                };

                return View(returnModel);
            }
        }

        // POST: Materials/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, MaterialAdminViewModel view, HttpPostedFileBase TrueView)
        {
            try
            {
                using (ApiConnector client = new ApiConnector())
                {
                    //save trueview image
                    string filePath = Path.Combine(Server.MapPath(this.trueViewFileDir), TrueView.FileName);
                    if (Request.Files["TrueView"] != null && Path.GetDirectoryName(filePath) != null)
                    {
                        Request.Files["TrueView"].SaveAs(filePath);
                    }
                    Image image = Image.FromStream(TrueView.InputStream);
                    ImageFile imageFile = new ImageFile()
                    {
                        DisplayName = TrueView.FileName,
                        FilePath = filePath,
                        ImageType = new ImageType(),
                        ThumbnailPath = filePath,
                        HeightPx = image.Height,
                        WidthPx = image.Width,
                        Active = true
                    };

                    view.Material.TrueView = imageFile;

                    //design color
                    var newDesColors = new List<DesignColor>();

                    if (view.SelectedDesignColorIds != null)
                    {
                        newDesColors = view.SelectedDesignColorIds.Select(m => new DesignColor() { Id = m }).ToList();
                    }

                    // calculate hsb from hex value
                    var color = ColorTranslator.FromHtml(view.Material.Hex);
                    double hue, saturation, brightness;
                    hue = color.GetHue();
                    saturation = color.GetSaturation();
                    brightness = color.GetBrightness();
                    view.Material.Id = id;
                    view.Material.Hue = hue;
                    view.Material.Saturation = saturation;
                    view.Material.Brightness = brightness;
                    view.Material.MatchingDesignColors = newDesColors;
                    await client.Update(apiMaterial, id, view.Material);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Edit", id);
            }
        }

        // GET: Materials/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                using (ApiConnector client = new ApiConnector())
                {
                    Material mat = await client.Read<Material>(apiMaterial + id);
                    return View(mat);
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Materials/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, Material material)
        {
            try
            {
                using (ApiConnector client = new ApiConnector())
                {
                    var responseMessage = await client.DeleteAsync(apiMaterial + id);
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
