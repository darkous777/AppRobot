using AppRobot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppRobot.Views
{
    /// <summary>
    /// Logique d'interaction pour GestionUser.xaml
    /// </summary>
    public partial class GestionUser : Window
    {
        private User  _user;

        public User UserConnecter
        {
            get { return _user; }
            set { _user = value; }
        }

        public GestionUser(User user)
        {
            UserConnecter = user;
        }

        public GestionUser()
        {
            InitializeComponent();
        }
    }
}
