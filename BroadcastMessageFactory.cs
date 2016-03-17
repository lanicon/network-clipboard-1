using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkClipboard
{
    public static class BroadcastMessageFactory
    {

        public static BroadcastMessage CreatePasteMessage(string channel, string text)
        {
            BroadcastMessage output = new BroadcastMessage()
            {
                Channel = channel,
                MessageType = BroadcastMessageType.Paste,
                Body = Encoding.UTF8.GetBytes(text)
            };

            return output;
        }

        public static BroadcastMessage CreateHistoryRequest(string channel)
        {
            BroadcastMessage output = new BroadcastMessage()
            {
                Channel = channel,
                MessageType = BroadcastMessageType.HistoryRequest
            };

            return output;
        }

        public static BroadcastMessage CreateHistoryReply(string channel, SortedList<DateTime, string> history)
        {
            byte[] historyBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(ms, CompressionMode.Compress))
                {
                    BinaryFormatter f = new BinaryFormatter();
                    f.Serialize(gz, history);
                }

                historyBytes = ms.ToArray();
            }

            BroadcastMessage output = new BroadcastMessage()
            {
                    Channel = channel,
                    MessageType = BroadcastMessageType.HistoryReply,
                    Body = historyBytes
            };

            return output;
        }
    }
}

