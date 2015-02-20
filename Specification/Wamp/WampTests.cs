namespace Specification.Wamp
{
    using System;
    using System.Threading.Tasks;
    using Halp;
    using NUnit.Framework;
    using WampSharp.V2;
    using WampSharp.V2.Client;
    using WampSharp.V2.Core.Contracts;

    [TestFixture]
    class WampTests : WampBaseTestFixture
    {
        [Test]
        public void ClientShouldGetPublishedEvent()
        {
            //given
            const string realmName = "asdf";
            const string topicText = "qwer";

            var pubProxy = WampHostHelper.GetNewProxy(ServerAddress, realmName);
            var pubSubject = pubProxy.Services.GetSubject<int>(topicText);

            const int expected = 2;

            var subProxy = WampHostHelper.GetNewProxy(ServerAddress, realmName);
            int actual = -1;
            subProxy.Services.GetSubject<int>(topicText).Subscribe(x => actual = x);

            //when
            pubSubject.OnNext(expected);

            //then
            WaitForExpected(ref actual, expected);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void SubjectSubscriptionsAreOverwritten()
        {
            //given
            const string realmName = "asdf";
            const string topicText = "qwer";

            var pubProxy = WampHostHelper.GetNewProxy(ServerAddress, realmName);
            var pubStringSubject = pubProxy.Services.GetSubject<string>(topicText);
            var pubIntSubject = pubProxy.Services.GetSubject<int>(topicText);

            const string expected = "testtest";
            const int expectedInt = 123;

            var subProxy = WampHostHelper.GetNewProxy(ServerAddress, realmName);
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

            var pubProxy = WampHostHelper.GetNewProxy(ServerAddress, realmName);
            var pubStringSubject = pubProxy.Services.GetSubject<string>(topicText);
            var pubIntSubject = pubProxy.Services.GetSubject<int>(topicText);

            const int expected = 2;

            var subProxy = WampHostHelper.GetNewProxy(ServerAddress, realmName);
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

            var pubProxy = WampHostHelper.GetNewProxy(ServerAddress, realmName);
            var subProxy = WampHostHelper.GetNewProxy(ServerAddress, realmName);

            int actual = -1;
            subProxy.Services.GetSubject<int>(topicText).Subscribe(x => actual = x);
            var pubSubject = pubProxy.Services.GetSubject<int>(topicText);

            //when
            pubSubject.OnNext(expected);

            //then
            WaitForExpected(ref actual, expected);
        }
    }
}
