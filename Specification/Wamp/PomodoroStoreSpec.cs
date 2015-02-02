namespace Specification.Wamp
{
    using Moq;
    using NUnit.Framework;
    using Pomodoro;
    using Pomodoro.Wamp.Server;

    class PomodoroStoreSpec
    {
        private InMemoryPomodoroStore _pomodoroStore;

        [SetUp]
        public void Setup()
        {
            _pomodoroStore = new InMemoryPomodoroStore(Mock.Of<TimeMaster>());
        }

        [Test]
        public void ShouldStoreMultiplePomodoroTimers()
        {
            //given
            var config = new PomodoroConfig();
            var pomodoroId = _pomodoroStore.SetupNewPomodoro(config);
            var anotherPomodoroId = _pomodoroStore.SetupNewPomodoro(config);

            //when
            var pomodoro = _pomodoroStore[pomodoroId];
            var anotherPomodoro = _pomodoroStore[anotherPomodoroId];

            //then
            Assert.That(pomodoroId, Is.Not.EqualTo(anotherPomodoroId));
            Assert.That(pomodoro, Is.Not.EqualTo(anotherPomodoro));
        }

        [Test]
        public void ShouldAssignIdsToPomodoros()
        {
            //given
            var config = new PomodoroConfig();
            var pomodoroId = _pomodoroStore.SetupNewPomodoro(config);

            //when
            var pomodoro = _pomodoroStore[pomodoroId];

            //then
            Assert.That(pomodoro.Id, Is.EqualTo(pomodoroId));
        }
        
    }
}
