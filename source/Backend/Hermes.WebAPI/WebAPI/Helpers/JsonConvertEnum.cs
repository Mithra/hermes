using System;
using System.Collections.Generic;
using Nancy.Json;

namespace Hermes.WebAPI.WebAPI.Helpers
{
    public class JsonConvertEnum : JavaScriptPrimitiveConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                yield return typeof(Enum);
            }
        }

        public override object Deserialize(object primitiveValue, Type type, JavaScriptSerializer serializer)
        {
            // ERegion is serialized as int not a string
            // (string)primitiveValue won't work
            return !type.IsEnum ? null : Enum.Parse(type, primitiveValue.ToString());
        }

        public override object Serialize(object obj, JavaScriptSerializer serializer)
        {
            return !obj.GetType().IsEnum ? null : obj.ToString();
        }
    }
}
