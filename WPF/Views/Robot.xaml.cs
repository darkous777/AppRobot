using AppRobot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using System.Windows.Threading;

namespace AppRobot.Views
{
    /// <summary>
    /// Logique d'interaction pour Robot.xaml
    /// </summary>
    public partial class Robot : Window
    {
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
        }
        private void KeyCheckTimer_Tick(object sender, EventArgs e)
        {
            string commandToSend = "";

            if (_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.D))
                commandToSend = "forward_right";
            else if (_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.A))
                commandToSend = "forward_left";
            else if (_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.D))
                commandToSend = "backward_right";
            else if (_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.A))
                commandToSend = "backward_left";
            else if (_pressedKeys.Contains(Key.W))
                commandToSend = "forward";
            else if (_pressedKeys.Contains(Key.S))
                commandToSend = "backward";
            else if (_pressedKeys.Contains(Key.Space) && !spacePressed)
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
            DialogResult = true;

        }

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
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W || e.Key == Key.A || e.Key == Key.S || e.Key == Key.D || e.Key == Key.Space)
            {
                _pressedKeys.Add(e.Key);
            }

            if (_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.D))
            {
                var triangleForwardRight = (Path)btnForwardRight.Template.FindName("triangleForwardRight", btnForwardRight);
                triangleForwardRight.Fill = new SolidColorBrush(Color.FromRgb(240, 138, 212));
                triangleForwardRight.Stroke = new SolidColorBrush(Color.FromRgb(218, 52, 174));
            }
            else if (_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.A))
            {
                var triangleForwardLeft = (Path)btnForwardLeft.Template.FindName("triangleForwardLeft", btnForwardLeft);
                triangleForwardLeft.Fill = new SolidColorBrush(Color.FromRgb(240, 138, 212));
                triangleForwardLeft.Stroke = new SolidColorBrush(Color.FromRgb(218, 52, 174));
            }
            else if (_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.D))
            {
                var triangleBackwardRight = (Path)btnBackwardRight.Template.FindName("triangleBackwardRight", btnBackwardRight);
                triangleBackwardRight.Fill = new SolidColorBrush(Color.FromRgb(240, 138, 212));
                triangleBackwardRight.Stroke = new SolidColorBrush(Color.FromRgb(218, 52, 174));
            }
            else if (_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.A))
            {
                var triangleBackwardLeft = (Path)btnBackwardLeft.Template.FindName("triangleBackwardLeft", btnBackwardLeft);
                triangleBackwardLeft.Fill = new SolidColorBrush(Color.FromRgb(240, 138, 212));
                triangleBackwardLeft.Stroke = new SolidColorBrush(Color.FromRgb(218, 52, 174));
            }
            else if (_pressedKeys.Contains(Key.W) && _pressedKeys.Count == 1)
            {
                var triangleForward = (Path)btnForward.Template.FindName("triangleForward", btnForward);
                triangleForward.Fill = new SolidColorBrush(Color.FromRgb(40, 174, 237));
                triangleForward.Stroke = new SolidColorBrush(Color.FromRgb(70, 42, 216));
            }
            else if (_pressedKeys.Contains(Key.S) && _pressedKeys.Count == 1)
            {
                var triangleBackward = (Path)btnBackward.Template.FindName("triangleBackward", btnBackward);
                triangleBackward.Fill = new SolidColorBrush(Color.FromRgb(40, 174, 237));
                triangleBackward.Stroke = new SolidColorBrush(Color.FromRgb(70, 42, 216));
            }
            else if (_pressedKeys.Contains(Key.Space) && !spacePressed)
            {
                btnMusic.Background = new SolidColorBrush(Color.FromRgb(240, 138, 212));
                btnMusic.BorderBrush = new SolidColorBrush(Color.FromRgb(218, 52, 174));
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

            if (_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.D))
            {
                var triangleForwardRight = (Path)btnForwardRight.Template.FindName("triangleForwardRight", btnForwardRight);
                triangleForwardRight.Fill = new SolidColorBrush(Color.FromRgb(218, 52, 174));
                triangleForwardRight.Stroke = new SolidColorBrush(Color.FromRgb(240, 138, 212));
            }
            if (_pressedKeys.Contains(Key.W) && _pressedKeys.Contains(Key.A))
            {
                var triangleForwardLeft = (Path)btnForwardLeft.Template.FindName("triangleForwardLeft", btnForwardLeft);
                triangleForwardLeft.Fill = new SolidColorBrush(Color.FromRgb(218, 52, 174));
                triangleForwardLeft.Stroke = new SolidColorBrush(Color.FromRgb(240, 138, 212));
            }
            if (_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.D))
            {
                var triangleBackwardRight = (Path)btnBackwardRight.Template.FindName("triangleBackwardRight", btnBackwardRight);
                triangleBackwardRight.Fill = new SolidColorBrush(Color.FromRgb(218, 52, 174));
                triangleBackwardRight.Stroke = new SolidColorBrush(Color.FromRgb(240, 138, 212));
            }
            if (_pressedKeys.Contains(Key.S) && _pressedKeys.Contains(Key.A))
            {
                var triangleBackwardLeft = (Path)btnBackwardLeft.Template.FindName("triangleBackwardLeft", btnBackwardLeft);
                triangleBackwardLeft.Fill = new SolidColorBrush(Color.FromRgb(218, 52, 174));
                triangleBackwardLeft.Stroke = new SolidColorBrush(Color.FromRgb(240, 138, 212));
            }
            if (_pressedKeys.Contains(Key.W) && _pressedKeys.Count == 1)
            {
                var triangleForward = (Path)btnForward.Template.FindName("triangleForward", btnForward);
                triangleForward.Fill = new SolidColorBrush(Color.FromRgb(70, 42, 216));
                triangleForward.Stroke = new SolidColorBrush(Color.FromRgb(40, 174, 237));
            }
            if (_pressedKeys.Contains(Key.S) && _pressedKeys.Count == 1)
            {
                var triangleBackward = (Path)btnBackward.Template.FindName("triangleBackward", btnBackward);
                triangleBackward.Fill = new SolidColorBrush(Color.FromRgb(70, 42, 216));
                triangleBackward.Stroke = new SolidColorBrush(Color.FromRgb(40, 174, 237));
            }
            if (_pressedKeys.Contains(Key.Space))
            {
                btnMusic.Background = new SolidColorBrush(Color.FromRgb(218, 52, 174));
                btnMusic.BorderBrush = new SolidColorBrush(Color.FromRgb(240, 138, 212));
                spacePressed = false;
            }

            if (e.Key == Key.W || e.Key == Key.A || e.Key == Key.S || e.Key == Key.D || e.Key == Key.Space)
            {
                _pressedKeys.Remove(e.Key);
            }
        }


        private void btnRetourMenuConnection_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
