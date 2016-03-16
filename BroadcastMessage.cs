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
        public BroadcastMessageType MessageType { get; set; }
        public byte[] Body { get; set; }
        public byte PacketId { get; private set; }

        private static byte packetCount = 0;

        public BroadcastMessage()
        {
            Channel = "";
            Body = new byte[0];
            PacketId = packetCount;

            packetCount++;
            if (packetCount == Byte.MaxValue)
            {
                packetCount = 0;
            }
        }
    }
}

