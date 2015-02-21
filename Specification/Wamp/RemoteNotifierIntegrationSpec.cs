namespace Specification.Wamp
{
    using Halp;
    using Moq;
    using NUnit.Framework;
    using Pomodoro.Timer;
    using Pomodoro.Wamp.Server;
    using PomodoroClient;
    using PomodoroServer;

    [TestFixture]
    class RemoteNotifierIntegrationSpec : WampBaseTestFixture
    {
        private RemotePomodoroNotifier _clientNotifier;
        private PomodoroIdentifier _identifier;
        private Mock<PomodoroNotifier> _serverNotifier;
        private const string RealmName = "testRealm";

        [SetUp]
        public void Setup()
        {
            _identifier = new PomodoroIdentifier(1);
            _clientNotifier = new RemotePomodoroNotifier(_identifier, WampHostHelper.GetNewProxy(ServerAddress, RealmName));
            _serverNotifier = new Mock<PomodoroNotifier>();
            var eventPublisher = new PomodoroEventPublisher(WampHostHelper.GetNewProxy(ServerAddress, RealmName));
            eventPublisher.Subscribe(_serverNotifier.Object);
        }

        [Test]
        public void ShouldNotifyAboutRemoteStartedEvent()
        {
            //given
            bool started = false;
            _clientNotifier.IntervalStarted += (sender, intervalStartedArgs) => started = true;

            //when
            _serverNotifier.Raise(notifier => notifier.IntervalStarted += null, new IntervalStartedEventArgs{Id = _identifier});

            //then
            WaitForExpected(ref started, true);
        }
    }
}
