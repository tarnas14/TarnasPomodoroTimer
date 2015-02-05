namespace Specification.Wamp
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using WampSharp.V2;
    using WampSharp.V2.Client;
    using WampSharp.V2.Core.Contracts;

    class WampTests
    {
        private const string ServerAddress = "ws://127.0.0.1:8080/ws";
        private DefaultWampHost _host;

        [SetUp]
        public void Setup()
        {
            _host = new DefaultWampHost(ServerAddress);

            _host.Open();
        }

        [TearDown]
        public void Teardown()
        {
            _host.Dispose();
        }

        [Test]
        public void ClientShouldGetPublishedEvent()
        {
            //given
            const string realmName = "asdf";
            const string topicText = "qwer";

            var pubProxy = GetNewProxy(realmName);
            var pubSubject = pubProxy.Services.GetSubject<int>(topicText);

            const int expected = 2;

            var subProxy = GetNewProxy(realmName);
            subProxy.Services.GetSubject<int>(topicText).Subscribe(x => Assert.That(x, Is.EqualTo(expected)));

            //when
            pubSubject.OnNext(expected);
        }

        [Test]
        public void MultipleTypeSubjectsInTheSameTopicShouldWork()
        {
            //given
            const string realmName = "asdf";
            const string topicText = "qwer";

            var pubProxy = GetNewProxy(realmName);
            var pubIntSubject = pubProxy.Services.GetSubject<int>(topicText);
            var pubStringSubject = pubProxy.Services.GetSubject<string>(topicText);

            const string expected = "testtest";

            var subProxy = GetNewProxy(realmName);
            subProxy.Services.GetSubject<int>(topicText).Subscribe(x => Assert.Fail(x.ToString()));
            subProxy.Services.GetSubject<string>(topicText).Subscribe(x => Assert.That(x, Is.EqualTo(expected)));

            //when
            pubStringSubject.OnNext(expected);
        }

        [Test]
        [ExpectedException(typeof(WampException))]
        public void AddingSubjectsAfterHostIsOpenIsNotPossible()
        {
            //given
            const string realmName = "asdf'";
            const string topicText = "qawer";
            const int expected = 2;

            var hostRealm = _host.RealmContainer.GetRealmByName(realmName);
            var clientProxy = GetNewProxy(realmName);

            clientProxy.Services.GetSubject<int>(topicText).Subscribe(x => Assert.That(x, Is.EqualTo(expected)));

            var hostSubject = hostRealm.Services.GetSubject<int>(topicText);

            //when
            hostSubject.OnNext(expected);
        }

        [Test]
        public void AddingSubjectsToTopicsAfterOtherClientAlreadySubscribed()
        {
            //given
            const string realmName = "asdf'";
            const string topicText = "qawer";
            const int expected = 2;

            var pubProxy = GetNewProxy(realmName);
            var subProxy = GetNewProxy(realmName);

            subProxy.Services.GetSubject<int>(topicText).Subscribe(x => Assert.That(x, Is.EqualTo(expected)));
            var pubSubject = pubProxy.Services.GetSubject<int>(topicText);

            //when
            pubSubject.OnNext(expected);
        }

        private static IWampRealmProxy GetNewProxy(string realmName)
        {
            DefaultWampChannelFactory channelFactory = new DefaultWampChannelFactory();
            IWampChannel channel = channelFactory.CreateJsonChannel(ServerAddress, realmName);
            Task openTask = channel.Open();
            openTask.Wait(5000);
            return channel.RealmProxy;
        }
    }
}
