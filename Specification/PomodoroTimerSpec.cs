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
        private PomodoroConfig _config;

        [SetUp]
        public void Setup()
        {
            _eventHelper = new PomodoroEventHelper();
            _timeMaster = new ControlledTimeMaster();
            _config = new PomodoroConfig
            {
                LongBreak = new TimeSpan(),
                Productivity = new TimeSpan(),
                ShortBreak = new TimeSpan(),
                LongBreakAfter = 4
            };
            SetupPomodoro();
        }

        private void SetupPomodoro()
        {
            _pomodoro = new PomodoroTimer(_timeMaster, _config);
            _pomodoro.IntervalFinished += _eventHelper.EndOfInterval;
            _pomodoro.Tick += _eventHelper.OnTick;
        }

        [Test]
        public void ShouldStartWithProductivityInterval()
        {
            //given
            var expectedIntervals = new List<IntervalType> { IntervalType.Productive };

            //when
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldStartShortBreakAfterEndingProductivityInterval()
        {
            //given
            var expectedIntervals = new List<IntervalType> { IntervalType.Productive, IntervalType.ShortBreak };

            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            //when
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldGoIntoLongBreakAfterGivenAmountOfProductivityIntervals()
        {
            //given
            _config.LongBreakAfter = 1;
            SetupPomodoro();

            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            var expectedIntervals = new List<IntervalType> {IntervalType.Productive, IntervalType.LongBreak};

            //when
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldStartFromTheBeginningAfterWholeCycle()
        {
            //given
            _config.LongBreakAfter = 1;
            SetupPomodoro();
            _pomodoro.StartNext();
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

        [Test]
        [ExpectedException(typeof(PreviousIntervalHasNotFinishedException))]
        public void ShouldNotStartNextIfFirstIntervalHasNotFinished()
        {
            //given
            _pomodoro.StartNext();

            //when
            _pomodoro.StartNext();
        }

        [Test]
        [ExpectedException(typeof(PreviousIntervalHasNotFinishedException))]
        public void ShouldNotStartNextIfCurrentIntervalHasNotFinished()
        {
            //given
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();
            _pomodoro.StartNext();

            //when
            _pomodoro.StartNext();
        }

        [Test]
        public void ShouldAllowInterruptingFirstInterval()
        {
            //given
            _pomodoro.StartNext();

            //when
            _pomodoro.Interrupt();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.Empty);
        }

        [Test]
        public void ShouldAllowInterruptingIntervals()
        {
            //given
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();
            _pomodoro.StartNext();

            var expectedIntervals = new List<IntervalType> { IntervalType.Productive };

            //when
            _pomodoro.Interrupt();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldAllowRestartingInterruptedFirstInterval()
        {
            //given
            _pomodoro.StartNext();
            _pomodoro.Interrupt();

            var expectedIntervals = new List<IntervalType> {IntervalType.Productive};

            //when
            _pomodoro.Restart();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldRestartInterruptedIntervals()
        {
            //given
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();
            _pomodoro.StartNext();
            _pomodoro.Interrupt();

            var expectedIntervals = new List<IntervalType> { IntervalType.Productive, IntervalType.ShortBreak };

            //when
            _pomodoro.Restart();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.FinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldNotifyAboutTimeMasterTicks()
        {
            //given
            _pomodoro.StartNext();

            //when
            _timeMaster.DoTick(new TimeRemainingEventArgs(TimeSpan.FromDays(2)));
            _timeMaster.DoTick(new TimeRemainingEventArgs(TimeSpan.FromDays(1)));

            //then
            Assert.That(_eventHelper.TicksCount, Is.EqualTo(2));
        }
    }
}
