﻿using System;
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

            Console.WriteLine(ImportParts(db,partsJsonData));
        }
    }
}