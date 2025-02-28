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
using System.Windows.Controls.Ribbon.Primitives;

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
        public static string ChercheImageUser(User utilisateur)
        {
            MySqlConnection cn = Connection();

            string img = null;

            try
            {
                cn.Open();

                string requete = "SELECT Image FROM User WHERE Id = @id;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@id", utilisateur.Id);

                MySqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    img = dr.GetString(0);
                }

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

            return img;
        }
        public static bool ModifyInfoUser(User utilisateur)
        {
            MySqlConnection cn = Connection();
            bool estUpdate = false;
            try
            {
                cn.Open();

                string source = (DAL.ConnectionUtilisateur(utilisateur)).Image;

                string img = "";

                if (source != utilisateur.Image)
                {
                    string extension = Path.GetExtension(utilisateur.Image);
                    string nomImage = Guid.NewGuid().ToString() + extension;
                    string destination = _configuration[PRODUIT_IMAGES];

                    File.Copy(utilisateur.Image, destination + nomImage);
                    File.Delete(source);
                    utilisateur.Image = nomImage;
                    img = nomImage;
                }
                else
                {
                    img = DAL.ChercheImageUser(utilisateur);
                }

                string requete = "UPDATE User SET Username = @username, Image = @image WHERE Id = @id;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@username", utilisateur.Username);
                cmd.Parameters.AddWithValue("@id", utilisateur.Id);
                cmd.Parameters.AddWithValue("@image", img);


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
        public static bool DeleteUser(User user)
        {

            if(user.TypeUtilisateurs is User.TypeUser.Admin)
                throw new UserException();

            MySqlConnection cn = Connection();

            bool estSupprimee = false;



            try
            {
                cn.Open();

                string requete = "DELETE FROM User WHERE Id = @id;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@id", user.Id);

                int rowsAffected = cmd.ExecuteNonQuery();

                estSupprimee = rowsAffected > 0;
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

            return estSupprimee;
        }

        public static List<User> ObtainListUsers(User.TypeUser typeUser, string username = null)
        {
            MySqlConnection cn = Connection();

            List<User> users = new List<User>();
            User user = null;
            try
            {
                cn.Open();

                string requete = "SELECT * FROM User ";

                if (typeUser == User.TypeUser.Admin)
                {
                    if (username is not null)
                    {
                        requete += "WHERE Username LIKE @username AND TypeUser != 'Admin';";
                    }
                    else
                    {
                        requete += "WHERE Id > 0 AND TypeUser != 'Admin';";
                    }
                }
                else if (typeUser == User.TypeUser.Moderator)
                {
                    if (username is not null)
                    {
                        requete += "WHERE Username LIKE @username AND TypeUser != 'Admin';";
                    }
                    else
                    {
                        requete += "WHERE TypeUser != 'Admin';";
                    }
                }
                
                
                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@username", $"%{username}%");

                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Utilisateur u = new Utilisateur(
                    dr.GetInt32(0),
                    dr.GetString(1),
                    dr.GetString(2),
                    dr.GetDateOnly(3),
                    Enum.Parse<User.TypeUser>(dr.GetString(4)),
                    _configuration[PRODUIT_IMAGES] + dr.GetString(5));

                    user = User.ObtenirTypeUser(u);

                    users.Add(user);
                }

                dr.Close();

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

            return users;
        }
    }
}
