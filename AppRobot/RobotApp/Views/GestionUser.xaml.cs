using RobotApp.Classes;
using RobotApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
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
using System.Windows.Threading;

namespace RobotApp.Views
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
        private TcpClient _connectionRobot;

        public TcpClient ConnectionRobot
        {
            get { return _connectionRobot; }
            set { _connectionRobot = value; }
        }

        private NetworkStream _reseauEchange;

        public NetworkStream ReseauEchange
        {
            get { return _reseauEchange; }
            set { _reseauEchange = value; }
        }

        public const string PRODUIT_IMAGES = "Images:Path";

        public const string CONNECTION_ROBOT_IP = "ConnectionRobot:IP";

        public const string CONNECTION_ROBOT_PORT = "ConnectionRobot:Port";

        IConfiguration _configuration;

        public GestionUser(User user)
        {
            InitializeComponent();
            videoExpliquative.MediaFailed += (sender, e) =>
            {
                MessageBox.Show($"Video failed to load: {e.ErrorException.Message}");
            };
            videoExpliquative.MediaOpened += (sender, e) =>
            {
                MessageBox.Show("Video loaded successfully!");
            };
            try
            {
                UserConnecter = User.ObtenirTypeUser(user);
                txtUser.Text = UserConnecter.Username;
                datePicker.SelectedDate = user.DateOfBirth;
                datePicker.IsEnabled = false;
                AfficherImage(user.Image);

                tbLstFonctionnalite.Visibility = Visibility.Collapsed;

                _configuration = new ConfigurationBuilder().AddJsonFile(DAL.APPSETTING_FILE, false, true).Build();

                afficherListUser(UserConnecter.TypeUtilisateurs, "", UserConnecter);

                switch (UserConnecter.TypeUtilisateurs)
                {
                    case User.TypeUser.User:
                        tbLstUser.Visibility = Visibility.Collapsed;
                        btnDelete.IsEnabled = true;

                        break;
                    case User.TypeUser.Moderator:
                        btnAttribuerModeratorSelected.IsEnabled = false;
                        btnEnleverModeratorSelected.IsEnabled = false;
                        btnDelete.IsEnabled = true;

                        break;
                    case User.TypeUser.Admin:
                        tbLstFonctionnalite.Visibility = Visibility.Visible;
                        if (UserConnecter is Admin admin)
                            cboUtilisateur.ItemsSource = admin.ListUsernames(UserConnecter);
                        cboFonctionnalitee.ItemsSource = DAL.ChercherListeDesFonctionnalites();
                        btnDelete.IsEnabled = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Chargement de fenêtre");
            }


        }
        /// <summary>
        /// Méthode pour afficher la liste des utilisateurs
        /// </summary>
        /// <param name="typeUser"></param>
        /// <param name="usernameRechercher"></param>
        /// <param name="user"></param>
        private void afficherListUser(User.TypeUser typeUser, string usernameRechercher, User user)
        {
            lstUsers.ItemsSource = null;
            if (user is Admin admin)
            {
                lstUsers.ItemsSource = admin.ListUser(admin, usernameRechercher);
            }
            else if (user is Moderator moderator)
            {
                lstUsers.ItemsSource = moderator.ListUser(moderator, usernameRechercher);
            }
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
            if (btnVerifierConnectionRobot.IsEnabled is true)
                if (ConnectionRobot != null)
                {
                    ConnectionRobot.Close();
                }
            Application.Current.Shutdown();
        }
        /// <summary>
        /// Méthode pour valider le formulaire de modification d'utilisateur
        /// </summary>
        /// <returns></returns>
        private bool ValiderFormulaire()
        {

            string pattern = @"^[A-Za-z0-9]{8,20}$";


            string message = "";

            if (!Regex.IsMatch(txtUser.Text, pattern))
                message += "Le nom d'utilisateur ne doit pas contenir de caractères spéciaux et doit être entre 8 et 20 caractères.";

            if (txtUser.Text.ToLower().Trim() != UserConnecter.Username.ToLower().Trim())
            {
                if (DAL.FindUserByUsername(txtUser.Text.Trim()) != null)
                    message += "Le nom d'utilisateur est déjà utilisé par un autre utilisateur.";
            }
            if (txtUser.Text.ToLower().Trim() == UserConnecter.Username.ToLower().Trim() && (imgAvatar.Source as BitmapImage).UriSource == new Uri(UserConnecter.Image))
                message += "Veuillez effectuer un changement!";

            if (message.Length > 0)
            {
                MessageBox.Show(message, "Validation du mot de passe", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    BitmapImage bi = imgAvatar.Source as BitmapImage;
                    string source = bi.UriSource.LocalPath;

                    UserConnecter.Image = source;
                    UserConnecter.Username = txtUser.Text.Trim();





                    txtOldPassword.Password = null;
                    txtNewPassword.Password = null;
                    txtConfirmNewPassword.Password = null;




                    if (User.ModifierUser(UserConnecter) != null)
                    {
                        UserConnecter = DAL.FindUserById(UserConnecter.Id);

                        AfficherImage(UserConnecter.Image);
                        afficherListUser(UserConnecter.TypeUtilisateurs, "", UserConnecter);
                        MessageBox.Show("Les informations de l'utilisateur ont été mises à jour avec succès.", "Modification des informations", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("La mise à jour des informations de l'utilisateur a échoué.", "Modification des informations", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else
                {
                    txtUser.Text = UserConnecter.Username;
                    AfficherImage(UserConnecter.Image);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Modification d'un compte", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Ajout d'une image", MessageBoxButton.OK, MessageBoxImage.Error);
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


            imgAvatar.Source = bi;
        }
        /// <summary>
        /// Méthode pour valider le mot de passe
        /// </summary>
        /// <returns></returns>
        public bool ValiderPassword()
        {
            string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]).{8,20}$";

            string message = "";

            if (!Regex.IsMatch(txtNewPassword.Password, pattern))
                message += "Le mot de passe doit contenir au moins 1 caractère spécial, 1 numéro, 1 majuscule et doit être entre 8 et 20 caractères.";

            if (message.Length > 0)
            {
                MessageBox.Show(message, "Validation du mot de passe", MessageBoxButton.OK, MessageBoxImage.Information);
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



                        if (User.ModifyPassword(UserConnecter))
                        {
                            MessageBox.Show("Le mot de passe a bien été changé!", "Modification du mot de passe", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("La mise à jour du mot de passe a échoué.", "Modification du mot de passe", MessageBoxButton.OK, MessageBoxImage.Error);
                            UserConnecter.Password = txtOldPassword.Password;
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
        /// <summary>
        /// Méthode pour supprimer le compte de l'utilisateur connecté
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool ActionSupprimerSonCompte(User user)
        {
            if (user is Admin admin)
            {
                return false;
            }
            else if (user is Moderator modo)
            {
                return modo.DeleteOwnUser(modo);
            }
            else
            {
                return DAL.DeleteUser(user);
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult messageBoxResult = MessageBox.Show($"Êtes-vous sûre de vouloir supprimer votre compte?", "Suppression d'un compte", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    if (ActionSupprimerSonCompte(UserConnecter))
                    {
                        MessageBox.Show("Le compte a bien été supprimé", "Suppression d'un compte", MessageBoxButton.OK);

                        SignIn inscription = new SignIn();
                        inscription.Show();

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Le compte n'a pas été suprimé, une erreur c'est produite.", "Suppression d'un compte", MessageBoxButton.OK, MessageBoxImage.Error);

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Suppression d'un compte utilisateur", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private void btnRechercherUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                afficherListUser(UserConnecter.TypeUtilisateurs, txtRechercher.Text, UserConnecter);

                txtRechercher.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Recherche d'un compte", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Méthode pour supprimer un utilisateur sélectionné
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userSelected"></param>
        /// <returns></returns>
        private bool ActionDeSuppressionSelectionnee(User user, User userSelected)
        {
            if (user is Admin admin)
            {
                return admin.DeleteSelectedUser(userSelected);
            }
            else if (user is Moderator moderator)
            {
                return moderator.DeleteSelectedUser(userSelected);
            }
            else
            {
                return false;
            }
        }
        private void btnDeleteSelectedUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstUsers.SelectedItem != null)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"Êtes-vous sûre de vouloir supprimer le compte selectionne?", "Suppression d'un compte d'utilisateur", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {

                        bool aEteSupprimer = ActionDeSuppressionSelectionnee(UserConnecter, lstUsers.SelectedItem as User);


                        if (aEteSupprimer)
                        {
                            afficherListUser(UserConnecter.TypeUtilisateurs, "", UserConnecter);
                            MessageBox.Show("Le compte sélectionné a bien été supprimé", "Suppression d'un compte", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        else
                        {
                            MessageBox.Show("Le compte sélectionné n'a pas été suprimé, une erreur c'est produite.", "Suppression d'un compte", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner un utilisateur dans la liste!", "Suppression d'un compte", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produit : " + ex.Message, "Suppression d'un utilisateur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Méthode pour attribuer le rôle de modérateur à un utilisateur sélectionné
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool AttributionDeRole(User user)
        {
            if (user is Moderator)
            {
                return false;
            }
            else if (user is Utilisateur && UserConnecter is Admin admin)
            {
                return admin.AttributionDeRole(user);
            }
            else
            {
                return false;
            }
        }
        private void btnAttribuerModeratorSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstUsers.SelectedItem != null)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"Êtes-vous sûre de vouloir attribuer le role de modérateur à l'utilisateur sélectionné?", "Attribution de role", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {


                        if (AttributionDeRole(lstUsers.SelectedItem as User))
                        {
                            MessageBox.Show("L'utilisateur a bien été promu au rôle de modérateur!", "Attribution de role", MessageBoxButton.OK, MessageBoxImage.Information);

                            afficherListUser(UserConnecter.TypeUtilisateurs, "", UserConnecter);
                        }
                        else
                        {
                            MessageBox.Show("L'utilisateur n'a pas été promu au rôle de modérateur, une erreur c'est produite.", "Attribution de role", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner un utilisateur dans la liste!", "Attribution de role", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Une erreur s'est produit : " + ex.Message, "Attribution de role", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        ///     Méthode pour déattribuer le rôle de modérateur à un utilisateur sélectionné
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool DeattribuerUser(User user)
        {
            if (user is Utilisateur)
            {
                return false;
            }
            else if (user is Moderator && UserConnecter is Admin admin)
            {
                return admin.DeattributionDeRole(user);
            }
            else
            {
                return false;
            }
        }
        private void btnEnleverModeratorSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstUsers.SelectedItem != null)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"Êtes-vous sûre de vouloir attribuer le role de modérateur à l'utilisateur sélectionné?", "Attribution de role", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {


                        if (DeattribuerUser(lstUsers.SelectedItem as User))
                        {
                            MessageBox.Show("L'utilisateur a bien été promu au rôle d'utilisateur!", "Attribution de role", MessageBoxButton.OK, MessageBoxImage.Information);

                            afficherListUser(UserConnecter.TypeUtilisateurs, "", UserConnecter);
                        }
                        else
                        {
                            MessageBox.Show("L'utilisateur n'a pas été promu au rôle d'utilisateur, une erreur c'est produite.", "Attribution de role", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner un utilisateur dans la liste!", "Attribution de role", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Une erreur s'est produit : " + ex.Message, "Attribution de role", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Méthode pour bloquer un utilisateur sélectionné
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool bloquerUserSelected(User user)
        {
            if ((UserConnecter is Moderator && user is Moderator) || user.Acces is false)
            {
                return false;
            }
            else if ((user is Utilisateur || user is Moderator) && UserConnecter is Admin admin)
            {
                return admin.BloquerUser(user);
            }
            else if (user is Utilisateur && UserConnecter is Moderator moderator)
            {
                return moderator.BloquerUser(user);
            }

            return false;
        }
        private void btnBloquerUserSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstUsers.SelectedItem != null)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"Êtes-vous sûre de vouloir bloquer l'utilisateur sélectionné?", "Bloquer un utilisateur", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {


                        if (bloquerUserSelected(lstUsers.SelectedItem as User))
                        {
                            MessageBox.Show("L'utilisateur a bien été bloquer!", "Bloquer un utilisateur", MessageBoxButton.OK, MessageBoxImage.Information);

                            afficherListUser(UserConnecter.TypeUtilisateurs, "", UserConnecter);
                        }
                        else
                        {
                            MessageBox.Show("L'utilisateur n'a pas été bloquer, une erreur c'est produite.", "Bloquer un utilisateur", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner un utilisateur dans la liste!", "Bloquer un utilisateur", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Une erreur s'est produit : " + ex.Message, "Bloquer un utilisateur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        /// <summary>
        /// Méthode pour débloquer un utilisateur sélectionné
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool debloquerUserSelected(User user)
        {
            if ((UserConnecter is Moderator && user is Moderator) || user.Acces is true)
            {
                return false;
            }
            else if ((user is Utilisateur || user is Moderator) && UserConnecter is Admin admin)
            {
                return admin.DebloquerUser(user);
            }
            else if (user is Utilisateur && UserConnecter is Moderator moderator)
            {
                return moderator.DebloquerUser(user);
            }

            return false;
        }
        private void btnDeloquerUserSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstUsers.SelectedItem != null)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"Êtes-vous sûre de vouloir débloquer l'utilisateur sélectionné?", "Débloquer un utilisateur", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        if (debloquerUserSelected(lstUsers.SelectedItem as User))
                        {
                            MessageBox.Show("L'utilisateur a bien été débloquer!", "Débloquer un utilisateur", MessageBoxButton.OK, MessageBoxImage.Information);

                            afficherListUser(UserConnecter.TypeUtilisateurs, "", UserConnecter);
                        }
                        else
                        {
                            MessageBox.Show("L'utilisateur n'a pas été débloquer, une erreur c'est produite.", "Débloquer un utilisateur", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner un utilisateur dans la liste!", "Débloquer un utilisateur", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Une erreur s'est produit : " + ex.Message, "Débloquer un utilisateur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Méthode pour établir la connection avec le robot
        /// </summary>
        private void ConnectionAvecRobot()
        {

            ConnectionRobot = new TcpClient(_configuration[CONNECTION_ROBOT_IP], int.Parse(_configuration[CONNECTION_ROBOT_PORT]));
            ReseauEchange = ConnectionRobot.GetStream();
        }
        /// <summary>
        /// Méthode pour envoyer et recevoir les données du robot
        /// </summary>
        /// <param name="msg"></param>
        private void EnvoyerEtRecevoirDonnees(string msg)
        {
            byte[] data = Encoding.ASCII.GetBytes(msg);
            ReseauEchange.Write(data, 0, data.Length);

            byte[] response = new byte[1024];

            int bytesRead = ReseauEchange.Read(response, 0, response.Length);

            string message = Encoding.ASCII.GetString(response, 0, bytesRead);

            if (message != "")
            {
                MessageBox.Show($"Voici le message du robot : {message}", "Connection au robot", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        private void btnConnecterAvecLeRobot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectionAvecRobot();
                btnFermerConnectionAvecLeRobot.IsEnabled = true;
                btnVerifierConnectionRobot.IsEnabled = true;
                btnUtiliserLeRobot.IsEnabled = true;
                MessageBox.Show("la connection avec le robot à été établi!", "Connection avec robot", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Une erreur s'est produit : " + ex.Message, "Connection avec le robot", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnVerifierConnectionRobot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnvoyerEtRecevoirDonnees("salut");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produit : " + ex.Message, "Test la connection avec le robot", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnFermerConnectionAvecLeRobot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConnectionRobot != null)
                {
                    EnvoyerEtRecevoirDonnees("!DISCONNECT");
                    ConnectionRobot.Close();
                    btnFermerConnectionAvecLeRobot.IsEnabled = false;
                    btnVerifierConnectionRobot.IsEnabled = false;
                    btnUtiliserLeRobot.IsEnabled = false;
                    MessageBox.Show("Connection à bien été fermée!", "Fermeture de la connection avec le robot", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Aucun connection détecté pour être fermée!", "Fermeture de la connection avec le robot", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produit : " + ex.Message, "Fermeture de la connection avec le robot", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnUtiliserLeRobot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConnectionRobot != null)
                {
                    Robot robot = new Robot(UserConnecter, ConnectionRobot, ReseauEchange);

                    if (robot.ShowDialog() is true)
                    {
                        if (ConnectionRobot != null)
                        {
                            EnvoyerEtRecevoirDonnees("!DISCONNECT");
                            ConnectionRobot.Close();
                            btnFermerConnectionAvecLeRobot.IsEnabled = false;
                            btnVerifierConnectionRobot.IsEnabled = false;
                            btnUtiliserLeRobot.IsEnabled = false;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Aucun connection détecté pour utiliser le robot!", "Utilisation du robot", MessageBoxButton.OK);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnDebloquerFonctionnalitee_Click(object sender, RoutedEventArgs e)
        {
            if (cboFonctionnalitee.SelectedItem != null && cboUtilisateur.SelectedItem != null)
            {
                string username = cboUtilisateur.SelectedItem.ToString();
                User user = DAL.FindUserByUsername(username);
                Fonctionnalite fonctionnalite = (Fonctionnalite)cboFonctionnalitee.SelectedItem;

                if (!DAL.UtilisateurPossedeFonctionnalite(user, fonctionnalite))
                {
                    if (DAL.ModifierAccesFonctionnalite(user, fonctionnalite, true))
                    {
                        cboFonctionnalitee.SelectedItem = null;
                        cboUtilisateur.SelectedItem = null;
                        MessageBox.Show("La fonctionnalité a bien été débloquée!", "Débloquer une fonctionnalité", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("La fonctionnalité n'a pas pu être débloquée, une erreur c'est produite.", "Débloquer une fonctionnalité", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else
                {
                    MessageBox.Show("Cette fonctionnalité est déjà débloquée pour cet utilisateur!", "Débloquer une fonctionnalité", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            else
                MessageBox.Show("Vous devez sélectionner une fonctionnalité et un utilisateur!", "Débloquer une fonctionnalité", MessageBoxButton.OK, MessageBoxImage.Warning);





        }

        private void btnBloquerFonctionnalitee_Click(object sender, RoutedEventArgs e)
        {
            if (cboFonctionnalitee.SelectedItem != null && cboUtilisateur.SelectedItem != null)
            {
                string username = cboUtilisateur.SelectedItem.ToString();
                User user = DAL.FindUserByUsername(username);
                Fonctionnalite fonctionnalite = (Fonctionnalite)cboFonctionnalitee.SelectedItem;

                if (!DAL.UtilisateurPossedeFonctionnalite(user, fonctionnalite))
                {
                    if (DAL.ModifierAccesFonctionnalite(user, fonctionnalite, false))
                    {
                        cboFonctionnalitee.SelectedItem = null;
                        cboUtilisateur.SelectedItem = null;
                        MessageBox.Show("La fonctionnalité a bien été bloquée!", "Bloquer une fonctionnalité", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("La fonctionnalité n'a pas pu être débloquée, une erreur c'est produite.", "Bloquer une fonctionnalité", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else
                {
                    MessageBox.Show("Cette fonctionnalité est déjà bloquée pour cet utilisateur!", "Bloquer une fonctionnalité", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            else
                MessageBox.Show("Vous devez sélectionner une fonctionnalité et un utilisateur!", "Bloquer une fonctionnalité", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            videoExpliquative.Source = new Uri("Resources/Videos/videoTest.mp4", UriKind.Relative);

            videoExpliquative.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            videoExpliquative.Pause();
        }
    }
}
