using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.IO;
using AppRobot.Models;

namespace AppRobot.Classes
{
    public static class DAL
    {
        public const string APPSETTING_FILE = "appsettings.json";

        private const string CONNECTION_STRING = "DefaultConnection";

        public const string PRODUIT_IMAGES = "Images:Path";

        private static IConfiguration _configuration;

        /// <summary>
        /// Constructeur static permettant de charger les configurations de l'application
        /// </summary>
        static DAL()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile(APPSETTING_FILE, false, true).Build();
        }

        private static MySqlConnection Connection()
        {
            return new MySqlConnection(_configuration.GetConnectionString(CONNECTION_STRING));
        }



        public static User ConnectionUtilisateur(Utilisateur utilisateur)
        {
            MySqlConnection cn = Connection();

            User user = null;
            try
            {
                cn.Open();

                string requete = "SELECT Id, Username, Password, Acces FROM User WHERE Username = @username AND Password = @password;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@username", utilisateur.Username);

                cmd.Parameters.AddWithValue("@password", utilisateur.Password);

                MySqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    user.Id = dr.GetGuid(0);
                    user.Username = dr.GetString(1);
                    user.Password = dr.GetString(2);
                    user.TypeUtilisateurs = (User.TypeUser)dr.GetInt32(3);
                }

                user = User.ObtenirTypeUser(user);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (cn is not null && cn.State == System.Data.ConnectionState.Open)
                    cn.Close();
            }

            return user;
        }
    }
}
