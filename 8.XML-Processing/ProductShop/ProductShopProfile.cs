using AutoMapper;
using ProductShop.Dtos.Input;
using ProductShop.Dtos.Output;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UsersInputDTO,User>().ReverseMap();

            CreateMap<ProductsInputDTO,Product>().ReverseMap();

            CreateMap<CategoriesInputDTO, Category>().ReverseMap();

            CreateMap<CategoriesProductsInputDTO,CategoryProduct>().ReverseMap();

            CreateMap<Product,ProductOutputDTO>().ReverseMap();
        }
    }
}
