using System;
using Microsoft.Data.SqlClient;

namespace _2.Villain_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            string target = @"Server=.;Database=MinionsDB;Integrated Security=true";

            var dbCon = new SqlConnection(target);

            string query = @"
                            SELECT V.Name, COUNT(MV.MinionId) AS MinionCount FROM MinionsVillains AS MV
                            JOIN Villains AS V ON V.Id = MV.VillainId
                            GROUP BY V.ID, V.Name
                            HAVING COUNT(MV.MinionId) > 3
                            ORDER BY V.Name";

            dbCon.Open();

            using (dbCon)
            {
                SqlCommand command = new SqlCommand(query, dbCon);
                var reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]} - {reader["MinionCount"]}");
                    }
                }
            }
        }
    }
}
