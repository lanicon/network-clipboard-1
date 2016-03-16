using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace NetworkClipboard
{
    public class Listener
    {
        public event Action<BroadcastMessage> NewMessage;
        public event EventHandler<EventArgs> SocketClosed;

        public bool Listening { get; private set; }

        private UdpClient udpListener;
        private byte lastPacketId;
        private Task<UdpReceiveResult> receiveTask;

        public Listener(int port)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
            udpListener = new UdpClient(ep);
        }

        public void Start()
        {
            Listening = true;
            Listen();
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

        private async void Listen()
        {
            await Task.Run()(() =>
            {
                while (Listening)
                {
                    receiveTask = BeginReceive();
                    receiveTask.ContinueWith((task) =>
                        {
                            if (task != null)
                            {
                                Task<UdpReceiveResult> udptask = task as Task<UdpReceiveResult>;
                                ProcessDatagram(udptask.Result.Buffer);
                            }
                        });
                }
            });
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
                    if (NewMessage != null &&
                        i != null &&
                        i.PacketId != lastPacketId)
                    {
                        NewMessage(i);
                    }

                    lastPacketId = i.PacketId;
                }
            }
            catch (SerializationException)
            {}
        }

        private async Task<UdpReceiveResult> BeginReceive()
        {
            try
            {
                return await udpListener.ReceiveAsync();
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

            return null;
        }
    }
}

