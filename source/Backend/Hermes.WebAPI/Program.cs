using Hermes.Services;
using Topshelf;

namespace Hermes.WebAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings.LoadSettings();

            HostFactory.Run(x =>
            {
                x.Service<HermesWebAPIService>(s =>
                {
                    s.ConstructUsing(name => new HermesWebAPIService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription(Settings.ServiceDescription);
                x.SetDisplayName(Settings.ServiceDisplayName);
                x.SetServiceName(Settings.ServiceName);
            });
        }
    }
}
