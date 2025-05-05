using RobotApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotApp.Models
{
    public class Admin : User, IPowerAdminModerator
    {
        public Admin(int id, string username, string password, DateTime dateOfBirth, TypeUser user, string img, bool acces, Dictionary<string, Fonctionnalite> listeFonctionnalite) : base(id, username, password, dateOfBirth, user, img, acces, listeFonctionnalite)
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

            if (userSelected.TypeUtilisateurs == TypeUser.Admin)
            {
                throw new UserException();
            }

            return DAL.DeleteUser(userSelected);
        }

        public List<User> ListUser(User user, string rechercheUsername)
        {
            return DAL.ObtainListUsers(user, rechercheUsername).Where(u => u.TypeUtilisateurs != TypeUser.Admin).ToList();
        }

        public List<string> ListUsernames(User user)
        {
            List<User> users = ListUser(user, "");

            List<string> usernames = new List<string>();

            foreach (User utilisateur in users)
            {
                usernames.Add(utilisateur.Username);
            }

            return usernames;
        }

        public bool BloquerUser(User user)
        {
            return DAL.BloquerUser(user);
        }

        public bool DebloquerUser(User user)
        {
            return DAL.DebloquerUser(user);
        }

        public bool AttributionDeRole(User user)
        {
            return DAL.AttribueRoleDeModerator(user);
        }

        public bool DeattributionDeRole(User user)
        {
            return DAL.DeattribueRoleDeModerator(user);
        }
    }
}
