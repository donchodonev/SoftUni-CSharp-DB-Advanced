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

        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

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
            */

            //4. Import Categories and Products

            string serializedJsonData = File.ReadAllText(@".\..\..\..\Datasets\categories-products.json");

            Console.WriteLine(ImportCategoryProducts(db, serializedJsonData));
        }
    }
}