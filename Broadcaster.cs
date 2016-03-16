using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkClipboard
{
    public class Broadcaster
    {
        public event Action<string> NewPaste;
        public event EventHandler<EventArgs> SocketClosed;

        private const int APP_PORT = 45454;

        public string Channel { get; set; }
        public int MaxBufferSize
        {
            get
            {
                return Math.Min(
                    udpClient.Client.SendBufferSize,
                    udpClient.Client.ReceiveBufferSize);
            }
        }

        private UdpClient udpClient;
        private int userId;
        private byte lastPacketId;

        public Broadcaster()
        {
            userId = new Random().Next();
            Channel = "";

            udpClient = new UdpClient(APP_PORT);
            BeginReceive();
        }

        private async void BeginReceive()
        {
            try
            {
                UdpReceiveResult r = await udpClient.ReceiveAsync();
                ProcessDatagram(r.Buffer);
                BeginReceive();
            }
            catch (ObjectDisposedException)
            {
                if (SocketClosed != null)
                {
                    SocketClosed(this, EventArgs.Empty);
                }
            }
            catch (SocketException)
            {
                if (SocketClosed != null)
                {
                    SocketClosed(this, EventArgs.Empty);
                }
            }
        }

        private void ProcessDatagram(byte[] datagram)
        {
            if (datagram == null)
            {
                return;
            }

            try
            {
                BinaryFormatter f = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(datagram))
                {
                    BroadcastMessage i = f.Deserialize(ms) as BroadcastMessage;
                    if (NewPaste != null &&
                        i != null &&
                        i.PacketId != lastPacketId)
                    {
                        NewPaste(i.Body);
                    }

                    lastPacketId = i.PacketId;
                }
            }
            catch (SerializationException)
            {}
        }

        public void Paste(string text)
        {
            BroadcastMessage i = new BroadcastMessage(userId, Channel, text);

            BinaryFormatter f = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                f.Serialize(ms, i);
                byte[] bytes = ms.ToArray();
                IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, APP_PORT);
                udpClient.Send(bytes, bytes.Length, ep);
            }
        }
    }
}

