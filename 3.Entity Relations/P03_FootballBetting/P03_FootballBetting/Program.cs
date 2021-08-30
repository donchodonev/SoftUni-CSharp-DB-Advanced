using System;
using P03_FootballBetting.Data;
using P03_FootballBetting.Data.Models;


namespace P03_FootballBetting
{
    public class Program
    {
        static void Main(string[] args)
        {
            var footballBettingContext = new FootballBettingContext();
            footballBettingContext.Database.EnsureDeleted();
            footballBettingContext.Database.EnsureCreated();
        }
    }
}
