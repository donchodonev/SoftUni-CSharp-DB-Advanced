using System;
using AutoMapper;
using ProductShop.Data;
using System.Xml.Linq;
using ProductShop.Dtos.Input;
using System.Xml.Serialization;
using System.IO;
using ProductShop.Models;
using System.Linq;
using System.Xml;
using ProductShop.Dtos.Output;
using System.Text;

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

            reader.Close();

            Category[] categories = mapper.Map<Category[]>(categoryDTOs);

            context.Categories.AddRange(categories);

            int countOfCategoriesImported = context.SaveChanges();

            return $"Successfully imported {countOfCategoriesImported}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            Initialize();

            var reader = new StringReader(inputXml);

            var serialzier = new XmlSerializer(typeof(CategoriesProductsInputDTO[]), new XmlRootAttribute("CategoryProducts"));

            CategoriesProductsInputDTO[] categoryProductsDTOs;

            using (reader)
            {
                categoryProductsDTOs = (CategoriesProductsInputDTO[])serialzier.Deserialize(reader);
            }

            CategoryProduct[] categoryProducts = mapper.Map<CategoryProduct[]>(categoryProductsDTOs);

            context.CategoryProducts.AddRange(categoryProducts);

            int countOfCategoryProductsImpored = context.SaveChanges();

            return $"Successfully imported {countOfCategoryProductsImpored}";
        }
        public static string GetProductsInRange(ProductShopContext context)
        {
            Initialize();

            var products = context
                .Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new ProductOutputDTO
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToArray();

            var serialzier = new XmlSerializer(typeof(ProductOutputDTO[]), new XmlRootAttribute("Products"));

            var writer = new StringWriter();

            using (writer)
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                serialzier.Serialize(writer, products, ns);
            }

            return writer.ToString();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            Initialize();

            var users = context
                .Users
                .Where(x => x.ProductsSold.Count > 0)
                .Select(x => new UserOutputDTO
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(y => new ProductOutputWithoutBuyerDTO
                    {
                        Name = y.Name,
                        Price = y.Price
                    })
                    .ToArray()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToArray();

            var serialzier = new XmlSerializer(typeof(UserOutputDTO[]), new XmlRootAttribute("Users"));

            var writer = new StringWriter();

            using (writer)
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                serialzier.Serialize(writer, users, ns);
            }

            return writer.ToString();
        }
        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            /*   
                
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();


            //1. Import Users

            var xmlUsersData = File.ReadAllText(@".\..\..\..\Datasets\users.xml");

            Console.WriteLine(ImportUsers(db, xmlUsersData));

            //2. Import Products

            var xmlProductsData = File.ReadAllText(@".\..\..\..\Datasets\products.xml");

            Console.WriteLine(ImportProducts(db,xmlProductsData));

            //3.Import Categories

            var xmlCategoriesData = File.ReadAllText(@".\..\..\..\Datasets\categories.xml");

            Console.WriteLine(ImportCategories(db, xmlCategoriesData));

            //4.Import Categories and Products

            var xmlCategoriesAndProdcutsData = File.ReadAllText(@".\..\..\..\Datasets\categories-products.xml");

            Console.WriteLine(ImportCategoryProducts(db, xmlCategoriesAndProdcutsData));

            //5.Products In Range

            Console.WriteLine(GetProductsInRange(db));

            */

            //6. Sold Products

            Console.WriteLine(GetSoldProducts(db));
        }
    }
}