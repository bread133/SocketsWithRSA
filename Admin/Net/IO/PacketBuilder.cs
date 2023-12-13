using System;
using System.IO;
using System.Text;

namespace Admin.Net.IO
{
    public class PacketBuilder
    {
        MemoryStream ms;
        public PacketBuilder() 
        {
            ms = new();

        }

        public void WriteOpCode(byte opcode)
        {
            ms.WriteByte(opcode);
        }

        public void WriteMessage(string msg)
        {
            var msgLength = msg.Length;
            ms.Write(BitConverter.GetBytes(msgLength));
            ms.Write(Encoding.UTF8.GetBytes(msg));
        }

        public byte[] GetPacketBytes() =>
            ms.ToArray();
    }
}
