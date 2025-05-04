using AppRobot.Classes;
using AppRobot.Models;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace AppRobot.Views
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        public SignIn()
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

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPassword.Password;
            string img = "";

            Utilisateur utilisateur = new Utilisateur(0, username, password, DateTime.Now, User.TypeUser.User, img, true,null);
            User user = DAL.ConnectionUtilisateur(utilisateur);

            if(user.Acces is false)
            {
                txtUser.Text = null;
                txtPassword.Password = null;
                MessageBox.Show("Ce compte est présentement bloqué", "Erreur compte bloqué", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (user.Id > 0)
            {
                GestionUser gestion = new GestionUser(user);

                gestion.Show();

                this.Close();
            }
            else
            {
                txtUser.Text = null;
                txtPassword.Password = null;
                MessageBox.Show("Nom d'utilisateur et/ou mot de passe invalide", "Erreur de connection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Navigate_to_SignUp(object sender, RoutedEventArgs e)
        {
            SignUp signUpPage = new SignUp();
            signUpPage.Show();
            this.Close();
        }
    }
}