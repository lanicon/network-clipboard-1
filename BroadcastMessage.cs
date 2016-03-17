using System;

namespace NetworkClipboard
{
    public enum BroadcastMessageType
    {
        Paste,
        HistoryRequest,
        HistoryReply
    }

    [Serializable]
    public class BroadcastMessage
    {
        public int UserId { get; set; }
        public string Channel { get; set; }
        public DateTime Timestamp { get; set; }
        public BroadcastMessageType MessageType { get; set; }
        public byte[] Body { get; set; }
        public int PacketId { get; set; }

        public BroadcastMessage()
        {
            Channel = "";
            Body = new byte[0];
            Timestamp = DateTime.Now;
            PacketId = new Random().Next();
        }
    }
}

