using System;

namespace Scraps
{
    public enum LogLevel
    {
        //Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5
    }

    public interface ILogger
    {
        void Log(LogLevel level, object message);
        void Log(LogLevel level, string format, params object[] arguments);
        void Log(Exception exception);
    }

    public interface ILogManager
    {
        ILogger GetLogger(string name);
    }

    public static class LogExtensions
    {
        public static ILogger GetLogger<T>(this ILogManager logManager)
        {
            return logManager.GetLogger(typeof(T).Name);
        }

        public static void Debug(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Debug, message);
        }

        public static void Debug(this ILogger logger, string format, params object[] args)
        {
            logger.Log(LogLevel.Debug, format, args);
        }

        public static void Info(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Info, message);
        }

        public static void Info(this ILogger logger, string format, params object[] args)
        {
            logger.Log(LogLevel.Info, format, args);
        }

        public static void Warn(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Warning, message);
        }

        public static void Warn(this ILogger logger, string format, params object[] args)
        {
            logger.Log(LogLevel.Warning, format, args);
        }

        public static void Error(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Error, message);
        }

        public static void Error(this ILogger logger, string format, params object[] args)
        {
            logger.Log(LogLevel.Error, format, args);
        }
    }
}