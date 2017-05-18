using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Conversions.Utility;
using AForge.Imaging.ColorReduction;

namespace nec.modules.bitmap
{
    public class BitmapManager
    {
        private Bitmap _background;
        public Bitmap BackgroundImage
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
            }
        }

        private delegate byte PerChannelProcessDelegate(ref byte nSrc, ref byte nDst);

        public BitmapManager()
        {

        }
        //public BitmapManager(Bitmap background)
        //{
        //    //_background = background;
        //}
        //public BitmapManager(MemoryStream stream)
        //{
        //   // _background = new Bitmap(stream);
        //}
        //public BitmapManager(byte[] byteArray)
        //{
        //    //MemoryStream ms = new MemoryStream(byteArray);
        //    //_background = new Bitmap(ms);
        //}

        //public string GetRecoloredBase64String(byte[] byteArray, RecolorOptions options)
        //{
        //    MemoryStream ms = new MemoryStream(byteArray);
        //    Bitmap returnImage = new Bitmap(ms);
        //    Recolor(ref returnImage, options.Color, options.Alpha, options.BlendMode);
        //    return ConvertToBase64(returnImage);
        //}
        //public string GetRecoloredBase64String(Bitmap image, RecolorOptions options)
        //{
        //    Recolor(ref image, options.Color, options.Alpha, options.BlendMode);
        //    return ConvertToBase64(image);
        //}

        //public byte[] GetRecoloredByteArray(Bitmap image, RecolorOptions options)
        //{
        //    Recolor(ref image, options.Color, options.Alpha, options.BlendMode);
        //    return ConvertToByteArray(image);
        //}

        public byte[] GetRecoloredArray(Bitmap background, Bitmap layer, RecolorOptions options)
        {
            Recolor(background, layer, options.Color, options.Alpha, options.BlendMode);
            return this.ConvertToByteArray((options.Trim) ? this.TrimBitmap(layer) : layer);
        }

        public byte[] GetRecoloredArray(ref Bitmap background, ref Bitmap layer, RecolorOptions options)
        {
            Recolor(ref background, ref layer, options.Color, options.Alpha, options.BlendMode);
            return this.ConvertToByteArray(layer);
        }


        //public byte[] GetRecoloredByteArray(byte[] arrBase,byte[] arrLayer, RecolorOptions options)
        //{
        //    this.BackgroundImage = new Bitmap(Image.FromStream(new MemoryStream(arrBase)));
        //
        //    return Recolor(arrBase, arrLayer, options.Color, options.Alpha, options.BlendMode);
        //}


        public void Recolor(Bitmap background, Bitmap image, Color color, int alpha, string blend)
        {
            switch (blend)
            {
                case "MULTIPLY":
                    recolorImage(background, image, color, alpha, new PerChannelProcessDelegate(BlendMultiply));
                    break;
                case "SCREEN":
                    recolorImage(background, image, color, alpha, new PerChannelProcessDelegate(BlendScreen));
                    break;
                case "OVERLAY":
                    recolorImage(background, image, color, alpha, new PerChannelProcessDelegate(BlendOverlay));
                    break;
                case "NORMAL":
                    recolorImage(background, image, color, alpha, new PerChannelProcessDelegate(BlendNormal));
                    break;
            }
        }



        public void Recolor(ref Bitmap background, ref Bitmap image, Color color, int alpha, string blend)
        {
            switch (blend)
            {
                case "MULTIPLY":
                    recolorImage(ref background, ref image, color, alpha, new PerChannelProcessDelegate(BlendMultiply));
                    break;
                case "SCREEN":
                    recolorImage(ref background, ref image, color, alpha, new PerChannelProcessDelegate(BlendScreen));
                    break;
                case "OVERLAY":
                    recolorImage(ref background, ref image, color, alpha, new PerChannelProcessDelegate(BlendOverlay));
                    break;
                case "NORMAL":
                    recolorImage(ref background, ref image, color, alpha, new PerChannelProcessDelegate(BlendNormal));
                    break;
            }
        }

        public byte[] Recolor(byte[] arrBase, byte[] arrLayer, Color color, int alpha, string blend)
        {
            switch (blend)
            {
                case "MULTIPLY":
                    return recolorImage(arrBase, arrLayer, color, alpha, new PerChannelProcessDelegate(BlendMultiply));
                case "SCREEN":
                    return recolorImage(arrBase, arrLayer, color, alpha, new PerChannelProcessDelegate(BlendScreen));
                case "OVERLAY":
                    return recolorImage(arrBase, arrLayer, color, alpha, new PerChannelProcessDelegate(BlendOverlay));
                case "NORMAL":
                    return recolorImage(arrBase, arrLayer, color, alpha, new PerChannelProcessDelegate(BlendNormal));
                default:
                    return null;
            }
        }


        private byte[] recolorImage(byte[] bufferBase,byte[] buffer, Color color, int alpha, PerChannelProcessDelegate blend)
        {
            int alphaThresh = 75;
            int x = 0;
            byte b = (byte)color.B;
            byte g = (byte)color.G;
            byte r = (byte)color.R;
            while (x < buffer.Length)
            {

                buffer[x] = blend(ref b, ref bufferBase[x]);
                buffer[x + 1] = blend(ref g, ref bufferBase[x + 1]);
                buffer[x + 2] = blend(ref r, ref bufferBase[x + 2]);
                buffer[x + 3] = (buffer[x + 3] >= alphaThresh && bufferBase[x + 3] >= alphaThresh) ? (byte)alpha : (byte)0;
                x += 4;
            }
            return buffer;
        }

        private void recolorImage(Bitmap background, Bitmap image, Color color, int alpha, PerChannelProcessDelegate blend)
        {
            BitmapData data = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.WriteOnly,
                        image.PixelFormat);
            byte[] buffer = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            BitmapData dataBase = background.LockBits(
                        new Rectangle(0, 0, background.Width, background.Height),
                        ImageLockMode.WriteOnly,
                        background.PixelFormat);
            byte[] bufferBase = new byte[dataBase.Stride * dataBase.Height];
            Marshal.Copy(dataBase.Scan0, bufferBase, 0, bufferBase.Length);
            int alphaThresh = 75;
            int x = 0;
            byte b = (byte)color.B;
            byte g = (byte)color.G;
            byte r = (byte)color.R;
            while (x < buffer.Length)
            {

                buffer[x] = blend(ref b, ref bufferBase[x]);
                buffer[x + 1] = blend(ref g, ref bufferBase[x + 1]);
                buffer[x + 2] = blend(ref r, ref bufferBase[x + 2]);
                buffer[x + 3] = (buffer[x + 3] >= alphaThresh && bufferBase[x + 3] >= alphaThresh) ? (byte)alpha : (byte)0;
                x += 4;
            }
            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
            background.UnlockBits(dataBase);
            image.UnlockBits(data);
        }


        private void recolorImage(ref Bitmap background, ref Bitmap image, Color color, int alpha, PerChannelProcessDelegate blend)
        {
            BitmapData data = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.WriteOnly,
                        image.PixelFormat);
            byte[] buffer = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            BitmapData dataBase = background.LockBits(
                        new Rectangle(0, 0, background.Width, background.Height),
                        ImageLockMode.WriteOnly,
                        background.PixelFormat);
            byte[] bufferBase = new byte[dataBase.Stride * dataBase.Height];
            Marshal.Copy(dataBase.Scan0, bufferBase, 0, bufferBase.Length);
            int alphaThresh = 75;
            //byte a = 0;
            //for (int k = 0; k < buffer.Length; k += 4)
            //{
            //    a = buffer[k + 3];
            //    if (a != 0)
            //    {
            //        byte b = (byte)color.B;
            //        byte g = (byte)color.G;
            //        byte r = (byte)color.R;
            //
            //        //buffer[k] = blend(ref bufferBase[k],ref b);
            //        //buffer[k + 1] = blend(ref bufferBase[k + 1],ref g);
            //        //buffer[k + 2] = blend(ref bufferBase[k + 2],ref r);
            //        //buffer[k + 3] = (byte)alpha;
            //
            //
            //
            //        buffer[k] = blend(ref b, ref bufferBase[k]);
            //        buffer[k + 1] = blend(ref g, ref bufferBase[k+1]);
            //        buffer[k + 2] = blend(ref r, ref bufferBase[k+2]);
            //        buffer[k + 3] = (byte)alpha;
            //    }
            //}
            int x = 0;
            byte b = (byte)color.B;
            byte g = (byte)color.G;
            byte r = (byte)color.R;
            while(x < buffer.Length)
            {

                buffer[x] = blend(ref b, ref bufferBase[x]);
                buffer[x + 1] = blend(ref g, ref bufferBase[x+1]);
                buffer[x + 2] = blend(ref r, ref bufferBase[x+2]);
                buffer[x + 3] = (buffer[x + 3] >= alphaThresh && bufferBase[x + 3] >= alphaThresh) ? (byte)alpha : (byte)0;
                x += 4;
            }







            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
            background.UnlockBits(dataBase);
            image.UnlockBits(data);
        }




        public Bitmap TrimBitmap(Bitmap source)
        {
            Rectangle srcRect = default(Rectangle);
            BitmapData data = null;
            try
            {
                data = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                byte[] buffer = new byte[data.Height * data.Stride];
                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

                int xMin = int.MaxValue,
                    xMax = int.MinValue,
                    yMin = int.MaxValue,
                    yMax = int.MinValue;

                bool foundPixel = false;

                // Find xMin
                for (int x = 0; x < data.Width; x++)
                {
                    bool stop = false;
                    for (int y = 0; y < data.Height; y++)
                    {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
                        if (alpha != 0)
                        {
                            xMin = x;
                            stop = true;
                            foundPixel = true;
                            break;
                        }
                    }
                    if (stop)
                        break;
                }

                // Image is empty...
                if (!foundPixel)
                    return null;

                // Find yMin
                for (int y = 0; y < data.Height; y++)
                {
                    bool stop = false;
                    for (int x = xMin; x < data.Width; x++)
                    {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
                        if (alpha != 0)
                        {
                            yMin = y;
                            stop = true;
                            break;
                        }
                    }
                    if (stop)
                        break;
                }

                // Find xMax
                for (int x = data.Width - 1; x >= xMin; x--)
                {
                    bool stop = false;
                    for (int y = yMin; y < data.Height; y++)
                    {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
                        if (alpha != 0)
                        {
                            xMax = x;
                            stop = true;
                            break;
                        }
                    }
                    if (stop)
                        break;
                }

                // Find yMax
                for (int y = data.Height - 1; y >= yMin; y--)
                {
                    bool stop = false;
                    for (int x = xMin; x <= xMax; x++)
                    {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
                        if (alpha != 0)
                        {
                            yMax = y;
                            stop = true;
                            break;
                        }
                    }
                    if (stop)
                        break;
                }

                srcRect = Rectangle.FromLTRB(xMin, yMin, xMax, yMax);
            }
            finally
            {
                if (data != null)
                    source.UnlockBits(data);
            }

            Bitmap dest = new Bitmap(srcRect.Width, srcRect.Height);
            Rectangle destRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
            using (Graphics graphics = Graphics.FromImage(dest))
            {
                graphics.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
            }
            return dest;
        }

        #region Color Reduction

        public Dictionary<Color,int> GetColorCountFromPalette(Bitmap image, Color[] palette)
        {
            ColorImageQuantizer quant = new ColorImageQuantizer(new MedianCutQuantizer());
            using (Bitmap reduced = new Bitmap(quant.ReduceColors(
                new Bitmap(image.GetThumbnailImage(ThumbSize, ThumbSize, ThumbnailCallback, IntPtr.Zero)),
                palette)))
            {
                return this._GetColors(reduced, 6, true);
            }
        }
         

        #endregion



        #region Conversion Functions

        public byte[] ConvertToByteArray(Bitmap image)
        {
            ImageFormat format = ImageFormat.Png;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        public string ConvertToBase64(Bitmap image)
        {
            try
            {
                string b64;
                ImageFormat format = ImageFormat.Png;
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, format);
                    b64 = Convert.ToBase64String(ms.ToArray());
                }
                return b64;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Bitmap ConvertFromBase64(string base64)
        {
            Bitmap bitmap;
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
                bitmap = new Bitmap(ms);
            return bitmap;
        }
        #endregion

        #region Blend Functions
        private byte BlendMultiply(ref byte Src, ref byte Dst)
        {
            return (byte)Math.Max(Math.Min((Src / 255.0f * Dst / 255.0f) * 255.0f, 255), 0);
        }
        private byte BlendScreen(ref byte Src, ref byte Dst)
        {
            return (byte)Math.Max(Math.Min(255 - ((255 - Src) / 255.0f * (255 - Dst) / 255.0f) * 255.0f, 255), 0);
        }
        private byte BlendOverlay(ref byte Src, ref byte Dst)
        {
            return ((Dst < 128) ? (byte)Math.Max(Math.Min((Src / 255.0f * Dst / 255.0f) * 255.0f * 2, 255), 0) : (byte)Math.Max(Math.Min(255 - ((255 - Src) / 255.0f * (255 - Dst) / 255.0f) * 255.0f * 2, 255), 0));
        }
        private byte BlendNormal(ref byte Src, ref byte Dst)
        {
            return Src;
        }
        #endregion


        #region Color Functions

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

        public int ThumbSize = 128;
        private bool ThumbnailCallback() { return false; }

        //public Dictionary<Color, int> GetColors(Stream stream, int max)
        public Dictionary<IHsb, int> GetColors(Stream stream, int max)
        //public List<IHsb> GetColors(Stream stream, int max)
        //public List<Color> GetColors(Stream stream, int max)    
        {
            using (Bitmap b = new Bitmap(Image.FromStream(stream).GetThumbnailImage(ThumbSize, ThumbSize, ThumbnailCallback, IntPtr.Zero)))
            {
                //b.Save(@"C:\Temporary\_img\colorgrab.png");
                return this._GetColors(b, max);
            }
        }
        //public Dictionary<Color, int> GetColors(Image image, int max)
        public Dictionary<IHsb, int> GetColors(Image image, int max)
        //public List<IHsb> GetColors(Image image, int max)
        //public List<Color> GetColors(Image image, int max)
        {
            using (Bitmap b = new Bitmap(image.GetThumbnailImage(ThumbSize, ThumbSize, ThumbnailCallback, IntPtr.Zero)))
            {
                return this._GetColors(b, max);
            }
        }
        //public Dictionary<Color, int> GetColors(Bitmap bitmap, int max)
        public Dictionary<IHsb, int> GetColors(Bitmap bitmap, int max)
        //public List<IHsb> GetColors(Bitmap bitmap, int max)
        //public List<Color> GetColors(Bitmap bitmap, int max)
        {
            using (Bitmap b = new Bitmap(bitmap.GetThumbnailImage(ThumbSize, ThumbSize, ThumbnailCallback, IntPtr.Zero)))
            {
                return this._GetColors(b, max);
            }
        }

        //private Dictionary<Color, int> _GetColors(Bitmap image, int max=6)
        private Dictionary<IHsb, int> _GetColors(Bitmap image, int max = 6)
        //private List<IHsb> _GetColors(Bitmap image, int max = 6)
        //private List<Color> _GetColors(Bitmap image, int max = 6)
        {
            try
            {
                //Dictionary<Color, int> colors = new Dictionary<Color, int>();
                Dictionary<IHsb, int> colors = new Dictionary<IHsb, int>(new HsbComparer());

                // BitmapData allows for quick processing
                BitmapData bData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
                int depth = System.Drawing.Bitmap.GetPixelFormatSize(image.PixelFormat);
                /*the size of the image in bytes */
                int size = bData.Stride * bData.Height;
                /*Allocate buffer for image*/
                byte[] data = new byte[size];
                /*This overload copies data of /size/ into /data/ from location specified (/Scan0/)*/
                System.Runtime.InteropServices.Marshal.Copy(bData.Scan0, data, 0, size);

                //List<Color> test = new List<Color>();
                for (int i = 0; i < size; i += depth / 8)
                {
                    /*
                    Color col = Color.FromArgb(
                            data[i + 3],
                            data[i],
                            data[i + 1],
                            data[i + 2]
                            );

                    



                    if (colors.ContainsKey(col))
                    {
                        colors[col]++;
                    }
                    else
                    {
                        colors.Add(col, 1);
                    }
                    
                    Color col = Color.FromArgb(
                            data[i + 3],

                            data[i + 2],
                            data[i + 1],
                            data[i]

                            /*
                            data[i],
                            data[i + 1],
                            data[i + 2]
                             
                            );*/
                    IHsb hsb = (new Rgb { R = data[i + 2], G = data[i + 1], B = data[i] }).To<Hsb>();
                    hsb.H = Math.Floor(hsb.H);
                    hsb.S = Math.Round(hsb.S,2);
                    hsb.B = Math.Round(hsb.B,2);
                    //test.Add(col);
                    if (colors.ContainsKey(hsb))
                    {
                        colors[hsb]++;
                    }
                    else
                    {
                        colors.Add(hsb, 1);
                    }
                }
                //List<IHsb> distinctColors = colors.Select(x => x.Key).ToList();//new List<IHsb>();
                //List<Color> distinctColors = colors.Select(x => x.Key).ToList();//new List<IHsb>();
                /*
                //Dictionary<IHsb, int> distinctColors = new Dictionary<IHsb, int>();
                int degree = 0;
                while (degree <= 360)
                {
                    double floor = (degree-30) >= 0 ? (degree-30) : 0;
                    double ceil = degree + 30;
                    // See if there is a color in Hue Range, then take the one with the highest occurence
                    IHsb top = colors.Where(x => x.Key.H >= floor && x.Key.H <= ceil)
                        .OrderBy(x => (x.Value*((x.Key.S*100)+(x.Key.B*100)))).Select(x=>x.Key).FirstOrDefault();
                    if (top != null)
                        distinctColors.Add(top);
                    degree += 30;
                }*/

                //return test;
                
                if (colors.Count > max && max != 0)
                {
                    return colors.OrderByDescending(x => x.Value).Take(max).ToDictionary(x => x.Key, y => y.Value);
                    //return colors.OrderBy(x => x.Value).Take(max).ToDictionary(x=>x.Key,y=>y.Value);
                }
                else
                {
                    return colors.OrderBy(x => x.Value).ToDictionary(x => x.Key, y => y.Value);
                    //return colors.OrderBy(x => x.Value).ToDictionary(x => x.Key, y => y.Value);
                }
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private Dictionary<Color, int> _GetColors(Bitmap image, int max=6, bool ReturnColors = true)
        {
            try
            {
                Dictionary<Color, int> colors = new Dictionary<Color, int>();
                // BitmapData allows for quick processing
                BitmapData bData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
                int depth = System.Drawing.Bitmap.GetPixelFormatSize(image.PixelFormat);
                /*the size of the image in bytes */
                int size = bData.Stride * bData.Height;
                /*Allocate buffer for image*/
                byte[] data = new byte[size];
                /*This overload copies data of /size/ into /data/ from location specified (/Scan0/)*/
                System.Runtime.InteropServices.Marshal.Copy(bData.Scan0, data, 0, size);
                for (int i = 0; i < size; i += depth / 8)
                {
                    Color col = Color.FromArgb(
                            data[i + 3],
                            data[i + 2],
                            data[i + 1],
                            data[i]
                            );
                    if (colors.ContainsKey(col))
                    {
                        colors[col]++;
                    }
                    else
                    {
                        colors.Add(col, 1);
                    }
                }
                if (colors.Count > max && max != 0)
                {
                    return colors.OrderByDescending(x => x.Value).Take(max).ToDictionary(x => x.Key, y => y.Value);
                }
                else
                {
                    return colors.OrderBy(x => x.Value).ToDictionary(x => x.Key, y => y.Value);
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }




        #endregion

    }
}
