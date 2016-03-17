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
        private byte lastPacketId;
        private Task<UdpReceiveResult> receiveTask;
        private List<IPAddress> ownAddresses;

        public Listener(int port)
        {
            ownAddresses = GetOwnAddresses();
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
            udpListener = new UdpClient(ep);
        }

        public void Start()
        {
            Listening = true;
            Receive();
        }

        public void Stop()
        {
            if (Listening)
            {
                if (!receiveTask.IsCompleted)
                {
                    // throws and exception which cancels the task
                    udpListener.Close();
                }
                else
                {
                    Listening = false;
                }
            }
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
                        i.PacketId != lastPacketId &&
                        !ownAddresses.Contains(source))
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

        // TODO: make more robust, detect IP address changes
        private List<IPAddress> GetOwnAddresses()
        {
            List<IPAddress> output = new List<IPAddress>();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output.Add(ip.Address);
                        }
                    }
                }
            }

            return output;
        }
    }
}

