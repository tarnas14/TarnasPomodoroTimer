namespace Specification.RemotePomodoro
{
    using System;
    using System.Reactive.Subjects;
    using System.Threading;
    using Halp;
    using Moq;
    using NUnit.Framework;
    using Pomodoro;
    using Pomodoro.Timer;
    using WampSharp.V2.Client;
    using WampSharp.V2.Realm;

    [TestFixture]
    class CentralPomodoroWithSubscribers : WampBaseTestFixture
    {
        private PomodoroTimer _pomodoro;
        private PomodoroEventHelper _eventHelper;
        private const string RealmName = "realmName";

        [SetUp]
        public new void Setup()
        {
            _pomodoro = new PomodoroTimer(Mock.Of<TimeMaster>(), PomodoroConfig.Standard);
            var realm = Host.RealmContainer.GetRealmByName(RealmName);
            new PomodoroServer(realm, _pomodoro);
            _eventHelper = new PomodoroEventHelper();
        }

        [Test]
        public void ShouldNotifyOneSubscriberAboutPomodoroStart()
        {
            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetNewProxy(ServerAddress, RealmName)));

            //when
            _pomodoro.StartNext();

            //then
            WaitForExpected(ref _eventHelper.StartedIntervalsCounter, 1);

            Assert.That(_eventHelper.StartedIntervals.Count, Is.EqualTo(1));
        }

        [Test]
        public void ShouldNotifyMultipleSubscribersAboutPomodoroStart()
        {
            //given
            int counter = 0;
            var client1 = new RemotePomodoroClient(WampHostHelper.GetNewProxy(ServerAddress, RealmName));
            client1.IntervalStarted += (sender, args) => Interlocked.Increment(ref counter);
            var client2 = new RemotePomodoroClient(WampHostHelper.GetNewProxy(ServerAddress, RealmName));
            client2.IntervalStarted += (sender, args) => Interlocked.Increment(ref counter);

            //when
            _pomodoro.StartNext();

            //then
            WaitForExpected(ref counter, 2);

            Assert.That(counter, Is.EqualTo(2));
        }
    }

    internal class RemotePomodoroClient : PomodoroNotifier
    {
        public RemotePomodoroClient(IWampRealmProxy clientProxy)
        {
            SubscribeToSubjects(clientProxy);
        }

        private void SubscribeToSubjects(IWampRealmProxy clientProxy)
        {
            clientProxy.Services.GetSubject<IntervalStartedEventArgs>(PomodoroServer.StartSubject)
                .Subscribe(OnStartedInterval);
        }

        private void OnStartedInterval(IntervalStartedEventArgs intervalStartedEventArgs)
        {
            if (IntervalStarted != null)
            {
                IntervalStarted(this, intervalStartedEventArgs);
            }
        }

        public event EventHandler<IntervalStartedEventArgs> IntervalStarted;
        public event EventHandler<IntervalInterruptedEventArgs> IntervalInterrupted;
        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;
        public event EventHandler<TimeRemainingEventArgs> Tick;
    }

    internal class PomodoroServer
    {
        private ISubject<IntervalStartedEventArgs> _startSubject;
        public const string StartSubject = "pomodoro.start";

        public PomodoroServer(IWampHostedRealm realm, PomodoroNotifier pomodoroNotifier)
        {
            pomodoroNotifier.IntervalStarted += Started;

            SetupSubjects(realm);
        }

        private void SetupSubjects(IWampHostedRealm realm)
        {
            _startSubject = realm.Services.GetSubject<IntervalStartedEventArgs>(StartSubject);
        }

        private void Started(object sender, IntervalStartedEventArgs e)
        {
            _startSubject.OnNext(e);
        }
    }
}
