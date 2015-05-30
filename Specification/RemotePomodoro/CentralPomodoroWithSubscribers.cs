namespace Specification.RemotePomodoro
{
    using System;
    using System.Collections.Generic;
    using Halp;
    using NUnit.Framework;
    using Pomodoro;
    using Pomodoro.Client;
    using Pomodoro.Server;
    using Pomodoro.Timer;
    using WampSharp.V2;

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
}
