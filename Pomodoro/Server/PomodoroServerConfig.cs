namespace Pomodoro.Server
{
    using Configuration;

    public class PomodoroServerConfig : Configuration<PomodoroServerConfig>
    {
        public string Server { get; set; }
        public string RealmName { get; set; }

        public PomodoroServerConfig Default
        {
            get
            {
                return new PomodoroServerConfig
                {
                    Server = "ws://127.0.0.1:8080/pomodoro",
                    RealmName = "PomodoroRealm"
                };
            }
        }
    }
}