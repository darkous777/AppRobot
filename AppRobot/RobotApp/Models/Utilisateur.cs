using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotApp.Models
{
    public class Utilisateur : User
    {
        public Utilisateur(int id, string username, string password, DateTime dateOfBirth, TypeUser user, string img, bool acces, Dictionary<string, Fonctionnalite> listeFonctionnalite) : base(id, username, password, dateOfBirth, user, img, acces, listeFonctionnalite)
        {
        }
    }
}
