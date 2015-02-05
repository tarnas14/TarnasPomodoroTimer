namespace PomodoroServer
{
    using System;
    using System.Threading.Tasks;
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
