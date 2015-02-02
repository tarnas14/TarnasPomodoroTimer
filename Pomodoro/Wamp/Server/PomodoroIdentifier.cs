namespace Pomodoro.Wamp.Server
{
    public class PomodoroIdentifier
    {
        public int Id { get; private set; }

        public PomodoroIdentifier(int id)
        {
            Id = id;
        }
    }
}