using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRobot.Models
{
    public class Fonctionnalite
    {
        private int _id;
        private string _nom;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Nom
        {
            get { return _nom; }
            set
            {
                if (String.IsNullOrWhiteSpace(value.Trim()))
                    throw new ArgumentException("Le nom ne peut pas être nul ou vide.", nameof(Nom));

                _nom = value.Trim();
            }
        }

        public Fonctionnalite()
        {
            Id = 0;
            Nom = "vide";
        }

        public Fonctionnalite(int id, string nom)
        {
            Id = id;
            Nom = nom;
        }

        /// <summary>
        /// Représentation de l'objet sous forme de chaîne de carcatère.
        /// </summary>
        /// <returns>Retourne le nom de la catégorie</returns>
        public override string ToString()
        {
            return Nom;
        }



    }
}
