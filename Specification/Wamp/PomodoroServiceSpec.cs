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
        private DefaultPomodoroService _pomodoroService;
        private PomodoroEventHelper _eventHelper;
        private Mock<TimeMasterFactory> _timeMasterFactoryMock;

        [SetUp]
        public void Setup()
        {
            _timeMasterFactoryMock = new Mock<TimeMasterFactory>();
            _timeMasterFactoryMock.Setup(mockFactory => mockFactory.GetTimeMaster()).Returns(Mock.Of<TimeMaster>);
            _pomodoroService = new DefaultPomodoroService(new InMemoryPomodoroStore(_timeMasterFactoryMock.Object));
            _eventHelper = new PomodoroEventHelper();
            _eventHelper.Subscribe(_pomodoroService);
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