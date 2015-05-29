namespace Specification.RemotePomodoro
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Subjects;
    using Halp;
    using NUnit.Framework;
    using Pomodoro;
    using Pomodoro.Timer;
    using WampSharp.V2;
    using WampSharp.V2.Client;
    using WampSharp.V2.Realm;

    [TestFixture]
    class CentralPomodoroWithSubscribers
    {
        private const int MaxWaitTime = 5;

        private PomodoroTimer _pomodoro;
        private const string RealmName = "realmName";
        private PomodoroEventHelper _eventHelper;

        private int _counter;
        private const string _serverAddress = "ws://127.0.0.1:8080/ws";
        protected string ServerAddress { get; set; }
        protected DefaultWampHost Host;
        private ControlledTimeMaster _timeMaster;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            ServerAddress = string.Format("{0}{1}", _serverAddress, ++_counter);

            Host = new DefaultWampHost(ServerAddress);
            Host.Open();
        }

        [TestFixtureTearDown]
        public void TearDownFixture()
        {
            Host.Dispose();
        }

        protected void WaitForExpected<T>(IList<T> list, int expectedCount)
        {
            var start = DateTime.Now;
            while (list.Count != expectedCount)
            {
                if (DateTime.Now - start > TimeSpan.FromSeconds(MaxWaitTime))
                {
                    return;
                }
            }
        }

        [SetUp]
        public void Setup()
        {
            _timeMaster = new ControlledTimeMaster();
            _pomodoro = new PomodoroTimer(_timeMaster, PomodoroConfig.Standard);
            _eventHelper = new PomodoroEventHelper();
            var realm = Host.RealmContainer.GetRealmByName(RealmName);
            new PomodoroServer(realm, _pomodoro);
        }

        [Test]
        public void MultipleClientsShouldBeInformedAboutIntervalStart()
        {

            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));

            //when
            _pomodoro.StartNext();

            //then
            WaitForExpected(_eventHelper.StartedIntervals, 3);

            Assert.That(_eventHelper.StartedIntervals.Count, Is.EqualTo(3));
        }

        [Test]
        public void SingleClientShouldBeInformedAboutIntervalStart()
        {
            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));

            //when
            _pomodoro.StartNext();

            //then
            WaitForExpected(_eventHelper.StartedIntervals, 1);

            Assert.That(_eventHelper.StartedIntervals.Count, Is.EqualTo(1));
        }

        [Test]
        public void SingleClientShouldBeInformedAboutIntervalEnd()
        {
            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _pomodoro.StartNext();

            //when
            _timeMaster.FinishLatestInterval();

            //then
            WaitForExpected(_eventHelper.FinishedIntervals, 1);

            Assert.That(_eventHelper.FinishedIntervals.Count, Is.EqualTo(1));
        }

        [Test]
        public void MultipleClientsShouldBeInformedAboutIntervalEnd()
        {
            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _pomodoro.StartNext();

            //when
            _timeMaster.FinishLatestInterval();

            //then
            WaitForExpected(_eventHelper.FinishedIntervals, 2);

            Assert.That(_eventHelper.FinishedIntervals.Count, Is.EqualTo(2));
        }

        [Test]
        public void SingleClientShouldBeInformedAboutIntervalInterruption()
        {
            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _pomodoro.StartNext();

            //when
            _pomodoro.Interrupt();

            //then
            WaitForExpected(_eventHelper.InterruptedIntervals, 1);

            Assert.That(_eventHelper.InterruptedIntervals.Count, Is.EqualTo(1));
        }

        [Test]
        public void MultipleClientsShouldBeInformedAboutIntervalInterruption()
        {
            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _pomodoro.StartNext();

            //when
            _pomodoro.Interrupt();

            //then
            WaitForExpected(_eventHelper.InterruptedIntervals, 3);

            Assert.That(_eventHelper.InterruptedIntervals.Count, Is.EqualTo(3));
        }

        [Test]
        public void SingleClientShouldBeInformedAboutIntervalRestart()
        {
            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _pomodoro.StartNext();
            _pomodoro.Interrupt();

            //when
            _pomodoro.RestartInterval();

            //then
            WaitForExpected(_eventHelper.StartedIntervals, 2);

            Assert.That(_eventHelper.StartedIntervals.Count, Is.EqualTo(2));
        }

        [Test]
        public void MultipleClientsShouldBeInformedAboutIntervalRestart()
        {
            //given
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _eventHelper.Subscribe(new RemotePomodoroClient(WampHostHelper.GetRealmProxy(ServerAddress, RealmName)));
            _pomodoro.StartNext();
            _pomodoro.Interrupt();

            //when
            _pomodoro.RestartInterval();

            //then
            WaitForExpected(_eventHelper.StartedIntervals, 6);

            Assert.That(_eventHelper.StartedIntervals.Count, Is.EqualTo(6));
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

            clientProxy.Services.GetSubject<IntervalFinishedEventArgs>(PomodoroServer.EndSubject).Subscribe(OnFinishedInterval);

            clientProxy.Services.GetSubject<IntervalInterruptedEventArgs>(PomodoroServer.InterruptSubject)
                .Subscribe(OnInterruptedInterval);
        }

        private void OnStartedInterval(IntervalStartedEventArgs intervalStartedEventArgs)
        {
            if (IntervalStarted != null)
            {
                IntervalStarted(this, intervalStartedEventArgs);
            }
        }

        private void OnFinishedInterval(IntervalFinishedEventArgs intervalFinishedEventArgs)
        {
            if (IntervalFinished != null)
            {
                IntervalFinished(this, intervalFinishedEventArgs);
            }
        }

        private void OnInterruptedInterval(IntervalInterruptedEventArgs interruptedEventArgs)
        {
            if (IntervalInterrupted != null)
            {
                IntervalInterrupted(this, interruptedEventArgs);
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
        private ISubject<IntervalFinishedEventArgs> _endSubject;
        private ISubject<IntervalInterruptedEventArgs> _interruptSubject;
        public const string StartSubject = "pomodoro.start";
        public const string EndSubject = "pomodoro.end";
        public const string InterruptSubject = "pomodoro.interrupt";

        public PomodoroServer(IWampHostedRealm realm, PomodoroNotifier pomodoroNotifier)
        {
            SetupSubjects(realm);

            pomodoroNotifier.IntervalStarted += Started;
            pomodoroNotifier.IntervalFinished += Finished;
            pomodoroNotifier.IntervalInterrupted += Interrupted;
        }

        private void SetupSubjects(IWampHostedRealm realm)
        {
            _startSubject = realm.Services.GetSubject<IntervalStartedEventArgs>(StartSubject);
            _endSubject = realm.Services.GetSubject<IntervalFinishedEventArgs>(EndSubject);
            _interruptSubject = realm.Services.GetSubject<IntervalInterruptedEventArgs>(InterruptSubject);
        }

        private void Started(object sender, IntervalStartedEventArgs e)
        {
            _startSubject.OnNext(e);
        }

        private void Finished(object sender, IntervalFinishedEventArgs e)
        {
            _endSubject.OnNext(e);
        }

        private void Interrupted(object sender, IntervalInterruptedEventArgs e)
        {
            _interruptSubject.OnNext(e);
        }
    }
}
