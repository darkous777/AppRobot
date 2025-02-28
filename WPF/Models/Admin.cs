using AppRobot.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRobot.Models
{
    public class Admin : User, IPowerAdminModerator
    {
        public Admin(int id, string username, string password, DateOnly dateOfBirth, TypeUser user, String img) : base(id, username, password, dateOfBirth, user, img)
        {
        }

        public bool DeleteOwnUser(User user)
        {
            return DeleteSelectedUser(user);
        }

        public bool DeleteSelectedUser(User userSelected)
        {
            if (userSelected is null)
                throw new ArgumentNullException(nameof(userSelected));

            if ( userSelected.TypeUtilisateurs == TypeUser.Admin)
            {
                throw new UserException();
            }
            

            return DAL.DeleteUser(userSelected);
        }

        public List<User> ListUser(User.TypeUser type, string rechercheUsername)
        {
            return DAL.ObtainListUsers(type, rechercheUsername).Where(u => u.TypeUtilisateurs != TypeUser.Admin).ToList(); ;
        }
    }
}
