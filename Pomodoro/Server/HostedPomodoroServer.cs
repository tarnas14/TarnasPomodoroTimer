namespace Pomodoro.Server
{
    using System;
    using System.IO;
    using Configuration;
    using Timer;
    using WampSharp.V2;

    public class HostedPomodoroServer : IDisposable
    {
        private readonly PomodoroNotifier _pomodoroNotifier;
        private readonly DefaultWampHost _host;
        private readonly PomodoroServerConfig _config;
        private readonly PomodoroServer _server;

        public HostedPomodoroServer(PomodoroNotifier pomodoroNotifier, string configurationFile)
        {
            _pomodoroNotifier = pomodoroNotifier;
            _config = ConfigurationFactory.FromFile<PomodoroServerConfig>(configurationFile);
            _host = new DefaultWampHost(_config.Server);
            var realm = _host.RealmContainer.GetRealmByName(_config.RealmName);
            _server = new PomodoroServer(realm, _pomodoroNotifier);
            OpenServerWithoutPrintingToConsole();
        }

        private void OpenServerWithoutPrintingToConsole()
        {
            var tmpConsoleOut = Console.Out;
            Console.SetOut(TextWriter.Null);
            _host.Open();
            Console.SetOut(tmpConsoleOut);
        }

        public void StartServer()
        {
            _server.Start();
        }

        public void StopServer()
        {
            _server.Stop();
        }

        public void Dispose()
        {
            _server.Stop();
            _host.Dispose();
        }
    }
}
