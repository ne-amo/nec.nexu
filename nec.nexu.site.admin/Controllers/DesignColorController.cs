using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using nec.nexu.Models;
using ColorMine;

namespace nec.nexu.site.admin.Controllers
{
    public class DesignColorController : Controller
    {
        private string apiUrl = "api/masterdata/designcolor/";
        private string apiUrlMultiple = "api/masterdata/designcolors/";


        public async Task<ActionResult> Index()
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrlMultiple);
                if (response.IsSuccessStatusCode)
                {
                    List<DesignColor> list = await response.Content.ReadAsAsync<List<DesignColor>>();
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
                HttpResponseMessage response = await client.GetAsync(apiUrl+id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    DesignColor ret = await response.Content.ReadAsAsync<DesignColor>();
                    return View(ret);
                }
                else
                {
                    return View();
                }
            }
        }

        public ActionResult Create()
        {
            return View(new DesignColor());
        }

        [HttpPost]
        public async Task<ActionResult> Create(DesignColor designColor)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(designColor);

                if (string.IsNullOrEmpty(designColor.Hex))
                    return View(designColor);

                //entity.Hex = entity.Hex.Replace("#", string.Empty);
                // Manually set the HSB values based on the HEX
                //int red = int.Parse(entity.Hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                //int green = int.Parse(entity.Hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                //int blue = int.Parse(entity.Hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                //ColorMine.ColorSpaces.Rgb rgb = new ColorMine.ColorSpaces.Rgb { R = red, G = green, B = blue };
                //ColorMine.ColorSpaces.Hsb hsb = rgb.To<ColorMine.ColorSpaces.Hsb>();
                //entity.Hue = hsb.H;
                //entity.Saturation = hsb.S;
                //entity.Brightness = hsb.B;



                using (ApiConnector client = new ApiConnector())
                {
                    var color = ColorTranslator.FromHtml(designColor.Hex);
                    double hue, saturation, brightness;
                    hue = color.GetHue();
                    saturation = color.GetSaturation();
                    brightness = color.GetBrightness();
                    DesignColor newDesignColor = new DesignColor()
                    {
                        Active = true,
                        BlendMode = designColor.BlendMode,
                        Brightness = brightness,
                        Hue = hue,
                        Saturation = saturation,
                        DisplayName = designColor.DisplayName,
                        Hex = designColor.Hex,
                        Opacity = designColor.Opacity,
                        Pms = designColor.Pms,
                        SortOrder = designColor.SortOrder,
                        TextColor = designColor.TextColor
                    };
                    DesignColor ret = await client.Create<DesignColor>(apiUrl, newDesignColor);
                    if (ret != null)
                    {
                        return RedirectToAction("Details", new { id = ret.Id });
                    }
                    else
                    {
                        return View(designColor);
                    }
                }
            }
            catch
            {
                return View(designColor);
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            using (ApiConnector client = new ApiConnector())
            {
                HttpResponseMessage response = await client.GetAsync("api/masterdata/designcolor/" + id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    DesignColor ret = await response.Content.ReadAsAsync<DesignColor>();
                    return View(ret);
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, DesignColor designColor)
        {
            try
            {
                if(!ModelState.IsValid)
                    return RedirectToAction("Edit", new { id = id });

                if (string.IsNullOrEmpty(designColor.Hex))
                    return RedirectToAction("Edit", new { id = id });

                //entity.Hex = entity.Hex.Replace("#", string.Empty);
                // Manually set the HSB values based on the HEX
                //int red = int.Parse(entity.Hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                //int green = int.Parse(entity.Hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                //int blue = int.Parse(entity.Hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                //ColorMine.ColorSpaces.Rgb rgb = new ColorMine.ColorSpaces.Rgb { R = red, G = green, B = blue };
                //ColorMine.ColorSpaces.Hsb hsb = rgb.To<ColorMine.ColorSpaces.Hsb>();
                //entity.Hue = hsb.H;
                //entity.Saturation = hsb.S;
                //entity.Brightness = hsb.B;
                using (ApiConnector client = new ApiConnector())
                {
                    // calculate hsb from hex value
                    var color = ColorTranslator.FromHtml(designColor.Hex);
                    double hue, saturation, brightness;
                    hue = color.GetHue();
                    saturation = color.GetSaturation();
                    brightness = color.GetBrightness();

                    designColor.Hue = hue;
                    designColor.Saturation = saturation;
                    designColor.Brightness = brightness;

                    HttpResponseMessage response = await client.PutAsJsonAsync(apiUrl + id.ToString(), designColor);
                    return RedirectToAction("Details", new { id = id });
                }
            }
            catch
            {
                return View(designColor);
            }
        }

        /*
         * Unused. Master Data should only be deactivated
         * 
        public ActionResult Delete(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        */ 
    }
}
