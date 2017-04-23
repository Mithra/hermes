using Hermes.DataObjects.Channel;
using Hermes.Services;

namespace Hermes.WebAPI.WebAPI.Modules
{
    public class ChannelsModule : CommonModule
    {
        private readonly ChannelService _service;

        public ChannelsModule()
            : base("/channel")
        {
            _service = new ChannelService();

            // Get all channels
            Get[""] = x => CallWithResponse("get-channels", logger => _service.GetChannels(logger));

            // Get channel by ID
            Get["{channelId:int}"] = x => CallWithResponse("get-channel", logger => _service.GetChannel(logger, x.channelId));

            // Create new channel
            Post[""] = x => PostRequest<ChannelCreationDto>("create-channel", (logger, input) => _service.CreateChannel(logger, input));

            // Delete channel
            //Delete["{channelId:int}"] = x => CallWithStatus("delete-channel", logger => _service.DeleteChannel(logger, x.channelId));
        }
    }
}
