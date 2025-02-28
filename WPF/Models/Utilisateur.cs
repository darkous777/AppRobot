using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRobot.Models
{
    public class Utilisateur : User
    {
        public Utilisateur(int id, string username, string password, DateOnly dateOfBirth, TypeUser user, String img, bool acces) : base(id, username, password, dateOfBirth, user, img, acces)
        {
        }
    }
}
