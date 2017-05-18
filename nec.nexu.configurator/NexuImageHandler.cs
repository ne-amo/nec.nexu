using System;
using System.Drawing;
using System.Drawing.Imaging;
using nec.modules.bitmap;
using nec.nexu.Models;
using System.IO;
using System.Web;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using nec.nexu.configurator;

public class NexuImageHandler : IHttpAsyncHandler
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
                string l = _context.Request.QueryString["l"]; // Layer ID
                string b = _context.Request.QueryString["b"]; // Base Image Id
                string hex = _context.Request.QueryString["c"]; // Color in HEX Format
                string a = _context.Request.QueryString["a"]; // Alpha
                string t = _context.Request.QueryString["t"]; // Enum of Blend Mode
                string m = _context.Request.QueryString["m"]; // Whether to trim the bitmap
                string r = _context.Request.QueryString["r"]; // Resolution
                string imageType = _context.Request.QueryString["it"]; // Whether its a template(t) or design image(d)
                string x = _context.Request.QueryString["x"];
                string y = _context.Request.QueryString["y"];
                string w = _context.Request.QueryString["w"];
                string h = _context.Request.QueryString["h"];
                if (
                    string.IsNullOrEmpty(l) ||
                    string.IsNullOrEmpty(b) ||
                    string.IsNullOrEmpty(hex) ||
                    string.IsNullOrEmpty(a) ||
                    string.IsNullOrEmpty(t) ||
                    string.IsNullOrEmpty(imageType)
                    )

                    throw new Exception("Invalid Parameters");
                else
                {
                    int layerImageDataId = int.Parse(l);
                    int baseImageDataId = int.Parse(b);

                    float xPos = 0;
                    if (!string.IsNullOrEmpty(x))
                        float.TryParse(x, out xPos);
                    float yPos = 0;
                    if (!string.IsNullOrEmpty(y))
                        float.TryParse(y, out yPos);
                    int width = 0;
                    if (!string.IsNullOrEmpty(w))
                        int.TryParse(w, out width);
                    int height = 0;
                    if (!string.IsNullOrEmpty(h))
                        int.TryParse(h, out height);

                    ImageDataCache layerCache = new ImageDataCache();
                    ImageDataCache baseCache = new ImageDataCache();
                    object cacheLayer = cache.Get("image" + layerImageDataId);
                    if (cacheLayer != null)
                    {
                        layerCache = (ImageDataCache)cacheLayer;
                    }
                    else
                    {
                        layerCache = await CacheHelper.LoadImage(layerImageDataId, imageType);
                    }

                    object cacheBase = cache.Get("image" + baseImageDataId);
                    if (cacheBase != null)
                    {
                        baseCache = (ImageDataCache)cacheBase;
                    }
                    else
                    {
                        baseCache = await CacheHelper.LoadImage(baseImageDataId, imageType);
                    }
                    if (layerImageDataId == baseImageDataId)
                    {
                        _context.Response.ContentType = "image/png";
                        _context.Response.BinaryWrite(baseCache.Data);
                        _context.Response.Flush();
                    }

                    BitmapManager bm = new BitmapManager();
                    RecolorOptions options = new RecolorOptions();

                    int red = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    int green = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    int blue = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

                    options.Color = Color.FromArgb(red, green, blue);
                    options.BlendMode = ((BlendMode)int.Parse(t)).ToString().ToUpper();
                    options.Alpha = (int)Math.Floor((double)(255 * (float.Parse(a)/100)));
                    if (!string.IsNullOrEmpty(m))
                        bool.TryParse(m, out options.Trim);
                    int resolution = 72;
                    if (!string.IsNullOrEmpty(r))
                        int.TryParse(r, out resolution);
                    byte[] ret = null;

                    if (imageType == "t")
                    {
                        using (Bitmap rootImage = new Bitmap(Image.FromStream(new MemoryStream(baseCache.Data))))
                        {
                            rootImage.SetResolution(resolution, resolution);
                            using (Bitmap newLayer = new Bitmap((int)baseCache.W, (int)baseCache.H))
                            {
                                newLayer.SetResolution(resolution, resolution);
                                newLayer.MakeTransparent();
                                Graphics g = Graphics.FromImage(newLayer);
                                g.DrawImage(Image.FromStream(new MemoryStream(layerCache.Data)), layerCache.X, layerCache.Y);
                                //new Point((float)layerCache.X, (float)layerCache.Y));
                                ret = bm.GetRecoloredArray(rootImage, newLayer, options);
                            }
                        }
                    }
                    else if (imageType == "d")
                    {
                        using (Bitmap rootImage = new Bitmap(Image.FromStream(new MemoryStream(baseCache.Data))))
                        {
                            rootImage.SetResolution(resolution, resolution);
                            using (Bitmap newLayer = new Bitmap((int)baseCache.W, (int)baseCache.H))
                            {
                                newLayer.MakeTransparent();
                                Graphics g = Graphics.FromImage(newLayer);
                                Bitmap layer = new Bitmap(Image.FromStream(new MemoryStream(layerCache.Data)));
                                layer.SetResolution(resolution, resolution);
                                g.DrawImage(layer, layerCache.X, layerCache.Y);
                                newLayer.SetResolution(resolution, resolution);
                                ret = bm.GetRecoloredArray(rootImage, newLayer, options);
                            }
                        }
                    }
                    //_context.Response.Filter = new System.IO.Compression.GZipStream(_context.Response.Filter, System.IO.Compression.CompressionMode.Compress);
                    //_context.Response.AppendHeader("Content-Encoding", "gzip");
                    _context.Response.ContentType = "image/png";
                    _context.Response.BinaryWrite(ret);
                    //_context.Response.Flush();
                }
            }
            catch (Exception ex)
            {
                // TODO: add logging and error handling
                //_context.Response.StatusCode = 500;
                _context.Response.Write(ex);
                //_context.Response.Flush();
            }
            _completed = true;
            _callback(this);
        }
    }



}