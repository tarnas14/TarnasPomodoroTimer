namespace Specification
{
    using System.Collections.Generic;
    using Moq;
    using NUnit.Framework;
    using Pomodoro;

    [TestFixture]
    class PomodoroTimerSpec
    {
        [Test]
        public void ShouldStartWithFirstInterval()
        {
            //given
            var intervalMock1 = new Mock<ITimeInterval>();
            var intervalMock2 = new Mock<ITimeInterval>();
            var timer = new PomodoroTimer(new List<ITimeInterval>{ intervalMock1.Object, intervalMock2.Object});

            //when
            timer.StartNext();

            //then
            intervalMock1.Verify(mock => mock.Start(), Times.Once);
            intervalMock2.Verify(mock => mock.Start(), Times.Never);
        }

        [Test]
        public void ShouldStartNextIntervalAfterThePreviousOneHasEnded()
        {
            //given
            var intervalMock1 = new Mock<ITimeInterval>();
            var intervalMock2 = new Mock<ITimeInterval>();
            var timer = new PomodoroTimer(new List<ITimeInterval> { intervalMock1.Object, intervalMock2.Object });
            timer.StartNext();
            intervalMock1.Raise(interval => interval.Finished += null, new IntervalFinishedEventArgs());

            //when
            timer.StartNext();

            //then
            intervalMock1.Verify(mock => mock.Start(), Times.Once);
            intervalMock2.Verify(mock => mock.Start(), Times.Once);
        }

        [Test]
        [ExpectedException(typeof(IntervalInProgressException))]
        public void ShouldNotAllowStartingNextIntervalBeforePreviousEnds()
        {
            //given
            var intervalMock = new Mock<ITimeInterval>();
            var timer = new PomodoroTimer(new List<ITimeInterval> { intervalMock.Object });
            timer.StartNext();

            //when
            timer.StartNext();
        }
    }
}
