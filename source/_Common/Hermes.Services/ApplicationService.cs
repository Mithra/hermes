using System;
using System.Collections.Generic;
using System.Linq;
using Hermes.DataObjects.Application;
using Hermes.DataObjects.Channel;
using Hermes.Entity;
using Hermes.Entity.Models;
using Hermes.Services.Helpers;
using Hermes.Services.Helpers.Logging;

namespace Hermes.Services
{
    public class ApplicationService
    {
        public ApplicationDto GetApplication(AppLogger logger, int applicationId)
        {
            using (HermesContext db = new HermesContext())
            {
                Application application = db.Applications.FirstOrDefault(a => a.application_id == applicationId);
                if (application == null)
                    throw new Exception("Application not found: " + applicationId);

                return application.ToDto();
            }
        }

        public List<ApplicationDto> GetApplications(AppLogger logger)
        {
            using (HermesContext db = new HermesContext())
                return db.Applications.AsEnumerable().Select(a => a.ToDto()).ToList();
        }

        public List<ChannelDto> GetApplicationChannels(AppLogger logger, int applicationId)
        {
            using (HermesContext db = new HermesContext())
            {
                List<Channel> channels = db.Channels.Where(c => c.application_id == applicationId).ToList();
                return channels.Select(c => c.ToDto()).ToList();
            }
        }
    }
}
