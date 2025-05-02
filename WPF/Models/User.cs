using AppRobot.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppRobot.Models
{
    public abstract class User
    {
        public enum TypeUser
        {
            User = 1,
            Moderator,
            Admin,
        }

        private string _username;
        private int _id;
        private string _password;
        private DateOnly _dateOfBirth;
        private TypeUser _typeUtilisateurs;
        private String _image;

        private bool _acces;

        private List<Fonctionnalite> _listeFonctionnalite;
        public bool Acces
        {
            get { return _acces; }
            set { _acces = value; }
        }


        public List<Fonctionnalite> ListeFonctionnalite
        {
            get { return _listeFonctionnalite; }
            set { _listeFonctionnalite = value; }
        }


        public String Image
        {
            get { return _image; }
            set { _image = value; }
        }


        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public DateOnly DateOfBirth
        {
            get { return _dateOfBirth; }
            set { _dateOfBirth = value; }
        }
        public TypeUser TypeUtilisateurs
        {
            get { return _typeUtilisateurs; }
            set { _typeUtilisateurs = value; }
        }

        /// <summary>
        /// Obtient ou définit l'identifiant unique d'une course
        /// </summary>
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        public User(int id, string username, string password, DateOnly dateOfBirth, TypeUser user, String img, bool acces, List<Fonctionnalite> listeFonctionnalite)
        {
            Id = id;
            Username = username;
            Password = password;
            DateOfBirth = dateOfBirth;
            TypeUtilisateurs = user;
            Image = img;
            Acces = acces;

            ListeFonctionnalite = listeFonctionnalite;
        }

        public static User ObtenirTypeUser(User user)
        {
            switch (user.TypeUtilisateurs)
            {
                case TypeUser.User:
                    return new Utilisateur(user.Id, user.Username, user.Password, user.DateOfBirth, user.TypeUtilisateurs, user.Image, user.Acces, user.ListeFonctionnalite);
                case TypeUser.Moderator:
                    return new Moderator(user.Id, user.Username, user.Password, user.DateOfBirth, user.TypeUtilisateurs, user.Image, user.Acces, user.ListeFonctionnalite);
                case TypeUser.Admin:
                    return new Admin(user.Id, user.Username, user.Password, user.DateOfBirth, user.TypeUtilisateurs, user.Image, user.Acces, user.ListeFonctionnalite);
                default:
                    return null;
            }
        }

        

        public static bool ModifierUser(User user)
        {
            return DAL.ModifyInfoUser(user);
        }
        public static bool ModifyPassword(User user)
        {
            return DAL.ModifyPasswordUser(user);
        }

        public override string ToString()
        {
            string accesStatus = "";

            return  accesStatus = Acces ? "Non bloqué" : "Bloqué";
            
        }

    }
    public class UserException : Exception
    {
        public UserException() : base("L'utilisateur n'a pas les droits à effectuer une telle action.")
        {
        }
    }
}
