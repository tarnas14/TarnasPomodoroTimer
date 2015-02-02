namespace Specification.Wamp
{
    using System.Collections.Generic;
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
            _pomodoroStoreMock.Setup(mock => mock[It.IsAny<PomodoroIdentifier>()]).Returns((PomodoroIdentifier id) =>
            {
                pomodoro.Id = id;
                return pomodoro;
            });

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

        [Test]
        public void ShouldStartPomodoroWithGivenId()
        {
            //given
            var identifier = new PomodoroIdentifier(2);

            //when
            _pomodoroService.StartNext(identifier);

            //then
            Assert.That(_pomodoroEventHelper.StartedIntervals.Count, Is.EqualTo(1));

            Assert.That(_pomodoroEventHelper.StartedIntervals.All(interval => interval.Id == identifier));
        }

        [Test]
        public void ShouldStopPomodoroWithGivenId()
        {
            //given
            var identifier = new PomodoroIdentifier(2);
            _pomodoroService.StartNext(identifier);

            //when
            _pomodoroService.Interrupt(identifier);

            //then
            Assert.That(_pomodoroEventHelper.InterruptedIntervals.Count, Is.EqualTo(1));

            Assert.That(_pomodoroEventHelper.InterruptedIntervals.All(interval => interval.Id == identifier));
        }

        [Test]
        public void ShouldRestartPomodoroWithGivenId()
        {
            //given
            var identifier = new PomodoroIdentifier(2);
            _pomodoroService.StartNext(identifier);
            _pomodoroService.Interrupt(identifier);

            //when
            _pomodoroService.Restart(identifier);

            //then
            Assert.That(_pomodoroEventHelper.InterruptedIntervals.Count, Is.EqualTo(1));
            Assert.That(_pomodoroEventHelper.StartedIntervals.Count, Is.EqualTo(2));

            Assert.That(_pomodoroEventHelper.StartedIntervals.All(interval => interval.Id == identifier));
            Assert.That(_pomodoroEventHelper.InterruptedIntervals.All(interval => interval.Id == identifier));
        }

    }
}