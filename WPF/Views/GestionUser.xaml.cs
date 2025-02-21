using AppRobot.Models;
using Microsoft.Win32;
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


            //switch (UserConnecter.TypeUtilisateurs)
            //{
            //    case User.TypeUser.User:

            //    case User.TypeUser.Moderator:
            //    case User.TypeUser.Admin:
            //    default:
            //        return null;
            //}

            InitializeComponent();

            UserConnecter = user;
            string pass =
            txtUser.Text = UserConnecter.Username;
            datePicker.SelectedDate = user.DateOfBirth.ToDateTime(TimeOnly.MinValue);
            AfficherImage(user.Image);

        }

        public GestionUser()
        {
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {

        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnModifyImageParcourir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.Title = "Sélectionner l'image";

                ofd.Filter = "image PNG | *.png|image JPG | *.jpg|image avif | *.avif";

                if (ofd.ShowDialog() == true)
                {
                    AfficherImage(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Ajout d'une image");
            }
        }
        /// <summary>
        /// Permet l'affichage de l'image de l'employé
        /// </summary>
        /// <param name="cheminFichier">Chemin d'accès à l'image</param>
        private void AfficherImage(string cheminFichier)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            //bi.UriSource = Etat == EtatFormulaire.Ajouter ? new Uri(cheminFichier) : new Uri(_configuration[PRODUIT_IMAGES] + cheminFichier);

            bi.UriSource = new Uri(cheminFichier);
            bi.CacheOption = BitmapCacheOption.OnLoad;

            bi.EndInit();


            imgProduit.Source = bi;
        }
    }
}
