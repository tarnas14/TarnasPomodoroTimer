namespace Specification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            _pomodoro.IntervalStarted += _eventHelper.StartOfInterval;
            _pomodoro.IntervalInterrupted += _eventHelper.IntervalInterrupted;
            _pomodoro.Tick += _eventHelper.OnTick;
        }

        private IList<IntervalType> TypesOfFinishedIntervals
        {
            get { return _eventHelper.FinishedIntervals.Select(interval => interval.Type).ToList(); }
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
            Assert.That(TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            Assert.That(TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            Assert.That(TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            Assert.That(TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            Assert.That(TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            Assert.That(TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            Assert.That(TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldNotifyAboutTimeMasterTicks()
        {
            //given
            _pomodoro.StartNext();

            //when
            _timeMaster.DoTick();
            _timeMaster.DoTick();

            //then
            Assert.That(_eventHelper.Ticks.Count, Is.EqualTo(2));
        }

        [Test]
        public void ShouldIncludeCurrentIntervalTypeInTickNotification()
        {
            //given
            _pomodoro.StartNext();
            _timeMaster.DoTick();
            _timeMaster.FinishLatestInterval();
            _pomodoro.StartNext();
            _timeMaster.DoTick();

            var expectedNotification = new List<IntervalType> {IntervalType.Productive, IntervalType.ShortBreak};

            //when
            var intervals = _eventHelper.Ticks.Select(tick => tick.IntervalType);

            //then
            Assert.That(intervals, Is.EquivalentTo(expectedNotification));
        }

        [Test]
        public void ShouldIndicateCurrentIntervalRightFromTheStart()
        {
            //given
            var expected = new {
                IntervalType = IntervalType.Productive,
                Length = _config.Productivity
            };

            //when
            var actual = _pomodoro.CurrentInterval;

            //then
            Assert.That(actual.Type, Is.EqualTo(expected.IntervalType));
            Assert.That(actual.TimeSpan, Is.EqualTo(expected.Length));
        }

        [Test]
        public void ShouldHoldInformationAboutNextIntervalInIntervalFinishedNotification()
        {
            //given
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            var expectedNextIntervalIndicators = new List<IntervalType>
            {
                IntervalType.ShortBreak,
                IntervalType.Productive
            };

            //when
            var actualNextIntervalIndicators = _eventHelper.FinishedIntervals.Select(interval => interval.NextIntervalType).ToList();

            //then
            Assert.That(actualNextIntervalIndicators, Is.EquivalentTo(expectedNextIntervalIndicators));

        }

        [Test]
        public void ShouldNotifyAboutStartedInterval()
        {
            //given
            _pomodoro.StartNext();

            const IntervalType expectedType = IntervalType.Productive;
            var expectedDuration = _config.Productivity;

            //when
            var startEvents = _eventHelper.StartedIntervals;

            //then
            Assert.That(startEvents.Count, Is.EqualTo(1));
            var firstStartEvent = startEvents.First();
            Assert.That(firstStartEvent.Type, Is.EqualTo(expectedType));
            Assert.That(firstStartEvent.Duration, Is.EqualTo(expectedDuration));
        }

        [Test]
        public void ShouldNotifyAboutIntervalInterruptions()
        {
            //given
            _pomodoro.StartNext();
            _pomodoro.Interrupt();

            const IntervalType expectedType = IntervalType.Productive;

            //when
            var interruptEvents = _eventHelper.InterruptedIntervals;
            
            //then
            Assert.That(interruptEvents.Count, Is.EqualTo(1));
            var firstInterruptEvent = interruptEvents.First();
            Assert.That(firstInterruptEvent.Type, Is.EqualTo(expectedType));
        }
    }
}
