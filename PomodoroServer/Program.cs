namespace PomodoroServer
{
    using System;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using Pomodoro.Timer;
    using Pomodoro.Wamp;
    using Pomodoro.Wamp.Server;
    using WampSharp.V2;
    using WampSharp.V2.Realm;

    class Program
    {
        static void Main(string[] args)
        {
            const string location = "ws://127.0.0.1:8080/";

            using (IWampHost host = new DefaultWampHost(location))
            {
                PomodoroService instance = new DefaultPomodoroService();

                IWampHostedRealm realm = host.RealmContainer.GetRealmByName("tarnasRealm");

                ISubject<IntervalStartedEventArgs> subject =
                    realm.Services.GetSubject<IntervalStartedEventArgs>("com.tarnas.pomodoro");

                Task registrationTask = realm.Services.RegisterCallee(instance);
                // await registrationTask;
                registrationTask.Wait();

                host.Open();

                Console.WriteLine("Server is running on " + location);
                Console.ReadLine();
            }
        }
    }
}
