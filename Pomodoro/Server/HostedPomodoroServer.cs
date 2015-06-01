namespace Pomodoro.Server
{
    using Configuration;
    using Timer;
    using WampSharp.V2;

    public class HostedPomodoroServer
    {
        private readonly PomodoroNotifier _pomodoroNotifier;
        private DefaultWampHost _host;
        private readonly PomodoroServerConfig _config;
        private PomodoroServer _server;

        public HostedPomodoroServer(PomodoroNotifier pomodoroNotifier, string configurationFile)
        {
            _pomodoroNotifier = pomodoroNotifier;
            _config = ConfigurationFactory.FromFile<PomodoroServerConfig>(configurationFile);
        }

        public void StartServer()
        {
            if (_host != null)
            {
                return;
            }

            _host = new DefaultWampHost(_config.Server);
            _server = new PomodoroServer(_host.RealmContainer.GetRealmByName(_config.RealmName), _pomodoroNotifier);
            _host.Open();
        }

        public void StopServer()
        {
            if (_host == null)
            {
                return;
            }

            _server.Stop();
            _server = null;
            _host.Dispose();
            _host = null;
        }
    }
}
