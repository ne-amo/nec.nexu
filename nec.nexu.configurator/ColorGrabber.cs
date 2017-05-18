using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using nec.modules.bitmap;
using System.Linq;
using nec.nexu.Models;
using System.IO;
using System.Web;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using nec.nexu.configurator;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
using ColorMine.ColorSpaces.Conversions.Utility;
using AForge.Imaging.ColorReduction;

public class ColorGrabber : IHttpAsyncHandler
{
    public void ProcessRequest(HttpContext context)
    {
        throw new NotImplementedException();
    }
    public bool IsReusable
    {
        get { return false; }
    }
    public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
    {
        AsynchOperation asynch = new AsynchOperation(cb, context, extraData);
        asynch.StartAsyncWork();
        return asynch;
    }
    public void EndProcessRequest(IAsyncResult result)
    {

    }

    internal class HsbComparer : IEqualityComparer<IHsb>
    {
        public bool Equals(IHsb x, IHsb y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.H == y.H && x.S == y.S && x.B == y.B;
        }
        public int GetHashCode(IHsb obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;
            return obj.H.GetHashCode() ^ obj.S.GetHashCode() ^ obj.B.GetHashCode();
        }

    }



    class AsynchOperation : IAsyncResult
    {

        private bool _completed;
        private Object _state;
        private AsyncCallback _callback;
        private HttpContext _context;
        private ObjectCache cache = MemoryCache.Default;

        bool IAsyncResult.IsCompleted { get { return _completed; } }
        WaitHandle IAsyncResult.AsyncWaitHandle { get { return null; } }
        Object IAsyncResult.AsyncState { get { return _state; } }
        bool IAsyncResult.CompletedSynchronously { get { return false; } }

        public AsynchOperation(AsyncCallback callback, HttpContext context, Object state)
        {
            _callback = callback;
            _context = context;
            _state = state;
            _completed = false;
        }

        public void StartAsyncWork()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartAsyncTask), null);
        }

        private async void StartAsyncTask(Object workItemState)
        {

            try
            {
                if (!string.IsNullOrEmpty(_context.Request.QueryString["upload"]))
                {
                    int num = (!string.IsNullOrEmpty(_context.Request.QueryString["numcol"])) ? int.Parse(_context.Request.QueryString["numcol"]) : 10;
                    HttpPostedFile postedFile = _context.Request.Files["files"];
                    List<nec.nexu.JsonModels.Material> matchedMaterial = new List<nec.nexu.JsonModels.Material>();
                    BitmapManager manager = new BitmapManager();


                    //Dictionary<IHsb,int> colors = manager.GetColors(postedFile.InputStream, num);


                    double tolerance = 15.00;
                    List<nec.nexu.JsonModels.Material> allMaterials;
                    object cacheObj = cache.Get("ColorGrabColors");
                    if (cacheObj != null && ((List<nec.nexu.JsonModels.Material>)cacheObj).Count > 0)
                    {
                        allMaterials = (List<nec.nexu.JsonModels.Material>)cacheObj;
                    }
                    else
                    {
                        allMaterials = await CacheHelper.LoadColors();
                    }


                    allMaterials = allMaterials.Where(x => x.Pattern == null).ToList();

                    Dictionary<Color, nec.nexu.JsonModels.Material> colorsToMaterials = allMaterials.ToDictionary(x => ColorTranslator.FromHtml("#" + x.Hex), x => x);
                    using(Bitmap b = new Bitmap(Image.FromStream(postedFile.InputStream)))
                    {
                        Dictionary<Color,int>results = manager.GetColorCountFromPalette(b, colorsToMaterials.Select(x => x.Key).ToArray());
                        foreach (KeyValuePair<Color, int> result in results)
                        {
                            nec.nexu.JsonModels.Material m = colorsToMaterials[result.Key];
                            m.SortOrder = result.Value;
                            matchedMaterial.Add(m);
                        }
                        _context.Response.StatusCode = 200;
                        _context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(
                                matchedMaterial.OrderByDescending(x => x.SortOrder)
                        ));
                    }

                    /*
                    using (Bitmap bitmap = )){
                        ColorImageQuantizer quant = new ColorImageQuantizer( new MedianCutQuantizer( ) );
                        Image reduced = quant.ReduceColors(bitmap, allMaterials.Select(x => ColorTranslator.FromHtml("#"+x.Hex)).ToArray());
                        reduced.Save(@"C:\Temporary\reduced.png", ImageFormat.Png);
                        ImageFormat format = ImageFormat.Png;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            reduced.Save(ms, format);
                            _context.Response.ContentType = "image/png";
                            _context.Response.BinaryWrite(ms.ToArray());
                            _context.Response.Flush();
                        }
                    }
                    */

                    /*
                    double degree = 0;
                    while (degree <= 360)
                    {

                            

                        foreach (KeyValuePair<IHsb, int> item in colors.Where(x=>x.Key.H >= (degree-30) && x.Key.H < degree))
                        {
                            double currentdeltaE = tolerance;
                            Material bestMatch = null;
                            foreach (Material m in allMaterials)
                            {
                                IHsb materialHsb = new Hsb { H = m.Hue, S = m.Saturation, B = m.Brightness };
                                double deltaE = item.Key.Compare(materialHsb, new CieDe2000Comparison());
                                if (deltaE < currentdeltaE)
                                {
                                    currentdeltaE = deltaE;
                                    bestMatch = m;
                                    bestMatch.SortOrder = item.Value;
                                    //bestMatch.Opacity = item.Value;
                                    //bestMatch.Pms = deltaE.ToString();
                                }
                            }
                            if (bestMatch != null && !matchedMaterial.Exists(x => x.Id == bestMatch.Id))
                                matchedMaterial.Add(bestMatch);
                        }

                        degree += 30;

                    }
                    double total = matchedMaterial.Sum(x => x.Opacity);
                        
                    _context.Response.StatusCode = 200;
                    _context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(
                            matchedMaterial.OrderByDescending(x=>x.SortOrder)
                    ));
                    */



                }
            }
            catch (Exception ex)
            {
                // TODO: add logging and error handling
                _context.Response.StatusCode = 500;
                _context.Response.Write(ex);
                //_context.Response.Flush();
            }
            _completed = true;
            _callback(this);
        }
    }



}
