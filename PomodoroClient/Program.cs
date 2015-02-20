namespace PomodoroClient
{
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
            DefaultWampChannelFactory factory =
                            new DefaultWampChannelFactory();

            const string serverAddress = "ws://127.0.0.1:8080/ws";

            IWampChannel channel =
                factory.CreateMsgpackChannel(serverAddress, "tarnasPomodoroRealm");

            channel.Open().Wait(5000);

            PomodoroService proxy =
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

            var remotePomodoro = new PomodoroProxy(pomodoroIdentifier, channel.RealmProxy);
            ;

            Console.ReadLine();

            SubscribeToPomodoro(pomodoroIdentifier, channel.RealmProxy);

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

        private static void SubscribeToPomodoro(PomodoroIdentifier pomodoroIdentifier, IWampRealmProxy realmProxy)
        {
            realmProxy.Services.GetSubject<IntervalInterruptedEventArgs>(pomodoroIdentifier.GetTopic(TopicType.interrupted)).Subscribe(Interrupted);

            realmProxy.Services.GetSubject<IntervalStartedEventArgs>(pomodoroIdentifier.GetTopic(TopicType.started)).Subscribe(Started);
        }

        private static void Interrupted(IntervalInterruptedEventArgs interrupted)
        {
            Console.WriteLine("interrupted {0}", interrupted.Id);
        }

        private static void Started(IntervalStartedEventArgs started)
        {
            Console.WriteLine("started {0}", started.Id);
        }
    }

    internal class PomodoroProxy : PomodoroNotifier
    {
        public PomodoroProxy(PomodoroIdentifier pomodoroIdentifier, IWampRealmProxy realmProxy)
        {
            SubscribeToTopics(pomodoroIdentifier, realmProxy);
        }

        private void SubscribeToTopics(PomodoroIdentifier pomodoroIdentifier, IWampRealmProxy realmProxy)
        {
            realmProxy.Services.GetSubject<IntervalInterruptedEventArgs>(pomodoroIdentifier.GetTopic(TopicType.interrupted)).Subscribe(Interrupted);

            realmProxy.Services.GetSubject<IntervalStartedEventArgs>(pomodoroIdentifier.GetTopic(TopicType.started)).Subscribe(Started);
        }

        private void Interrupted(IntervalInterruptedEventArgs interrupted)
        {
            if (IntervalInterrupted != null)
            {
                IntervalInterrupted(this, interrupted);
            }
        }

        private void Started(IntervalStartedEventArgs started)
        {
            if (IntervalStarted != null)
            {
                IntervalStarted(this, started);
            }
        }

        public event EventHandler<IntervalStartedEventArgs> IntervalStarted;
        public event EventHandler<IntervalInterruptedEventArgs> IntervalInterrupted;
        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;
        public event EventHandler<TimeRemainingEventArgs> Tick;
    }
}
