using System;

namespace NetworkClipboard
{
    [Serializable]
    public class NetworkClipboardItem
    {
        public int UserId { get; set; }
        public string Channel { get; set; }
        public string Body { get; set; }

        public NetworkClipboardItem(int id, string channel, string body)
        {
            UserId = id;
            Channel = channel;
            Body = body;
        }
    }
}

