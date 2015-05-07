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
            _eventHelper.Subscribe(_pomodoro);
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
            Assert.That(_eventHelper.TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            Assert.That(_eventHelper.TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            Assert.That(_eventHelper.TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            Assert.That(_eventHelper.TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldStartNextIntervalEvenIfCurrentOneIsNotFinished()
        {
            //given
            _pomodoro.StartNext();
            _pomodoro.StartNext();

            var expectedIntervals = new List<IntervalType>
            {
                IntervalType.ShortBreak
            };

            //when
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            Assert.That(_eventHelper.TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        public void ShouldAllowRestartingInterruptedFirstInterval()
        {
            //given
            _pomodoro.StartNext();
            _pomodoro.Interrupt();

            var expectedIntervals = new List<IntervalType> {IntervalType.Productive};

            //when
            _pomodoro.RestartInterval();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            _pomodoro.RestartInterval();
            _timeMaster.FinishLatestInterval();

            //then
            Assert.That(_eventHelper.TypesOfFinishedIntervals, Is.EquivalentTo(expectedIntervals));
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
            
            const IntervalType expectedType = IntervalType.Productive;

            //when
            _pomodoro.Interrupt();
            
            //then
            var interruptEvents = _eventHelper.InterruptedIntervals;
            Assert.That(interruptEvents.Count, Is.EqualTo(1));
            var firstInterruptEvent = interruptEvents.First();
            Assert.That(firstInterruptEvent.Type, Is.EqualTo(expectedType));
        }

        [Test]
        public void ShouldFireStartEventWhenRestartingInterval()
        {
            //given
            _pomodoro.StartNext();
            _pomodoro.Interrupt();
            _pomodoro.RestartInterval();

            const IntervalType expectedType = IntervalType.Productive;
            var expectedDuration = _config.Productivity;

            //when
            var startEvents = _eventHelper.StartedIntervals;

            //then
            Assert.That(startEvents.Count, Is.EqualTo(2));
            var restartEvent = startEvents[1];
            Assert.That(restartEvent.Type, Is.EqualTo(expectedType));
            Assert.That(restartEvent.Duration, Is.EqualTo(expectedDuration));
        }

        [Test]
        public void ShouldStopTimerWhenIntervalIsInterrupted()
        {
            //given
            _pomodoro.StartNext();

            //when
            _pomodoro.Interrupt();

            //then
            Assert.That(_timeMaster.Ticking, Is.False);
        }

        [Test]
        public void ShouldNotNotifyAboutInterruptionWhenIntervalIsNotInProgress()
        {
            //given
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            //when
            _pomodoro.Interrupt();

            //then
            Assert.That(_eventHelper.InterruptedIntervals, Is.Empty);
        }

        [Test]
        public void ShouldResetPomodoroTimer()
        {
            //given
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            //when
            _pomodoro.Reset();

            //then
            _pomodoro.StartNext();
            _timeMaster.FinishLatestInterval();

            Assert.That(_eventHelper.StartedIntervals.Last().Type, Is.EqualTo(IntervalType.Productive));
        }

        [Test]
        public void ShouldStopCurrentIntervalOnReset()
        {
            //given
            _pomodoro.StartNext();

            //when
            _pomodoro.Reset();

            //then
            Assert.That(_timeMaster.Ticking, Is.Not.True);
        }

        [Test]
        public void ShouldSendFinishTimeWithNotificationAboutFinishedInterval()
        {
            //given
            var expectedTime = DateTime.UtcNow;
            _timeMaster.SetTime(expectedTime);
            _pomodoro.StartNext();

            //when
            _timeMaster.FinishLatestInterval();
            
            //then
            var intervalFinishedEventArgs = _eventHelper.FinishedIntervals.First();
            Assert.That(intervalFinishedEventArgs.UtcFinishedAt, Is.EqualTo(expectedTime));
        }
    }
}
