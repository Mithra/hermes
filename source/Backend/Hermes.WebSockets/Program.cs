using Hermes.Services;
using Topshelf;

namespace Hermes.WebSockets
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings.LoadSettings();

            HostFactory.Run(x =>
            {
                x.Service<HermesWebSocketService>(s =>
                {
                    s.ConstructUsing(name => new HermesWebSocketService());
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
