using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Admin.Net.IO
{
    public class PacketReader : BinaryReader
    {
        NetworkStream ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            this.ns = ns;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            var lenght = ReadInt32();
            msgBuffer = new byte[lenght];
            ns.Read(msgBuffer, 0, lenght);

            var msg = Encoding.UTF8.GetString(msgBuffer);

            return msg;
        }
    }
}
