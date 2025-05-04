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
        bool BloquerUser(User user);
        bool DebloquerUser(User user);
        List<User> ListUser(User user, string rechercheUsername);

    }
}
