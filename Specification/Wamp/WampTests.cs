namespace Specification.Wamp
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using WampSharp.V2;
    using WampSharp.V2.Client;
    using WampSharp.V2.Core.Contracts;

    [TestFixture]
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
            int actual = -1;
            subProxy.Services.GetSubject<int>(topicText).Subscribe(x => actual = x);

            //when
            pubSubject.OnNext(expected);

            //then
            WaitForExpected(ref actual, expected);
            Assert.That(actual, Is.EqualTo(expected));
        }

        private void WaitForExpected<T>(ref T actual, T expected)
        {
            var start = DateTime.Now;
            while (!actual.Equals(expected))
            {
                if (DateTime.Now - start > TimeSpan.FromSeconds(2))
                {
                    return;
                }
            }
        }

        [Test]
        public void SubjectSubscriptionsAreOverwritten()
        {
            //given
            const string realmName = "asdf";
            const string topicText = "qwer";

            var pubProxy = GetNewProxy(realmName);
            var pubStringSubject = pubProxy.Services.GetSubject<string>(topicText);
            var pubIntSubject = pubProxy.Services.GetSubject<int>(topicText);

            const string expected = "testtest";
            const int expectedInt = 123;

            var subProxy = GetNewProxy(realmName);
            int actualInt = -1;
            string actual = string.Empty;

            subProxy.Services.GetSubject<string>(topicText).Subscribe(x => actual = x);
            subProxy.Services.GetSubject<int>(topicText).Subscribe(x => actualInt = x);

            //when
            pubIntSubject.OnNext(expectedInt);
            pubStringSubject.OnNext(expected);

            //then
            WaitForExpected(ref actual, expected);
            WaitForExpected(ref actualInt, expectedInt);
            Assert.That(actualInt, Is.EqualTo(expectedInt));
            Assert.That(string.IsNullOrEmpty(actual));
        }

        [Test]
        public void MultipleTypeSubjectsInTheSameTopicShouldWorkWhenCallingSubjectBoundFirst()
        {
            //given
            const string realmName = "asdf";
            const string topicText = "qwer";

            var pubProxy = GetNewProxy(realmName);
            var pubStringSubject = pubProxy.Services.GetSubject<string>(topicText);
            var pubIntSubject = pubProxy.Services.GetSubject<int>(topicText);

            const int expected = 2;

            var subProxy = GetNewProxy(realmName);
            int actual = -1;
            bool stringSubjectCalled = false;
            subProxy.Services.GetSubject<string>(topicText).Subscribe(x => stringSubjectCalled = true);
            subProxy.Services.GetSubject<int>(topicText).Subscribe(x => actual = x);

            //when
            pubIntSubject.OnNext(expected);

            //then
            WaitForExpected(ref actual, expected);
            Assert.That(actual, Is.EqualTo(expected));
            Assert.False(stringSubjectCalled);
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

            int actual = -1;
            subProxy.Services.GetSubject<int>(topicText).Subscribe(x => actual = x);
            var pubSubject = pubProxy.Services.GetSubject<int>(topicText);

            //when
            pubSubject.OnNext(expected);

            //then
            WaitForExpected(ref actual, expected);
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
