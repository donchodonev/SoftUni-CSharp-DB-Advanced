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

        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            string serializedJsonData = File.ReadAllText(@".\..\..\..\Datasets\users.json");

            Console.WriteLine(ImportUsers(db,serializedJsonData));
        }
    }
}