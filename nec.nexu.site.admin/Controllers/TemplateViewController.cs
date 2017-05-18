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
    public class TemplateViewController : Controller
    {

        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage rsp = await client.GetAsync(ApiRoutes.HIERARCHY_API_URL);
                if (rsp.IsSuccessStatusCode)
                {
                    IEnumerable<TemplateView> list = await rsp.Content.ReadAsAsync<IEnumerable<TemplateView>>();
                    return View(list);
                }
                else 
                {
                    return View();
                }
            }
        }


        public async Task<ActionResult> Details(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                    return RedirectToAction("Index");
            }
        }
        //public async Task<ActionResult> Svg(int viewId, string svg)
        //{

        //}

        public async Task<ActionResult> Create()
        {
            //using (ApiConnector client = new ApiConnector())
            //{
            //    
            //}
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> Create(TemplateView entity, int templateId)
        {
            // Storage of any ImageData entity Ids that were created. If there's an error, we may need to rollback
            List<int> imageDataIds = new List<int>();
            // Storage to delete image files when we're done
            List<string> filesToDelete = new List<string>();
            
            try
            {
                if (!ModelState.IsValid)
                    return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                int TemplateId = templateId;
                TemplateView model = entity;
                int createdId=0;

                using(ApiConnector client = new ApiConnector())
                {

                    // Reconvert SVG Text
                    string svgMarkup = FromBase64(model.SvgContent);

                    // Setup ROOT image
                    TemplateImage root = model.Images.First(x => x.Code == "root");
                    // Create ImageData Entity
                    ImageData rootData = this._CreateImageDataEntity(root.FilePath);
                    // Send Root Image Data Entity to Service
                    rootData = await client.Create<ImageData>("api/masterdata/imagedata", rootData);
                    // Check to make sure it worked
                    if(rootData == null)
	                    throw new Exception("Ummm..... i don't think that worked");
                    // Log Image Data Id for rollback
                    imageDataIds.Add(rootData.Id);
                    root.ImageDataId = rootData.Id;
                    // Replace the path for the RooT 
                    filesToDelete.Add(root.FilePath);
                    string newPath = this._CreateImageUrl(rootData.Id,rootData.Id,"100","FFFFFF");
                    svgMarkup = svgMarkup.Replace(root.FilePath, newPath);

                    // Repeat similiar steps for each template image
                    foreach(TemplateImage image in model.Images.Where(x=>x.Code != "root"))
                    {
                        // Create ImageData Entity
                        ImageData imgData = this._CreateImageDataEntity(image.FilePath);
                        // Send Image Data Entity to Service
                        imgData = await client.Create<ImageData>("api/masterdata/imagedata", imgData);
                        // Check to make sure it worked
                        if (imgData == null)
                            throw new Exception("Ummm..... i don't think that worked");
                        // Log Image Data Id for rollback
                        imageDataIds.Add(imgData.Id);
                        image.ImageDataId = imgData.Id;
                        // Replace the path for the RooT 
                        filesToDelete.Add(image.FilePath);
                        // Dummy up url to exist in SVG
                        string imagePath = this._CreateImageUrl(imgData.Id, rootData.Id, "100", "FFFFFF");
                        svgMarkup = svgMarkup.Replace(image.FilePath, imagePath);
                    }
                    // Set Svg of Model
                    model.SvgContent = ToBase64(svgMarkup);

                    // Retrieve template and update Views collection
                    HttpResponseMessage response = await client.PostAsJsonAsync<TemplateView>("api/producttemplate/" + templateId + "/addview",model);
                    if (response.IsSuccessStatusCode)
                    {
                        createdId = await response.Content.ReadAsAsync<int>();
                    }

                }
                return new JsonResult { Data = createdId, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch(Exception ex)
            {
                // Rollback any imagedata entities that were created.
                return new JsonResult { Data = ex.Message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        private string FromBase64(string base64)
        {
            var data = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(data);
        }
        private string ToBase64(string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }
        private string _CreateImageUrl(int LayerImageData, int BaseImageData, string Alpha, string Color)
        {
            return string.Format(@"a.nexuimage?l={0}&b={1}&a={2}&t=0&c={3}&it=t",
                LayerImageData,
                BaseImageData,
                Alpha,
                Color);
        }
        private ImageData _CreateImageDataEntity(string imagePath)
        {
            ImageData data = new ImageData();
            data.Active = true;
            string path = Server.MapPath(imagePath);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.Drawing.Bitmap b = new System.Drawing.Bitmap(path))
                {
                    b.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }
                data.Data = ms.ToArray();
            }
            return data;
        }

        public class RenderSettingRequest
        {
            public int width { get; set; }
            public int height { get; set; } 
            public int locationId { get; set; } 
            public int viewId { get; set; } 
            public float[] matrix { get; set; }
            public string clipSvgB64 { get; set; }
        }
        [HttpPost]
        public async Task<JsonResult> CreateDesignRenderSetting(RenderSettingRequest entity)
        {
            string svg = FromBase64(entity.clipSvgB64);
            NexuRender render = new NexuRender();
            byte[] clipData = render.SvgToBitmap(svg, entity.width, entity.height, 96);
            DesignRenderSetting setting = GetDesignRenderSetting(entity.locationId, entity.viewId, entity.matrix, clipData);
            using (ApiConnector api = new ApiConnector())
            {
                var resp = await api.Create<DesignRenderSetting>("api/templateview/rendersetting/create", setting);
            }
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        [HttpPost]
        public async Task<JsonResult> UpdateSVG(int viewId, string svg)
        {
            using (ApiConnector api = new ApiConnector())
            {
                await api.Update<TemplateView>("api/templateview/updatesvg/", viewId, new TemplateView { SvgContent = svg });
            }
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        private DesignRenderSetting GetDesignRenderSetting(int locationId, int viewId, float[] matrix, byte[] clipData)
        {
            DesignRenderSetting settting = new DesignRenderSetting
            {
                TemplateViewId = viewId,
                DesignLocationId = locationId,
                ClippingMask = clipData,
                M1 = matrix[0],
                M2 = matrix[1],
                M3 = matrix[2],
                M4 = matrix[3]*(-1),
                M5 = matrix[4],
                M6 = matrix[5]
            };
            return settting;
        }

        [HttpPost]
        public void UploadFile()
        {
            string path = Server.MapPath("~/Content/Uploads/");
            HttpPostedFileBase postedFile = Request.Files["files"];
            string fileExt = Path.GetExtension(postedFile.FileName);
            string fileName = Path.GetRandomFileName().Replace(".", string.Empty);
            string temporaryPath = path + fileName + fileExt;
            postedFile.SaveAs(temporaryPath);
            Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize("/Content/Uploads/"+fileName + fileExt));
        }


        [HttpPost]
        public JsonResult SaveImage(string data, string path, int w, int h, float x, float y)
        {
            string file = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            string _path = Server.MapPath("~/Content/Uploads/") + file + ".png";
            if (w != 0 && h != 0)
            {
                BitmapManager bm = new BitmapManager();
                Bitmap rootImg = new Bitmap(w, h);
                rootImg.SetResolution(72,72);
                rootImg.MakeTransparent();
                Bitmap layer = bm.ConvertFromBase64(data);
                Graphics g = Graphics.FromImage(rootImg);
                g.DrawImage(layer, x, y);
                rootImg.Save(_path);
            }
            else
            {
                BitmapManager bm = new BitmapManager();
                Bitmap img = bm.ConvertFromBase64(data);
                img.Save(_path);
            }
            return new JsonResult { Data = "/Content/Uploads/" + file + ".png", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult>  Edit(int id, TemplateView view)
        {
            try
            {
                using (ApiConnector client = new ApiConnector())
                {
				
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Edit", id);
            }
        }
		
        public ActionResult Delete(int id)
        {
            return View();
        }
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
