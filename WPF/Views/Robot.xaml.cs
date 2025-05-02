using AppRobot.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using AppRobot.Classes;

namespace AppRobot.Views
{
    /// <summary>
    /// Logique d'interaction pour Robot.xaml
    /// </summary>
    public partial class Robot : Window
    {
        private CancellationTokenSource _cancellationTokenSource;
        private int _frameCount = 0;

        private User _user;
        private TcpClient _connectionRobot;
        private NetworkStream _reseauEchange;
        private HashSet<Key> _pressedKeys = new HashSet<Key>();
        private DispatcherTimer _keyCheckTimer;
        private string _lastCommandSent = "";
        private bool music = false;
        private bool spacePressed = false;
        public User UserConnecter
        {
            get { return _user; }
            set { _user = value; }
        }
        public TcpClient ConnectionRobot
        {
            get { return _connectionRobot; }
            set { _connectionRobot = value; }
        }
        public NetworkStream ReseauEchange
        {
            get { return _reseauEchange; }
            set { _reseauEchange = value; }
        }
        private const string APPSETTING_FILE = "appsettings.json";


        private const string CONNECTION_CAM_ROBOT = "ConnectionRobot:VideoURL";

        private const string CONNECTION_ROBOT_IP = "ConnectionRobot:IP";

        private const string CONNECTION_ROBOT_PORT = "ConnectionRobot:Port";

        private const string CONNECTION_ROBOT_VIDEO_USER = "ConnectionRobot:User";

        private const string CONNECTION_ROBOT_VIDEO_PASSWORD = "ConnectionRobot:Password";

        private static IConfiguration _configuration;

        /// <summary>
        /// Constructeur static permettant de charger les configurations de l'application
        /// </summary>
        static Robot()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile(APPSETTING_FILE, false, true).Build();
        }

        public Robot(User user, TcpClient tcp, NetworkStream stream)
        {
            UserConnecter = user;
            ConnectionRobot = tcp;
            ReseauEchange = stream;

            InitializeComponent();

            _keyCheckTimer = new DispatcherTimer();
            _keyCheckTimer.Interval = TimeSpan.FromMilliseconds(50);
            _keyCheckTimer.Tick += KeyCheckTimer_Tick;
            _keyCheckTimer.Start();

            ControlDeDroitDeFonctionnalite();
        }

        /// <summary>
        /// Contrôle les droits de fonctionnalité de l'utilisateur connecté.
        /// </summary>
        private void ControlDeDroitDeFonctionnalite()
        {
            foreach (Fonctionnalite fonctionnalite in UserConnecter.ListeFonctionnalite.Values)
            {
                switch (fonctionnalite.Nom)
                {
                    case "Avancer":
                        btnForward.IsEnabled = true;
                        break;
                    case "Avancer/Gauche":
                        btnForwardLeft.IsEnabled = true;
                        break;
                    case "Avancer/Droite":
                        btnForwardRight.IsEnabled = true;
                        break;
                    case "Reculer":
                        btnBackward.IsEnabled = true;
                        break;
                    case "Reculer/Gauche":
                        btnBackwardLeft.IsEnabled = true;
                        break;
                    case "Reculer/Droite":
                        btnBackwardRight.IsEnabled = true;
                        break;
                    case "Rotation/Gauche":
                        btnRotationLeft.IsEnabled = true;
                        break;
                    case "Rotation/Droite":
                        btnRotationRight.IsEnabled = true;
                        break;
                    case "Suivre/Ligne":
                        btnFollowLine.IsEnabled = true;
                        break;
                    case "Camera":
                        btnStart.IsEnabled = true;
                        btnStop.IsEnabled = true;
                        break;
                    case "Musique":
                        btnMusic.IsEnabled = true;
                        break;

                        //}
                        //else
                        //{
                        //    switch (fonctionnalite.Nom)
                        //    {
                        //        case "Avancer":
                        //            btnForward.IsEnabled = false;
                        //            break;
                        //        case "Avancer/Gauche":
                        //            btnForwardLeft.IsEnabled = false;
                        //            break;
                        //        case "Avancer/Droite":
                        //            btnForwardRight.IsEnabled = false;
                        //            break;
                        //        case "Reculer":
                        //            btnBackward.IsEnabled = false;
                        //            break;
                        //        case "Reculer/Gauche":
                        //            btnBackwardLeft.IsEnabled = false;
                        //            break;
                        //        case "Reculer/Droite":
                        //            btnBackwardRight.IsEnabled = false;
                        //            break;
                        //        case "Rotation/Gauche":
                        //            btnRotationLeft.IsEnabled = false;
                        //            break;
                        //        case "Rotation/Droite":
                        //            btnRotationRight.IsEnabled = false;
                        //            break;
                        //        case "Suivre/Ligne":
                        //            btnFollowLine.IsEnabled = false;
                        //            break;
                        //        case "Camera":
                        //            btnStart.IsEnabled = false;
                        //            btnStop.IsEnabled = false;
                        //            break;
                        //        case "Musique":
                        //            btnMusic.IsEnabled = false;
                        //            break;
                        //    }
                }
            }

        }
        /// <summary>
        /// Gestionnaire d'événements pour le minuteur de vérification des touches.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyCheckTimer_Tick(object sender, EventArgs e)
        {
            string commandToSend = "";

            if ((_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.D)) && PermissionUserAController("forward_right"))
                commandToSend = "forward_right";
            else if ((_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.A)) && PermissionUserAController("forward_left"))
                commandToSend = "forward_left";
            else if ((_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.D)) && PermissionUserAController("backward_right"))
                commandToSend = "backward_right";
            else if ((_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.A)) && PermissionUserAController("backward_left"))
                commandToSend = "backward_left";
            else if ((_pressedKeys.Contains(Key.W)) && PermissionUserAController("forward"))
                commandToSend = "forward";
            else if ((_pressedKeys.Contains(Key.A)) && PermissionUserAController("rotation_left"))
                commandToSend = "rotation_left";
            else if ((_pressedKeys.Contains(Key.D)) && PermissionUserAController("rotation_right"))
                commandToSend = "rotation_right";
            else if ((_pressedKeys.Contains(Key.S)) && PermissionUserAController("backward"))
                commandToSend = "backward";
            else if ((_pressedKeys.Contains(Key.Space) && !spacePressed) && PermissionUserAController("music_on"))
            {
                spacePressed = true;

                if (!music)
                {
                    commandToSend = "music_on";
                    music = true;
                }
                else
                {
                    commandToSend = "music_off";
                    music = false;
                }

            }
            else
            {
                commandToSend = "stop";
            }

            if (commandToSend != _lastCommandSent)
            {
                _lastCommandSent = commandToSend;
                EnvoyerEtRecevoirDonnees(commandToSend);
            }
        }
        /// <summary>
        /// Convertit le message de commande en une chaîne de caractères lisible par la base de donnée.
        /// </summary>
        /// <param name="commande"></param>
        /// <returns></returns>
        private string PermissionDeControler(string commande)
        {
            switch (commande)
            {
                case "forward":
                    return "Avancer";
                case "forward_right":
                    return "Avancer/Droite";
                case "forward_left":
                    return "Avancer/Gauche";
                case "backward":
                    return "Reculer";
                case "backward_right":
                    return "Reculer/Droite";
                case "backward_left":
                    return "Reculer/Gauche";
                case "rotation_right":
                    return "Rotation/Droite";
                case "rotation_left":
                    return "Rotation/Gauche";
                case "music_on":
                    return "Musique";
                case "camera_on":
                    return "Camera";
                case "follow_line":
                    return "Suivre/Ligne";
                default:
                    return "";
            }
        }
        /// <summary>
        /// Vérifie si l'utilisateur a la permission de contrôler une fonctionnalité spécifique.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private bool PermissionUserAController(string command)
        {
            Fonctionnalite fonctionnalite = DAL.ChercherFonctionnalite(PermissionDeControler(command));

            return DAL.UtilisateurPossedeFonctionnalite(UserConnecter, fonctionnalite);
        }

        /// <summary>
        /// Gestionnaire d'événements pour le clic de la souris sur la fenêtre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }

        }
        /// <summary>
        /// Gestionnaire d'événements pour le clic sur le bouton de réduction de la fenêtre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }
        /// <summary>
        /// Gestionnaire d'événements pour le clic sur le bouton de fermeture de la fenêtre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

        }
        /// <summary>
        /// Envoie une commande au robot et reçoit la réponse.
        /// </summary>
        /// <param name="commande">Commande envoyer au robot</param>
        private void EnvoyerEtRecevoirDonnees(string commande)
        {
            byte[] data = Encoding.ASCII.GetBytes(commande);
            ReseauEchange.Write(data, 0, data.Length);

            byte[] response = new byte[1024];

            ReseauEchange.ReadTimeout = 5000;
            int bytesRead = ReseauEchange.Read(response, 0, response.Length);

            string message = Encoding.ASCII.GetString(response, 0, bytesRead);

            if (message != "-_-")
            {
                statusLabel.Text = $"Voici le message du robot : {message}";
            }
            else
            {
                statusLabel.Text = "";
            }


        }
        /// <summary>
        /// Gestionnaire d'événements pour la pression d'une touche sur la fenêtre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W || e.Key == Key.A || e.Key == Key.S || e.Key == Key.D || e.Key == Key.Space || e.Key == Key.D || e.Key == Key.A)
            {
                _pressedKeys.Add(e.Key);
            }

            if ((_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.D)) && PermissionUserAController("forward_right"))
            {
                var triangleForwardRight = (System.Windows.Shapes.Path)btnForwardRight.Template.FindName("triangleForwardRight", btnForwardRight);
                triangleForwardRight.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 138, 212));
                triangleForwardRight.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(218, 52, 174));
            }
            else if ((_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.A)) && PermissionUserAController("forward_left"))
            {
                var triangleForwardLeft = (System.Windows.Shapes.Path)btnForwardLeft.Template.FindName("triangleForwardLeft", btnForwardLeft);
                triangleForwardLeft.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 138, 212));
                triangleForwardLeft.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(218, 52, 174));
            }
            else if ((_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.D)) && PermissionUserAController("backward_right"))
            {
                var triangleBackwardRight = (System.Windows.Shapes.Path)btnBackwardRight.Template.FindName("triangleBackwardRight", btnBackwardRight);
                triangleBackwardRight.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 138, 212));
                triangleBackwardRight.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(218, 52, 174));
            }
            else if ((_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.A)) && PermissionUserAController("backward_left"))
            {
                var triangleBackwardLeft = (System.Windows.Shapes.Path)btnBackwardLeft.Template.FindName("triangleBackwardLeft", btnBackwardLeft);
                triangleBackwardLeft.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 138, 212));
                triangleBackwardLeft.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(218, 52, 174));
            }
            else if ((_pressedKeys.Contains(Key.W) && _pressedKeys.Count == 1) && PermissionUserAController("forward"))
            {
                var triangleForward = (System.Windows.Shapes.Path)btnForward.Template.FindName("triangleForward", btnForward);
                triangleForward.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 174, 237));
                triangleForward.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(70, 42, 216));
            }
            if ((_pressedKeys.Contains(Key.D) && _pressedKeys.Count == 1) && PermissionUserAController("rotation_right"))
            {
                var triangleRotationRight = (System.Windows.Shapes.Path)btnRotationRight.Template.FindName("triangleRotationRight", btnRotationRight);
                triangleRotationRight.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 174, 237));
                triangleRotationRight.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(70, 42, 216));
            }
            else if ((_pressedKeys.Contains(Key.A) && _pressedKeys.Count == 1) && PermissionUserAController("rotation_left"))
            {
                var triangleRotationLeft = (System.Windows.Shapes.Path)btnRotationLeft.Template.FindName("triangleRotationLeft", btnRotationLeft);
                triangleRotationLeft.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 174, 237));
                triangleRotationLeft.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(70, 42, 216));
            }
            else if ((_pressedKeys.Contains(Key.S) && _pressedKeys.Count == 1) && PermissionUserAController("backward"))
            {
                var triangleBackward = (System.Windows.Shapes.Path)btnBackward.Template.FindName("triangleBackward", btnBackward);
                triangleBackward.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 174, 237));
                triangleBackward.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(70, 42, 216));
            }
            else if ((_pressedKeys.Contains(Key.Space) && !spacePressed) && PermissionUserAController("music_on"))
            {
                btnMusic.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 138, 212));
                btnMusic.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(218, 52, 174));
            }
        }

        /// <summary>
        /// Gestionnaire d'événements pour la relâchement d'une touche sur la fenêtre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

            if ((_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.D)) && PermissionUserAController("forward_right"))
            {
                var triangleForwardRight = (System.Windows.Shapes.Path)btnForwardRight.Template.FindName("triangleForwardRight", btnForwardRight);
                triangleForwardRight.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(218, 52, 174));
                triangleForwardRight.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 138, 212));
            }
            if ((_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.A)) && PermissionUserAController("forward_left"))
            {
                var triangleForwardLeft = (System.Windows.Shapes.Path)btnForwardLeft.Template.FindName("triangleForwardLeft", btnForwardLeft);
                triangleForwardLeft.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(218, 52, 174));
                triangleForwardLeft.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 138, 212));
            }
            if ((_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.D)) && PermissionUserAController("backward_right"))
            {
                var triangleBackwardRight = (System.Windows.Shapes.Path)btnBackwardRight.Template.FindName("triangleBackwardRight", btnBackwardRight);
                triangleBackwardRight.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(218, 52, 174));
                triangleBackwardRight.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 138, 212));
            }
            if ((_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.A)) && PermissionUserAController("backward_left"))
            {
                var triangleBackwardLeft = (System.Windows.Shapes.Path)btnBackwardLeft.Template.FindName("triangleBackwardLeft", btnBackwardLeft);
                triangleBackwardLeft.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(218, 52, 174));
                triangleBackwardLeft.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 138, 212));
            }
            if ((_pressedKeys.Contains(Key.W) && _pressedKeys.Count == 1) && PermissionUserAController("forward"))
            {
                var triangleForward = (System.Windows.Shapes.Path)btnForward.Template.FindName("triangleForward", btnForward);
                triangleForward.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(70, 42, 216));
                triangleForward.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 174, 237));
            }
            if ((_pressedKeys.Contains(Key.A) && _pressedKeys.Count == 1) && PermissionUserAController("rotation_left"))
            {
                var triangleRotationLeft = (System.Windows.Shapes.Path)btnRotationLeft.Template.FindName("triangleRotationLeft", btnRotationLeft);
                triangleRotationLeft.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(70, 42, 216));
                triangleRotationLeft.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 174, 237));
            }
            if ((_pressedKeys.Contains(Key.D) && _pressedKeys.Count == 1) && PermissionUserAController("rotation_right"))
            {
                var triangleRotationRight = (System.Windows.Shapes.Path)btnRotationRight.Template.FindName("triangleRotationRight", btnRotationRight);
                triangleRotationRight.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(70, 42, 216));
                triangleRotationRight.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 174, 237));
            }
            if ((_pressedKeys.Contains(Key.S) && _pressedKeys.Count == 1) && PermissionUserAController("backward"))
            {
                var triangleBackward = (System.Windows.Shapes.Path)btnBackward.Template.FindName("triangleBackward", btnBackward);
                triangleBackward.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(70, 42, 216));
                triangleBackward.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 174, 237));
            }
            if (_pressedKeys.Contains(Key.Space) && PermissionUserAController("music_on"))
            {
                btnMusic.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(218, 52, 174));
                btnMusic.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 138, 212));
                spacePressed = false;
            }

            if (e.Key == Key.W || e.Key == Key.A || e.Key == Key.S || e.Key == Key.D || e.Key == Key.Space || e.Key == Key.A || e.Key == Key.D)
            {
                _pressedKeys.Remove(e.Key);
            }
        }

        /// <summary>
        /// Gestionnaire d'événements pour le clic sur le bouton de retour au menu de connexion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRetourMenuConnection_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Gestionnaire d'événements pour le clic sur le bouton Démarrer.
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement.</param>
        /// <param name="e">Les arguments de l'événement.</param>
        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PermissionUserAController("camera_on"))
                {
                    if (_cancellationTokenSource != null)
                    {
                        _cancellationTokenSource.Cancel();
                        _cancellationTokenSource = null;
                    }

                    EnvoyerEtRecevoirDonnees("camera_on");

                    Thread.Sleep(3000);

                    string url = _configuration[CONNECTION_CAM_ROBOT];

                    _frameCount = 0;
                    _cancellationTokenSource = new CancellationTokenSource();

                    await Task.Run(() => ReadMjpegStreamAsync(url, _cancellationTokenSource.Token));
                }
                else
                {
                    MessageBox.Show("Vous ne posséder pas les droits pour activer la caméra!", "Activation de la caméra", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de streaming : " + ex.Message);
            }
        }
        /// <summary>
        /// Lit le flux MJPEG à partir de l'URL spécifiée.
        /// </summary>
        /// <param name="url">L'URL du flux MJPEG.</param>
        /// <param name="cancellationToken">Le jeton d'annulation pour annuler l'opération asynchrone.</param>
        /// <returns>Une tâche représentant l'opération asynchrone.</returns>
        private async Task ReadMjpegStreamAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    handler.Credentials = new NetworkCredential(_configuration[CONNECTION_ROBOT_VIDEO_USER], _configuration[CONNECTION_ROBOT_VIDEO_PASSWORD]);

                    using (var client = new HttpClient(handler))
                    {
                        using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                        {
                            response.EnsureSuccessStatusCode();

                            var contentType = response.Content.Headers.ContentType?.ToString();
                            Trace.WriteLine($"Content-Type: {contentType}");

                            if (contentType?.Contains("multipart/x-mixed-replace") == true)
                            {
                                using (var stream = await response.Content.ReadAsStreamAsync())
                                {
                                    await ProcessMjpegStreamAsync(stream, cancellationToken);
                                }
                            }
                            else
                            {
                                throw new Exception("MJPEG stream non valide!");
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Trace.WriteLine("Lecture du flux à été annulé");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Erreur en lisant le flux: {ex.Message}");
            }
        }
        /// <summary>
        /// Traite le flux MJPEG et extrait les images.
        /// </summary>
        /// <param name="stream">Le flux MJPEG à traiter.</param>
        /// <param name="cancellationToken">Le jeton d'annulation pour annuler l'opération asynchrone.</param>
        /// <returns>Une tâche représentant l'opération asynchrone.</returns>
        private async Task ProcessMjpegStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--frame\r\n");
            byte[] headerEnd = Encoding.ASCII.GetBytes("\r\n\r\n");

            using (var reader = new BinaryReader(stream))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (!await FindBytesInStreamAsync(stream, boundaryBytes, cancellationToken))
                            break;

                        if (!await FindBytesInStreamAsync(stream, headerEnd, cancellationToken))
                            break;

                        int bytesRead = 0;
                        int totalRead = 0;

                        using (MemoryStream jpegStream = new MemoryStream())
                        {
                            byte[] buffer = new byte[4096];
                            bool foundBoundary = false;

                            while (!foundBoundary && !cancellationToken.IsCancellationRequested)
                            {
                                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                                if (bytesRead == 0)
                                    break;

                                int boundaryPos = IndexOfBytes(buffer, boundaryBytes, Math.Min(bytesRead, buffer.Length));
                                if (boundaryPos >= 0)
                                {
                                    jpegStream.Write(buffer, 0, boundaryPos);
                                    foundBoundary = true;

                                    int resetPos = boundaryPos + boundaryBytes.Length;
                                    if (resetPos < bytesRead)
                                    {
                                        byte[] remaining = new byte[bytesRead - resetPos];
                                        Array.Copy(buffer, resetPos, remaining, 0, remaining.Length);
                                    }
                                }
                                else
                                {
                                    jpegStream.Write(buffer, 0, bytesRead);
                                }

                                totalRead += bytesRead;
                                if (totalRead > 1024 * 1024)
                                    break;
                            }

                            _frameCount++;

                            if (jpegStream.Length > 0)
                            {
                                try
                                {
                                    jpegStream.Position = 0;
                                    using (var bitmap = new Bitmap(jpegStream))
                                    {
                                        await Dispatcher.InvokeAsync(() =>
                                        {
                                            try
                                            {
                                                imgVideo.Source = BitmapToImageSource(bitmap);
                                            }
                                            catch (Exception ex)
                                            {
                                                Trace.WriteLine($"Erreur mise à jour UI: {ex.Message}");
                                            }
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Trace.WriteLine($"Erreur de l'image: {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Erreur de lecture de flux: {ex.Message}");
                        await Task.Delay(100, cancellationToken);
                    }
                }
            }
        }
        /// <summary>
        /// Trouve le motif de bytes spécifié dans le flux.
        /// </summary>
        /// <param name="stream">Le flux dans lequel rechercher le motif.</param>
        /// <param name="pattern">Le motif de bytes à rechercher.</param>
        /// <param name="cancellationToken">Le jeton d'annulation pour annuler l'opération asynchrone.</param>
        /// <returns>Une tâche représentant l'opération asynchrone, avec un résultat indiquant si le motif a été trouvé.</returns>
        private async Task<bool> FindBytesInStreamAsync(Stream stream, byte[] pattern, CancellationToken cancellationToken)
        {
            int patternPos = 0;
            int b;
            while ((b = stream.ReadByte()) != -1)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                if (b == pattern[patternPos])
                {
                    patternPos++;
                    if (patternPos == pattern.Length)
                        return true;
                }
                else
                {
                    patternPos = (b == pattern[0]) ? 1 : 0;
                }

                await Task.Delay(0);
            }
            return false;
        }
        /// <summary>
        /// Trouve l'index du motif de bytes spécifié dans le buffer.
        /// </summary>
        /// <param name="buffer">Le buffer dans lequel rechercher le motif.</param>
        /// <param name="pattern">Le motif de bytes à rechercher.</param>
        /// <param name="bufferLength">La longueur du buffer à examiner.</param>
        /// <returns>L'index du motif dans le buffer, ou -1 si le motif n'est pas trouvé.</returns>
        private int IndexOfBytes(byte[] buffer, byte[] pattern, int bufferLength)
        {
            for (int i = 0; i <= bufferLength - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (buffer[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// Convertit un objet Bitmap en BitmapSource pour l'affichage dans WPF.
        /// </summary>
        /// <param name="bitmap">L'objet Bitmap à convertir.</param>
        /// <returns>Un BitmapSource représentant l'image convertie.</returns>
        private BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);

                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        /// <summary>
        /// Gestionnaire d'événements pour le clic sur le bouton Arrêter.
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement.</param>
        /// <param name="e">Les arguments de l'événement.</param>
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {


            if (PermissionUserAController("camera_on"))
            {
                if (imgVideo.Source is not null)
                    imgVideo.Source = null;

                if (_cancellationTokenSource != null)
                {
                    EnvoyerEtRecevoirDonnees("camera_off");
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;

                    Trace.WriteLine("Stream arrêter");
                }
            }
            else
            {
                MessageBox.Show("Vous ne posséder pas les droits pour désactiver la caméra!", "Désactivation de la caméra", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnFollowLine_Click(object sender, RoutedEventArgs e)
        {
            if (PermissionUserAController("follow_line"))
            {
                EnvoyerEtRecevoirDonnees("follow_line");
                btnStopFollowLine.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Vous ne posséder pas les droits pour activer le suivi de ligne!", "Activation du suivi de ligne", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnStopFollowLine_Click(object sender, RoutedEventArgs e)
        {
            if (PermissionUserAController("follow_line"))
            {
                EnvoyerEtRecevoirDonnees("stop_follow_line");
                btnStopFollowLine.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Vous ne posséder pas les droits pour désactiver le suivi de ligne!", "Désactivation du suivi de ligne", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
