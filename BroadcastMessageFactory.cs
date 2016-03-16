using System;
using System.Text;

namespace NetworkClipboard
{
    public class BroadcastMessageFactory
    {
        private int userId;

        public BroadcastMessageFactory(int userid)
        {
            userId = userid;
        }

        public BroadcastMessage CreatePasteMessage(string channel, string text)
        {
            BroadcastMessage output = new BroadcastMessage()
            {
                    UserId = userId,
                    Channel = channel,
                    MessageType = BroadcastMessageType.Paste,
                    Body = Encoding.UTF8.GetBytes(text)
            };

            return output;
        }

        public BroadcastMessage CreateHistoryRequest(string channel)
        {
            BroadcastMessage output = new BroadcastMessage()
            {
                    UserId = userId,
                    Channel = channel,
                    MessageType = BroadcastMessageType.HistoryRequest
            };

            return output;
        }
    }
}

