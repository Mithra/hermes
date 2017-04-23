using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Hermes.Services.Helpers
{
    public static class ProtoBufHelper
    {
        public static byte[] Serialize<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] body)
        {
            using (MemoryStream ms = new MemoryStream(body))
                return Serializer.Deserialize<T>(ms);
        }

        public static void RegisterType(Type type)
        {
            if (RuntimeTypeModel.Default.IsDefined(type))
                return;

            MetaType metaType = RuntimeTypeModel.Default.Add(type, false);

            int i = 1;
            foreach (PropertyInfo property in type.GetProperties().OrderBy(p => p.Name))
                metaType.Add(i++, property.Name);
        }
    }
}
