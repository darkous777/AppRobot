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
        private TypeUser _typeUtilisateurs;

        public TypeUser TypeUtilisateurs
        {
            get { return _typeUtilisateurs; }
            set { _typeUtilisateurs = value; }
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
        protected User(int id, string username, string password, TypeUser user)
        {
            Id = id;
            Username = username;
            Password = password;
            TypeUtilisateurs = user;
        }
        public static User ObtenirTypeUser(User user)
        {
            switch (user.TypeUtilisateurs)
            {
                case User.TypeUser.User:
                    return new Utilisateur(user.Id, user.Username, user.Password, user.TypeUtilisateurs);
                case User.TypeUser.Moderator:
                    return new Moderator(user.Id, user.Username, user.Password, user.TypeUtilisateurs);
                case User.TypeUser.Admin:
                    return new Admin(user.Id, user.Username, user.Password, user.TypeUtilisateurs);
                default:
                    return null;
            }
        }
    }
}
