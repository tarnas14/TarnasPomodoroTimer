﻿namespace Pomodoro.Wamp.Server
{
    using System.Collections.Generic;
    using Timer;

    public class InMemoryPomodoroStore
    {
        private readonly IList<PomodoroTimer> _pomodoros;
        private readonly TimeMaster _timeMaster;

        public InMemoryPomodoroStore(TimeMaster timeMaster)
        {
            _timeMaster = timeMaster;
            _pomodoros = new List<PomodoroTimer>();
        }

        public PomodoroIdentificator SetupNewPomodoro(PomodoroConfig config)
        {
            var newTimer = new PomodoroTimer(_timeMaster, config);
            _pomodoros.Add(newTimer);

            return new PomodoroIdentificator(_pomodoros.Count);
        }

        public PomodoroTimer this[PomodoroIdentificator pomodoroId]
        {
            get { return _pomodoros[pomodoroId.Id - 1]; }
        }
    }
}