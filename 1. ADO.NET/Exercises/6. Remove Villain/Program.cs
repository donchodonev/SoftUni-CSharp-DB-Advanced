using System;
using Microsoft.Data.SqlClient;

namespace _6._Remove_Villain
{
    class Program
    {
        private const string connectionString = "Server=.;Database=MinionsDB;Integrated Security=true;";

        static void Main(string[] args)
        {
            SqlConnection dbCon = new SqlConnection(connectionString);
            SqlCommand getVillainName = new SqlCommand("SELECT Name FROM Villains WHERE Id = @villainId",dbCon);
            SqlCommand deleteMinionsAndVillain =
                new SqlCommand("DELETE FROM MinionsVillains WHERE VillainId = @villainId", dbCon);

            int villainId = int.Parse(Console.ReadLine());

            getVillainName.Parameters.AddWithValue("@villainId", villainId);
            deleteMinionsAndVillain.Parameters.AddWithValue("@villainId", villainId);

            string villainName = null;
            int minionsReleased;
            using (dbCon)
            {
                dbCon.Open();

                SqlTransaction transaction = dbCon.BeginTransaction();
                getVillainName.Transaction = transaction;
                deleteMinionsAndVillain.Transaction = transaction;
                try
                {
                    villainName = (string) getVillainName.ExecuteScalar();
                    minionsReleased = deleteMinionsAndVillain.ExecuteNonQuery();

                    if (villainName == null)
                    {
                        Console.WriteLine("No such villain was found.");
                        transaction.Rollback();
                        dbCon.Close();
                        return;
                    }

                    Console.WriteLine($"{villainName} was deleted.");
                    Console.WriteLine($"{minionsReleased} were released.");

                    transaction.Commit();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }
    }
}
