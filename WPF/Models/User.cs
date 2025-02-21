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

        public DateOnly DateOfBirth
        {
            get { return _dateOfBirth; }
            set { _dateOfBirth = value; }
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
        protected User(int id, string username, string password, DateOnly dateOfBirth, TypeUser user )
        {
            Id = id;
            Username = username;
            Password = password;
            DateOfBirth = dateOfBirth;
            TypeUtilisateurs = user;
        }
        public static User ObtenirTypeUser(User user)
        {
            switch (user.TypeUtilisateurs)
            {
                case TypeUser.User:
                    return new Utilisateur(user.Id, user.Username, user.Password, user.DateOfBirth, user.TypeUtilisateurs );
                case TypeUser.Moderator:
                    return new Moderator(user.Id, user.Username, user.Password, user.DateOfBirth, user.TypeUtilisateurs);
                case TypeUser.Admin:
                    return new Admin(user.Id, user.Username, user.Password, user.DateOfBirth, user.TypeUtilisateurs);
                default:
                    return null;
            }
        }
    }
}
