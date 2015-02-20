﻿namespace Specification.Halp
{
    using System.Threading.Tasks;
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
            Task openTask = channel.Open();
            openTask.Wait(5000);
            return channel.RealmProxy;
        }
    }
}