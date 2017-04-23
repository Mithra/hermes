using System;
using System.Collections.Generic;
using System.Net;
using Hermes.Client.Common;
using Hermes.DataObjects;
using Hermes.DataObjects.Application;
using Hermes.DataObjects.Channel;
using Hermes.DataObjects.Notification;
using Newtonsoft.Json;
using RestSharp;

namespace Hermes.Client
{
    public class HermesApiClient : IHermesApiClient
    {
        private readonly RestClient _client;

        public HermesApiClient(string endPoint, int timeout = 1)
        {
            endPoint = !endPoint.EndsWith("/") ? endPoint + '/' : endPoint;

            _client = new RestClient(endPoint)
            {
                Timeout = timeout * 60 * 1000,
                ReadWriteTimeout = timeout * 60 * 1000
            };
        }

        public ApplicationDto GetApplication(int applicationId)
        {
            return GetData<ApplicationDto>(Method.GET, "application/{applicationId}", new Dictionary<string, string>
            {
                {"applicationId", applicationId.ToString()}
            });
        }

        public List<ApplicationDto> GetApplications()
        {
            return GetData<List<ApplicationDto>>(Method.GET, "application");
        }

        public ChannelDto GetChannel(int channelId)
        {
            return GetData<ChannelDto>(Method.GET, "channel/{channelId}", new Dictionary<string, string>
            {
                {"channelId", channelId.ToString()}
            });
        }

        public List<ChannelDto> GetChannels()
        {
            return GetData<List<ChannelDto>>(Method.GET, "channel");
        }

        public List<ChannelDto> GetApplicationChannels(int applicationId)
        {
            return GetData<List<ChannelDto>>(Method.GET, "application/{applicationId}/channels", new Dictionary<string, string>
            {
                {"applicationId", applicationId.ToString()}
            });
        }

        public long CreateChannel(ChannelCreationDto channel)
        {
            return GetData<ChannelCreationResultDto>(Method.POST, "channel", null, null, channel).ChannelId;
        }

        //public void DeleteChannel(int channelId)
        //{
        //    Execute(Method.DELETE, "channel/{channelId}", new Dictionary<string, string>
        //    {
        //        {"channelId", channelId.ToString()}
        //    });
        //}

        public long? PushNotification(NotificationCreationDto notification)
        {
            return GetData<NotificationCreationResultDto>(Method.POST, "notification", null, null, notification).NotificationId;
        }

        public void AknowledgeNotification(long notificationId, Guid clientId)
        {
            Execute(Method.PUT, "notification/{notificationId}/aknowledge", new Dictionary<string, string>
            {
                {"notificationId", notificationId.ToString()}
            }, new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("clientId", clientId)
            });
        }

        #region Helpers
        protected T GetData<T>(Method method, string call, Dictionary<string, string> urlSegments = null, List<KeyValuePair<string, object>> urlParameters = null, object body = null) where T : new()
        {
            RestRequest request = GetRestRequest(method, call, urlSegments, urlParameters, body);

            IRestResponse restResponse = _client.Execute(request);

            if (restResponse.StatusCode != HttpStatusCode.OK)
            {
                HermesApiExceptionDto vaultFeedsApiExceptionDto = null;
                try
                {
                    vaultFeedsApiExceptionDto = JsonConvert.DeserializeObject<HermesApiExceptionDto>(restResponse.Content);
                }
                catch (Exception serializationException)
                {
                    // ignored
                }
                if (vaultFeedsApiExceptionDto != null)
                    throw new HermesApiException(vaultFeedsApiExceptionDto.Message, vaultFeedsApiExceptionDto.StackTrace);

                throw new HermesApiException(!String.IsNullOrEmpty(restResponse.Content) ? restResponse.Content : restResponse.StatusDescription, null, restResponse.ErrorException);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(restResponse.Content);
            }
            catch (Exception serializationException)
            {
                throw new HermesApiException("Unable to deserialize API response to desired type: " + serializationException.Message);
            }
        }

        protected void Execute(Method method, string call, Dictionary<string, string> urlSegments = null, List<KeyValuePair<string, object>> urlParameters = null, object body = null)
        {
            RestRequest request = GetRestRequest(method, call, urlSegments, urlParameters, body);

            IRestResponse restResponse = _client.Execute(request);
            if (restResponse.StatusCode != HttpStatusCode.OK)
                throw new HermesApiException(!String.IsNullOrEmpty(restResponse.Content) ? restResponse.Content : restResponse.StatusDescription, null, restResponse.ErrorException);
        }

        private static RestRequest GetRestRequest(Method method, string call, Dictionary<string, string> urlSegments, List<KeyValuePair<string, object>> urlParameters = null, object body = null)
        {
            var request = new RestRequest(call, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new RestSharpJsonNetSerializer()
            };

            if (urlSegments != null)
                foreach (KeyValuePair<string, string> urlParameter in urlSegments)
                    request.AddUrlSegment(urlParameter.Key, urlParameter.Value);

            if (urlParameters != null)
                foreach (KeyValuePair<string, object> urlParameter in urlParameters)
                    request.AddParameter(urlParameter.Key, urlParameter.Value);

            if (body != null)
                request.AddBody(body);

            return request;
        }
        #endregion
    }
}
