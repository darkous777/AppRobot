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



        public static User ConnectionUtilisateur(User utilisateur)
        {
            MySqlConnection cn = Connection();

            User user = null;
            try
            {
                cn.Open();

                string hashedPassword = PasswordHelper.HashPassword(utilisateur.Password);


                string requete = "SELECT * FROM User WHERE Username = @username AND Password = @password;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@username", utilisateur.Username);

                cmd.Parameters.AddWithValue("@password", hashedPassword);

                MySqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    utilisateur.Id = dr.GetInt32(0);
                    utilisateur.Username = dr.GetString(1);
                    utilisateur.Password = dr.GetString(2);
                    utilisateur.DateOfBirth = dr.GetDateOnly(3);
                    utilisateur.TypeUtilisateurs = Enum.Parse<User.TypeUser>(dr.GetString(4));
                    utilisateur.Image = _configuration[PRODUIT_IMAGES] + dr.GetString(5);
                }

                user = User.ObtenirTypeUser(utilisateur);
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

        public static bool ModifyPasswordUser(User utilisateur)
        {
            MySqlConnection cn = Connection();
            bool estUpdate = false;
            try
            {
                cn.Open();

                string hashedPassword = PasswordHelper.HashPassword(utilisateur.Password);

                string requete = "UPDATE User SET Password = @password WHERE Id = @id;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@password", hashedPassword);
                cmd.Parameters.AddWithValue("@id", utilisateur.Id);

                int excuter = cmd.ExecuteNonQuery();

                estUpdate = excuter > 0;

                utilisateur.Password = hashedPassword;
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
            return estUpdate;
        }

        public static bool ModifyInfoUser(User utilisateur)
        {
            MySqlConnection cn = Connection();
            bool estUpdate = false;
            try
            {
                cn.Open();

                string requete = "UPDATE User SET Username = @username, Image = @image WHERE Id = @id;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@username", utilisateur.Username);
                cmd.Parameters.AddWithValue("@id", utilisateur.Id);
                cmd.Parameters.AddWithValue("@image", utilisateur.Image);


                int excuter = cmd.ExecuteNonQuery();

                estUpdate = excuter > 0;

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
            return estUpdate;
        }
    }
}
