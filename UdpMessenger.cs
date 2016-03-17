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
            using (UdpClient udp = new UdpClient())
            {
                udp.EnableBroadcast = true;

                byte[] datagram;
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter f = new BinaryFormatter();
                    f.Serialize(ms, msg);
                    datagram = ms.ToArray();
                }

                IPEndPoint ep = new IPEndPoint(
                    dest,
                    Program.Config.Port);
                udp.SendAsync(datagram, datagram.Length, ep);
            }
        }
    }
}

