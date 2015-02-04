namespace Pomodoro
{
    public class SystemTimeMasterFactory : TimeMasterFactory
    {
        public TimeMaster GetTimeMaster()
        {
            return new SystemTimeMaster();
        }
    }
}