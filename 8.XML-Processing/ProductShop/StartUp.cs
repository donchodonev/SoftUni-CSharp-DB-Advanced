using System;
using AutoMapper;
using ProductShop.Data;
using System.Xml.Linq;
using ProductShop.Dtos;
using System.Xml.Serialization;
using System.IO;
using ProductShop.Models;
using System.Linq;

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
            Initialize();

            var serializer = new XmlSerializer(typeof(UsersInputDTO[]), new XmlRootAttribute("Users"));

            var textRead = new StringReader(inputXml);

            UsersInputDTO[] usersDTOs = (UsersInputDTO[])serializer.Deserialize(textRead);

            textRead.Close();

            User[] users = mapper.Map<User[]>(usersDTOs);

            context.Users.AddRange(users);

            int countOfUsersImported = context.SaveChanges();

            return $"Successfully imported {countOfUsersImported}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            Initialize();

            var serializer = new XmlSerializer(typeof(ProductsInputDTO[]), new XmlRootAttribute("Products"));

            var productData = new StringReader(inputXml);

            ProductsInputDTO[] productDtos = (ProductsInputDTO[])serializer.Deserialize(productData);

            productData.Close();

            Product[] products = mapper.Map<Product[]>(productDtos);

            context.Products.AddRange(products);

            int countOfProductsImported = context.SaveChanges();

            return $"Successfully imported {countOfProductsImported}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            Initialize();

            var reader = new StringReader(inputXml);

            var serializer = new XmlSerializer(typeof(CategoriesInputDTO[]), new XmlRootAttribute("Categories"));

            CategoriesInputDTO[] categoryDTOs = (CategoriesInputDTO[])serializer.Deserialize(reader);

            Category[] categories = mapper.Map<Category[]>(categoryDTOs);

            context.Categories.AddRange(categories);

            int countOfCategoriesImported = context.SaveChanges();

            return $"Successfully imported {countOfCategoriesImported}";
        }

        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            /*            db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();


                        //1. Import Users

                        var xmlUsersData = File.ReadAllText(@".\..\..\..\Datasets\users.xml");

                        Console.WriteLine(ImportUsers(db, xmlUsersData));

                        //2. Import Products

                        var xmlProductsData = File.ReadAllText(@".\..\..\..\Datasets\products.xml");

                        Console.WriteLine(ImportProducts(db,xmlProductsData));

                        */

            //3.Import Categories

            var xmlCategoriesData = File.ReadAllText(@".\..\..\..\Datasets\categories.xml");

            Console.WriteLine(ImportCategories(db, xmlCategoriesData));
        }
    }
}