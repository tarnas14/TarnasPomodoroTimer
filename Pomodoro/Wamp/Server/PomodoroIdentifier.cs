namespace Pomodoro.Wamp.Server
{
    public class PomodoroIdentifier
    {
        public int Id { get; private set; }

        public PomodoroIdentifier(int id)
        {
            Id = id;
        }

        public string GetTopic(TopicType type)
        {
            return string.Format("com.tarnas.pomodoro.{0}.{1}", type, Id);
        }
    }

    public enum TopicType
    {
        started,
        interrupted
    }
}