using System;
using BepInEx.Logging;
using UnityEngine;
using ZeepSDK.Messaging;

namespace ZeepUtils.Zeepkist;

/// <summary>
///     Provides methods for displaying toast notifications in Zeepkist, with integrated BepInEx logging.
/// </summary>
public static class ToastNotification
{
    private static ITaggedMessenger _messenger;
    private static ManualLogSource _logger;

    /// <summary>
    ///     Gets the tag used by the messenger. Returns null if not initialized.
    /// </summary>
    public static string Tag => _messenger?.Tag;

    /// <summary>
    ///     Initializes the toast notification system with a logger and a tag.
    /// </summary>
    /// <param name="logger">The BepInEx <see cref="ManualLogSource" /> to use for logging.</param>
    /// <param name="tag">The tag for the messenger. If null, uses the logger's source name.</param>
    public static void Initialize(ManualLogSource logger, string tag = null)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        _logger = logger;
        _messenger = MessengerApi.CreateTaggedMessenger(tag ?? logger.SourceName);
    }

    private static void Log(LogLevel level, string message)
    {
        _logger?.Log(level, message);
    }

    /// <summary>
    ///     Displays an info toast notification.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="duration">The duration in seconds.</param>
    public static void Info(string message, float duration = 2.5f)
    {
        Log(LogLevel.Info, message);
        _messenger?.Log(message, duration);
    }

    /// <summary>
    ///     Displays a success toast notification.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="duration">The duration in seconds.</param>
    public static void Success(string message, float duration = 2.5f)
    {
        Log(LogLevel.Info, message);
        _messenger?.LogSuccess(message, duration);
    }

    /// <summary>
    ///     Displays a warning toast notification.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="duration">The duration in seconds.</param>
    public static void Warning(string message, float duration = 2.5f)
    {
        Log(LogLevel.Warning, message);
        _messenger?.LogWarning(message, duration);
    }

    /// <summary>
    ///     Displays an error toast notification.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="duration">The duration in seconds.</param>
    public static void Error(string message, float duration = 2.5f)
    {
        Log(LogLevel.Error, message);
        _messenger?.LogError(message, duration);
    }

    /// <summary>
    ///     Displays a custom toast notification with specified colors.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="textColor">The text color.</param>
    /// <param name="duration">The duration in seconds.</param>
    public static void Custom(string message,
        Color backgroundColor = default,
        Color textColor = default,
        float duration = 2.5f)
    {
        if (backgroundColor == default)
        {
            backgroundColor = Color.black;
        }

        if (textColor == default)
        {
            textColor = Color.white;
        }

        Log(LogLevel.Info, message);
        _messenger?.LogCustomColors(message, textColor, backgroundColor, duration);
    }
}
