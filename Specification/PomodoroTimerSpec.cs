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
        private PomodoroTimer _pomodoro;

        [SetUp]
        public void Setup()
        {
            _eventHelper = new PomodoroEventHelper();
            _timeMaster = new ImmediateTimeMaster();
            _pomodoro = new PomodoroTimer(_timeMaster);
            _pomodoro.IntervalFinished += _eventHelper.EndOfInterval;
        }

        [Test]
        public void ShouldStartWithProductivityInterval()
        {
            //given
            var config = new PomodoroConfig
            {
                Productivity = new TimeSpan()
            };
            var expectedIntervals = new List<IntervalType> { IntervalType.Productive };

            //when
            _pomodoro.Start(config);

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldStartShortBreakAFterEndingProductivityInterval()
        {
            //given
            var config = new PomodoroConfig
            {
                Productivity = new TimeSpan(),
                ShortBreak = new TimeSpan()
            };
            var expectedIntervals = new List<IntervalType> { IntervalType.Productive, IntervalType.ShortBreak };

            _pomodoro.Start(config);

            //when
            _pomodoro.StartNext();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));

        }
    }
}
