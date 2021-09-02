namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context
                .Books
                .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                .OrderBy(x => x.BookId)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var item in goldenBooks)
            {
                sb.AppendLine(item.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Price > 40m)
                .OrderByDescending(x => x.Price)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            DateTime parsedYear = new DateTime(year, 1, 1);

            var books = context
                .Books
                .Where(x => x.ReleaseDate.Value.Year != parsedYear.Year)
                .OrderBy(x => x.BookId);

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            List<string> categories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var books = context
                .BooksCategories
                .Where(bc => categories.Contains(bc.Category.Name.ToLower()))
                .Select(books => books.Book.Title)
                .OrderBy(title => title)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString();
        }

        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            /*
            //1.Age Restriction

            string ageRestriction = Console.ReadLine();
            Console.WriteLine(GetBooksByAgeRestriction(db, ageRestriction));
             
            //2.Golden Books

            Console.WriteLine(GetGoldenBooks(db));

            //3.Books By Price
            
            Console.WriteLine(GetBooksByPrice(db));

            //4. Not Released In

            int year = int.Parse(Console.ReadLine());

            Console.WriteLine(GetBooksNotReleasedIn(db, year));
             */

            //5. Book Titles by Category

            string input = Console.ReadLine().ToLower();

            Console.WriteLine(GetBooksByCategory(db, input));
        }
    }
}
