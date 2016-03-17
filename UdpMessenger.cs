using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkClipboard
{
    public static class UdpMessenger
    {
        public static int MaxBufferSize
        {
            get
            {
                UdpClient u = new UdpClient();
                return Math.Min(
                    u.Client.SendBufferSize,
                    u.Client.ReceiveBufferSize);
            }
        }

        public static void Broadcast(BroadcastMessage msg)
        {
            SendTo(msg, IPAddress.Broadcast);
        }

        public static void SendTo(BroadcastMessage msg, IPAddress dest)
        {
            UdpClient udp = new UdpClient(Program.Config.Port);

            byte[] datagram;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter f = new BinaryFormatter();
                f.Serialize(ms, msg);
                datagram = ms.ToArray();
            }

            if (datagram.Length > Math.Min(
                udp.Client.SendBufferSize,
                udp.Client.ReceiveBufferSize))
            {
                Console.Error.WriteLine(
                    "Created a too large datagram. God what have I done. Bailing!");
                return;
            }

            IPEndPoint ep = new IPEndPoint(
                // well this is fucking broken
                IPAddress.Broadcast,
                Program.Config.Port);
            udp.Send(datagram, datagram.Length, ep);
        }
    }
}

