using AppRobot.Classes;
using AppRobot.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// Logique d'interaction pour SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        private User userManipulation;

        public User UserManipulation
        {
            get { return userManipulation; }
            set { userManipulation = value; }
        }


        public SignUp()
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

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValiderFormulaire())
                {
                    string username = txtUser.Text;
                    string password = txtPassword.Password;
                    string confirmPassword = txtConformePassword.Password;
                    DateTime? selectedDate = datePicker.SelectedDate;

                    if (password != confirmPassword)
                    {
                        MessageBox.Show("Le mot de passe de confirmation doit être le même que le mot de passe.", "Confirmation de mot de passe", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    int age = (int)(((DateTime.Now - selectedDate.Value).TotalDays) / 365);

                    if (selectedDate.HasValue && age >=18 )
                    {
                        BitmapImage bi = imgAvatar.Source as BitmapImage;
                        string source = bi?.UriSource?.LocalPath ?? "";

                        User newUser = new Utilisateur(0, username, password, DateOnly.FromDateTime(selectedDate.Value), User.TypeUser.User, source, true,null);

                        DAL.CreateUser(newUser);

                        MessageBox.Show($"Utilisateur créé avec succès : {newUser.Username}, Date de naissance : {newUser.DateOfBirth.ToShortDateString()}", "Création d'un utilisateur", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Veuillez sélectionner une date de naissance.", "Date de naissance manquante", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Création d'un compte", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValiderFormulaire()
        {
            string pattern = @"^[A-Za-z0-9]{8,20}$";
            string message = "";

            if (!Regex.IsMatch(txtUser.Text, pattern))
                message += "Le nom d'utilisateur ne doit pas contenir de caractères spéciaux et doit être entre 8 et 20 caractères.\n";

            string passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]).{8,20}$";
            if (!Regex.IsMatch(txtPassword.Password, passwordPattern))
                message += "Le mot de passe doit contenir au moins 1 caractère spécial, 1 numéro, 1 majuscule et doit être entre 8 et 20 caractères.\n";

            if (txtPassword.Password != txtConformePassword.Password)
                message += "Les mots de passe ne correspondent pas.\n";

            if (!datePicker.SelectedDate.HasValue)
                message += "Veuillez sélectionner une date de naissance.\n";

            if (message.Length > 0)
            {
                MessageBox.Show(message, "Validation du formulaire", MessageBoxButton.OK);
                return false;
            }
            return true;
        }


        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null && datePicker.SelectedDate.HasValue)
            {
                if (userManipulation != null)
                {
                    userManipulation.DateOfBirth = DateOnly.FromDateTime(datePicker.SelectedDate.Value);
                }
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
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Ajout d'une image", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AfficherImage(string cheminFichier)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();


            bi.UriSource = new Uri(cheminFichier);
            bi.CacheOption = BitmapCacheOption.OnLoad;

            bi.EndInit();


            imgAvatar.Source = bi;
        }
    }
}
