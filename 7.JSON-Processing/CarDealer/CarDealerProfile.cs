using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<SupplierInput, Supplier>()
                .ReverseMap();

            CreateMap<PartInput, Part>()
                .ForMember(x => x.SupplierId, y => y.MapFrom(z => z.SupplierId));

            CreateMap<Part, PartInput>()
            .ForMember(x => x.SupplierId, y => y.MapFrom(z => z.SupplierId));

            CreateMap<CarInput,Car>()
                .ReverseMap();
        }
    }
}
