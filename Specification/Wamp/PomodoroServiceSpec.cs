namespace Specification.Wamp
{
    using System.Linq;
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
        private PomodoroIdentifier _identifier;

        [SetUp]
        public void Setup()
        {
            var pomodoro = new PomodoroTimer(Mock.Of<TimeMaster>(), new PomodoroConfig());
            _pomodoroEventHelper = new PomodoroEventHelper();
            _pomodoroEventHelper.SubscribeToPomodoro(pomodoro);

            _identifier = new PomodoroIdentifier(2);
            _pomodoroStoreMock = new Mock<PomodoroStore>();
            _pomodoroStoreMock.Setup(mock => mock.SetupNewPomodoro(It.IsAny<PomodoroConfig>())).Returns(_identifier);

            _pomodoroService = new PomodoroService(_pomodoroStoreMock.Object);
        }

        [Test]
        public void ShouldSetupNewPomodoroTimer()
        {
            //given

            //when
            var actual = _pomodoroService.SetupNewPomodoro(new PomodoroConfig());

            //then
            Assert.That(actual, Is.EqualTo(_identifier));
        }
    }
}