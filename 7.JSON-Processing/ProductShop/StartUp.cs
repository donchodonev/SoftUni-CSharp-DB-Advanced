using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
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

        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            /*
            //1. Import Users

            string serializedJsonData = File.ReadAllText(@".\..\..\..\Datasets\users.json");

            Console.WriteLine(ImportUsers(db,serializedJsonData));

            //2. Import Products

            string serializedJsonData = File.ReadAllText(@".\..\..\..\Datasets\products.json");

            Console.WriteLine(ImportProducts(db,serializedJsonData));
            */

            //3. Import Categories

            string serializedJsonData = File.ReadAllText(@".\..\..\..\Datasets\categories.json");

            Console.WriteLine(ImportCategories(db,serializedJsonData));
        }
    }
}