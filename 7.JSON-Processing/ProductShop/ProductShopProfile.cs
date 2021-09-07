using AutoMapper;
using ProductShop.Models;
using ProductShop.Models.DTO_Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<CategoryProductDTO, CategoryProduct>();
        }
    }
}
