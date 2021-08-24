using System;
using Microsoft.Data.SqlClient;

namespace _4._Add_Minion
{
    class Program
    {
        static void TryAddTown(string townName, SqlConnection dbCon)
        {
            bool townIsMissing = true;

            //setup check query
            string checkTown = "SELECT NAME FROM Towns WHERE NAME = @townName";
            SqlCommand sqlCheckTown = new SqlCommand(checkTown, dbCon);
            sqlCheckTown.Parameters.AddWithValue("@townName", townName);

            //check town existance
            dbCon.Open();
            using (dbCon)
            {
                if (sqlCheckTown.ExecuteReader().HasRows)
                {
                    townIsMissing = false;
                }

            }

            if (townIsMissing)
            {
                dbCon.Open();
                using (dbCon)
                {
                    string insertTown = "INSERT INTO Towns (NAME) VALUES (@townName)";
                    SqlCommand sqlInsertTown = new SqlCommand(insertTown, dbCon);
                    sqlInsertTown.Parameters.AddWithValue(@"townName", townName);
                    sqlInsertTown.ExecuteNonQuery();
                }
            }
        }

        static void TryAddVillain(string villainName, SqlConnection dbCon)
        {
            bool villainIsMissing = true;

            //setup check query
            string checkVillain = "SELECT NAME FROM Villains WHERE Name = @villainName";
            SqlCommand sqlCheckVillain = new SqlCommand(checkVillain, dbCon);
            sqlCheckVillain.Parameters.AddWithValue("@villainName", villainName);

            //check town existance
            dbCon.Open();
            using (dbCon)
            {
                if (sqlCheckVillain.ExecuteReader().HasRows)
                {
                    villainIsMissing = false;
                }

                //release datareader connection to the db
                dbCon.Close();

                if (villainIsMissing)
                {
                    dbCon.Open();
                    string insertVillain = "INSERT INTO Villains (NAME, EvilnessFactorId) VALUES (@villainName, @evilnessFactor)";
                    SqlCommand sqlInsertVillain = new SqlCommand(insertVillain, dbCon);
                    sqlInsertVillain.Parameters.AddWithValue("villainName", villainName);
                    sqlInsertVillain.Parameters.AddWithValue("evilnessFactor", 4);
                    sqlInsertVillain.ExecuteNonQuery();
                }
            }
        }
        static void Main(string[] args)
        {
            string connectionString = "Server=.;Database=MinionsDB;Integrated Security=true";
            var dbCon = new SqlConnection(connectionString);

            for (int i = 0; i < 2; i++)
            {
                string[] input = Console.ReadLine()
                    .Split();

                string minionOrVillain = input[0];

                string name = input[1];

                if (minionOrVillain == "Minion:")
                {
                    int age = int.Parse(input[2]);
                    string town = input[3];
                    TryAddTown(town, dbCon);
                }
                else
                {
                    dbCon.ConnectionString = connectionString;
                    TryAddVillain(name,dbCon);
                }
            }
        }
    }
}
