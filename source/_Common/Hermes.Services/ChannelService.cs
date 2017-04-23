using System;
using System.Collections.Generic;
using System.Linq;
using Hermes.DataObjects.Channel;
using Hermes.Entity;
using Hermes.Entity.Models;
using Hermes.Services.Helpers;
using Hermes.Services.Helpers.Logging;

namespace Hermes.Services
{
    public class ChannelService
    {
        public ChannelDto GetChannel(AppLogger logger, int channelId)
        {
            using (HermesContext db = new HermesContext())
            {
                Channel channel = db.Channels.FirstOrDefault(c => c.channel_id == channelId);
                if (channel == null)
                    throw new Exception("Channel not found: " + channelId);

                return channel.ToDto();
            }
        }

        public List<ChannelDto> GetChannels(AppLogger logger)
        {
            using (HermesContext db = new HermesContext())
                return db.Channels.AsEnumerable().Select(c => c.ToDto()).ToList();
        }

        public ChannelCreationResultDto CreateChannel(AppLogger logger, ChannelCreationDto input)
        {
            using (HermesContext db = new HermesContext())
            {
                bool applicationAlreadyExists = db.Applications.Any(a => a.application_id == input.ApplicationId);
                if (!applicationAlreadyExists)
                    throw new Exception("Application not found: " + input.ApplicationId);

                bool channelAlreadyExists = db.Channels.Any(c => c.application_id == input.ApplicationId && c.channel_name.Equals(input.Name));
                if (channelAlreadyExists)
                    throw new Exception(String.Format("There is already a channel named '{0}' in application {1}", input.Name, input.ApplicationId));

                Channel channel = db.Channels.Add(new Channel
                {
                    application_id = input.ApplicationId,
                    channel_name = input.Name
                });
                db.SaveChanges();

                return new ChannelCreationResultDto {ChannelId = channel.channel_id};
            }
        }

        //public void DeleteChannel(CentralizedLogger logger, int channelId)
        //{
        //    using (HermesContext db = new HermesContext())
        //    using (DbContextTransaction transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            // Delete receipts
        //            db.Database.ExecuteSqlCommand(String.Format(@"
        //                delete r
        //                from notification_client_receipt r
        //                join notification n on r.notification_id = n.notification_id
        //                where n.channel_id = {0};
        //            ", channelId));

        //            // Delete notifications
        //            db.Database.ExecuteSqlCommand(String.Format(@"
        //                delete from notification where channel_id = {0};
        //            ", channelId));

        //            // Delete channel
        //            db.Database.ExecuteSqlCommand(String.Format(@"
        //                delete from channel where channel_id = {0};
        //            ", channelId));

        //            db.SaveChanges();

        //            transaction.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            transaction.Rollback();
        //        }
        //    }
        //}
    }
}