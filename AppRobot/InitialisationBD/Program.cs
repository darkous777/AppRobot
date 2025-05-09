using System;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace InitialisationBD
{
    /// <summary>
    /// This class is responsible for executing a SQL script to initialize a database.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main method to execute the SQL script.
        /// </summary>
        static void Main()
        {
            try
            {
                string cheminScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bd_robot_app.sql");
                string script = File.ReadAllText(cheminScript);

                // Mot de ppasse de la base de données à changer pour champ vide
                string connectionserver = "server=localhost;user=root;password=";


                using (var connection = new MySqlConnection(connectionserver))
                {
                    connection.Open();
                    foreach (var cmdText in script.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!string.IsNullOrWhiteSpace(cmdText))
                        {
                            try
                            {
                                using (var cmd = new MySqlCommand(cmdText, connection))
                                    cmd.ExecuteNonQuery();
                            }
                            catch (Exception innerEx)
                            {
                                LogError("Erreur SQL dans la commande :\n" + cmdText, innerEx);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("Erreur générale lors de l'exécution du script SQL.", ex);
            }
        }
        static void LogError(string message, Exception ex)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            using (StreamWriter writer = new StreamWriter(logPath, true))
            {
                writer.WriteLine("-----");
                writer.WriteLine($"Date : {DateTime.Now}");
                writer.WriteLine("Message : " + message);
                writer.WriteLine("Exception : " + ex.Message);
                writer.WriteLine("StackTrace : " + ex.StackTrace);
                writer.WriteLine("-----\n");
            }
        }
    }
}