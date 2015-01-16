namespace Pomodoro
{
    using System;

    public interface TimeMaster
    {
        void Pass(TimeSpan timeInterval);
    }
}
