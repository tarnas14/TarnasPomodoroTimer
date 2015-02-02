namespace Pomodoro.Wamp.Server
{
    internal class PomodoroIdentificator
    {
        public int Id { get; private set; }

        public PomodoroIdentificator(int id)
        {
            Id = id;
        }
    }
}