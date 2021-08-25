using System;
using Microsoft.Data.SqlClient;

namespace _9._Increase_Age_Stored_Procedure
{
    class Program
    {
        private const string connectionString = "Server=.;Database=MinionsDB;Integrated Security=true";
        static void Main(string[] args)
        {
            SqlConnection dbCon = new SqlConnection(connectionString);
            int minionId = int.Parse(Console.ReadLine());

            SqlCommand increaseAgeBy1 = new SqlCommand("EXECUTE usp_GetOlder @Id", dbCon);
            increaseAgeBy1.Parameters.AddWithValue("@Id", minionId);

            SqlCommand getMinionNameAndAge = new SqlCommand("SELECT Name, Age From Minions WHERE Id = @Id", dbCon);
            getMinionNameAndAge.Parameters.AddWithValue("@Id", minionId);


            string minionName;
            int minionAge;

            dbCon.Open();
            using (dbCon)
            {
                increaseAgeBy1.ExecuteNonQuery();

                var minionReader = getMinionNameAndAge.ExecuteReader();
                minionReader.Read();

                minionName = (string)minionReader["Name"];
                minionAge = (int)minionReader["Age"];
            }

            Console.WriteLine($"{minionName} - {minionAge} old now, happy birthday !");
        }
    }
}
