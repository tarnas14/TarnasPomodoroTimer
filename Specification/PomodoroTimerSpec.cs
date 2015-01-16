namespace Specification
{
    using System;
    using System.Collections.Generic;
    using Halp;
    using Moq;
    using NUnit.Framework;
    using Pomodoro;

    [TestFixture]
    class PomodoroTimerSpec
    {
        private TimeMaster _timeMaster;
        private TimeSpan _productivityInterval;

        private PomodoroEventHelper _eventHelper;

        [SetUp]
        public void Setup()
        {
            _productivityInterval = new TimeSpan(0, 0, 2, 0);
            _eventHelper = new PomodoroEventHelper();
            _timeMaster = new ImmediateTimeMaster();
        }

        [Test]
        public void ShouldStartWithProductivityInterval()
        {
            //given
            var config = new PomodoroConfig
            {
                Productivity = _productivityInterval
            };
            var pomodoro = new PomodoroTimer(config, _timeMaster);
            pomodoro.EndOfProductivityInterval += _eventHelper.EndOfProductivityInterval;
            var expectedIntervals = new List<IntervalType> { IntervalType.Productive };

            //when
            pomodoro.Start();
            var actualFinishedIntervals = _eventHelper.FinishedIntervals;

            //then
            Assert.That(actualFinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

    }
}
