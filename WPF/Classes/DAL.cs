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


        public static void AjouterUtilisateur(Utilisateur utilisateur)
        {
            if (utilisateur is null)
            {
                throw new ArgumentNullException(nameof(utilisateur), "Le produit ne peut pas être nul.");
            }

            MySqlConnection cn = Connection();

            try
            {
                string image = _configuration[PRODUIT_IMAGES];
                string extension = Path.GetExtension(utilisateur.Image);
                string nomImage = Guid.NewGuid().ToString() + extension;
                string cheminImage = image + nomImage;

                File.Copy(utilisateur.Image, cheminImage);

                utilisateur.Image = nomImage;

                cn.Open();

                string hashedPassword = PasswordHelper.HashPassword(utilisateur.Password);
                string requete = "INSERT INTO User (Username, Password, DateOfBirth, TypeUser, Image) VALUES (@username, @password, @dateOfBirth, @typeUser, @image);";
                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@username", utilisateur.Username);
                cmd.Parameters.AddWithValue("@password", hashedPassword);
                cmd.Parameters.AddWithValue("@dateOfBirth", utilisateur.DateOfBirth);
                cmd.Parameters.AddWithValue("@typeUser", utilisateur.TypeUtilisateurs.ToString());
                cmd.Parameters.AddWithValue("@image", utilisateur.Image);
                cmd.ExecuteNonQuery();

                cn.Close();
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
        }


        
    }
}
