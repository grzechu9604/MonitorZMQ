using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Monitor.Serialization
{
    public static class BinarySerializator<T>
    {
        public static byte[] ToByteArray(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T ToObject(byte[] bytes)
        {
            if (bytes == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}
