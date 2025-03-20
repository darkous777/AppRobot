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
            ReseauEchange= stream;

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
            else if (_pressedKeys.Contains(Key.Space))
                commandToSend = "music";
            else
                commandToSend = "stop";

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
            Application.Current.Shutdown();

        }

        private void EnvoyerEtRecevoirDonnees(string commande)
        {
            byte[] data = Encoding.ASCII.GetBytes(commande);
            ReseauEchange.Write(data, 0, data.Length);

            //byte[] response = new byte[1024];

            //int bytesRead = ReseauEchange.Read(response, 0, response.Length);

            //string message = Encoding.ASCII.GetString(response, 0, bytesRead);

            //if (message is not null)
            //{
            //    MessageBox.Show($"Voici le message du robot : {message}");
            //}
            //else
            //{
            //    MessageBox.Show($"Problème d'envoie de données vers le robot.");
            //}
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W || e.Key == Key.A || e.Key == Key.S || e.Key == Key.D || e.Key == Key.Space)
            {
                _pressedKeys.Add(e.Key);
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W || e.Key == Key.A || e.Key == Key.S || e.Key == Key.D || e.Key == Key.Space)
            {
                _pressedKeys.Remove(e.Key);
            }
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {

        }

        

        private void btnBackward_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRotationRight_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRotationLeft_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
