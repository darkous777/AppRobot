using AppRobot.Classes;
using AppRobot.Models;
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

            Utilisateur utilisateur = new Utilisateur(0, username, password, DateOnly.FromDateTime(DateTime.Now), User.TypeUser.User, img);
            User user = DAL.ConnectionUtilisateur(utilisateur);

            if (user.Id > 0)
            {
                GestionUser gestion = new GestionUser(user);

                gestion.Show();

                this.Close();
            }
            else
            {
                txtUser.Text = null;
                txtPassword.Password = null;
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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