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

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            int[] dateTimeDataArray = date
                .Split('-')
                .Select(int.Parse)
                .ToArray();

            int day = dateTimeDataArray[0];
            int month = dateTimeDataArray[1];
            int year = dateTimeDataArray[2];

            DateTime dateCondition = new DateTime(year, month, day);

            var books = context
                .Books
                .Select(x => new
                {
                    x.ReleaseDate,
                    x.EditionType,
                    x.Title,
                    x.Price
                })
                .Where(x => x.ReleaseDate.Value.Date < dateCondition.Date)
                .OrderByDescending(x => x.ReleaseDate.Value.Date)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(x => $"{x.Title} - {x.EditionType} - ${x.Price:F2}"));
        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context
                .Authors
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName
                })
                .Where(x => x.FirstName.EndsWith(input))
                .ToList()
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName);

            return string.Join(Environment.NewLine, authors.Select(x => $"{x.FirstName} {x.LastName}"));
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Select(x => x.Title)
                .Where(x => x.ToLower().Contains(input.ToLower()))
                .OrderBy(x => x)
                .ToList();

            return string.Join(Environment.NewLine, books);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            string authorLastNameToLower = input.ToLower();

            var booksAndAuthors = context
                .Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(authorLastNameToLower))
                .Select(x => new
                {
                    x.BookId,
                    x.Title,
                    x.Author.FirstName,
                    x.Author.LastName
                })
                .OrderBy(x => x.BookId)
                .ToList();

            return string.Join(Environment.NewLine, booksAndAuthors.Select(x => $"{x.Title} ({x.FirstName} {x.LastName})"));
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var bookTitlesLongerThanCount = context
                .Books
                .Select(x => x.Title)
                .ToList()
                .Count(x => x.Count() > lengthCheck);

            return bookTitlesLongerThanCount;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var bookCopiesCountAndAuthor =
                context
                .Authors
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Books
                })
                .OrderByDescending(x => x.Books.Sum(x => x.Copies))
                .ToList();


            return string.Join(Environment.NewLine, bookCopiesCountAndAuthor.Select(x => $"{x.FirstName} {x.LastName} - {x.Books.Sum(x => x.Copies)}"));
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profitAndCategory = context.Categories
                .Select(x => new
                {
                    x.CategoryBooks,
                    x.Name
                })
                .Select(y => new {
                    TotalSum = y.CategoryBooks.Sum(y => y.Book.Copies * y.Book.Price),
                    CategoryName = y.Name
                    })
                .OrderByDescending(x => x.TotalSum)
                .ThenBy(x => x.CategoryName)
                .ToList();


            return string.Join(Environment.NewLine, profitAndCategory.Select(x => $"{x.CategoryName} ${x.TotalSum:F2}"));
        }

        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            /*
            //1. Age Restriction

            string ageRestriction = Console.ReadLine();
            Console.WriteLine(GetBooksByAgeRestriction(db, ageRestriction));
             
            //2. Golden Books

            Console.WriteLine(GetGoldenBooks(db));

            //3. Books By Price
            
            Console.WriteLine(GetBooksByPrice(db));

            //4. Not Released In

            int year = int.Parse(Console.ReadLine());

            Console.WriteLine(GetBooksNotReleasedIn(db, year));

            //5. Book Titles by Category

            string input = Console.ReadLine().ToLower();

            Console.WriteLine(GetBooksByCategory(db, input));

            //6. Released before date

            string date = Console.ReadLine();
            Console.WriteLine(GetBooksReleasedBefore(db, date));

            //7. Author Search

            string endingCharacter = Console.ReadLine();

            Console.WriteLine(GetAuthorNamesEndingIn(db, endingCharacter));
            
            //8. Book Search
            string input = Console.ReadLine();
            Console.WriteLine(GetBookTitlesContaining(db, input));

            //9. Book Search by Author

            string authorLastName = Console.ReadLine();

            Console.WriteLine(GetBooksByAuthor(db,authorLastName));

            10. Count Books

            int titleLength = int.Parse(Console.ReadLine());

            Console.WriteLine(CountBooks(db, titleLength));

            //12. Total Book Copies

            Console.WriteLine(CountCopiesByAuthor(db));

             */

            Console.WriteLine(GetTotalProfitByCategory(db));

        }
    }
}
