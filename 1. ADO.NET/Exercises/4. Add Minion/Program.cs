using System;
using Microsoft.Data.SqlClient;

namespace _4._Add_Minion
{
    internal class Program
    {
        private const string connectionString = "Server=.;Database=MinionsDB;Integrated Security=true";
        /// <summary>
        /// Add minion to Minions Table
        /// </summary>
        /// <param name="minionName">Minion's name</param>
        /// <param name="minionAge">Minion's age</param>
        /// <param name="townName">Minion's town name</param>
        /// <param name="dbConnection">SQL Connection</param>
        /// <returns>True if minion is added, false if otherwise</returns>
        private static bool TryAddMinion(string minionName, int minionAge, string townName, SqlConnection dbConnection)
        {
            bool minionIsPresent = true;

            int townId = GetTownId(townName, dbConnection);

            SqlCommand checkMinion =
                new SqlCommand("SELECT * FROM Minions WHERE Name = @minionName AND TownId = @townId", dbConnection);
            checkMinion.Parameters.AddWithValue("@minionName", minionName);
            checkMinion.Parameters.AddWithValue("@townId", townId);

            dbConnection.ConnectionString = connectionString;
            dbConnection.Open();

            using (dbConnection)
            {
                minionIsPresent = checkMinion.ExecuteReader().HasRows;
            }

            if (minionIsPresent)
            {
                return false;
            }

            SqlCommand insertMinion =
                new SqlCommand("INSERT INTO Minions (Name,Age,TownId) VALUES(@minionName, @minionAge, @townId)",
                    dbConnection);

            insertMinion.Parameters.AddWithValue("@minionName", minionName);
            insertMinion.Parameters.AddWithValue("@minionAge", minionAge);
            insertMinion.Parameters.AddWithValue("@townId", townId);

            dbConnection.ConnectionString = connectionString;
            dbConnection.Open();
            using (dbConnection)
            {
                return insertMinion.ExecuteNonQuery() > 0 ? true : false;
            }

            return true;
        }
        /// <summary>
        /// Adds a town in the Towns table if not already present
        /// </summary>
        /// <param name="townName">Name of the town being added</param>
        /// <param name="dbCon">SQLConnection</param>
        private static void TryAddTown(string townName, SqlConnection dbCon)
        {
            bool townIsMissing = true;

            //setup check query
            string checkTown = "SELECT NAME FROM Towns WHERE NAME = @townName";
            SqlCommand sqlCheckTown = new SqlCommand(checkTown, dbCon);
            sqlCheckTown.Parameters.AddWithValue("@townName", townName);

            //check town existance
            dbCon.ConnectionString = connectionString;
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
                dbCon.ConnectionString = connectionString;
                dbCon.Open();
                using (dbCon)
                {
                    string insertTown = "INSERT INTO Towns (NAME) VALUES (@townName)";
                    SqlCommand sqlInsertTown = new SqlCommand(insertTown, dbCon);
                    sqlInsertTown.Parameters.AddWithValue(@"townName", townName);
                    sqlInsertTown.ExecuteNonQuery();
                }

                Console.WriteLine($"Town {townName} was added to the database.");
            }
        }

        /// <summary>
        /// Adds a villain if not already present in Villains table
        /// </summary>
        /// <param name="villainName">Villain's name</param>
        /// <param name="dbCon">SqlConnection</param>
        private static void TryAddVillain(string villainName, SqlConnection dbCon)
        {
            bool villainIsMissing = true;

            //setup check query
            SqlCommand sqlCheckVillain = new SqlCommand("SELECT NAME FROM Villains WHERE Name = @villainName", dbCon);
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
                    SqlCommand sqlInsertVillain = new SqlCommand("INSERT INTO Villains (NAME, EvilnessFactorId) VALUES (@villainName, @evilnessFactor)", dbCon);
                    sqlInsertVillain.Parameters.AddWithValue("villainName", villainName);
                    sqlInsertVillain.Parameters.AddWithValue("evilnessFactor", 4);
                    sqlInsertVillain.ExecuteNonQuery();

                    Console.WriteLine($"Villain {villainName} was added to the database.");
                }
            }
        }
        /// <summary>
        /// Link minion to villain via ID
        /// </summary>
        /// <param name="minionId">Minion's ID</param>
        /// <param name="villainId">Villain's ID</param>
        /// <param name="dbConnection">SQLConnection</param>
        private static bool LinkMinionToVillain(int minionId, int villainId, SqlConnection dbConnection)
        {
            SqlCommand linkMinionToVillain = new SqlCommand("INSERT INTO MinionsVillains VALUES(@minionId,@villainId)", dbConnection);
            linkMinionToVillain.Parameters.AddWithValue("@minionId", minionId);
            linkMinionToVillain.Parameters.AddWithValue("@villainId", villainId);

            dbConnection.ConnectionString = connectionString;
            dbConnection.Open();
            using (dbConnection)
            {
                try
                {
                    return linkMinionToVillain.ExecuteNonQuery() > 0;
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }
        /// <summary>
        /// Get minion's ID based on minions name and town data
        /// </summary>
        /// <param name="minionName">Minion's name</param>
        /// <param name="minionTown">Minion's town</param>
        /// <param name="dbConnection">SQLConnection</param>
        /// <returns>INT32 Minion's ID</returns>
        private static int GetMinionId(string minionName, string minionTown, SqlConnection dbConnection)
        {
            SqlCommand getMinionId =
                new SqlCommand(
                    "SELECT Id FROM Minions WHERE Name = @minionName AND TownId = (SELECT Id FROM Towns WHERE Name = @minionTown)", dbConnection);
            getMinionId.Parameters.AddWithValue("@minionName", minionName);
            getMinionId.Parameters.AddWithValue("@minionTown", minionTown);

            dbConnection.Open();
            using (dbConnection)
            {
                return (int)getMinionId.ExecuteScalar();
            }
        }
        /// <summary>
        /// Get villain ID by name
        /// </summary>
        /// <param name="villainName">Villain's name</param>
        /// <param name="dbConnection">SQL Connection</param>
        /// <returns>Returns INT32 - the villain's ID</returns>
        private static int GetVillainId(string villainName, SqlConnection dbConnection)
        {
            SqlCommand getVillainId =
                new SqlCommand(
                    "SELECT Id FROM Villains WHERE Name = @villainName", dbConnection);
            getVillainId.Parameters.AddWithValue("@villainName", villainName);

            dbConnection.ConnectionString = connectionString;
            dbConnection.Open();
            using (dbConnection)
            {
                return (int)getVillainId.ExecuteScalar();
            }
        }
        /// <summary>
        /// Gets town ID from town name
        /// </summary>
        /// <param name="townName">Town's name</param>
        /// <param name="dbConnection">SQLConnection</param>
        /// <returns>INT32 Town's id</returns>
        private static int GetTownId(string townName, SqlConnection dbConnection)
        {
            SqlCommand getTownId = new SqlCommand("SELECT * FROM Towns WHERE Name = @townName", dbConnection);
            getTownId.Parameters.AddWithValue("@townName", townName);

            dbConnection.Open();
            using (dbConnection)
            {
                return (int)getTownId.ExecuteScalar();
            }
        }
        private static void Main(string[] args)
        {
            //connection setup
            var dbCon = new SqlConnection(connectionString);

            string[] input = Console.ReadLine()
                .Split();

            //get minion input

            string minionName = input[1];
            int age = int.Parse(input[2]);
            string town = input[3];

            //get villain input

            input = Console.ReadLine()
                .Split();
            string villainName = input[1];


            dbCon.ConnectionString = connectionString;
            TryAddTown(town, dbCon);

            dbCon.ConnectionString = connectionString;
            TryAddVillain(villainName, dbCon);

            dbCon.ConnectionString = connectionString;
            TryAddMinion(minionName, age, town, dbCon);

            dbCon.ConnectionString = connectionString;
            int minionId = GetMinionId(minionName, town, dbCon);
            int villainId = GetVillainId(villainName, dbCon);

            if (LinkMinionToVillain(minionId, villainId, dbCon))
            {
                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}");
            }
        }
    }
}