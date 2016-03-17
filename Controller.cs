using System;
using System.Text;
using System.Net;
using System.Collections.Generic;

namespace NetworkClipboard
{
    // TODO: change name!
    public class Controller
    {
        public event Action<string, DateTime, string> NewPaste;

        private Listener listener;
        public Dictionary<string, SortedList<DateTime, string>> Pastes;

        public Controller(Listener source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            Pastes = new Dictionary<string, SortedList<DateTime, string>>();

            listener = source;
            listener.NewMessage += Listener_NewMessage;
            listener.Start();
        }

        public void Paste(string channel, string text)
        {
            BroadcastMessage msg = BroadcastMessageFactory.CreatePasteMessage(channel, text);
            AddPaste(msg.Timestamp, channel, text);

            UdpMessenger.Broadcast(msg);
        }

        private void Listener_NewMessage(BroadcastMessage msg, IPAddress source)
        {
            switch (msg.MessageType)
            {
                case BroadcastMessageType.Paste:
                    ProcessPaste(msg);
                    break;
                case BroadcastMessageType.HistoryRequest:
                    ProcessHistoryRequest(msg, source);
                    break;
                case BroadcastMessageType.HistoryReply:
                    ProcessHistoryReply(msg);
                    break;
                default:
                    Console.Error.WriteLine(
                        "Skipping unknown broadcast message type: " +
                        msg.MessageType.ToString());
                    break;
            }
        }

        private void AddPaste(DateTime timestamp, string channel, string text)
        {
            if (!Pastes.ContainsKey(channel))
            {
                Pastes.Add(
                    channel,
                    new SortedList<DateTime, string>());
            }

            Pastes[channel].Add(
                timestamp, 
                text);

            if (NewPaste != null)
            {
                NewPaste(channel, timestamp, text);
            }
        }

        private void ProcessPaste(BroadcastMessage paste)
        {
            AddPaste(
                paste.Timestamp,
                paste.Channel,
                Encoding.UTF8.GetString(paste.Body));
        }

        private void ProcessHistoryRequest(BroadcastMessage request, IPAddress source)
        {
            if (!Pastes.ContainsKey(request.Channel))
            {
                return;
            }

            SortedList<DateTime, string> history = Pastes[request.Channel];
            BroadcastMessage reply = BroadcastMessageFactory.CreateHistoryReply(request.Channel, history);
            UdpMessenger.SendTo(reply, source);
        }

        private void ProcessHistoryReply(BroadcastMessage reply)
        {
            
        }
    }
}

