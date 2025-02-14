using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRobot.Models
{
    public class Utilisateur : User
    {
        public Utilisateur(string username, string password) : base(username, password)
        {
        }
    }
}
