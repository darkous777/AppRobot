using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRobot.Models
{
    public abstract class Admin : User
    {
        protected Admin(Guid id, string username, string password, TypeUser user) : base(id, username, password, user)
        {
        }


    }
}
