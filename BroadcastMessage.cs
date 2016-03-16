using System;

namespace NetworkClipboard
{
    [Serializable]
    public class BroadcastMessage
    {
        public int UserId { get; set; }
        public string Channel { get; set; }
        public string Body { get; set; }
        public byte PacketId
        {
            get
            {
                return packetId;
            }
        }

        private static byte packetId = 0;

        public BroadcastMessage(int id, string channel, string body)
        {
            UserId = id;
            Channel = channel;
            Body = body;

            packetId++;
            if (packetId == Byte.MaxValue)
            {
                packetId = 0;
            }
        }
    }
}

