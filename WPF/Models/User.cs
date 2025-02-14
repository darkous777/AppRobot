using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRobot.Models
{
    public abstract class User
    {
		private string _username;

		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		private string _password;

		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

        protected User(string username, string password)
        {
            Username = username;
            Password = password;
        }

    }
}
