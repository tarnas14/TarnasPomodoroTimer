namespace PomodoroClient
{
    using Pomodoro;
    using Pomodoro.Wamp.Server;
    using WampSharp.V2;
    using WampSharp.V2.Realm;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            DefaultWampChannelFactory factory =
                            new DefaultWampChannelFactory();

            const string serverAddress = "ws://127.0.0.1:8080/ws";

            IWampChannel channel =
                factory.CreateJsonChannel(serverAddress, "tarnasRealm");

            channel.Open().Wait(5000);

            PomodoroService proxy =
                channel.RealmProxy.Services.GetCalleeProxy<PomodoroService>();

            Console.ReadLine();

            var pomodoroIdentifier = proxy.SetupNewPomodoro(new PomodoroConfig
            {
                Productivity = TimeSpan.FromMinutes(25),
                LongBreak = TimeSpan.FromMinutes(20),
                ShortBreak = TimeSpan.FromMinutes(5),
                LongBreakAfter = 4
            });
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
