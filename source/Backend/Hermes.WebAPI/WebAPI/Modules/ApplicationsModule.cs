using Hermes.Services;

namespace Hermes.WebAPI.WebAPI.Modules
{
    public class ApplicationsModule : CommonModule
    {
        private readonly ApplicationService _service;

        public ApplicationsModule()
            : base("/application")
        {
            _service = new ApplicationService();

            // Get all applications
            Get[""] = x => CallWithResponse("create-notification", logger => _service.GetApplications(logger));

            // Get application by ID
            Get["{applicationId:int}"] = x => CallWithResponse("get-application", logger => _service.GetApplication(logger, x.applicationId));

            // Get all channels of a specific application
            Get["{applicationId:int}/channels"] = x => CallWithResponse("get-application-channels", logger => _service.GetApplicationChannels(logger, x.applicationId));
        }
    }
}
