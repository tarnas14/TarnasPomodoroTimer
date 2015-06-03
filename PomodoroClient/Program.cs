namespace PomodoroClient
{
    using System;
    using ConsoleApp;
    using Pomodoro.Client;
    using Pomodoro.Configuration;
    using Pomodoro.Server;
    using WampSharp.V2;

    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }

        private void Run()
        {
            try
            {
                var config = ConfigurationFactory.FromFile<PomodoroServerConfig>("serverConfig.json");
                var channelFactory = new DefaultWampChannelFactory();
                IWampChannel channel = channelFactory.CreateJsonChannel(config.Server, config.RealmName);
                var task = channel.Open();
                task.Wait(5000);
                var realmProxy = channel.RealmProxy;
                var client = new RemotePomodoroClient(realmProxy);

                channel.RealmProxy.Monitor.ConnectionBroken += (sender, args) => Console.WriteLine("connection broken");
                channel.RealmProxy.Monitor.ConnectionEstablished += (sender, args) => Console.WriteLine("connection established");
                channel.RealmProxy.Monitor.ConnectionError += (sender, args) => Console.WriteLine("connection error");

                var ui = new Ui();
                ui.Subscribe(client);

                while (Console.ReadLine() != "q");
                channel.Close();
                client.Dispose();
            }
            catch (AggregateException e)
            {
                foreach (var innerException in e.InnerExceptions)
                {
                    Console.WriteLine(innerException.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
