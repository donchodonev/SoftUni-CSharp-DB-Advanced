using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace _5._Change_Town_Names_Casing
{
    class Program
    {
        private const string connectionString = "Server=.;Database=MinionsDB;Integrated Security=true;";

        static void Main(string[] args)
        {
            SqlConnection dbCon = new SqlConnection(connectionString);

            SqlCommand getCountryId = new SqlCommand("SELECT Id FROM Countries WHERE Name = @countryName",dbCon);
            SqlCommand getTowns = new SqlCommand("SELECT * FROM Towns WHERE CountryCode = @countryCode", dbCon);
            SqlCommand capitaliseTownName = new SqlCommand("UPDATE Towns SET Name = @newName WHERE Name = @oldName", dbCon);

            int countryCode;

            getCountryId.Parameters.AddWithValue("@countryName", Console.ReadLine());

            Dictionary<string, string> towns = new Dictionary<string, string>();

            using (dbCon)
            {
                dbCon.Open();
                try
                {
                    countryCode = (int)getCountryId.ExecuteScalar();
                }
                catch (Exception)
                {
                    Console.WriteLine("No town names were affected.");
                    dbCon.Close();
                    return;
                }

                getTowns.Parameters.AddWithValue("@countryCode", countryCode);

                var reader = getTowns.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["Name"].ToString() != reader["Name"].ToString().ToUpper())
                    {
                        towns.Add(reader["Name"].ToString(),reader["Name"].ToString().ToUpper());
                    }
                }

                reader.Close();

                if (towns.Count == 0)
                {
                    Console.WriteLine("No town names were affected.");
                    return;
                }

                foreach (var town in towns)
                {
                    capitaliseTownName.Parameters.AddWithValue("@newName", town.Value);
                    capitaliseTownName.Parameters.AddWithValue("@oldName", town.Key);

                    capitaliseTownName.ExecuteNonQuery();

                    capitaliseTownName.Parameters.Clear();
                }
            }
        }
    }
}
