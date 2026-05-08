using System.Text;
using UnityEngine;
using YoloDev.Text;
using ZeepkistClient;
using ZeepSDK.Chat;

namespace YoloDev.Zeepkist;

/// <summary>
///     Provides methods for interacting with the Zeepkist message system.
/// </summary>
public class MessageApi
{
    /// <summary>
    ///     Removes the current server message if in a valid game state.
    /// </summary>
    public static void RemoveServerMessage()
    {
        if (ZeepkistNetwork.NetworkClient != null && ZeepkistNetwork.CurrentLobby?.GameState == 1)
        {
            ChatApi.SendMessage("/servermessage remove");
        }
    }

    /// <summary>
    ///     Sets a server message with a specified color and duration.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="color">The color of the message.</param>
    /// <param name="duration">The duration in seconds (0 for default).</param>
    public static void SetServerMessage(string message, MessageColor color = MessageColor.White, int duration = 0)
    {
        if (ZeepkistNetwork.NetworkClient != null && ZeepkistNetwork.CurrentLobby?.GameState == 1)
        {
            ChatApi.SendMessage($"/servermessage {color} {duration} {message}");
        }
    }

    /// <summary>
    ///     Sets a server message with a specified Unity Color and duration.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="color">The Unity Color to use.</param>
    /// <param name="duration">The duration in seconds (0 for default).</param>
    public static void SetServerMessage(string message, Color color, int duration = 0)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"<#{color.ToHexRgba()}>");
        sb.Append(message);
        sb.Append("</color>");
        message = sb.ToString();

        SetServerMessage(message, MessageColor.White, duration);
    }

    /// <summary>
    ///     Sends a private custom chat message to specific Steam IDs.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="tag">The tag to display (default "HOST").</param>
    /// <param name="steamIds">The list of Steam IDs to send the message to.</param>
    public static void SendPrivateCustomChatMessage(string message, string tag = "HOST", params ulong[] steamIds)
    {
        if (ZeepkistNetwork.NetworkClient != null)
        {
            return;
        }

        foreach (ulong steamId in steamIds)
        {
            ZeepkistNetwork.SendCustomChatMessage(false, steamId, message, tag);
        }
    }

    /// <summary>
    ///     Sends a broadcast custom chat message to all players.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="tag">The tag to display (default "HOST").</param>
    public static void SendBroadcastCustomChatMessageTo(string message, string tag = "HOST")
    {
        if (ZeepkistNetwork.NetworkClient != null)
        {
            return;
        }

        ZeepkistNetwork.SendCustomChatMessage(true, 0, message, tag);
    }

    /// <summary>
    ///     Sets the join message for the current lobby.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="color">The color of the message.</param>
    public static void SetJoinMessage(string message, MessageColor color = MessageColor.White)
    {
        ChatApi.SendMessage($"/joinmessage {color} {message}");
    }

    /// <summary>
    ///     Removes/disables the join message for the current lobby.
    /// </summary>
    public static void RemoveJoinMessage()
    {
        ChatApi.SendMessage("/joinmessage disable");
    }
}

/// <summary>
///     Represents supported message colors in Zeepkist.
/// </summary>
public enum MessageColor
{
    /// <summary>Red color.</summary>
    Red,
    /// <summary>Orange color.</summary>
    Orange,
    /// <summary>Yellow color.</summary>
    Yellow,
    /// <summary>Blue color.</summary>
    Blue,
    /// <summary>Green color.</summary>
    Green,
    /// <summary>Pink color.</summary>
    Pink,
    /// <summary>Purple color.</summary>
    Purple,
    /// <summary>Black color.</summary>
    Black,
    /// <summary>White color.</summary>
    White
}