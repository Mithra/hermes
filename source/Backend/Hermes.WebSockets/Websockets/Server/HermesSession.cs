using System;
using Newtonsoft.Json;
using SuperSocket.SocketBase;

namespace Hermes.WebSockets.Websockets.Server
{
    public class HermesSession : SuperSocket.WebSocket.WebSocketSession<HermesSession>
    {
        public HermesServer HermesServer {  get {  return (HermesServer)AppServer; } }

        protected override void OnSessionStarted()
        {
        }

        protected override void HandleException(Exception e)
        {
            Send("Application error: {0}", e.Message);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            // Unregister session from channels
            HermesServer.UnregisterClient(SessionID);

            base.OnSessionClosed(reason);
        }

        public void Send<T>(EAnswerType responseType, T data)
        {
            Send(JsonConvert.SerializeObject(new BaseAnswer<T>(responseType, data), Formatting.None));
        }
    }
}