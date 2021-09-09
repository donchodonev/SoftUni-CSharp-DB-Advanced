using System;
using AutoMapper;
using ProductShop.Data;
using System.Xml.Linq;
using ProductShop.Dtos;
using System.Xml.Serialization;
using System.IO;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;

        static void Initialize()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new ProductShopProfile()));
            mapper = new Mapper(config);
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(UsersInputDTO[]), new XmlRootAttribute("Users"));

            var textRead = new StringReader(inputXml);

            UsersInputDTO[] usersDTOs = (UsersInputDTO[])serializer.Deserialize(textRead);

            User[] users = mapper.Map<User[]>(usersDTOs);

            context.Users.AddRange(users);

            int countOfUsersImported = context.SaveChanges();

            return $"Successfully imported {countOfUsersImported}"; 
        }
        public static void Main(string[] args)
        {
            Initialize();

            var db = new ProductShopContext();

            /*          db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();
            */


            //1.Import Users

            var xmlData = File.ReadAllText(@".\..\..\..\Datasets\users.xml");

            Console.WriteLine(ImportUsers(db,xmlData));
        }
    }
}