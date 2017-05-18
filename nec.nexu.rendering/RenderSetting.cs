using System;
using System.Data;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace nec.nexu.rendering
{
    public class RenderSetting
    {
        public int Id { get; set; }
        public int? TemplateViewId { get; set; }
        public int? LocationId { get; set; }
        public Matrix Transform { get; set; }
        public byte[] ClipData { get; set; }

        public RenderSetting()
        {

        }



        public RenderSetting(System.Data.DataRow row)
        {
            try
            {
                this.Id = (int)row["id"];
                this.TemplateViewId = (int?)row["template_view_id"];
                this.LocationId = (int?)row["location_id"];
                this.ClipData = (byte[])row["clip_data_array"];
                if (
                    !row.IsNull("m1") &&
                    !row.IsNull("m2") &&
                    !row.IsNull("m3") &&
                    !row.IsNull("m4") &&
                    !row.IsNull("m5") &&
                    !row.IsNull("m6")
                )
                {
                    this.Transform = new Matrix(
                        (float)row["m1"],
                        (float)row["m2"],
                        (float)row["m3"],
                        (float)row["m4"],
                        (float)row["m5"],
                        (float)row["m6"]);
                }
            }
            catch { }
        }
    }
    public class ImageDataObject
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public byte[] Data { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ObjectType { get; set; }
        public ImageDataObject() { }
        public ImageDataObject(DataRow row)
        {
            this.Id = (int)row["id"];
            this.Code = row["code"].ToString();
            this.Data = (byte[])row["data_array"];
            this.X = (int)row["y_pos"];
            this.Y = (int)row["x_pos"];
            this.Width = (int)row["width"];
            this.Height = (int)row["heght"];
            this.ObjectType = (int)row["type"];
        }
    }

    public class ProductRenderRequest
    {
        public string FilePath { get; set; }
        public int TemplateViewId { get; set; }
        //@TODO This should be int,int to pass in the specific material color ID. That will allow proper blending options.
        public Dictionary<int, string> ImageDataColors = new Dictionary<int, string>();
        public List<DesignRenderRequest> Designs = new List<DesignRenderRequest>();
    }


    public class DesignRenderRequest
    {
        public int TemplateViewId { get; set; }
        public int DesignId { get; set; }
        public int LocationId { get; set; }
        //@TODO This should be int,int to pass in the specific design color ID. That will allow proper blending options.
        public Dictionary<int, string> ImageDataColors = new Dictionary<int, string>();
    }
}
