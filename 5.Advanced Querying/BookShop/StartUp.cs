namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ageRestrictionEnum = (AgeRestriction)Enum
              .Parse(typeof(AgeRestriction), command, true);

            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(x => x.AgeRestriction == ageRestrictionEnum)
                .OrderBy(x => x.Title)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            /*
            //1.Age Restriction

            string ageRestriction = Console.ReadLine();
            Console.WriteLine(GetBooksByAgeRestriction(db, ageRestriction));
            
             
             
             
             
             
             */
        }
    }
}
