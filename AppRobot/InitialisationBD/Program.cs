using System;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace InitialisationBD
{

    class Program
    {
        static void Main()
        {
            string cheminScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "init_db.sql");
            string script = File.ReadAllText(cheminScript);
            string connectionserver = "Server=localhost;port=3306;Uid=root;Pwd=ZaqwsXcvbnm852*";


            using (var connection = new MySqlConnection(connectionserver))
            {
                connection.Open();
                foreach (var cmdText in script.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(cmdText))
                    {
                        using (var cmd = new MySqlCommand(cmdText, connection))
                            cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}