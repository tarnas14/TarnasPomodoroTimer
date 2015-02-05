namespace PomodoroServer
{
    using System;
    using System.Threading.Tasks;
    using Pomodoro.Wamp.Server;
    using WampSharp.V2;
    using WampSharp.V2.Client;
    using WampSharp.V2.Realm;

    class Program
    {
        static void Main(string[] args)
        {
            const string location = "ws://127.0.0.1:8080/";
            const string realmName = "tarnasPomodoroRealm";

            using (IWampHost host = new DefaultWampHost(location))
            {
                var pomodoroServiceInstance = new DefaultPomodoroService();

                IWampHostedRealm hostRealm = host.RealmContainer.GetRealmByName(realmName);

                Task registrationTask = hostRealm.Services.RegisterCallee(pomodoroServiceInstance);
                // await registrationTask;
                registrationTask.Wait();

                host.Open();

                var proxyRealm = GetRealmProxy(location, realmName);

                var publisher = new PomodoroEventPublisher(proxyRealm);
                publisher.Subscribe(pomodoroServiceInstance);

                Console.WriteLine("Server is running on " + location);
                Console.ReadLine();
            }
        }

        private static IWampRealmProxy GetRealmProxy(string location, string realmName)
        {
            var channel = new DefaultWampChannelFactory().CreateMsgpackChannel(location, realmName);
            Task openTask = channel.Open();
            openTask.Wait(5000);
            return channel.RealmProxy;
        }
    }
}
