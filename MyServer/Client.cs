using MyServer.Net.IO;
using System;
using System.Net.Sockets;

namespace MyServer
{
    public class Client
    {
        public string Username {  get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader packetReader {  get; set; }
        public Client(TcpClient client) 
        { 
            ClientSocket = client;
            UID = Guid.NewGuid();
            packetReader = new(ClientSocket.GetStream());

            var opcode = packetReader.ReadByte();
            Username = packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: " +
                $"Пользователь с ником {Username} подключился");

            Task.Run(() => Process());
        }

        void Process()
        {
            while(true)
            {
                try
                {
                    var opcode = packetReader.ReadByte();
                    switch(opcode)
                    {
                        case 5:
                            var msg = packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Sending Message: {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            break;
                        default: 
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{UID.ToString()}]: Disconnected.");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
