using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nec.nexu.Models;
using nec.nexu.ViewModels;

namespace nec.nexu.App_Start
{
    internal class MaterialComparer : IEqualityComparer<nec.nexu.Models.Material>
    {
        public bool Equals(nec.nexu.Models.Material x, nec.nexu.Models.Material y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.Id == y.Id;
        }
        public int GetHashCode(nec.nexu.Models.Material obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;
            return obj.Id.GetHashCode();
        }

    }

	public static class AutoMapperConfig
	{
		public static void RegisterMappings()
        {
            #region View Model Mapping

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ColorList, ColorListViewModel>()
				.ForMember(dest => dest.MaterialCount,
						   opts => opts.MapFrom(src => src.Colors.Count));

            // Finished Product
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FinishedProduct, FinishedProductViewViewModel>()
		        .ForMember(dest => dest.UserViewCount, opts => opts.MapFrom(src => src.UserViews.Count));

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FinishedProduct, FinishedProductViewModel>()
                .ForMember(map => map.Likes,
                    model => model.MapFrom(src => src.UserLikes.Count));
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FinishedProduct, FinishedProductViewModel>()
               .ForMember(map => map.Views,
                   model => model.MapFrom(src => src.UserViews.Count));
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FinishedProduct, FinishedProductViewModel>()
               .ForMember(map => map.Inspirations,
                   model => model.MapFrom(src => src.Inspirations.Count));

		    AutoMapper.Mapper.CreateMap<ApplicationUser, UserDesignerRankViewModel>();

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FinishedProduct, FinishedProductGalleryDetailView>()
                .ForMember(map => map.DisplayName,
                    model => model.MapFrom(src => src.DisplayName))
                .ForMember(map => map.CreationDate,
                    model => model.MapFrom(src=>src.DateCreated))
               .ForMember(map => map.CreatorName,
                    model => model.MapFrom(src => src.Creator.Nickname))
                .ForMember(map => map.CreatorCountry,
                    model => model.MapFrom(src => src.Creator.UserCountry))
                .ForMember(map => map.CreatorProfileImagePath,
                    model => model.MapFrom(src => src.Creator.ProfileImageFileName))
               .ForMember(map => map.TotalViews,
                   model => model.MapFrom(src => src.UserViews.Count()))
                .ForMember(map => map.TotalLikes,
                    model => model.MapFrom(src => src.UserLikes.Count()))
                .ForMember(map => map.TotalInspirations,
                   model => model.MapFrom(src => src.Inspirations.Count()))
                .ForMember(map => map.TemplateId,
                   model => model.MapFrom(src => src.Template.Id))
                .ForMember(map => map.HierarchyId,
                   model => model.MapFrom(src => src.Hierarchy.Id))
                .ForMember(map => map.ThumbnailPath,
                   model => model.MapFrom(src => src.Images.FirstOrDefault().FilePath))
                .ForMember(map => map.DetailPath,
                   model => model.MapFrom(src => src.Images.FirstOrDefault().FilePath))
                .ForMember(map => map.PriceDescription,
                   model => model.MapFrom(src => src.Template.PriceDescription))
                .ForMember(map => map.Materials,
                    model => model.MapFrom(src => src.Regions
                        .Select(x => new nec.nexu.Models.Material { Id = x.Material.Id, DisplayName = x.Material.DisplayName, Hex = x.Material.Hex })
                        .Distinct(new MaterialComparer())
                        .Select(x => AutoMapper.Mapper.Map<nec.nexu.Models.Material, nec.nexu.ViewModels.GalleryMaterialView>(x))
                        .ToList()
                        ))
                .ForMember(map => map.AllImages,
                    model => model.MapFrom(src => src.Images
                        .ToList()
                        ));

            //
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.Material, MaterialViewModel>()
                .ForMember(m=>m.MatchingDesignColorIds,
                    model=>model.MapFrom(s=>s.MatchingDesignColors.Select(x=>x.Id).ToList()));

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.DesignColor, DesignColorViewModel>();

            // Faceoff Data
		    AutoMapper.Mapper.CreateMap<nec.nexu.Models.FaceOff, FaceoffViewModel>()
		        .ForMember(map => map.FinishedProductAVoteCount,
                model => model.MapFrom(src => (src.FinishedProductAVotes != null? src.FinishedProductAVotes.Count : 0)))
		        .ForMember(map => map.FinishedProductBVoteCount,
                    model => model.MapFrom(src => (src.FinishedProductBVotes != null ? src.FinishedProductBVotes.Count : 0)))
		        .ForMember(map => map.FinishedProductA,
		            model => model.MapFrom(src => src.FinishedProductA))
		        .ForMember(map => map.FinishedProductB,
		            model => model.MapFrom(src => src.FinishedProductB));

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.Design, DesignResultMessage>()
                .ForMember(map => map.DefaultColorwayId,
                    model => model.MapFrom(s => (s.DefaultColorway != null) ? (int?)s.DefaultColorway.Id : null))
                .ForMember(map => map.OfficialColorwayId,
                    model => model.MapFrom(s => (s.OfficialColorway != null) ? (int?)s.OfficialColorway.Id : null))
                .ForMember(map => map.OfficialImage,
                    model => model.MapFrom(s => (s.OfficialImage != null) ? s.OfficialImage.FilePath : null))
                .ForMember(map => map.ThumbnailImage,
                    model => model.MapFrom(s => (s.OfficialImage != null) ? s.OfficialImage.ThumbnailPath : null));

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.Material, nec.nexu.ViewModels.GalleryMaterialView>();
            #endregion

            #region JsonSerialization Mapping

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ColorList, nec.nexu.JsonModels.ColorList>();

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ConfiguratorDirectLink, nec.nexu.JsonModels.ConfiguratorDirectLink>();

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ApplicationUser, nec.nexu.JsonModels.SerializedUser>();

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ColorContainer, nec.nexu.JsonModels.ColorContainer>()
                .ForMember(map => map.Products,
                model => model.MapFrom(s => s.Products.Select(x => x.Id).ToList()));

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.CustomBackground, nec.nexu.JsonModels.CustomBackground>()
                .ForMember(map => map.HierarchyIds,
                    model => model.MapFrom(s => s.AllowedHierarchies.Select(x => x.Id).ToArray()))
                .ForMember(map => map.TemplateIds,
                    model => model.MapFrom(s => s.AllowedTemplates.Select(x => x.Id).ToArray()));

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.Design, nec.nexu.JsonModels.Design>()
                .BeforeMap((src, dest) => Translator.Translate<nec.nexu.Models.Design>(ref src))
                .ForMember(map => map.LocationIds,
                    model => model.MapFrom(s => s.Locations.Select(x => x.Id).ToArray()));
                /*.ForMember(map => map.Locations,
                    model => model.MapFrom(s=> s.Locations.Select(x=> new nec.nexu.JsonModels.DesignLocation { Id = x.Id, DisplayName = x.DisplayName } )));*/

            AutoMapper.Mapper.CreateMap<nec.nexu.Models.DesignColor, nec.nexu.JsonModels.DesignColor>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.DesignColor, nec.nexu.JsonModels.DesignColorMatchMessage>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.DesignColorway, nec.nexu.JsonModels.DesignColorway>()
                .ForMember(map => map.Design,
                    model => model.MapFrom(s =>
                        new nec.nexu.JsonModels.Design
                        {
                            Id = s.Design.Id,
                            DisplayName = s.Design.DisplayName,
                            TapeNumber = s.Design.TapeNumber,
                            HeightPixel = s.Design.HeightPixel,
                            WidthPixel = s.Design.WidthPixel,
                            OfficialImage =
                                AutoMapper.Mapper.Map<nec.nexu.Models.ImageFile, nec.nexu.JsonModels.ImageFile>(s.Design.OfficialImage)
                        }));
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.DesignLayer, nec.nexu.JsonModels.DesignLayer>()
                .BeforeMap((src, dest) => Translator.Translate<nec.nexu.Models.DesignLayer>(ref src));
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.DesignLocation, nec.nexu.JsonModels.DesignLocation>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FaceOff, nec.nexu.JsonModels.FaceOff>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FaceoffVote, nec.nexu.JsonModels.FaceoffVote>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FabricContentType, nec.nexu.JsonModels.FabricContentType>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FinishedProduct, nec.nexu.JsonModels.FinishedProduct>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FinishedProductImage, nec.nexu.JsonModels.FinishedProductImage>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FinishedDesign, nec.nexu.JsonModels.FinishedDesign>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.FinishedRegion, nec.nexu.JsonModels.FinishedRegion>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.Hierarchy, nec.nexu.JsonModels.Hierarchy>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ImageData, nec.nexu.JsonModels.ImageData>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ImageFile, nec.nexu.JsonModels.ImageFile>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ImageType, nec.nexu.JsonModels.ImageType>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.Inspiration, nec.nexu.JsonModels.Inspiration>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.LayerRule, nec.nexu.JsonModels.LayerRule>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.Material, nec.nexu.JsonModels.Material>()
                .BeforeMap((src, dest) => Translator.Translate<nec.nexu.Models.Material>(ref src))
                .ForMember(map => map.MatchingDesignColorIds,
                    model => model.MapFrom(s => s.MatchingDesignColors.Select(x=>x.Id).ToList()));
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.Pattern, nec.nexu.JsonModels.Pattern>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.PatternViewImage, nec.nexu.JsonModels.PatternViewImage>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ProductRegion, nec.nexu.JsonModels.ProductRegion>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ProductRegionGroup, nec.nexu.JsonModels.ProductRegionGroup>()
                .BeforeMap((src, dest) => Translator.Translate<nec.nexu.Models.ProductRegionGroup>(ref src));
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ProductTemplate, nec.nexu.JsonModels.ProductTemplate>()
                .BeforeMap((src,dest) => src.Views.OrderBy(x=>x.SortOrder));
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.TemplateImage, nec.nexu.JsonModels.TemplateImage>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.TemplateView, nec.nexu.JsonModels.TemplateView>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.UserColorList, nec.nexu.JsonModels.UserColorList>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.UserProductLike, nec.nexu.JsonModels.UserProductLike>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.UserProductView, nec.nexu.JsonModels.UserProductView>();


            AutoMapper.Mapper.CreateMap<nec.nexu.Models.rules.ColorDifferentiationRule, nec.nexu.JsonModels.ColorDifferentiationRule>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.rules.DesignColorRule, nec.nexu.JsonModels.DesignColorRule>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.rules.DesignColorRuleValue, nec.nexu.JsonModels.DesignColorRuleValue>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.rules.ColorwayRule, nec.nexu.JsonModels.ColorwayRule>();
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.rules.ColorwayRuleValue, nec.nexu.JsonModels.ColorwayRuleValue>();


            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ConfiguratorDirectLink, nec.nexu.JsonModels.ConfiguratorDirectLink>()
                .ForMember(map => map.Designs,
                        model => model.MapFrom(
                            s => ((s.Designs.Count == 1) ? s.Designs : s.Designs.Select(x => new Design
                            {
                                Id = x.Id,
                                DisplayName = x.DisplayName,
                                TapeNumber = x.TapeNumber,
                                HeightPixel = x.HeightPixel,
                                WidthPixel = x.WidthPixel,
                                OfficialImage = x.OfficialImage,
                                Locations = x.Locations,
                                OfficialColorway = (x.OfficialColorway != null) ? new DesignColorway { Id = x.OfficialColorway.Id } : null,
                                DefaultColorway = (x.DefaultColorway != null) ? new DesignColorway { Id = x.DefaultColorway.Id } : null
                            }
                                ))
                        )
                    );


            #endregion

            #region List Item Map


            AutoMapper.Mapper.CreateMap<nec.nexu.Models.Hierarchy, nec.nexu.ViewModels.ListItem>()
                .BeforeMap((src, dest) => Translator.Translate<nec.nexu.Models.Hierarchy>(ref src));
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ProductTemplate, nec.nexu.ViewModels.ListItem>()
                .BeforeMap((src, dest) => Translator.Translate<nec.nexu.Models.ProductTemplate>(ref src));
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.ColorContainer, nec.nexu.ViewModels.ListItem>()
                .BeforeMap((src, dest) => Translator.Translate<nec.nexu.Models.ColorContainer>(ref src))
                .ForMember(map => map.DisplayName, model => model.MapFrom(src => src.ContainerName));
                   

            #endregion


            #region Translations
            /*
            AutoMapper.Mapper.CreateMap<nec.nexu.Models.LocalizedTest, nec.nexu.ViewModels.LocalizedTest>()
                .BeforeMap((src, dest) => Translator.Translate<nec.nexu.Models.LocalizedTest>(ref src) );
            */

            #endregion
        }
	}
}