using Client.MVVM.Core;
using Client.Net;
using System;
using Client.Net.IO;
using System.Collections.ObjectModel;
using System.Windows;
using Client.MVVM.Model;

namespace Client.MVVM.ViewModel
{
    public class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }

        private Server server;

        public MainViewModel() 
        {
            Users = new();
            Messages = new();
            server = new();

            server.connectedEvent += UserConnected;
            server.messageReceivedEvent += MessageReceived;
            server.userDisconnectEvent += UserDisconnect;

            ConnectToServerCommand = new(o => server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
            SendMessageCommand = new(o => server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
        }

        private void UserDisconnect()
        {
            var uid = server.packetReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageReceived()
        {
            var message = server.packetReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }

        private void UserConnected()
        {
            UserModel user = new UserModel
            {
                Username = server.packetReader.ReadMessage(),
                UID = server.packetReader.ReadMessage(),
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
    }
}
