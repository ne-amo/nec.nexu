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
using nec.nexu.rendering;
using nec.nexu.configurator;

public class QueryImage : IHttpAsyncHandler
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
                string w = _context.Request.QueryString["w"];
                string h = _context.Request.QueryString["h"];

                if (string.IsNullOrEmpty(w) && string.IsNullOrEmpty(h))
                {
                    _context.Response.Clear();
                    _context.Response.ContentType = getContentType(_context.Request.PhysicalPath);
                    _context.Response.WriteFile(_context.Request.PhysicalPath);
                    _context.Response.End();

                }
                else
                {
                    int width = 0;
                    int.TryParse(w, out width);
                    int height = 0;
                    int.TryParse(h, out height);

                    NexuRender render = new NexuRender();
                    using (Bitmap original = new Bitmap(_context.Request.PhysicalPath))
                    {
                        byte[] buffer = new byte[0];
                        if (width > 0 && height > 0)
                        {
                            buffer = render.ConvertImageToByteArray(render.ScaleImage(original, width, height));
                        }
                        if (width == 0 && height > 0)
                        {
                            double ratio = (double)height / (double)original.Height;
                            int newWidth = Convert.ToInt32(original.Width * ratio);
                            buffer = render.ConvertImageToByteArray(render.ScaleImage(original, newWidth, height));
                        }
                        if (height == 0 && width > 0)
                        {
                            double ratio = (double)width / (double)original.Width;
                            int newHeight = Convert.ToInt32(original.Height * ratio);
                            buffer = render.ConvertImageToByteArray(render.ScaleImage(original, width, newHeight));
                        }
                        _context.Response.Clear();

                        //_context.Response.Filter = new System.IO.Compression.GZipStream(_context.Response.Filter, System.IO.Compression.CompressionMode.Compress);
                        //_context.Response.AppendHeader("Content-encoding", "gzip");

                        _context.Response.ContentType = getContentType(_context.Request.PhysicalPath);
                        _context.Response.BinaryWrite(buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                _context.Response.Clear();
                _context.Response.StatusCode = 404;
            }
            finally
            {
                _context.Response.Flush();
            }
            _completed = true;
            _callback(this);
        }
        private string getContentType(String path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp": return "Image/bmp";
                case ".gif": return "Image/gif";
                case ".jpg": return "Image/jpeg";
                case ".png": return "Image/png";
                default: break;
            }
            return "";
        }
        private ImageFormat getImageFormat(String path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp": return ImageFormat.Bmp;
                case ".gif": return ImageFormat.Gif;
                case ".jpg": return ImageFormat.Jpeg;
                case ".png": return ImageFormat.Png;
                default: break;
            }
            return ImageFormat.Jpeg;
        }
    }
}