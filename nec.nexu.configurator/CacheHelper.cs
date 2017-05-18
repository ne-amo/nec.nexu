using System;
using System.Collections.Generic;
using System.Linq;
using nec.nexu.Models;
using nec.nexu.JsonModels;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace nec.nexu.configurator
{
    public class ImageCacheMessage
    {
        public int Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        public byte[] Data { get; set; }
    }
    public static class CacheHelper
    {

        public static async Task<List<nec.nexu.JsonModels.Material>> LoadColors()
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.NotRemovable;
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddDays(1);
            CacheEntryRemovedCallback callback = null;
            try
            {
                using (ApiConnector api = new ApiConnector())
                {
                    List<nec.nexu.JsonModels.Material> ret = await api.Read<List<nec.nexu.JsonModels.Material>>("ColorLists/allColors/");
                    cache.Set("ColorGrabColors", ret, policy);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                //@@todo logging
                throw ex;
            }

        }

        public static async Task<ImageDataCache> LoadImage(int id, string imageType)
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.NotRemovable;
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddDays(7);
            CacheEntryRemovedCallback callback = null;
            using (ApiConnector api = new ApiConnector())
            {
                try
                {

                    if (imageType == "t")
                    {
                        ImageDataCache image = await api.Read<ImageDataCache>("templateimage/byimagedata/" + id);
                        //ImageData data = await api.Read<ImageData>("masterdata/imagedata/" + id);
                        cache.Set("image" + image.Id, image, policy);
                        return image;
                        //if (image != null && data != null)
                        //{
                        //    ImageCacheMessage msg = new ImageCacheMessage
                        //    {
                        //        Id = image.Id,
                        //        X = image.xPos,
                        //        Y = image.yPos,
                        //        W = image.Width,
                        //        H = image.Height,
                        //        Data = data.Data
                        //    };
                        //
                        //}
                    }
                    else if (imageType == "d")
                    {
                        ImageDataCache design = await api.Read<ImageDataCache>("designs/bylayerimagedata/" + id);
                        cache.Set("image" + design.Id, design, policy);
                        return design;

                        //if (design != null)
                        //{
                        //    ImageData data = await api.Read<ImageData>("masterdata/imagedata/" + id);
                        //    ImageCacheMessage msg = new ImageCacheMessage
                        //    {
                        //        Id = id,
                        //        X = 0,
                        //        Y = 0,
                        //        W = design.WidthPixel,
                        //        H = design.HeightPixel,
                        //        Data = data.Data
                        //    };
                        //    cache.Set("image" + id, msg, policy);
                        //    return msg;
                        //}
                    }
                    return null;
                }
                catch(Exception ex) { 
                    //@@todo logging
                    throw ex;
                }
            }
        }
    }
}