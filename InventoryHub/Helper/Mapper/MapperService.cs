using AutoMapper;
using InventoryHub.Models;
using InventoryHub.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Helper.Mapper
{
    public class MapperService:Profile
    {
        public MapperService()
        {
            CreateMap<ProductViewModel, Product>()
            .ForMember(dest => dest.Category, opt => opt.Ignore()) // تأكد من عدم محاولة التعيين على كائن null
            .ForMember(dest => dest.Store, opt => opt.Ignore()); // نفس الشيء هنا

            CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Store, opt => opt.MapFrom(src => src.Store));
            // خريطة تحويل Category إلى CategoryViewModel
            CreateMap<Category, CategoryViewModel>()
                .ForMember(idx => idx.TotalProducts, opt => opt.Ignore());
            CreateMap<CategoryViewModel, Category>()
                .ForMember(idx=>idx.Products,opt=>opt.Ignore());

            CreateMap<Customer, CustomerViewModel>()
               // .ForMember(idx => idx.CustomerImageUrl, opt => opt.Ignore())
                .ForMember(idx => idx.CustomerImage, opt => opt.Ignore());
            

            CreateMap<CustomerViewModel, Customer>()
               // .ForMember(idx=>idx.CustomerImage,opt=>opt.Ignore())
                .ForMember(idx=>idx.Orders,opt=>opt.Ignore())
                .ForMember(idx=>idx.Invoices,opt=>opt.Ignore());

        }
    }
}
