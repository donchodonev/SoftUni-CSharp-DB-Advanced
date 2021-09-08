using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void InitializeAutomapper()
        {
            var config = new MapperConfiguration(x => x.AddProfile<CarDealerProfile>());
            mapper = new Mapper(config);
        }
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            InitializeAutomapper();

            var dtoData = JsonConvert.DeserializeObject<SupplierInput[]>(inputJson);

            Supplier[] suppliers = mapper.Map<Supplier[]>(dtoData);

            context.Suppliers.AddRange(suppliers);

            int countOfImportedSuppliers = context.SaveChanges();

            return $"Successfully imported {countOfImportedSuppliers}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            InitializeAutomapper();

            var dtoData = JsonConvert.DeserializeObject<PartInput[]>(inputJson);

            Part[] parts = mapper.Map<Part[]>(dtoData);

            context.Parts.AddRange(parts.Where(x => context.Suppliers.Any(y => y.Id == x.SupplierId)));

            int countOfPartsImported = context.SaveChanges();

            return $"Successfully imported {countOfPartsImported}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            InitializeAutomapper();

            var carDtoData = JsonConvert.DeserializeObject<CarInput[]>(inputJson);

            List<Car> cars = new List<Car>();

            foreach (var car in carDtoData)
            {
                var currentCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                };

                foreach (var partId in car.PartsId.Distinct())
                {
                    currentCar.PartCars.Add(new PartCar()
                    {
                        PartId = partId
                    });
                }

                cars.Add(currentCar);

            }

            context.Cars.AddRange(cars);

            int countOfCarsImported = context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            InitializeAutomapper();

            var customerDtoData = JsonConvert.DeserializeObject<CustomerInputDTO[]>(inputJson);

            Customer[] customers = mapper.Map<Customer[]>(customerDtoData);

            context.Customers.AddRange(customers);

            int countOfCustomersImported = context.SaveChanges();

            return $"Successfully imported {countOfCustomersImported}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            InitializeAutomapper();

            var saleDtoData = JsonConvert.DeserializeObject<SalesInputDTO[]>(inputJson);

            Sale[] sales = mapper.Map<Sale[]>(saleDtoData);

            context.Sales.AddRange(sales);

            int countOfSalesImported = context.SaveChanges();

            return $"Successfully imported {countOfSalesImported}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customersOrdered = context
                .Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(cust => new
                {
                    cust.Name,
                    BirthDate = cust.BirthDate.ToString("dd/MM/yyyy"),
                    cust.IsYoungDriver
                })

                .ToList();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(customersOrdered, settings);
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotas = context
                .Cars
                .Where(car => car.Make == "Toyota")
                .OrderBy(car => car.Model)
                .ThenByDescending(car => car.TravelledDistance)
                .Select(car => new
                {
                    car.Id,
                    car.Make,
                    car.Model,
                    car.TravelledDistance
                })
                .ToArray();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(toyotas, settings);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.
                Suppliers
                .Where(s => s.IsImporter == false)
                .Select(supp => new
                {
                    supp.Id,
                    supp.Name,
                    PartsCount = supp.Parts.Count
                })
                .ToArray();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(suppliers, settings);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carWithParts = context
                .Cars
                .Select(car => new
                {
                    car = new
                    {
                        Make = car.Make,
                        Model = car.Model,
                        TravelledDistance = car.TravelledDistance,
                    },

                    parts = car.PartCars.Select(part => new
                    {
                        Name = part.Part.Name,
                        Price = part.Part.Price.ToString("F2")
                    })
                })
                .ToArray();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };


            return JsonConvert.SerializeObject(carWithParts, settings);
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.Sales.Select(s => s.Car.PartCars.Sum(x => x.Part.Price)).Sum()
                })
                .OrderByDescending(x => x.SpentMoney)
                .ThenByDescending(x => x.BoughtCars)
                .ToArray();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                }
            };

            return JsonConvert.SerializeObject(customers, settings);
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var carsAndPricing =
                context
                .Sales
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Car.Make,
                        Model = c.Car.Model,
                        TravelledDistance = c.Car.TravelledDistance
                    },
                    customerName = c.Customer.Name,
                    Discount = c.Discount.ToString("F2"),
                    price = c.Car.PartCars.Sum(x => x.Part.Price).ToString("F2"),
                    priceWithDiscount = (c.Car.PartCars.Sum(x => x.Part.Price) * ((100.00M - c.Discount) / 100)).ToString("F2")
                })
                .Take(10)
                .ToArray();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
            };

            return JsonConvert.SerializeObject(carsAndPricing, settings);
        }

        public static void Main(string[] args)
        {
            var db = new CarDealerContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            //1. Import Suppliers

            string supplierJsonData = File.ReadAllText(@".\..\..\..\Datasets\suppliers.json");

            Console.WriteLine(ImportSuppliers(db, supplierJsonData));

            //2. Import Parts

            string partsJsonData = File.ReadAllText(@".\..\..\..\Datasets\parts.json");

            Console.WriteLine(ImportParts(db, partsJsonData));

            //3. Import Cars

            string carsJsonData = File.ReadAllText(@".\..\..\..\Datasets\cars.json");

            Console.WriteLine(ImportCars(db, carsJsonData));

            //4. Import Customers

            string customersJsonData = File.ReadAllText(@".\..\..\..\Datasets\customers.json");

            Console.WriteLine(ImportCustomers(db, customersJsonData));

            //5. Import Sales

            string salesJsonData = File.ReadAllText(@".\..\..\..\Datasets\sales.json");

            Console.WriteLine(ImportSales(db, salesJsonData));

            //6. Export Ordered Customers

            Console.WriteLine(GetOrderedCustomers(db));

            //7. Export Cars from Make Toyota

            Console.WriteLine(GetCarsFromMakeToyota(db));

            //8. Export Local Suppliers

            Console.WriteLine(GetLocalSuppliers(db));

            //9. Export Cars With Their List Of Parts

            Console.WriteLine(GetCarsWithTheirListOfParts(db));

            //10. Export Total Sales By Customer

            Console.WriteLine(GetTotalSalesByCustomer(db));

            //11. Export Sales With Applied Discount

            Console.WriteLine(GetSalesWithAppliedDiscount(db));
        }
    }
}