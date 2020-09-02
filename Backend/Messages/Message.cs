using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Backend.Messages
{
    [Serializable]
    public abstract class Message
    {
        public abstract MessageType MessageType { get; }

        public byte[] GetSerialized()
        {
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, this);
            return ms.ToArray();
        }

        public static T Parse<T>(byte[] data) where T : Message
        {
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream(data);
            return (T)bf.Deserialize(ms);
        }
    }
}