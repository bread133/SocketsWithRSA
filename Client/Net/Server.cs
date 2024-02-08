using Client.Net.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Net
{
    public class Server
    {
        TcpClient tcpClient;
        PacketBuilder packetBuilder;
        public PacketReader packetReader;

        public event Action connectedEvent;
        public event Action messageReceivedEvent;
        public event Action userDisconnectEvent;
        public Server() 
        {
            tcpClient = new();
        }

        public void ConnectToServer(string username)
        {
            try
            {
                if (!tcpClient.Connected)
                {
                    tcpClient.Connect("127.0.0.1", 7891);
                    packetReader = new(tcpClient.GetStream());

                    if (!string.IsNullOrEmpty(username))
                    {
                        PacketBuilder connectPacket = new();

                        connectPacket.WriteOpCode(0);
                        connectPacket.WriteMessage(username);
                        tcpClient.Client.Send(connectPacket.GetPacketBytes());
                    }
                    ReadPackets();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = packetReader.ReadByte();
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

        public void SendMessageToServer(string message)
        {
            PacketBuilder messagePacket = new();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            tcpClient.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
