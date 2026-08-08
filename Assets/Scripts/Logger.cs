using UnityEngine;
using System.Diagnostics;

public static class Logger
{
    // Global flag to enable or disable log output completely
    public static bool EnableLog = true;

    // Log level enumeration
    public enum LogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    // Minimum log level required to be printed
    public static LogLevel MinimumLogLevel = LogLevel.Info;

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void Log(object message, Object context = null)
    {
        if (!EnableLog || MinimumLogLevel > LogLevel.Info)
        {
            return;
        }

        UnityEngine.Debug.Log(message, context);
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message, Object context = null)
    {
        if (!EnableLog || MinimumLogLevel > LogLevel.Warning)
        {
            return;
        }

        UnityEngine.Debug.LogWarning(message, context);
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogError(object message, Object context = null)
    {
        if (!EnableLog || MinimumLogLevel > LogLevel.Error)
        {
            return;
        }

        UnityEngine.Debug.LogError(message, context);
    }

    /// <summary>
    /// Logs a message with a specific log level.
    /// </summary>
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void Log(LogLevel level, object message, Object context = null)
    {
        if (!EnableLog || MinimumLogLevel > level)
        {
            return;
        }

        switch (level)
        {
            case LogLevel.Info:
                UnityEngine.Debug.Log(message, context);
                break;
            case LogLevel.Warning:
                UnityEngine.Debug.LogWarning(message, context);
                break;
            case LogLevel.Error:
                UnityEngine.Debug.LogError(message, context);
                break;
        }
    }
}