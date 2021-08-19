using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace _3._Minions_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            //sql vars
            const string target = @"Server=.;Database=MinionsDB;Integrated Security=true";
            SqlConnection dbCon = new SqlConnection(target);

            //problem vars
            bool villainExists;
            int villainId = int.Parse(Console.ReadLine());
            string villainName;

            //queries
            string checkVillainCountQuery = "SELECT COUNT(*) as VillainExists FROM Villains WHERE Id = @Id";
            string checkVillainNameQuery = "SELECT Name FROM Villains WHERE Id = @Id";
            string getVillainMinionsQuery =
                @"
                SELECT M.Name, M.Age FROM MinionsVillains
                JOIN Minions as M ON MinionId = M.Id
                WHERE VillainId = @Id
                ORDER BY M.Name";

            //commands vars
            var checkIfVillainExists = new SqlCommand(checkVillainCountQuery,dbCon);
            checkIfVillainExists.Parameters.AddWithValue("@Id", villainId);

            var getVillainName = new SqlCommand(checkVillainNameQuery, dbCon);
            getVillainName.Parameters.AddWithValue("@Id", villainId);

            var getVillainMinions = new SqlCommand(getVillainMinionsQuery, dbCon);
            getVillainMinions.Parameters.AddWithValue("@Id", villainId);

            Dictionary<string, int> minions = new Dictionary<string, int>();

            dbCon.Open();
            using (dbCon)
            {
                villainExists = (int)checkIfVillainExists.ExecuteScalar() == 0 ? false : true;

                if (!villainExists)
                {
                    Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                    return; 
                }

                villainName = (string)getVillainName.ExecuteScalar();

                Console.WriteLine($"Villain: {villainName}");

                SqlDataReader minionReader = getVillainMinions.ExecuteReader();

                using (minionReader)
                {
                    while (minionReader.Read())
                    {
                        minions.Add((string)minionReader["Name"],(int)minionReader["Age"]);
                    }
                }
            }

            if (minions.Count == 0)
            {
                Console.WriteLine("(no minions)");
            }
            else
            {
                int counter = 1;

                foreach (var minion in minions)
                {
                    Console.WriteLine($"{counter}. {minion.Key} {minion.Value}");
                    counter++;
                }
            }
        }
    }
}
