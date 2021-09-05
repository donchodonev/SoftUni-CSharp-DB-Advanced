﻿namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Core.ViewModels.Orders;
    using FastFood.Models;
    using System.Linq;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Orders
                //Order input model
            this.CreateMap<CreateOrderInputModel, Order>()
                .ForMember(x => x.Customer, y => y.MapFrom(z => z.Customer))
                .ForMember(x => x.EmployeeId, y => y.MapFrom(z => z.EmployeeId));

            this.CreateMap<CreateOrderInputModel,Item>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.ItemId));

            this.CreateMap<CreateOrderInputModel,OrderItem>()
                .ForMember(x => x.Quantity, y => y.MapFrom(z => z.Quantity));

            this.CreateMap<Order, CreateOrderInputModel>()
                .ForMember(x => x.Customer, y => y.MapFrom(z => z.Customer))
                .ForMember(x => x.EmployeeId, y => y.MapFrom(z => z.EmployeeId))
                .ForMember(x => x.ItemId, y => y.MapFrom(z => z.OrderItems.Select(item => item.ItemId)))
                .ForMember(x => x.Quantity, y => y.MapFrom(z => z.OrderItems.Select(qty => qty.Quantity)));

                //Order view model
            this.CreateMap<CreateOrderViewModel, Employee>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Employees));

            this.CreateMap<CreateOrderViewModel, Item>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Items));

            this.CreateMap<Item, CreateOrderViewModel>()
                .ForMember(x => x.Employees, y => y.MapFrom(z => z.Id));

            this.CreateMap<Employee, CreateOrderViewModel>()
                .ForMember(x => x.Items, y => y.MapFrom(z => z.Id));

            //OrderAll view model

            this.CreateMap<Order, OrderAllViewModel>()
                .ForMember(x => x.OrderId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Customer, y=> y.MapFrom(z => z.Customer))
                .ForMember(x => x.Employee, y=> y.MapFrom(z => z.Employee.Name))
                .ForMember(x => x.DateTime, y=> y.MapFrom(z => z.DateTime));
        }
    }
}
