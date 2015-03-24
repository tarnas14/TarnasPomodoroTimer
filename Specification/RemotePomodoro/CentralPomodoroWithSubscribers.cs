namespace Specification.RemotePomodoro
{
    using System;
    using System.Reactive.Subjects;
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
        private const string RealmName = "realmName";
        private PomodoroEventHelper _eventHelper;

        [SetUp]
        public new void Setup()
        {
            _pomodoro = new PomodoroTimer(Mock.Of<TimeMaster>(), PomodoroConfig.Standard);
            _eventHelper = new PomodoroEventHelper();
            var realm = Host.RealmContainer.GetRealmByName(RealmName);
            new PomodoroServer(realm, _pomodoro);
        }

        [Test]
        public void A()
        {
            
            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));

            //when
            _pomodoro.StartNext();

            //then
            WaitForExpected(_eventHelper.StartedIntervals.Count, 2);

            Assert.That(_eventHelper.StartedIntervals.Count, Is.EqualTo(2));
        }

        [Test]
        public void B()
        {

            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));

            //when
            _pomodoro.StartNext();

            //then
            WaitForExpected(_eventHelper.StartedIntervals.Count, 3);

            Assert.That(_eventHelper.StartedIntervals.Count, Is.EqualTo(3));
        }

        //when this is the last test in test fixture
        //we get this exception:
        /*
         * WampSharp.V2.Core.Contracts.WampException : Exception of type 'WampSharp.V2.Core.Contracts.WampException' was thrown.
         */
        [Test]
        public void C()
        {
            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));

            //when
            _pomodoro.StartNext();

            //then
            WaitForExpected(_eventHelper.StartedIntervals.Count, 1);

            Assert.That(_eventHelper.StartedIntervals.Count, Is.EqualTo(1));
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
