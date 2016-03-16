using System;
using System.Text;
using System.Collections.Generic;

namespace NetworkClipboard
{
    // TODO: change name!
    public class Controller
    {
        public event EventHandler<EventArgs> NewPaste;

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

        private void Listener_NewMessage(BroadcastMessage msg)
        {
            switch (msg.MessageType)
            {
                case BroadcastMessageType.Paste:
                    ProcessPaste(msg);
                    break;
                case BroadcastMessageType.HistoryRequest:
                    ProcessHistoryRequest(msg);
                    break;
                case BroadcastMessageType.HistoryReply:
                    ProcessHistoryReply(msg);
                    break;
                default:
                    Console.Error.WriteLine(
                        "Skipping unknown broadcast message type: " + 
                        msg.MessageType.ToString());
            }
        }

        private void ProcessPaste(BroadcastMessage paste)
        {
            if (!Pastes.ContainsKey(paste.Channel))
            {
                Pastes.Add(
                    paste.Channel,
                    new SortedList<DateTime, string>());
            }

            Pastes[paste.Channel].Add(
                paste.Timestap, 
                Encoding.UTF8.GetString(paste));
        }

        private void ProcessHistoryRequest(BroadcastMessage request)
        {
            
        }

        private void ProcessHistoryReply(BroadcastMessage reply)
        {
        }
    }
}

