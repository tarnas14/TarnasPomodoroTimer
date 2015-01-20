namespace Pomodoro
{
    using System;

    public class PomodoroException : Exception
    {
        public PomodoroException(string message) : base(message)
        {
            
        }
    }
}