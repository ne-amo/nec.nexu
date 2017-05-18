using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using nec.nexu.model.Models.ComplexTypes;
using nec.nexu.Models;
using nec.nexu.site.admin.Models;
using nec.nexu.ViewModels;
using System.Drawing;
using nec.modules.bitmap;
using nec.nexu.rendering;


namespace nec.nexu.site.admin.Controllers
{
    public class DesignController : Controller
    {
        //
        // GET: /Design/

        public async Task<ActionResult> Index()
        {
            return View();
        }

        public async Task<ActionResult> Create()
        {
            return View();
        }

        private Bitmap ResizeImage(Bitmap original, float width, float height)
        {
            Bitmap resized = new Bitmap(original, new Size((int)width, (int)height));
            return resized;
        }

        [HttpPost]
        public async Task<JsonResult> Create(Design design)
        {
            int designW = 0;
            int designH = 0;
            float ratioW = 1;
            float ratioH = 1;

            Design created;
            string relPath = System.Configuration.ConfigurationManager.AppSettings["imageRelPath"].ToString();
            string absPath = System.Configuration.ConfigurationManager.AppSettings["imageAbsPath"].ToString();

            try
            {
                using (Bitmap official = new Bitmap(Server.MapPath(design.OfficialImage.FilePath)))
                {
                    
                    ratioW = 72 / official.HorizontalResolution;
                    ratioH = 72 / official.VerticalResolution;
                    designW = (int)(official.Width * ratioW);
                    designH = (int)(official.Height * ratioH);

                    using (Bitmap resized = this.ResizeImage(official, designW, designH))
                    {
                        resized.SetResolution(72, 72);
                        resized.Save(absPath + design.TapeNumber + "_official.png");
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            using (ApiConnector client = new ApiConnector())
            {

                foreach (DesignLayer layer in design.DefaultColorway.Layers)
                {
                    using (Bitmap layerImage = new Bitmap(Server.MapPath(layer.LayerFile.FilePath)))
                    {
                        using (Bitmap resized = this.ResizeImage(layerImage, designW, designH))
                        {
                            resized.SetResolution(72, 72);
                            ImageData imgd = new ImageData();
                            imgd.Active = true;
                            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                            {
                                resized.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                imgd.Data = ms.ToArray();
                            }
                            imgd = await client.Create<ImageData>(ApiRoutes.IMAGEDATA, imgd);
                            layer.ImageDataId = imgd.Id;
                        }
                    }
                    layer.LayerFile = null;
                }

                design.WidthPixel = designW;
                design.HeightPixel = designH;
                design.Active = true;

                design.OfficialImage = new ImageFile
                {
                    FilePath = relPath+design.TapeNumber+"_official.png",
                    ThumbnailPath = relPath + design.TapeNumber + "_official.png",
                    DisplayName = design.TapeNumber + " official"
                };

                created = await client.Create<Design>(ApiRoutes.DESIGNS_API_URL, design);
            }

            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public async Task<JsonResult> CreateColorway(DesignColorway colorway)
        {

            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [HttpPost]
        public void UploadFile()
        {
            string path = Server.MapPath("~/Content/Uploads/");
            HttpPostedFileBase postedFile = Request.Files["files"];
            string fileExt = Path.GetExtension(postedFile.FileName);
            string fileName = Path.GetRandomFileName().Replace(".", string.Empty);
            string temporaryPath = path + fileName + fileExt;


            //using (Bitmap uploaded = new Bitmap(Image.FromStream(postedFile.InputStream)))
            //{
            //
            //    int h = uploaded.Height;
            //    int w = uploaded.Width;
            //    uploaded.SetResolution(72, 72);
            //    using(Bitmap resized = this.ResizeImage(uploaded,w,h))
            //    {
            //        resized.Save(temporaryPath);
            //    }
            //}
            postedFile.SaveAs(temporaryPath);
            Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            Response.Write(
                Newtonsoft.Json.JsonConvert.SerializeObject("/Content/Uploads/" + fileName + fileExt)
            );
        }

        public async Task<ActionResult> Edit(int id)
        {
            using(ApiConnector api = new ApiConnector()){

                nec.nexu.JsonModels.Design d = await api.Read<nec.nexu.JsonModels.Design>(ApiRoutes.DESIGNS_API_URL + id);
                return View(d);
            }   
        }


        [HttpPost]
        public async Task<JsonResult> Edit(Design design)
        {
            int designW = 0;
            int designH = 0;
            float ratioW = 1;
            float ratioH = 1;
            string relPath = System.Configuration.ConfigurationManager.AppSettings["imageRelPath"].ToString();
            string absPath = System.Configuration.ConfigurationManager.AppSettings["imageAbsPath"].ToString();

            if (design.OfficialImage.Id == 0)
            {
                try
                {
                    using (Bitmap official = new Bitmap(Server.MapPath(design.OfficialImage.FilePath)))
                    {

                        ratioW = 72 / official.HorizontalResolution;
                        ratioH = 72 / official.VerticalResolution;
                        designW = (int)(official.Width * ratioW);
                        designH = (int)(official.Height * ratioH);

                        using (Bitmap resized = this.ResizeImage(official, designW, designH))
                        {
                            resized.SetResolution(72, 72);
                            resized.Save(absPath + design.TapeNumber + "_official.png");
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                design.OfficialImage = new ImageFile
                {
                    FilePath = relPath + design.TapeNumber + "_official.png",
                    ThumbnailPath = relPath + design.TapeNumber + "_official.png",
                    DisplayName = design.TapeNumber + " official"
                };
            }

            using (ApiConnector api = new ApiConnector())
            {
                await api.PutAsJsonAsync<Design>(ApiRoutes.DESIGNS_API_URL + design.Id, design);
            }

            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public async Task<ActionResult> Delete(int id)
        {
            if (id != 0)
            {
                using (ApiConnector client = new ApiConnector())
                {
                    HttpResponseMessage response = await client.GetAsync(string.Format("api/designs/{0}",id));
                    if (response.IsSuccessStatusCode)
                    {
                        var item = await response.Content.ReadAsAsync<Design>();
                        return View(item);
                    }
                    else
                    {
                        return RedirectToAction("index");
                    }
                }
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        [HttpPost]
        public async Task<ActionResult> Delete(Design model, int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.DeleteAsync(string.Format("api/designs/{0}", id));
                return RedirectToAction("index");
            }
        }

    }
}
