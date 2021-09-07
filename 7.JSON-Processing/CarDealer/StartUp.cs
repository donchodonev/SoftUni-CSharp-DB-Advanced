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

            var jsonData = JsonConvert.DeserializeObject<SupplierInput[]>(inputJson);

            Supplier[] suppliers = mapper.Map<Supplier[]>(jsonData);

            context.Suppliers.AddRange(suppliers);

            int countOfImportedSuppliers = context.SaveChanges();

            return $"Successfully imported {countOfImportedSuppliers}.";
        }
        public static void Main(string[] args)
        {
            var db = new CarDealerContext();

            //1. Import Suppliers

            string supplierJsonData = File.ReadAllText(@".\..\..\..\Datasets\suppliers.json");

            Console.WriteLine(ImportSuppliers(db, supplierJsonData));
        }
    }
}