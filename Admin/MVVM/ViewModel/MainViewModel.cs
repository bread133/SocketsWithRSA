using Admin.MVVM.Core;
using Admin.Net;
using System;
using Admin.Net.IO;
using Admin.MVVM.Model;
using System.Collections.ObjectModel;
using System.Windows;

namespace Admin.MVVM.ViewModel
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
            var uid = server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageReceived()
        {
            var message = server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));

        }

        private void UserConnected()
        {
            UserModel user = new UserModel
            {
                Username = server.PacketReader.ReadMessage(),
                UID = server.PacketReader.ReadMessage(),
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }

        private void removeMessage()
        {

        }
    }
}
