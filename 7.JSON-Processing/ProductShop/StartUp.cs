using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Models;
using ProductShop.Models.DTO_Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;
        public static void InitializeAutomapper()
        {
            var config = new MapperConfiguration(x => x.AddProfile(new ProductShopProfile()));
            mapper = new Mapper(config);
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            User[] users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.Users.AddRange(users);

            int countOfUsersAdded = context.SaveChanges();

            return $"Successfully imported {countOfUsersAdded}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            Product[] products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.Products.AddRange(products);

            int countOfProductsSaved = context.SaveChanges();

            return $"Successfully imported {countOfProductsSaved}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            Category[] categories = (Category[])JsonConvert.DeserializeObject<Category[]>(inputJson);

            context.Categories.AddRange(categories.Where(x => x.Name != null));

            int countOfCategoriesImported = context.SaveChanges();

            return $"Successfully imported {countOfCategoriesImported}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();

            var productsCategoriesDTO = JsonConvert.DeserializeObject<IEnumerable<CategoryProductDTO>>(inputJson);

            var categoryProducts = mapper.Map<IEnumerable<CategoryProduct>>(productsCategoriesDTO);

            context.CategoryProducts.AddRange(categoryProducts);

            int countOfCategoryProductsImported = context.SaveChanges();

            return $"Successfully imported {countOfCategoryProductsImported}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new
                {
                    x.Name,
                    x.Price,
                    Seller = x.Seller.FirstName + " " + x.Seller.LastName
                })
                .OrderBy(x => x.Price)
                .ToArray();

            var jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(products, Formatting.Indented, jsonSettings);
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var sellers = context
                .Users
                .Where(x => x.ProductsSold.Any(p => p.BuyerId != null))
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Where(b => b.BuyerId != null)
                    .Select(y => new
                    {
                        Name = y.Name,
                        Price = y.Price,
                        BuyerFirstName = y.Buyer.FirstName,
                        BuyerLastName = y.Buyer.LastName
                    })
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToArray();

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(sellers, settings);
        }

        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            /*

            1. Import Users

            string serializedJsonData = File.ReadAllText(@".\..\..\..\Datasets\users.json");

            Console.WriteLine(ImportUsers(db,serializedJsonData));

            2. Import Products

            string serializedJsonData = File.ReadAllText(@".\..\..\..\Datasets\products.json");

            Console.WriteLine(ImportProducts(db,serializedJsonData));

            //3. Import Categories

            string serializedJsonData = File.ReadAllText(@".\..\..\..\Datasets\categories.json");

            Console.WriteLine(ImportCategories(db,serializedJsonData));

            //4. Import Categories and Products

            string serializedJsonData = File.ReadAllText(@".\..\..\..\Datasets\categories-products.json");

            Console.WriteLine(ImportCategoryProducts(db, serializedJsonData));

            //5. Export Products In Range

            Console.WriteLine(GetProductsInRange(db));

            */

            //6. Export Successfully Sold Products

            Console.WriteLine(GetSoldProducts(db));

        }
    }
}