using AppRobot.Classes;
using AppRobot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private User _user;

        public User UserConnecter
        {
            get { return _user; }
            set { _user = value; }
        }

        public const string PRODUIT_IMAGES = "Images:Path";

        IConfiguration _configuration;

        public GestionUser(User user)
        {
            InitializeComponent();

            UserConnecter = user;
            string pass =
            txtUser.Text = UserConnecter.Username;
            datePicker.SelectedDate = user.DateOfBirth.ToDateTime(TimeOnly.MinValue);
            datePicker.IsEnabled = false;
            AfficherImage(user.Image);

            _configuration = new ConfigurationBuilder().AddJsonFile(DAL.APPSETTING_FILE, false, true).Build();

            afficherListUser(UserConnecter.TypeUtilisateurs, "");

            switch (user.TypeUtilisateurs)
            {
                case User.TypeUser.User:
                    tbLstUser.Visibility = Visibility.Collapsed;
                    btnDelete.IsEnabled = true;

                    break;
                case User.TypeUser.Moderator:
                    tbLstUser.Visibility = Visibility.Collapsed;

                    btnDelete.IsEnabled = true;

                    break;
                case User.TypeUser.Admin:
                    btnDelete.IsEnabled = false;
                    break;
            }
        }

        private void afficherListUser(User.TypeUser typeUser, string usernameRechercher)
        {
            lstUsers.ItemsSource = null;

            lstUsers.ItemsSource = DAL.ObtainListUsers(typeUser, usernameRechercher);

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
        private bool ValiderFormulaire()
        {

            string pattern = @"^[A-Za-z0-9]{8,20}$";


            string message = "";

            if (!Regex.IsMatch(txtUser.Text, pattern))
                message += "Le nom d'utilisateur ne doit pas contenir de caractères spéciaux et doit être entre 8 et 20 caractères.";

            if (message.Length > 0)
            {
                MessageBox.Show(message, "Validation du mot de passe");
                return false;
            }
            return true;
        }
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValiderFormulaire())
                {
                    UserConnecter.Username = txtUser.Text;

                    BitmapImage bi = imgProduit.Source as BitmapImage;
                    string source = bi.UriSource.LocalPath;

                    UserConnecter.Image = source;

                    bool isUpdated = DAL.ModifyInfoUser(UserConnecter);

                    AfficherImage(UserConnecter.Image);

                    txtOldPassword.Password = null;
                    txtNewPassword.Password = null;
                    txtConfirmNewPassword.Password = null;

                    afficherListUser(UserConnecter.TypeUtilisateurs, "");

                    if (isUpdated)
                    {
                        MessageBox.Show("Les informations de l'utilisateur ont été mises à jour avec succès.", "Modification des informations", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("La mise à jour des informations de l'utilisateur a échoué.", "Modification des informations", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Modification d'un compte");
            }

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


            bi.UriSource = new Uri(cheminFichier);
            bi.CacheOption = BitmapCacheOption.OnLoad;

            bi.EndInit();


            imgProduit.Source = bi;
        }
        public bool ValiderPassword()
        {
            string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]).{8,20}$";

            string message = "";

            if (!Regex.IsMatch(txtNewPassword.Password, pattern))
                message += "Le mot de passe doit contenir au moins 1 caractère spécial, 1 numéro, 1 majuscule et doit être entre 8 et 20 caractères.";

            if (message.Length > 0)
            {
                MessageBox.Show(message, "Validation du mot de passe");
                return false;
            }
            return true;
        }
        private void btnModifierPassword_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine($"Utilisateur: {UserConnecter.Username}, Type: {UserConnecter.TypeUtilisateurs}");

            if (PasswordHelper.VerifyPassword(txtOldPassword.Password, UserConnecter.Password))
            {
                if (ValiderPassword())
                    if (txtNewPassword.Password == txtConfirmNewPassword.Password)
                    {
                        UserConnecter.Password = txtConfirmNewPassword.Password;

                        bool isUpdated = DAL.ModifyPasswordUser(UserConnecter);

                        if (isUpdated)
                        {
                            MessageBox.Show("Le mot de passe a bien été changé!", "Modification du mot de passe", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("La mise à jour du mot de passe a échoué.", "Modification du mot de passe", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Le nouveau mot de passe entrée n'est pas le même que le mot de passe confirmé!", "Modification du mot de passe", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
            }
            else
            {
                MessageBox.Show("Le mot de passe entrée n'est pas le même que l'existant!", "Modification du mot de passe", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult messageBoxResult = MessageBox.Show($"Êtes-vous sûre de vouloir supprimer votre compte?", "Suppression d'un compte", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    bool aEteSupprimer = DAL.DeleteUser(UserConnecter);

                    afficherListUser(UserConnecter.TypeUtilisateurs, "");

                    if (aEteSupprimer)
                    {
                        MessageBox.Show("Le compte a bien été supprimé", "Suppression d'un compte");

                        this.Close();

                        SignIn inscription = new SignIn();
                        inscription.Show();
                    }
                    else
                    {
                        MessageBox.Show("Le compte n'a pas été suprimé, une erreur c'est produite.", "Suppression d'un compte");

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Suppression d'un produit");
            }


        }
        private void btnRechercherUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                afficherListUser(UserConnecter.TypeUtilisateurs, txtRechercher.Text);

                txtRechercher.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Recherche d'un produit");
            }
        }
        private void btnDeleteSelectedUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstUsers.SelectedItem != null)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"Êtes-vous sûre de vouloir supprimer le compte selectionne?", "Suppression d'un compte", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        bool aEteSupprimer = DAL.DeleteUser(lstUsers.SelectedItem as User);

                        if (aEteSupprimer)
                        {
                            MessageBox.Show("Le compte a bien été supprimé", "Suppression d'un compte");


                        }
                        else
                        {
                            MessageBox.Show("Le compte n'a pas été suprimé, une erreur c'est produite.", "Suppression d'un compte");

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner un utilisateur dans la liste!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produit : " + ex.Message, "Suppression d'un utilisateur", MessageBoxButton.OK);
            }
        }
    }
}
