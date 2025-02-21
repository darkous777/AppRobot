using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRobot.Models
{
    public class Moderator : User
    {
        public Moderator(int id, string username, string password, DateOnly dateOfBirth, TypeUser user) : base(id, username, password, dateOfBirth, user)
        {
        }

    }
}
