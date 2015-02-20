namespace PomodoroClient
{
    using ConsoleApp;
    using Pomodoro;
    using Pomodoro.Timer;
    using Pomodoro.Wamp.Server;
    using WampSharp.V2;
    using WampSharp.V2.Client;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var factory =
                            new DefaultWampChannelFactory();

            const string serverAddress = "ws://127.0.0.1:8080/ws";

            var channel =
                factory.CreateMsgpackChannel(serverAddress, "tarnasPomodoroRealm");

            channel.Open().Wait(5000);

            var proxy =
                channel.RealmProxy.Services.GetCalleeProxy<PomodoroService>();

            Console.ReadLine();

            var pomodoroConfig = new PomodoroConfig
            {
                Productivity = TimeSpan.FromMinutes(25),
                LongBreak = TimeSpan.FromMinutes(20),
                ShortBreak = TimeSpan.FromMinutes(5),
                LongBreakAfter = 4
            };

            var pomodoroIdentifier = proxy.SetupNewPomodoro(pomodoroConfig);

            var pomodoroProxy = new RemotePomodoroNotifier(pomodoroIdentifier, channel.RealmProxy);

            var ui = new Ui(pomodoroConfig);
            ui.Subscribe(pomodoroProxy);

            Console.ReadLine();

            proxy.StartNext(pomodoroIdentifier);
            Console.ReadLine();

            proxy.Interrupt(pomodoroIdentifier);
            Console.ReadLine();

            proxy.Restart(pomodoroIdentifier);
            Console.ReadLine();

            proxy.Interrupt(pomodoroIdentifier);
            Console.ReadLine();

            proxy.StartNext(pomodoroIdentifier);
            Console.ReadLine();
        }
    }
}
