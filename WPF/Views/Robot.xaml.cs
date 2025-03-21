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
            else if (_pressedKeys.Count == 0)
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

            ReseauEchange.ReadTimeout = 1500;
            int bytesRead = ReseauEchange.Read(response, 0, response.Length);

            string message = Encoding.ASCII.GetString(response, 0, bytesRead);

            if (message != "ok")
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

            if (e.Key == Key.W)
            {
                var triangleForward = (Path)btnForward.Template.FindName("triangleForward", btnForward);
                triangleForward.Fill = new SolidColorBrush(Color.FromRgb(40, 174, 237));
                triangleForward.Stroke = new SolidColorBrush(Color.FromRgb(70, 42, 216));
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W || e.Key == Key.A || e.Key == Key.S || e.Key == Key.D || e.Key == Key.Space)
            {
                _pressedKeys.Remove(e.Key);
            }

            if (e.Key == Key.W)
            {
                var triangleForward = (Path)btnForward.Template.FindName("triangleForward", btnForward);
                triangleForward.Fill = new SolidColorBrush(Color.FromRgb(70, 42, 216));
                triangleForward.Stroke = new SolidColorBrush(Color.FromRgb(40, 174, 237));
            }

            if (e.Key == Key.Space)
            {
                spacePressed = false;
            }
        }


        private void btnRetourMenuConnection_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
