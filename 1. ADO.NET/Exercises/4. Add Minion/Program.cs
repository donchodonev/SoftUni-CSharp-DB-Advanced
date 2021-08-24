using Microsoft.Data.SqlClient;

namespace _4._Add_Minion
{
    internal class Program
    {
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
                }
            }
        }

        private static void LinkMinionToVillain(int minionId, int villainId, SqlConnection dbConnection)
        {
        }
        /// <summary>
        /// Get minion's ID based on minions name and town data
        /// </summary>
        /// <param name="minionName">Minion's name</param>
        /// <param name="minionTown">Minion's town</param>
        /// <param name="dbConnection">SQLConnection</param>
        /// <returns>Minion's ID</returns>
        private static long GetMinionId(string minionName, string minionTown, SqlConnection dbConnection)
        {
            SqlCommand getMinionId =
                new SqlCommand(
                    "SELECT Id FROM Minions WHERE Name = @minionName AND TownId = (SELECT Id FROM Towns WHERE Name = @minionTown)", dbConnection);
            getMinionId.Parameters.AddWithValue("@minionName", minionName);
            getMinionId.Parameters.AddWithValue("@minionTown", minionTown);

            dbConnection.Open();
            using (dbConnection)
            {
                return (long)getMinionId.ExecuteScalar();
            }
        }

        private static void GetVillainId(string villainName, string villainTown)
        {
        }

        private static void Main(string[] args)
        {
            //connection setup
            string connectionString = "Server=.;Database=MinionsDB;Integrated Security=true";
            var dbCon = new SqlConnection(connectionString);

            /*   string[] input = Console.ReadLine()
                   .Split();

               //get minion input
               string minionName = input[1];
               int age = int.Parse(input[2]);
               string town = input[3];

               //add town if missing
               TryAddTown(town, dbCon);

               //get villain input
               input = Console.ReadLine()
                   .Split();
               string villainName = input[1];

               dbCon.ConnectionString = connectionString;
               //add villain if missing
               TryAddVillain(villainName, dbCon);*/

            GetMinionId("Bob", "Burgas", dbCon);
        }
    }
}