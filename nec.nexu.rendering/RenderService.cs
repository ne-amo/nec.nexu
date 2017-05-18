using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using nec.modules.bitmap;
using System.Xml;
using System.Runtime.InteropServices;
using Svg;
using ImageBlurFilter;

namespace nec.nexu.rendering
{

    public class NexuRender
    {
        private BitmapManager _bitmapManager = new BitmapManager();

        public byte[] ConvertImageToByteArray(Bitmap image)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                stream.Close();
                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        public byte[] SvgToBitmap(string svg, int width, int height, int resolution)
        {
            Bitmap bmp = new Bitmap(width, height);
            bmp.SetResolution(resolution, resolution);
            Graphics clipG = Graphics.FromImage(bmp);
            var path = new GraphicsPath();
            foreach (var segment in SvgPathBuilder.Parse(svg))
                segment.AddToPath(path);
            clipG.FillPath(Brushes.Black, path);
            return ConvertImageToByteArray(bmp);
        }



        private Color toColor(string hex)
        {
            Color colorValueFromHex = new Color();
            if (!hex.ToUpper().Contains("URL"))
                if (!hex.Equals(string.Empty))
                    if (!hex.Equals("GROUP"))
                        colorValueFromHex = ColorTranslator.FromHtml("#" + hex.Replace("#", string.Empty));
            return colorValueFromHex;
        }



        private int _ComputeAlphaForByteArray(double submitted)
        {
            if (submitted == 0)
            {
                return 0;
            }
            if (submitted <= 1)
            {
                return (int)Math.Floor(255 * submitted);
            }
            if (1 < submitted  && submitted < 101)
            {
                return (int)Math.Floor(255 * (submitted/100));
            }
            if (101 < submitted && submitted < 256)
            {
                return (int)submitted;
            }
            return 1;
        }

        private RecolorOptions _GetOptionsFromParams(string color, string BlendMode, double alpha, bool trim){
            return new RecolorOptions
            {
                Color = this.toColor(color),
                BlendMode = BlendMode,
                Alpha = this._ComputeAlphaForByteArray(alpha),
                Trim = trim
            };
        }

        public void DrawRegionLayer(ref Graphics baseGraphics,
            Bitmap baseImage,
            string colorHex,
            string BlendMode,
            int Alpha,
            int DPI,
            int Width,
            int Height,
            int X,
            int Y,
            Bitmap LayerImage)
        {
            RecolorOptions options = _GetOptionsFromParams(colorHex, BlendMode, Alpha, true);
            using (Bitmap newLayer = new Bitmap(Width, Height)) // Empty Layer which will have the region drawn into it. It's the same size as root.
            {
                newLayer.MakeTransparent();
                newLayer.SetResolution(DPI, DPI);
                Graphics g = Graphics.FromImage(newLayer);
                var pnt = new Point(X, Y); // Where the region is drawn
                //LayerImage.SetResolution(DPI, DPI);
                g.DrawImage(LayerImage, pnt); // Draw the region onto the new layer at specified coordiantes.

                Bitmap img = new Bitmap(Image.FromStream(
                        new MemoryStream(this._bitmapManager.GetRecoloredArray(baseImage, newLayer, options))
                    ));
                /*Image img = Image.FromStream(
                        new MemoryStream(this._bitmapManager.GetRecoloredArray(baseImage, newLayer, options))
                    ); // Recolored Image
                */
                
                baseGraphics.DrawImage(img, pnt); //Draw the recolored layer onto the main canvas.
            }
        }

        /// <summary>
        /// Places a layer, does not recolor
        /// </summary>
        /// <param name="baseGraphics"></param>
        /// <param name="baseImage"></param>
        /// <param name="DPI"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="BaseWidth"></param>
        /// <param name="BaseHeight"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Transform"></param>
        /// <param name="ClippingMask"></param>
        public void DrawPatternLayer(ref Graphics baseGraphics,
            Bitmap baseImage,
            int DPI,
            int Width,
            int Height,
            int X,
            int Y,
            Bitmap ClippingMask)
        {
            using (Bitmap newLayer = new Bitmap(Width, Height)) // Empty Layer which will have the region drawn into it. It's the same size as root.
            {
                newLayer.MakeTransparent();
                newLayer.SetResolution(DPI, DPI);
                Graphics g = Graphics.FromImage(newLayer);
                var pnt = new Point(X, Y); // Where the region is drawn
                g.DrawImage(ClippingMask, pnt); // Draw the region onto the new layer at specified coordiantes.
                //ClippingMask.SetResolution(DPI, DPI);
                Bitmap clipped = this.MaskImage(newLayer, baseImage);
                baseGraphics.DrawImage(clipped, 0,0); //Draw the pattern layer onto the main canvas.
                clipped.Dispose();
            }
        }






        /// <summary>
        /// Places a layer, does not recolor
        /// </summary>
        /// <param name="baseGraphics"></param>
        /// <param name="baseImage"></param>
        /// <param name="DPI"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="BaseWidth"></param>
        /// <param name="BaseHeight"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Transform"></param>
        /// <param name="ClippingMask"></param>
        public void DrawLogoLayer(ref Graphics baseGraphics,
            Bitmap baseImage,
            int DPI,
            int Width,
            int Height,
            int BaseWidth,
            int BaseHeight,
            int X,
            int Y,
            System.Drawing.Drawing2D.Matrix Transform,
            Bitmap ClippingMask)
        {
            // The SVG transform is based off of 72 dpi. When placing the recolored logo, the logo needs to be center registered and placed at 0,0.
            int logoW = Width * (DPI / 72);
            int logoH = Height * (DPI / 72);
            Point center = new Point(-(logoW / 2), -(logoH / 2));
            // Bitmap from existing layer data
            baseImage.SetResolution(DPI, DPI);
            Image recoloredImage = baseImage;
            using (Bitmap newLayer = new Bitmap(BaseWidth, BaseHeight))
            {
                newLayer.SetResolution(DPI, DPI);
                newLayer.MakeTransparent();
                Graphics g = Graphics.FromImage(newLayer);
                g.Transform = Transform; ;
                g.DrawImage(recoloredImage, center);
                ClippingMask.SetResolution(DPI, DPI);
                Bitmap clipped = this.MaskImage(ClippingMask, newLayer);
                // Not sure about the -2? Might be a result of floats being cast to ints?
                baseGraphics.DrawImage(clipped, -2, 0);
                //ClippingMask.Dispose();
                clipped.Dispose();
            }
        }

        /// <summary>
        /// Recolors and places a layer
        /// </summary>
        /// <param name="baseGraphics"></param>
        /// <param name="baseImage"></param>
        /// <param name="colorHex"></param>
        /// <param name="BlendMode"></param>
        /// <param name="Alpha"></param>
        /// <param name="DPI"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="BaseWidth"></param>
        /// <param name="BaseHeight"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Transform"></param>
        /// <param name="LayerImage"></param>
        /// <param name="ClippingMask"></param>
        public void DrawLogoLayer(ref Graphics baseGraphics,
            Bitmap baseImage,
            string colorHex,
            string BlendMode,
            int Alpha,
            int DPI,
            int Width,
            int Height,
            int BaseWidth,
            int BaseHeight,
            int X,
            int Y,
           System.Drawing.Drawing2D.Matrix Transform,
            Bitmap LayerImage,
            Bitmap ClippingMask)
        {
            RecolorOptions options = _GetOptionsFromParams(colorHex, BlendMode, Alpha, false);
            // The SVG transform is based off of 72 dpi. When placing the recolored logo, the logo needs to be center registered and placed at 0,0.
            int logoW = Width * (DPI / 72);
            int logoH = Height * (DPI / 72);
            Point center = new Point(-(logoW / 2), -(logoH / 2));
            // Bitmap from existing layer data
            LayerImage.SetResolution(DPI, DPI);
            byte[] recolored = this._bitmapManager.GetRecoloredArray(baseImage, LayerImage, options);
            Image recoloredImage = Image.FromStream(new MemoryStream(recolored));
            using (Bitmap newLayer = new Bitmap(BaseWidth, BaseHeight))
            {
                newLayer.SetResolution(DPI, DPI);
                newLayer.MakeTransparent();
                Graphics g = Graphics.FromImage(newLayer);
                g.Transform = Transform; ;
                g.DrawImage(recoloredImage, center);
                ClippingMask.SetResolution(DPI, DPI);
                Bitmap clipped = this.MaskImage(ClippingMask, newLayer);
                // Not sure about the -2? Might be a result of floats being cast to ints?
                baseGraphics.DrawImage(clipped, -2, 0);
                //ClippingMask.Dispose();
                clipped.Dispose();
            }
        }







        /// <summary>
        /// Modifies a RGB byte array of an image clipped to the masks drawn region
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="overlay"></param>
        /// <returns></returns>
        private Bitmap MaskImage(Bitmap mask, Bitmap overlay)
        {
            Bitmap clipRegion = mask;
            if (mask.Width != overlay.Width || mask.Height != overlay.Height)
            {
                clipRegion = this.ResizeImage(mask, new Size { Width = overlay.Width, Height = overlay.Height });
            }


            BitmapData maskData = clipRegion.LockBits(
                        new Rectangle(0, 0, clipRegion.Width, clipRegion.Height),
                        ImageLockMode.WriteOnly,
                        clipRegion.PixelFormat);
            byte[] maskBuffer = new byte[maskData.Stride * maskData.Height];
            Marshal.Copy(maskData.Scan0, maskBuffer, 0, maskBuffer.Length);

            BitmapData overlayData = overlay.LockBits(
                        new Rectangle(0, 0, overlay.Width, overlay.Height),
                        ImageLockMode.WriteOnly,
                        overlay.PixelFormat);
            byte[] overlayBuffer = new byte[overlayData.Stride * overlayData.Height];
            Marshal.Copy(overlayData.Scan0, overlayBuffer, 0, overlayBuffer.Length);


            int x = 0;
            while (x < maskBuffer.Length)
            {
                if (maskBuffer[x + 3] == 0 && (x + 3) <= overlayBuffer.Length)
                    overlayBuffer[x + 3] = (byte)0;
                x += 4;
            }

            Marshal.Copy(overlayBuffer, 0, overlayData.Scan0, overlayBuffer.Length);
            clipRegion.UnlockBits(maskData);
            overlay.UnlockBits(overlayData);
            //clipRegion.Dispose();

            return overlay;
        }

        /// <summary>
        /// Place an Image on a centered on a larger transparent canvas.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>New larger bitmap</returns>
        public Bitmap PlaceImage(Bitmap original, int width, int height)
        {
            Bitmap toPlace = original;
            if(original.Width > width || original.Height > height){
                toPlace = this.ResizeImage(toPlace, new Size{ Width=width, Height = height });
            }


            int wDiff = (width - toPlace.Width);
            int hDiff = (height - toPlace.Height);

            Bitmap newBitmap = new Bitmap(width, height);
            newBitmap.SetResolution(original.HorizontalResolution, original.VerticalResolution);
            newBitmap.MakeTransparent();
            Graphics g = Graphics.FromImage(newBitmap);

            g.DrawImage(toPlace, wDiff / 2, hDiff / 2);

            return newBitmap;
        }
        private Size scaleSize(Size source, Size max)
        {

            var ratioX = (double)max.Width / source.Width;
            var ratioY = (double)max.Height / source.Height;
            var ratio = Math.Min(ratioX, ratioY);

            return new Size { Width = (int)(source.Width * ratio), Height = (int)(source.Height * ratio) };
        }
        public Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            return this.ResizeImage(image, new Size { Width = width, Height = height });
        }
        private Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {
            try
            {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                return b;
            }
            catch (Exception ex) { throw ex; }
        }

        public string DecodeStringFromBase64(string stringToDecode)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(stringToDecode));
        }



        public Bitmap ScaleImage(Bitmap image, int canvasWidth, int canvasHeight)
        {

            int originalHeight = image.Height;
            int originalWidth = image.Width;

            Bitmap thumbnail =
                new Bitmap(canvasWidth, canvasHeight);
            thumbnail.MakeTransparent();
            System.Drawing.Graphics graphic =
                         System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            //graphic.Clear(); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            return new Bitmap(thumbnail);

            /*
            System.Drawing.Imaging.ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality,100L);




            thumbnail.Save(path + newWidth + "." + originalFilename, info[1],
                             encoderParameters);*/
        }





    }

}
