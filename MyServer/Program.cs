using MyServer;
using MyServer.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace MyServer
{
    class Program
    {
        static List<Client> users;
        static TcpListener listener;

        static void Main(string[] args)
        {
            listener = new(IPAddress.Parse("127.0.0.1"), 7891);
            users = new();
            listener.Start();
            while (true)
            {
                var client = new Client(listener.AcceptTcpClient());
                users.Add(client);
                if(users.Count > 2)
                {
                    string message = "На сервере больше 2 человек, соединение прервано.";
                    Console.WriteLine(message);
                    BroadcastMessage(message);
                    return;
                }

                BroadcastConnection();
            }
        }

        // Отправка сообщений с сервера на клиент
        public static void BroadcastConnection()
        {
            foreach (var user in users)
            {
                foreach (var _user in users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(_user.Username);
                    broadcastPacket.WriteMessage(_user.UID.ToString());

                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach(var user in users)
            {
                var massagePacket = new PacketBuilder();
                massagePacket.WriteOpCode(5);
                massagePacket.WriteMessage(message);

                user.ClientSocket.Client.Send(massagePacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var DisconnectUser = users.Where(x => x.UID.ToString() == uid)
                .FirstOrDefault();
            users.Remove(DisconnectUser);
            foreach (var user in users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);

                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            BroadcastMessage($"[{DisconnectUser.Username}] Disconnected.");
        }
    }
}
