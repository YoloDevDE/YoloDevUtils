namespace ZeepUtils.Text;

/// <summary>
///     Contains constants for various TMP sprites used in Zeepkist.
/// </summary>
public static class Emojis
{
    public const string YannicScared = "<sprite=\"Zeepkist\" name=\"YannicScared\">";
    public const string YannicSmile = "<sprite=\"Zeepkist\" name=\"YannicSmile\">";
    public const string YannicEyes = "<sprite=\"Zeepkist\" name=\"YannicEyes\">";
    public const string Eyes = "<sprite=\"Zeepkist\" name=\"Eyes\">";
    public const string Smile = "<sprite=\"Zeepkist\" name=\"Smile\">";
    public const string Skull = "<sprite=\"Zeepkist\" name=\"Skull\">";
    public const string Amaze = "<sprite=\"Zeepkist\" name=\"Amaze\">";
    public const string Good = "<sprite=\"Zeepkist\" name=\"Good\">";
    public const string Sparkle = "<sprite=\"Zeepkist\" name=\"Sparkle\">";
    public const string Heart = "<sprite=\"Zeepkist\" name=\"Heart\">";
    public const string Love = "<sprite=\"Zeepkist\" name=\"Love\">";
    public const string Cry = "<sprite=\"Zeepkist\" name=\"Cry\">";
    public const string Party = "<sprite=\"Zeepkist\" name=\"Party\">";

    public const string CoolOrange = "<sprite=\"moremojis\" name=\"CoolOrange\">";
    public const string WhereMoney = "<sprite=\"moremojis\" name=\"WhereMoney\">";
    public const string RainbowMoney = "<sprite=\"moremojis\" name=\"RainbowMoney\">";
    public const string Money = "<sprite=\"moremojis\" name=\"Money\">";
    public const string ZaagBladPadRood2 = "<sprite=\"moremojis\" name=\"ZaagBladPadRood2\">";
    public const string ZaagBladPadRood = "<sprite=\"moremojis\" name=\"ZaagBladPadRood\">";
    public const string ZaagBladPad2 = "<sprite=\"moremojis\" name=\"ZaagBladPad2\">";
    public const string ZaagBladPad = "<sprite=\"moremojis\" name=\"ZaagBladPad\">";
    public const string Work = "<sprite=\"moremojis\" name=\"Work\">";
    public const string Euro = "<sprite=\"moremojis\" name=\"Euro\">";
    public const string Exclemation = "<sprite=\"moremojis\" name=\"Exclemation\">";
    public const string MoremojisLove = "<sprite=\"moremojis\" name=\"Love\">";
    public const string T = "<sprite=\"moremojis\" name=\"T\">";
    public const string S = "<sprite=\"moremojis\" name=\"S\">";
    public const string I = "<sprite=\"moremojis\" name=\"I\">";
    public const string K = "<sprite=\"moremojis\" name=\"K\">";
    public const string P = "<sprite=\"moremojis\" name=\"P\">";
    public const string E = "<sprite=\"moremojis\" name=\"E\">";
    public const string E2 = "<sprite=\"moremojis\" name=\"E2\">";
    public const string Z = "<sprite=\"moremojis\" name=\"Z\">";

    public const string Wisdom = "<sprite=\"Zeepkist2\" name=\"Wisdom\">";
    public const string OhNo = "<sprite=\"Zeepkist2\" name=\"OhNo\">";

    /// <summary>
    ///     Adds a TMP color attribute to an existing sprite tag.
    /// </summary>
    /// <example>
    ///     WithColor(Emojis.YannicSmile, "#FF0000FF") -> "&lt;sprite=\"Zeepkist\" name=\"YannicSmile\" color=#FF0000FF&gt;"
    /// </example>
    /// <param name="spriteTag">The sprite tag to modify.</param>
    /// <param name="color">The color to apply.</param>
    /// <returns>The modified sprite tag with the color attribute.</returns>
    public static string WithColor(string spriteTag, string color)
    {
        if (string.IsNullOrEmpty(spriteTag) || string.IsNullOrEmpty(color))
        {
            return spriteTag;
        }

        color = ColorUtils.ParseOrDefault(color).ToMinimizedHex();
        int index = spriteTag.IndexOf('>');
        return index < 0 ? spriteTag : spriteTag.Insert(index, $" color={color}");
    }
}
