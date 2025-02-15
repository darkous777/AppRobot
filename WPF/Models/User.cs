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
            User,
            Moderators,
            Administrators,
        }

		private string _username;
        private Guid _id;
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
        public Guid Id
        {
            get { return _id; }
            set
            {

                if (value == Guid.Empty)
                {
                    throw new ArgumentException("Le id ne peut etre null!");
                }
                _id = value;
            }
        }
        protected User(Guid id, string username, string password, TypeUser user)
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
                    Utilisateur utilisateur = (Utilisateur)user;
                    return utilisateur;
                case User.TypeUser.Moderators:
                    Moderator moderator = (Moderator)user;
                    return moderator;
                case User.TypeUser.Administrators:
                    Admin admin = (Admin)user;
                    return admin;
                default:
                    throw new ArgumentException("Invalid user type");
            }
        }
    }
}
