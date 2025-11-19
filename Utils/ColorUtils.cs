using UnityEngine;

namespace YoloDev.Utils;

public static class ColorUtils
{
    // ─────────────────────────────────────────────────────────────
    // MAIN SAFE HELPERS
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    ///     Parses any HTML hex string (#RGB, #RRGGBB, #RRGGBBAA, etc.).
    ///     Always returns a valid color (fallback = white).
    /// </summary>
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
    public static Color FromRGB(int r, int g, int b, float a = 1f) => new Color(r / 255f, g / 255f, b / 255f, Mathf.Clamp01(a));

    /// <summary>
    ///     Create a color using RGBA 0–255.
    /// </summary>
    public static Color FromRGBA(int r, int g, int b, int a) => new Color(r / 255f, g / 255f, b / 255f, a / 255f);

    /// <summary>
    ///     Returns a new color with modified alpha.
    /// </summary>
    public static Color WithAlpha(this Color color, float a) => new Color(color.r, color.g, color.b, Mathf.Clamp01(a));

    /// <summary>
    ///     Multiply alpha (good for UI fading).
    /// </summary>
    public static Color Fade(this Color color, float factor) => new Color(color.r, color.g, color.b, Mathf.Clamp01(color.a * factor));

    // ─────────────────────────────────────────────────────────────
    // HEX UTILITIES
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    ///     Converts a color to #RRGGBB.
    /// </summary>
    public static string ToHexRGB(this Color color)
    {
        Color32 c = color;
        return $"#{c.r:X2}{c.g:X2}{c.b:X2}";
    }

    /// <summary>
    ///     Converts a color to #RRGGBBAA.
    /// </summary>
    public static string ToHexRGBA(this Color color)
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
    public static Color Lerp(Color a, Color b, float t) => Color.Lerp(a, b, Mathf.Clamp01(t));

    /// <summary>
    ///     Make a color brighter by a factor.
    /// </summary>
    public static Color Brighten(this Color c, float factor) => Color.Lerp(c, Color.white, Mathf.Clamp01(factor));

    /// <summary>
    ///     Make a color darker by a factor.
    /// </summary>
    public static Color Darken(this Color c, float factor) => Color.Lerp(c, Color.black, Mathf.Clamp01(factor));

    // ─────────────────────────────────────────────────────────────
    // RANDOM COLORS (USEFUL FOR DEBUGGING)
    // ─────────────────────────────────────────────────────────────

    public static Color RandomRGB(float a = 1f) => new Color(Random.value, Random.value, Random.value, Mathf.Clamp01(a));

    public static Color RandomRGBA() => new Color(Random.value, Random.value, Random.value, Random.value);

    /// <summary>
    ///     Converts a Color to a minimized hex string.
    ///     Produces shortest TMP-compatible format: #RGB, #RGBA, #RRGGBB, #RRGGBBAA.
    /// </summary>
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