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
        /// <summary>
        /// Vérifie si l'utilisateur existe dans la base de données.
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
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
                    utilisateur.Acces = dr.GetBoolean(6);
                    utilisateur.ListeFonctionnalite = ChercherListeFonctionnaliteDisponibleParUser(utilisateur.Id);
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
        /// <summary>
        /// Modifie le mot de passe d'un utilisateur dans la base de données.
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Cherche l'image d'un utilisateur dans la base de données.
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Modifie les informations d'un utilisateur dans la base de données.
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Supprime un utilisateur de la base de données.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UserException"></exception>
        public static bool DeleteUser(User user)
        {

            if (user.TypeUtilisateurs is User.TypeUser.Admin)
                throw new UserException();

            MySqlConnection cn = Connection();

            bool estSupprimee = false;



            try
            {
                cn.Open();

                string requete1 = "DELETE FROM gestion_fonctionnalite_user WHERE UserId = @id;";

                MySqlCommand cmd1 = new MySqlCommand(requete1, cn);

                cmd1.Parameters.AddWithValue("@id", user.Id);
                cmd1.ExecuteNonQuery();

                cn.Close();

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
        /// <summary>
        /// Obtient la liste des fonctionnalités disponibles pour un utilisateur donné.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Tuple<Fonctionnalite, bool>> ChercherListeFonctionnaliteDisponibleParUser(int userId)
        {
            MySqlConnection cn = Connection();
            List<Tuple<Fonctionnalite, bool>> listeFonctionnalite = new List<Tuple<Fonctionnalite, bool>>();
            try
            {
                cn.Open();
                string requete = "SELECT f.id, f.nom, gfu.acces FROM fonctionnalite f INNER JOIN gestion_fonctionnalite_user gfu ON f.id = gfu.FonctionnaliteId WHERE gfu.UserId = @id;";
                MySqlCommand cmd = new MySqlCommand(requete, cn);
                cmd.Parameters.AddWithValue("@id", userId);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Fonctionnalite fonctionnalite = new Fonctionnalite()
                    {
                        Id = dr.GetInt32(0),
                        Nom = dr.GetString(1)
                    };
                    bool acces = dr.GetBoolean(2);
                    listeFonctionnalite.Add(new Tuple<Fonctionnalite, bool>(fonctionnalite, acces));
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
            return listeFonctionnalite;
        }
        /// <summary>
        /// Obtient la liste des utilisateurs de la base de données.
        /// </summary>
        /// <param name="userDemandant"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static List<User> ObtainListUsers(User userDemandant, string username = null)
        {
            MySqlConnection cn = Connection();

            List<User> users = new List<User>();
            User user = null;
            try
            {
                cn.Open();

                


                string requete = "SELECT * FROM User ";

                if (userDemandant.TypeUtilisateurs == User.TypeUser.Admin)
                {
                    if (username is not null)
                    {
                        requete += "WHERE Username LIKE @username AND TypeUser != 'Admin' AND Id != @id;";
                    }
                    else
                    {
                        requete += "WHERE Id > 0 AND TypeUser != 'Admin' AND Id != @id;";
                    }
                }
                else if (userDemandant.TypeUtilisateurs == User.TypeUser.Moderator)
                {
                    if (username is not null)
                    {
                        requete += "WHERE Username LIKE @username AND TypeUser != 'Admin' AND Id != @id;";
                    }
                    else
                    {
                        requete += "WHERE TypeUser != 'Admin' AND Id != @id;";
                    }
                }


                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@username", $"%{username}%");
                cmd.Parameters.AddWithValue("@id", userDemandant.Id);


                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Utilisateur u = new Utilisateur(
                    dr.GetInt32(0),
                    dr.GetString(1),
                    dr.GetString(2),
                    dr.GetDateOnly(3),
                    Enum.Parse<User.TypeUser>(dr.GetString(4)),
                    _configuration[PRODUIT_IMAGES] + dr.GetString(5),
                    dr.GetBoolean(6),null);

                    user = User.ObtenirTypeUser(u);

                    user.ListeFonctionnalite = ChercherListeFonctionnaliteDisponibleParUser(user.Id);

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
        /// <summary>
        /// Attribue le rôle de modérateur à un utilisateur dans la base de données.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool AttribueRoleDeModerator(User user)
        {

            MySqlConnection cn = Connection();
            bool estUpdate = false;
            try
            {
                cn.Open();


                string requete = "UPDATE User SET TypeUser = @typeuser WHERE Id = @id;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@typeuser", User.TypeUser.Moderator.ToString());
                cmd.Parameters.AddWithValue("@id", user.Id);


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
        /// <summary>
        /// Déattribue le rôle de modérateur à un utilisateur dans la base de données.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool DeattribueRoleDeModerator(User user)
        {


            MySqlConnection cn = Connection();
            bool estUpdate = false;
            try
            {
                cn.Open();


                string requete = "UPDATE User SET TypeUser = @typeuser WHERE Id = @id;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@typeuser", User.TypeUser.User.ToString());
                cmd.Parameters.AddWithValue("@id", user.Id);


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
        /// <summary>
        /// Bloque un utilisateur dans la base de données.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool BloquerUser(User user)
        {
            MySqlConnection cn = Connection();
            bool estUpdate = false;
            try
            {
                cn.Open();


                string requete = "UPDATE User SET Acces = @acces WHERE Id = @id;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@acces", false);
                cmd.Parameters.AddWithValue("@id", user.Id);


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
        /// <summary>
        /// Débloque un utilisateur dans la base de données.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool DebloquerUser(User user)
        {
            MySqlConnection cn = Connection();
            bool estUpdate = false;
            try
            {
                cn.Open();


                string requete = "UPDATE User SET Acces = @acces WHERE Id = @id;";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@acces", true);
                cmd.Parameters.AddWithValue("@id", user.Id);


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
        /// <summary>
        /// Crée un nouvel utilisateur dans la base de données.
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static void CreateUser(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user), "L'utilisateur ne peut pas être nul.");
            }

            MySqlConnection cn = Connection();

            try
            {
                cn.Open();

                string checkUserQuery = "SELECT 1 FROM User WHERE Username = @Username LIMIT 1";
                MySqlCommand checkUserCmd = new MySqlCommand(checkUserQuery, cn);
                checkUserCmd.Parameters.AddWithValue("@Username", user.Username);

                MySqlDataReader reader = checkUserCmd.ExecuteReader();
                bool userExists = reader.HasRows;
                reader.Close();

                if (userExists)
                {
                    throw new Exception("Un utilisateur existe déjà avec le même nom.");
                }

                string imagePath = _configuration[PRODUIT_IMAGES];
                string extension = Path.GetExtension(user.Image);
                string nomImage = Guid.NewGuid().ToString() + extension;
                string cheminImage = imagePath + nomImage;

                if (!string.IsNullOrEmpty(user.Image))
                {
                    File.Copy(user.Image, cheminImage);
                    user.Image = nomImage;
                }

                cn.Open();

                string requete = @"INSERT INTO User (Username, Password, Age_date, TypeUser, Image, Acces) 
                           VALUES (@Username, @Password, @DateOfBirth, @TypeUtilisateurs, @Image, @Acces)";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", PasswordHelper.HashPassword(user.Password));
                cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth.ToDateTime(TimeOnly.MinValue));
                cmd.Parameters.AddWithValue("@TypeUtilisateurs", user.TypeUtilisateurs.ToString());
                cmd.Parameters.AddWithValue("@Image", user.Image);
                cmd.Parameters.AddWithValue("@Acces", user.Acces);

                cmd.ExecuteNonQuery();

                cn.Close();

                cn.Open();

                string requeteFonctionnalite = "INSERT INTO gestion_fonctionnalite_user (UserId, FonctionnaliteId, Acces) VALUES (@userId, @fonctionnaliteId, @acces)";
                MySqlCommand cmdFonctionnalite = new MySqlCommand(requeteFonctionnalite, cn);

                List<Fonctionnalite> fonctionnalites = ChercherListeDesFonctionnalites();

                foreach (Fonctionnalite fonctionnalite in fonctionnalites)
                {
                    cmdFonctionnalite.Parameters.Clear();
                    cmdFonctionnalite.Parameters.AddWithValue("@userId", user.Id);
                    cmdFonctionnalite.Parameters.AddWithValue("@fonctionnaliteId", fonctionnalite.Id);
                    cmdFonctionnalite.Parameters.AddWithValue("@acces", true);
                    cmdFonctionnalite.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la création de l'utilisateur", ex);
            }
            finally
            {
                if (cn is not null && cn.State == System.Data.ConnectionState.Open)
                    cn.Close();
            }
        }
        /// <summary>
        /// Recherche un utilisateur par son identifiant dans la base de données.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static User FindUserById(int id)
        {
            MySqlConnection cn = Connection();
            User user = null;
            Utilisateur u = null;
            try
            {
                cn.Open();

                string requete = "SELECT * FROM User WHERE Id = @id";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@id", id);

                MySqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();
                    u = new Utilisateur(
                                           dr.GetInt32(0),
                                           dr.GetString(1),
                                           dr.GetString(2),
                                           dr.GetDateOnly(3),
                                           Enum.Parse<User.TypeUser>(dr.GetString(4)),
                                           _configuration[PRODUIT_IMAGES] + dr.GetString(5),
                                           dr.GetBoolean(6),null);
                }

                user = User.ObtenirTypeUser(u);

                user.ListeFonctionnalite = ChercherListeFonctionnaliteDisponibleParUser(user.Id);
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
        /// <summary>
        /// Recherche la liste des fonctionnalités disponibles dans la base de données.
        /// </summary>
        /// <returns></returns>
        public static List<Fonctionnalite> ChercherListeDesFonctionnalites()
        {
            MySqlConnection cn = Connection();
            List<Fonctionnalite> fonctionnalites = new List<Fonctionnalite>();
            try
            {
                cn.Open();

                string requete = "SELECT id, nom FROM fonctionnalite";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Fonctionnalite f = new Fonctionnalite()
                    {
                        Id = dr.GetInt32(0),
                        Nom = dr.GetString(1)
                    };
                    fonctionnalites.Add(f);
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
            return fonctionnalites;
        }
        /// <summary>
        /// Vérifie si un utilisateur possède une fonctionnalité spécifique avec un accès donné.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fonctionnalite"></param>
        /// <param name="acces"></param>
        /// <returns></returns>
        public static bool UtilisateurPossedeFonctionnalite(User user, Fonctionnalite fonctionnalite, bool acces)
        {
            MySqlConnection cn = Connection();
            bool possede = false;
            try
            {
                cn.Open();
                string requete = "SELECT COUNT(*) FROM gestion_fonctionnalite_user WHERE UserId = @userId AND FonctionnaliteId = @fonctionnaliteId AND Acces = @acces";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@userId", user.Id);
                cmd.Parameters.AddWithValue("@fonctionnaliteId", fonctionnalite.Id);
                cmd.Parameters.AddWithValue("@acces", acces);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                possede = count > 0;
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
            return possede;
        }
        /// <summary>
        /// Modifie l'accès d'une fonctionnalité pour un utilisateur dans la base de données.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fonctionnalite"></param>
        /// <param name="acces"></param>
        /// <returns></returns>
        public static bool ModifierAccesFonctionnalite(User user, Fonctionnalite fonctionnalite, bool acces)
        {
            MySqlConnection cn = Connection();
            bool estUpdate = false;
            try
            {
                cn.Open();
                string requete = "UPDATE gestion_fonctionnalite_user SET Acces = @acces WHERE UserId = @userId AND FonctionnaliteId = @fonctionnaliteId";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@acces", acces);
                cmd.Parameters.AddWithValue("@userId", user.Id);
                cmd.Parameters.AddWithValue("@fonctionnaliteId", fonctionnalite.Id);

                int rowsAffected = cmd.ExecuteNonQuery();
                estUpdate = rowsAffected > 0;
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

        /// <summary>
        /// Recherche un utilisateur par son nom d'utilisateur dans la base de données.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static User FindUserByUsername(string username)
        {
            MySqlConnection cn = Connection();
            User user = null;
            Utilisateur u = null;
            try
            {
                cn.Open();

                string requete = "SELECT * FROM User WHERE Username = @username";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@username", username);

                MySqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();
                    u = new Utilisateur(
                                           dr.GetInt32(0),
                                           dr.GetString(1),
                                           dr.GetString(2),
                                           dr.GetDateOnly(3),
                                           Enum.Parse<User.TypeUser>(dr.GetString(4)),
                                           _configuration[PRODUIT_IMAGES] + dr.GetString(5),
                                           dr.GetBoolean(6),null);
                }

                user = User.ObtenirTypeUser(u);

                user.ListeFonctionnalite = ChercherListeFonctionnaliteDisponibleParUser(user.Id);
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
        /// <summary>
        /// Recherche une fonctionnalité par son nom dans la base de données.
        /// </summary>
        /// <param name="fonctionnaliteText"></param>
        /// <returns></returns>
        public static Fonctionnalite ChercherFonctionnalite(string fonctionnaliteText)
        {
            MySqlConnection cn = Connection();
            Fonctionnalite fonctionnalite = null;

            try
            {
                cn.Open();

                string requete = "SELECT id, nom FROM fonctionnalite WHERE nom = @nom";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@nom", fonctionnaliteText);

                MySqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    fonctionnalite = new Fonctionnalite(dr.GetInt32(0), dr.GetString(1));

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
            return fonctionnalite;

        }
        /// <summary>
        /// Vérifie si un utilisateur a accès à une fonctionnalité spécifique dans la base de données.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fonctionnalite"></param>
        /// <returns></returns>
        public static bool UserPermission(User user, Fonctionnalite fonctionnalite)
        {
            MySqlConnection cn = Connection();

            bool permission = false;
            try
            {
                cn.Open();

                string requete = "SELECT Acces";
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
            return permission;
        }
    }
}
