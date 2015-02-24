namespace Specification.Wamp
{
    using System;
    using System.Collections;
    using Halp;
    using Moq;
    using NUnit.Framework;
    using Pomodoro;
    using Pomodoro.Timer;
    using Pomodoro.Wamp.Server;
    using PomodoroClient;
    using PomodoroServer;

    [TestFixture]
    class RemoteNotifierSpec : WampBaseTestFixture
    {
        private RemotePomodoroNotifier _clientNotifier;
        private PomodoroIdentifier _identifier;
        private Mock<PomodoroNotifier> _serverNotifier;
        private ControlledTimeMaster _timeMaster;
        private PomodoroEventHelper _eventHelper;
        private const string RealmName = "testRealm";

        [SetUp]
        public void Setup()
        {
            _timeMaster = new ControlledTimeMaster();
            _identifier = new PomodoroIdentifier(1);
            _clientNotifier = new RemotePomodoroNotifier(_identifier, WampHostHelper.GetNewProxy(ServerAddress, RealmName), _timeMaster);
            _serverNotifier = new Mock<PomodoroNotifier>();
            var eventPublisher = new PomodoroEventPublisher(WampHostHelper.GetNewProxy(ServerAddress, RealmName));
            eventPublisher.Subscribe(_serverNotifier.Object);
            _eventHelper = new PomodoroEventHelper();
            _eventHelper.Subscribe(_clientNotifier);
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
            Assert.True(started);
        }

        [Test]
        public void ShouldCountTimeAfterReceivingStartSignal()
        {
            //given
            bool started = false;
            _clientNotifier.IntervalStarted += (sender, intervalStartedArgs) => started = true;
            _serverNotifier.Raise(notifier => notifier.IntervalStarted += null, new IntervalStartedEventArgs
            {
                Id = _identifier, 
                Duration = TimeSpan.FromMinutes(25), 
                Type = IntervalType.Productive
            });

            var expectedFinishedIntervals = new[]
            {
                IntervalType.Productive
            };

            //when
            WaitForExpected(ref started, true);
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.TypesOfFinishedIntervals, Is.EquivalentTo(expectedFinishedIntervals));
        }
    }
}
