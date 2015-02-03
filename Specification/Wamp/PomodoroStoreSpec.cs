namespace Specification.Wamp
{
    using System.Linq;
    using Halp;
    using Moq;
    using NUnit.Framework;
    using Pomodoro;
    using Pomodoro.Wamp.Server;

    class PomodoroStoreSpec
    {
        private PomodoroStore _pomodoroStore;
        private PomodoroEventHelper _pomodoroEventHelper;
        private PomodoroIdentifier _identifier;

        [SetUp]
        public void Setup()
        {
            _pomodoroEventHelper = new PomodoroEventHelper();
            _pomodoroStore = new InMemoryPomodoroStore(Mock.Of<TimeMaster>());
            _identifier = _pomodoroStore.SetupNewPomodoro(new PomodoroConfig());
            _pomodoroStore.SubscribeToPomodoro(_identifier, _pomodoroEventHelper);
        }

        [Test]
        public void ShouldStartPomodoroWithGivenId()
        {
            //given
            
            //when
            _pomodoroStore.StartNext(_identifier);

            //then
            Assert.That(_pomodoroEventHelper.StartedIntervals.Count, Is.EqualTo(1));

            Assert.That(_pomodoroEventHelper.StartedIntervals.All(interval => interval.Id == _identifier));
        }

        [Test]
        public void ShouldStopPomodoroWithGivenId()
        {
            //given
            _pomodoroStore.StartNext(_identifier);

            //when
            _pomodoroStore.Interrupt(_identifier);

            //then
            Assert.That(_pomodoroEventHelper.InterruptedIntervals.Count, Is.EqualTo(1));

            Assert.That(_pomodoroEventHelper.InterruptedIntervals.All(interval => interval.Id == _identifier));
        }

        [Test]
        public void ShouldRestartPomodoroWithGivenId()
        {
            //given
            _pomodoroStore.StartNext(_identifier);
            _pomodoroStore.Interrupt(_identifier);

            //when
            _pomodoroStore.Restart(_identifier);

            //then
            Assert.That(_pomodoroEventHelper.InterruptedIntervals.Count, Is.EqualTo(1));
            Assert.That(_pomodoroEventHelper.StartedIntervals.Count, Is.EqualTo(2));

            Assert.That(_pomodoroEventHelper.StartedIntervals.All(interval => interval.Id == _identifier));
            Assert.That(_pomodoroEventHelper.InterruptedIntervals.All(interval => interval.Id == _identifier));
        }
    }
}
