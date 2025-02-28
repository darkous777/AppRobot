using AppRobot.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AppRobot.Models.User;

namespace AppRobot.Models
{
    public interface IPowerAdminModerator
    {

        bool DeleteSelectedUser(User userSelected);


        List<User> ListUser(User.TypeUser type, string rechercheUsername)
        {
            return DAL.ObtainListUsers(type, rechercheUsername);
        }

    }
}
