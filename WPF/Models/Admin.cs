using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRobot.Models
{
    public abstract class Admin : User
    {
        protected Admin(string username, string password) :base(username, password)
        {
        }

    }
}
