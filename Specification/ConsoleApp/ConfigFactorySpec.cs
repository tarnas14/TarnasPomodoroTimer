namespace Specification.ConsoleApp
{
    using System;
    using global::ConsoleApp;
    using NUnit.Framework;
    using Pomodoro;

    [TestFixture]
    class ConfigFactorySpec
    {
        [Test]
        public void ShouldCreateConfigFrom4IntValues()
        {
            //given
            var args = new []{"25", "5", "15", "4"};

            var expectedConfig = new PomodoroConfig
            {
                LongBreak = TimeSpan.FromMinutes(15),
                Productivity = TimeSpan.FromMinutes(25),
                ShortBreak = TimeSpan.FromMinutes(5),
                LongBreakAfter = 4
            };

            //when
            var factory = new ConfigFactory();
            var actualConfig = factory.GetConfig(args);

            //then
            Assert.That(expectedConfig.LongBreak, Is.EqualTo(actualConfig.LongBreak));
            Assert.That(expectedConfig.ShortBreak, Is.EqualTo(actualConfig.ShortBreak));
            Assert.That(expectedConfig.LongBreakAfter, Is.EqualTo(actualConfig.LongBreakAfter));
            Assert.That(expectedConfig.Productivity, Is.EqualTo(actualConfig.Productivity));
        }
    }
}
