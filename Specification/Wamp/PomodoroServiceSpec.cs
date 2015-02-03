namespace Specification.Wamp
{
    using System.Linq;
    using Halp;
    using Moq;
    using NUnit.Framework;
    using Pomodoro;
    using Pomodoro.Wamp.Server;

    class PomodoroServiceSpec
    {
        private PomodoroService _pomodoroService;
        private PomodoroEventHelper _eventHelper;

        [SetUp]
        public void Setup()
        {
            _pomodoroService = new PomodoroService(new InMemoryPomodoroStore(Mock.Of<TimeMaster>()));
            _eventHelper = new PomodoroEventHelper();
            _pomodoroService.IntervalStarted += _eventHelper.StartOfInterval;
            _pomodoroService.IntervalInterrupted += _eventHelper.OnInterruptedInterval;
        }

        [Test]
        public void ShouldSetupNewPomodoroTimer()
        {
            //given

            //when
            var actual = _pomodoroService.SetupNewPomodoro(new PomodoroConfig());

            //then
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void ShouldNotifyAboutAllPomodoroIntervalStarts()
        {
            //given
            var identifier = _pomodoroService.SetupNewPomodoro(new PomodoroConfig());
            var otherIdentifier = _pomodoroService.SetupNewPomodoro(new PomodoroConfig());

            var expectedIds = new[] {identifier, otherIdentifier};

            //when
            _pomodoroService.StartNext(identifier);
            _pomodoroService.StartNext(otherIdentifier);

            //then
            var startedIntervalIds = _eventHelper.StartedIntervals.Select(interval => interval.Id).ToList();
            Assert.That(startedIntervalIds, Is.EquivalentTo(expectedIds));
        }

        [Test]
        public void ShouldNotifyAboutAllPomodoroInterruptions()
        {
            //given
            var identifier = _pomodoroService.SetupNewPomodoro(new PomodoroConfig());
            var otherIdentifier = _pomodoroService.SetupNewPomodoro(new PomodoroConfig());

            var expectedIds = new[] { identifier, identifier, otherIdentifier };

            //when
            _pomodoroService.StartNext(identifier);
            _pomodoroService.Interrupt(identifier);

            _pomodoroService.StartNext(otherIdentifier);
            _pomodoroService.StartNext(identifier);

            _pomodoroService.Interrupt(identifier);
            _pomodoroService.Interrupt(otherIdentifier);

            //then
            var interruptedIntervalIds = _eventHelper.InterruptedIntervals.Select(interval => interval.Id).ToList();
            Assert.That(interruptedIntervalIds, Is.EquivalentTo(expectedIds));
        }
    }
}