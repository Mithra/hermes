using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hermes.WebSockets.Websockets
{
    public enum EAnswerType
    {
        NewNotification 
    }

    public class BaseAnswer<T>
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public EAnswerType Type { get; set; }

        public T Data { get; set; }

        public BaseAnswer(EAnswerType type, T data)
        {
            Type = type;
            Data = data;
        }
    }
}
