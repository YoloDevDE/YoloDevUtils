using UnityEngine;

namespace YoloDev.Text;

public static class ColorUtils
{
    // ─────────────────────────────────────────────────────────────
    // MAIN SAFE HELPERS
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    ///     Parses any HTML hex string (#RGB, #RRGGBB, #RRGGBBAA, etc.).
    ///     Always returns a valid color (fallback = white).
    /// </summary>
    /// <param name="input">The HTML hex string to parse.</param>
    /// <param name="fallback">The fallback color if parsing fails. Defaults to white.</param>
    /// <returns>The parsed <see cref="Color"/> or the fallback.</returns>
    public static Color ParseOrDefault(string input, Color? fallback = null)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return fallback ?? Color.white;
        }

        if (ColorUtility.TryParseHtmlString(input, out Color c))
        {
            return c;
        }

        return fallback ?? Color.white;
    }

    /// <summary>
    ///     Create a color using RGB 0-255.
    /// </summary>
    /// <param name="r">Red component (0-255).</param>
    /// <param name="g">Green component (0-255).</param>
    /// <param name="b">Blue component (0-255).</param>
    /// <param name="a">Alpha component (0-1).</param>
    /// <returns>A new <see cref="Color"/>.</returns>
    public static Color FromRGB(int r, int g, int b, float a = 1f) => new Color(r / 255f, g / 255f, b / 255f, Mathf.Clamp01(a));

    /// <summary>
    ///     Create a color using RGBA 0–255.
    /// </summary>
    /// <param name="r">Red component (0-255).</param>
    /// <param name="g">Green component (0-255).</param>
    /// <param name="b">Blue component (0-255).</param>
    /// <param name="a">Alpha component (0-255).</param>
    /// <returns>A new <see cref="Color"/>.</returns>
    public static Color FromRgba(int r, int g, int b, int a) => new Color(r / 255f, g / 255f, b / 255f, a / 255f);

    /// <summary>
    ///     Returns a new color with modified alpha.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <param name="a">The new alpha value (0-1).</param>
    /// <returns>A new <see cref="Color"/> with the specified alpha.</returns>
    public static Color WithAlpha(this Color color, float a) => new Color(color.r, color.g, color.b, Mathf.Clamp01(a));

    /// <summary>
    ///     Multiply alpha (good for UI fading).
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <param name="factor">The factor to multiply alpha by.</param>
    /// <returns>A new <see cref="Color"/> with faded alpha.</returns>
    public static Color Fade(this Color color, float factor) => new Color(color.r, color.g, color.b, Mathf.Clamp01(color.a * factor));

    // ─────────────────────────────────────────────────────────────
    // HEX UTILITIES
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    ///     Converts a color to #RRGGBB.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>A hex string representing the color.</returns>
    public static string ToHexRGB(this Color color)
    {
        Color32 c = color;
        return $"#{c.r:X2}{c.g:X2}{c.b:X2}";
    }

    /// <summary>
    ///     Converts a color to #RRGGBBAA.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>A hex string representing the color with alpha.</returns>
    public static string ToHexRgba(this Color color)
    {
        Color32 c = color;
        return $"#{c.r:X2}{c.g:X2}{c.b:X2}{c.a:X2}";
    }

    // ─────────────────────────────────────────────────────────────
    // COLOR MANIPULATION
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    ///     Linearly blends 2 colors.
    /// </summary>
    /// <param name="a">The start color.</param>
    /// <param name="b">The end color.</param>
    /// <param name="t">The interpolation value (0-1).</param>
    /// <returns>The interpolated <see cref="Color"/>.</returns>
    public static Color Lerp(Color a, Color b, float t) => Color.Lerp(a, b, Mathf.Clamp01(t));

    /// <summary>
    ///     Make a color brighter by a factor.
    /// </summary>
    /// <param name="c">The source color.</param>
    /// <param name="factor">The factor to brighten by (0-1).</param>
    /// <returns>The brightened <see cref="Color"/>.</returns>
    public static Color Brighten(this Color c, float factor) => Color.Lerp(c, Color.white, Mathf.Clamp01(factor));

    /// <summary>
    ///     Make a color darker by a factor.
    /// </summary>
    /// <param name="c">The source color.</param>
    /// <param name="factor">The factor to darken by (0-1).</param>
    /// <returns>The darkened <see cref="Color"/>.</returns>
    public static Color Darken(this Color c, float factor) => Color.Lerp(c, Color.black, Mathf.Clamp01(factor));

    // ─────────────────────────────────────────────────────────────
    // RANDOM COLORS (USEFUL FOR DEBUGGING)
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    ///     Creates a random color with a specified alpha.
    /// </summary>
    /// <param name="a">The alpha value (0 to 1).</param>
    /// <returns>A random <see cref="Color"/>.</returns>
    public static Color RandomRGB(float a = 1f) => new Color(Random.value, Random.value, Random.value, Mathf.Clamp01(a));

    /// <summary>
    ///     Creates a random color with a random alpha.
    /// </summary>
    /// <returns>A random <see cref="Color"/> with random alpha.</returns>
    public static Color RandomRgba() => new Color(Random.value, Random.value, Random.value, Random.value);

    /// <summary>
    ///     Converts a Color to a minimized hex string.
    ///     Produces shortest TMP-compatible format: #RGB, #RGBA, #RRGGBB, #RRGGBBAA.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>A minimized hex string.</returns>
    public static string ToMinimizedHex(this Color color)
    {
        Color32 c = color;

        string r = c.r.ToString("X2").ToLower();
        string g = c.g.ToString("X2").ToLower();
        string b = c.b.ToString("X2").ToLower();
        string a = c.a.ToString("X2").ToLower();

        // Build full hex
        string full = r + g + b + (a == "ff" ? "" : a);

        // Try minimize pairs (rr gg bb aa)
        bool canShrink = true;
        for (int i = 0; i < full.Length; i += 2)
        {
            if (full[i] == full[i + 1])
            {
                continue;
            }

            canShrink = false;
            break;
        }

        if (!canShrink)
        {
            return "#" + full;
        }


        // shrink: rrggbb → rgb   or  rrggbbaa → rgba
        string shrunk = "";
        for (int i = 0; i < full.Length; i += 2)
        {
            shrunk += full[i];
        }

        // When opaque (alpha f) → drop alpha
        if (shrunk.Length == 4 && shrunk[3] == 'f')
        {
            shrunk = shrunk.Substring(0, 3);
        }

        return "#" + shrunk;


        // No shrinking possible → return full
    }
}