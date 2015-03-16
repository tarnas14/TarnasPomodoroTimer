namespace Specification.Halp
{
    using WampSharp.V2;
    using WampSharp.V2.Client;

    static class WampHostHelper
    {
        public static DefaultWampHost GetHost(string location)
        {
            return new DefaultWampHost(location);
        }

        public static IWampRealmProxy GetNewProxy(string serverAddress, string realmName)
        {
            var channelFactory = new DefaultWampChannelFactory();
            IWampChannel channel = channelFactory.CreateJsonChannel(serverAddress, realmName);
            var task = channel.Open();
            task.Wait(5000);
            return channel.RealmProxy;
        }
    }
}
