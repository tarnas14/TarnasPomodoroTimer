namespace Specification.Wamp
{
    using Halp;
    using Moq;
    using NUnit.Framework;
    using Pomodoro;
    using Pomodoro.Timer;
    using Pomodoro.Wamp.Server;

    class PomodoroServiceSpec
    {
        private PomodoroService _pomodoroService;
        private Mock<PomodoroStore> _pomodoroStoreMock;
        private PomodoroEventHelper _pomodoroEventHelper;

        [SetUp]
        public void Setup()
        {
            _pomodoroEventHelper = new PomodoroEventHelper();
            var pomodoro = new PomodoroTimer(Mock.Of<TimeMaster>(), new PomodoroConfig());
            pomodoro.IntervalStarted += _pomodoroEventHelper.StartOfInterval;
            _pomodoroStoreMock = new Mock<PomodoroStore>();
            _pomodoroStoreMock.Setup(mock => mock[It.IsAny<PomodoroIdentifier>()]).Returns(pomodoro);
            _pomodoroService = new PomodoroService(_pomodoroStoreMock.Object);
        }

        [Test]
        public void ShouldSetupNewPomodoroTimer()
        {
            //given
            var expected = new PomodoroIdentifier(2);
            _pomodoroStoreMock.Setup(mock => mock.SetupNewPomodoro(It.IsAny<PomodoroConfig>())).Returns(expected);

            //when
            var actual = _pomodoroService.SetupNewPomodoro(new PomodoroConfig());

            //then
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldStartPomodoroWithGivenId()
        {
            //given
            var identifier = new PomodoroIdentifier(2);

            //when
            _pomodoroService.StartNext(identifier);

            //then
            Assert.That(_pomodoroEventHelper.StartedIntervals.Count, Is.EqualTo(1));
        }
    }
}