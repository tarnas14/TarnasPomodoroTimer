namespace Specification.Wamp
{
    using System;
    using System.Linq;
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
        public new void Setup()
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
            PomodoroIdentifier actualIdentifier = null;
            _clientNotifier.IntervalStarted += (sender, args) =>
            {
                actualIdentifier = args.Id;
            };

            //when
            RaisePomodoroEventOnRemoteServer(new IntervalStartedEventArgs { Id = _identifier });

            //then
            Assert.That(actualIdentifier.Id, Is.EqualTo(_identifier.Id));
        }

        [Test]
        public void ShouldCountTimeAfterReceivingStartSignal()
        {
            //given
            var intervalStartedEventArgs = new IntervalStartedEventArgs
            {
                Id = _identifier,
                Duration = TimeSpan.FromMinutes(25),
                Type = IntervalType.Productive
            };
            RaisePomodoroEventOnRemoteServer(intervalStartedEventArgs);

            var expectedFinishedIntervals = new[]
            {
                IntervalType.Productive
            };

            //when
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.TypesOfFinishedIntervals, Is.EquivalentTo(expectedFinishedIntervals));
        }

        private void RaisePomodoroEventOnRemoteServer(EventArgs eventArgs)
        {
            bool started = false;

            if ((eventArgs as IntervalStartedEventArgs) != null)
            {
                _clientNotifier.IntervalStarted += (sender, intervalStartedArgs) => started = true;
                _serverNotifier.Raise(notifier => notifier.IntervalStarted += null, eventArgs as IntervalStartedEventArgs);
            }
            else if ((eventArgs as IntervalInterruptedEventArgs) != null)
            {
                _clientNotifier.IntervalInterrupted += (sender, args) => started = true;
                _serverNotifier.Raise(notifier => notifier.IntervalInterrupted += null, eventArgs as IntervalInterruptedEventArgs);
            }

            WaitForExpected(ref started, true);
        }

        [Test]
        public void ShouldNotifyAboutTimerTicks()
        {
            //given
            RaisePomodoroEventOnRemoteServer(new IntervalStartedEventArgs
            {
                Id = _identifier,
                Type = IntervalType.Productive,
                Duration = TimeSpan.FromMinutes(2)
            });

            //when
            _timeMaster.DoTick();
            _timeMaster.DoTick();

            //then
            Assert.That(_eventHelper.Ticks.Count, Is.EqualTo(2));
        }

        [Test]
        public void ShouldInterruptTimerWhenRemoteInterruptedEventReceived()
        {
            //given
            RaisePomodoroEventOnRemoteServer(new IntervalStartedEventArgs
            {
                Id = _identifier,
                Type = IntervalType.Productive,
                Duration = TimeSpan.FromMinutes(2)
            });

            //when
            RaisePomodoroEventOnRemoteServer(new IntervalInterruptedEventArgs
            {
                Id = _identifier,
                Type = IntervalType.Productive
            });

            //then
            Assert.That(_eventHelper.InterruptedIntervals.Count, Is.EqualTo(1));
        }

        [Test]
        public void ShouldIgnoreEventsFromPomodorosWithDifferentIdentifierThanSpecified()
        {
            //given
            var notSubscribedIdentifier = new PomodoroIdentifier(666);
            RaisePomodoroEventOnRemoteServer(new IntervalStartedEventArgs
            {
                Id = notSubscribedIdentifier,
                Type = IntervalType.Productive,
                Duration = TimeSpan.FromMinutes(2)
            });

            //when
            RaisePomodoroEventOnRemoteServer(new IntervalInterruptedEventArgs
            {
                Id = notSubscribedIdentifier,
                Type = IntervalType.Productive
            });

            //then
            Assert.That(!_eventHelper.StartedIntervals.Any());
            Assert.That(!_eventHelper.InterruptedIntervals.Any());
        }
    }
}
