namespace Specification
{
    using System;
    using System.Collections.Generic;
    using Halp;
    using NUnit.Framework;
    using Pomodoro;
    using Pomodoro.Timer;

    [TestFixture]
    class PomodoroTimerSpec
    {
        private ControlledTimeMaster _timeMaster;

        private PomodoroEventHelper _eventHelper;
        private PomodoroTimer _pomodoro;

        [SetUp]
        public void Setup()
        {
            _eventHelper = new PomodoroEventHelper();
            _timeMaster = new ControlledTimeMaster();
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
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldStartShortBreakAfterEndingProductivityInterval()
        {
            //given
            var config = new PomodoroConfig
            {
                Productivity = new TimeSpan(),
                ShortBreak = new TimeSpan()
            };
            var expectedIntervals = new List<IntervalType> { IntervalType.Productive, IntervalType.ShortBreak };

            _pomodoro.Start(config);
            _timeMaster.FinishLatestInterval();

            //when
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        [ExpectedException(typeof(CannotStartPomodoroMultipleTimesException))]
        public void ShouldNotStartPomodoroTwice()
        {
            //given
            var config = new PomodoroConfig
            {
                Productivity = new TimeSpan(),
                ShortBreak = new TimeSpan()
            };
            _pomodoro.Start(config);

            //when
            _pomodoro.Start(config);
        }

        [Test]
        public void ShouldGoIntoLongBreakAfterGivenAmountOfProductivityIntervals()
        {
            //given
            var config = new PomodoroConfig
            {
                Productivity = new TimeSpan(),
                LongBreak = new TimeSpan(),
                LongBreakAfter = 1
            };

            _pomodoro.Start(config);
            _timeMaster.FinishLatestInterval();

            var expectedIntervals = new List<IntervalType> {IntervalType.Productive, IntervalType.LongBreak};

            //when
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldStartFromTheBeginningAfterLongBreak()
        {
            //given
            var config = new PomodoroConfig
            {
                Productivity = new TimeSpan(),
                LongBreak = new TimeSpan(),
                LongBreakAfter = 1
            };
            _pomodoro.Start(config);
            _timeMaster.FinishLatestInterval(); //stop first productive
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval(); //stop long break

            var expectedIntervals = new List<IntervalType>
            {
                IntervalType.Productive,
                IntervalType.LongBreak,
                IntervalType.Productive
            };

            //when
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval(); //stop productive again

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }
    }
}
