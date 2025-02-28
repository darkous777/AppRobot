using AppRobot.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRobot.Models
{
    public class Moderator : User, IPowerAdminModerator
    {
        public Moderator(int id, string username, string password, DateOnly dateOfBirth, TypeUser user, String img) : base(id, username, password, dateOfBirth, user, img)
        {
        }

        public bool DeleteOwnUser(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if ( user.TypeUtilisateurs is TypeUser.Admin)
            {
                throw new UserException();
            }

            return DAL.DeleteUser(user);
        }

        public bool DeleteSelectedUser(User userSelected)
        {
            if (userSelected is null)
                throw new ArgumentNullException(nameof(userSelected));

            if (this.TypeUtilisateurs is TypeUser.Moderator && userSelected.TypeUtilisateurs is TypeUser.Moderator || userSelected.TypeUtilisateurs is TypeUser.Admin)
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
