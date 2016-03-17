using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace NetworkClipboard
{
    public class Listener
    {
        public event Action<BroadcastMessage, IPAddress> NewMessage;
        public event EventHandler<EventArgs> SocketClosed;

        public bool Listening { get; private set; }

        private UdpClient udpListener;
        private int lastPacketId;

        public Listener(int port)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
            udpListener = new UdpClient(ep);

            Listening = true;
            Receive();
        }

        private void ProcessDatagram(byte[] datagram, IPAddress source)
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
                    if (NewMessage != null &&
                        i != null &&
                        i.PacketId != lastPacketId)
                    {
                        NewMessage(i, source);
                    }

                    lastPacketId = i.PacketId;
                }
            }
            catch (SerializationException)
            {}
        }

        private async void Receive()
        {
            try
            {
                UdpReceiveResult r = await udpListener.ReceiveAsync();
                ProcessDatagram(r.Buffer, r.RemoteEndPoint.Address);
                Receive();
            }
            catch (ObjectDisposedException)
            {
                if (SocketClosed != null)
                {
                    SocketClosed(this, EventArgs.Empty);
                }

                Listening = false;
            }
            catch (SocketException)
            {
                if (SocketClosed != null)
                {
                    SocketClosed(this, EventArgs.Empty);
                }

                Listening = false;
            }
        }
    }
}

