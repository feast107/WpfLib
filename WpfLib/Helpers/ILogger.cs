namespace WpfLib.Helpers
{
    public interface ILogger
    {
        void Log(object message);
        void Debug(object message);
        void Info(object message);
        void Warn(object message);
        void Error(object message);
        void Fatal(object message);
    }
}
