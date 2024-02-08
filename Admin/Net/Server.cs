using Admin.Net.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Admin.Net
{
    public class Server
    {
        TcpClient tcpClient;
        public PacketReader PacketReader;

        public event Action connectedEvent;
        public event Action messageReceivedEvent;
        public event Action userDisconnectEvent;

        public Server() 
        {
            tcpClient = new();
        }

        public void ConnectToServer(string username)
        {
            // запуск сервера
            StartServer();

            // подключение к серверу
            if (!tcpClient.Connected) 
            {
                tcpClient.Connect("127.0.0.1", 7891);
                PacketReader = new(tcpClient.GetStream());

                if(!string.IsNullOrEmpty(username))
                {
                    PacketBuilder connectPacket = new();

                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    tcpClient.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        public void StartServer()
        {
            try
            {
                Process p = new();
                p.StartInfo = new ProcessStartInfo(
                    @"../net8.0/MyServer.exe");
                p.Start();
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void ReadPackets()
        {
            try
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        var opcode = PacketReader.ReadByte();
                        switch (opcode)
                        {
                            case 1:
                                connectedEvent?.Invoke();
                                break;
                            case 5:
                                messageReceivedEvent?.Invoke();
                                break;
                            case 10:
                                userDisconnectEvent?.Invoke();
                                break;
                            default:
                                Console.WriteLine("ah yes...");
                                break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }

        public void SendMessageToServer(string message)
        {
            PacketBuilder messagePacket = new();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            tcpClient.Client.Send(messagePacket.GetPacketBytes());
            
        }
    }
}
