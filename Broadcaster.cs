using System;
using System.Text;
using System.Net;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkClipboard
{
    public class Broadcaster
    {
        public event Action<string> NewPaste;

        private event EventHandler<EventArgs> NewMessage;

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
        private BroadcastMessageFactory msgFactory;
        private ConcurrentQueue<BroadcastMessage> msgQueue;
        private AutoResetEvent newMessageWaitHandle;

        public Broadcaster()
        {
            userId = new Random().Next();
            Channel = "";
            msgFactory = new BroadcastMessageFactory(userId);
            msgQueue = new ConcurrentQueue<BroadcastMessage>();

            NewMessage += Broadcaster_NewMessage;

            udpClient = new UdpClient(APP_PORT);
            udpClient.EnableBroadcast = true;
        }

        private void Broadcaster_NewMessage (object sender, EventArgs e)
        {
            if (newMessageWaitHandle != null)
            {
                newMessageWaitHandle.Set();
            }

            BroadcastMessage newMsg;
            if (msgQueue.TryPeek(out newMsg))
            {
                if (newMsg.MessageType == BroadcastMessageType.Paste)
                {
                    if (msgQueue.TryDequeue(out newMsg) &&
                        NewPaste != null)
                    {
                        NewPaste(Encoding.UTF8.GetString(newMsg.Body));
                    }
                }
                else if (newMsg.MessageType == BroadcastMessageType.HistoryRequest)
                {
                    System.Diagnostics.Debug.WriteLine("history");
                }
            }
        }

        private void FormatAndSend(BroadcastMessage msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException();
            }

            BinaryFormatter f = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                f.Serialize(ms, msg);
                byte[] bytes = ms.ToArray();
                IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, APP_PORT);
                int result = udpClient.SendAsync(bytes, bytes.Length, ep).Result;
            }
        }

        public async Task<string> RequestHistory(string channel)
        {
            BroadcastMessage r = msgFactory.CreateHistoryRequest(channel);
            FormatAndSend(r);

            newMessageWaitHandle = new AutoResetEvent(false);

            return await Task<string>.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    newMessageWaitHandle.WaitOne(500);
                    BroadcastMessage newMsg;
                    if (msgQueue.TryPeek(out newMsg) &&
                    newMsg.MessageType == BroadcastMessageType.HistoryReply)
                    {
                        return Encoding.UTF8.GetString(newMsg.Body);
                    }
                    newMessageWaitHandle.Reset();
                }

                return "";
            });
        }

        public void Paste(string text)
        {
            BroadcastMessage i = msgFactory.CreatePasteMessage(Channel, text);
            FormatAndSend(i);
        }
    }
}

