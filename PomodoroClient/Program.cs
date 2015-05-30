namespace PomodoroClient
{
    using System;
    using ConsoleApp;
    using Pomodoro.Client;
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
            var channelFactory = new DefaultWampChannelFactory();
            IWampChannel channel = channelFactory.CreateJsonChannel(PomodoroServer.DefaultServer, PomodoroServer.DefaultRealm);
            var task = channel.Open();
            task.Wait(5000);
            var client = new RemotePomodoroClient(channel.RealmProxy);

            var ui = new Ui();
            ui.Subscribe(client);

            while (Console.ReadLine() != "q") ;
        }
    }
}
