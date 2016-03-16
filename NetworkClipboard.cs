using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkClipboard
{
    public class NetworkClipboard
    {
        public event Action<string> NewPaste;
        public event EventHandler<EventArgs> SocketClosed;

        private const int APP_PORT = 45454;

        public string Channel { get; set; }

        private UdpClient udpClient;
        private int userId;

        public NetworkClipboard()
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
                    NetworkClipboardItem i = f.Deserialize(ms) as NetworkClipboardItem;
                    if (NewPaste != null && i != null)
                    {
                        NewPaste(i.Body);
                    }
                }
            }
            catch (SerializationException)
            {}
        }

        public void Paste(string text)
        {
            NetworkClipboardItem i = new NetworkClipboardItem(userId, Channel, text);

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

