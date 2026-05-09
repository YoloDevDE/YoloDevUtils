using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ZeepUtils.Text;

/// <summary>
///     A fluent builder for creating TMP rich text strings with tags, styling and inline formatting.
/// </summary>
public class RichText
{
    /// <summary>
    ///     Specifies text alignment within the builder.
    /// </summary>
    public enum AlignmentType
    {
        /// <summary>Aligns text to the left.</summary>
        Left,

        /// <summary>Aligns text to the center.</summary>
        Center,

        /// <summary>Aligns text to the right.</summary>
        Right,

        /// <summary>Justifies the text.</summary>
        Justify,

        /// <summary>Flushes the text.</summary>
        Flush
    }

    public enum FontType
    {
        Default,
        Anton,
        Bangers,
        LiberationSans,
        ElectronicHighwaySign,
        OswaldBold,
        RobotoBold
    }

    /// <summary>
    ///     Specifies which side of the margin to apply.
    /// </summary>
    public enum MarginType
    {
        /// <summary>Apply margin to the left side.</summary>
        Left,

        /// <summary>Apply margin to the right side.</summary>
        Right,

        /// <summary>Apply margin to both sides.</summary>
        Both
    }

    /// <summary>
    ///     Specifies the unit type for various tags.
    /// </summary>
    public enum UnitType
    {
        /// <summary>A positive relative value.</summary>
        Plus,

        /// <summary>A percentage value.</summary>
        Percent,

        /// <summary>Pixels value.</summary>
        Pixels,

        /// <summary>Em units value.</summary>
        Em,

        /// <summary>No unit specified.</summary>
        None
    }

    private struct Tag
    {
        public string Open;
        public string Close;
    }

    private readonly List<Tag> _tags = new();
    private readonly List<Tag> _innerTags = new();
    private readonly StringBuilder _content = new StringBuilder();

    /// <summary>
    ///     Initializes a new instance of the <see cref="RichText" /> class.
    /// </summary>
    public RichText() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RichText" /> class with initial text.
    /// </summary>
    /// <param name="text">The initial text to add.</param>
    public RichText(string text)
    {
        Append(text);
    }

    private static string AlignmentTypeToString(AlignmentType align) => align.ToString().ToLowerInvariant();

    private static string MarginTypeToString(MarginType marginType) =>
        marginType switch
        {
            MarginType.Left => "margin-left",
            MarginType.Right => "margin-right",
            MarginType.Both => "margin",
            _ => throw new ArgumentOutOfRangeException(nameof(marginType), marginType, null)
        };

    private static string FontTypeToString(FontType font)
    {
        return font switch
        {
            FontType.Anton => "Anton SDF",
            FontType.Bangers => "Bangers SDF",
            FontType.LiberationSans => "LiberationSans SDF",
            FontType.ElectronicHighwaySign => "ELECTRONIC HIGHWAY SIGN SDF",
            FontType.OswaldBold => "Oswald Bold SDF",
            FontType.RobotoBold => "RobotoBold SDF",
            _ => throw new ArgumentOutOfRangeException(nameof(font), font, null)
        };
    }

    private static string WrapWithUnitType(string value, UnitType unitType)
    {
        return unitType switch
        {
            UnitType.Plus => $"+{value}",
            UnitType.Percent => $"{value}%",
            UnitType.Pixels => $"{value}px",
            UnitType.Em => $"{value}em",
            _ => value
        };
    }

    /// <summary>Appends raw text to the builder.</summary>
    /// <param name="text">The text to append.</param>
    /// <returns>The builder instance.</returns>
    /// <example><code>builder.Append("Hello").Build(); // "Hello"</code></example>
    public RichText Append(string text)
    {
        _content.Append(text);
        return this;
    }

    /// <summary>Appends a custom sub-builder with text.</summary>
    /// <param name="text">The initial text for the sub-builder.</param>
    /// <param name="customizer">An action to customize the sub-builder.</param>
    /// <returns>The builder instance.</returns>
    public RichText Append(string text, Action<RichText> customizer)
    {
        RichText builder = new RichText().Append(text);
        customizer(builder);
        _content.Append(builder.Build());
        return this;
    }

    /// <summary>Appends a custom sub-builder.</summary>
    /// <param name="customizer">An action to customize the sub-builder.</param>
    /// <returns>The builder instance.</returns>
    public RichText Append(Action<RichText> customizer)
    {
        RichText builder = new RichText();
        customizer(builder);
        _content.Append(builder.Build());
        return this;
    }

    /// <summary>Appends an existing builder's content.</summary>
    /// <param name="builder">The builder whose content to append.</param>
    /// <returns>The builder instance.</returns>
    public RichText Append(RichText builder)
    {
        _content.Append(builder.Build());
        return this;
    }

    /// <summary>Appends raw text to the builder. Alias for Append.</summary>
    /// <param name="text">The text to append.</param>
    /// <returns>The builder instance.</returns>
    public RichText AddLayer(string text) => Append(text);

    /// <summary>Appends a custom sub-builder with text. Alias for Append.</summary>
    /// <param name="text">The initial text for the sub-builder.</param>
    /// <param name="customizer">An action to customize the sub-builder.</param>
    /// <returns>The builder instance.</returns>
    public RichText AddLayer(string text, Action<RichText> customizer) => Append(text, customizer);

    /// <summary>Appends a custom sub-builder. Alias for Append.</summary>
    /// <param name="customizer">An action to customize the sub-builder.</param>
    /// <returns>The builder instance.</returns>
    public RichText AddLayer(Action<RichText> customizer) => Append(customizer);

    /// <summary>Appends an existing builder's content. Alias for Append.</summary>
    /// <param name="builder">The builder whose content to append.</param>
    /// <returns>The builder instance.</returns>
    public RichText AddLayer(RichText builder) => Append(builder);

    /// <summary>Makes the content bold.</summary>
    /// <returns>The builder instance.</returns>
    /// <example><code>builder.Append("Hello").Bold().Build(); // "&lt;b&gt;Hello&lt;/b&gt;"</code></example>
    public RichText Bold() => SurroundWithTag("b");

    /// <summary>Appends bold text to the builder.</summary>
    /// <param name="text">The text to append.</param>
    /// <returns>The builder instance.</returns>
    /// <example><code>builder.Bold("Hello").Build(); // "&lt;b&gt;Hello&lt;/b&gt;"</code></example>
    public RichText Bold(string text) => Append(text, b => b.Bold());

    /// <summary>Makes the content italic.</summary>
    /// <returns>The builder instance.</returns>
    public RichText Italic() => SurroundWithTag("i");

    /// <summary>Appends italic text to the builder.</summary>
    /// <param name="text">The text to append.</param>
    /// <returns>The builder instance.</returns>
    public RichText Italic(string text) => Append(text, b => b.Italic());

    /// <summary>Prevents line breaks within the content.</summary>
    /// <returns>The builder instance.</returns>
    public RichText NoBreak() => SurroundWithTag("nobr");

    /// <summary>Adds a manual line break.</summary>
    /// <returns>The builder instance.</returns>
    public RichText Break() => AddLayer("<br>");

    /// <summary>Prevents TMP from parsing tags within the content.</summary>
    /// <returns>The builder instance.</returns>
    public RichText NoParse() => SurroundWithInnerTag("noparse");

    /// <summary>Underlines the content.</summary>
    /// <returns>The builder instance.</returns>
    public RichText Underline() => SurroundWithTag("u");

    /// <summary>Appends underlined text to the builder.</summary>
    /// <param name="text">The text to append.</param>
    /// <returns>The builder instance.</returns>
    public RichText Underline(string text) => Append(text, b => b.Underline());

    /// <summary>Strikes through the content.</summary>
    /// <returns>The builder instance.</returns>
    public RichText StrikeThrough() => SurroundWithTag("s");

    /// <summary>Appends strike-through text to the builder.</summary>
    /// <param name="text">The text to append.</param>
    /// <returns>The builder instance.</returns>
    public RichText StrikeThrough(string text) => Append(text, b => b.StrikeThrough());

    /// <summary>Converts the content to all caps.</summary>
    /// <returns>The builder instance.</returns>
    public RichText AllCaps() => SurroundWithTag("allcaps");

    /// <summary>Converts the content to upper case.</summary>
    /// <returns>The builder instance.</returns>
    public RichText UpperCase() => AllCaps();

    /// <summary>Converts the content to small caps.</summary>
    /// <returns>The builder instance.</returns>
    public RichText SmallCaps() => SurroundWithTag("smallcaps");

    /// <summary>Converts the content to lower case.</summary>
    /// <returns>The builder instance.</returns>
    public RichText LowerCase() => SurroundWithTag("lowercase");

    /// <summary>
    ///     Sets the global line height.
    /// </summary>
    /// <param name="height">The height value.</param>
    /// <param name="type">The unit type for the height.</param>
    /// <returns>The builder instance.</returns>
    public RichText LineHeight(int height, UnitType type = UnitType.Percent) => SurroundWithTag("line-height", height.ToString(), type);

    /// <summary>
    ///     Sets the position offset for the next character.
    /// </summary>
    /// <param name="pos">The position value.</param>
    /// <param name="type">The unit type for the position.</param>
    /// <returns>The builder instance.</returns>
    public RichText Pos(int pos, UnitType type = UnitType.Percent)
    {
        _content.Append($"<pos={WrapWithUnitType(pos.ToString(), type)}>");
        return this;
    }

    /// <summary>
    ///     Sets indentation for the content.
    /// </summary>
    /// <param name="indent">The indentation value.</param>
    /// <param name="type">The unit type for the indentation.</param>
    /// <returns>The builder instance.</returns>
    public RichText Indent(int indent, UnitType type = UnitType.Percent)
    {
        _content.Append($"<indent={WrapWithUnitType(indent.ToString(), type)}>");
        return this;
    }

    private RichText SurroundWithTag(string tag)
    {
        tag = tag.ToLowerInvariant();
        _tags.Add(new Tag { Open = $"<{tag}>", Close = $"</{tag}>" });
        return this;
    }

    private RichText SurroundWithInnerTag(string tag)
    {
        tag = tag.ToLowerInvariant();
        _innerTags.Add(new Tag { Open = $"<{tag}>", Close = $"</{tag}>" });
        return this;
    }

    private RichText SurroundWithTag(string tag, string value, UnitType type)
    {
        tag = tag.ToLowerInvariant();
        string val = WrapWithUnitType(value.Trim(), type);
        _tags.Add(new Tag { Open = $"<{tag}={val}>", Close = $"</{tag}>" });
        return this;
    }

    private RichText SurroundWithTag(string tag, string value) => SurroundWithTag(tag, value, UnitType.None);

    /// <summary>Adds character spacing.</summary>
    /// <param name="cspace">The spacing value.</param>
    /// <param name="type">The unit type for the spacing.</param>
    /// <returns>The builder instance.</returns>
    public RichText CSpace(int cspace, UnitType type) => SurroundWithTag("cspace", cspace.ToString(), type);

    /// <summary>Adds character spacing in percent.</summary>
    /// <param name="cspace">The spacing value.</param>
    /// <returns>The builder instance.</returns>
    public RichText CSpace(int cspace) => CSpace(cspace, UnitType.Percent);

    /// <summary>Adds horizontal space.</summary>
    /// <param name="space">The space value.</param>
    /// <param name="type">The unit type for the space.</param>
    /// <returns>The builder instance.</returns>
    public RichText Space(int space, UnitType type) => AddLayer($"<space={WrapWithUnitType(space.ToString(), type)}>");

    /// <summary>Adds horizontal space in percent.</summary>
    /// <param name="space">The space value.</param>
    /// <returns>The builder instance.</returns>
    public RichText Space(int space) => Space(space, UnitType.Percent);

    /// <summary>Adds a single space character.</summary>
    /// <returns>The builder instance.</returns>
    public RichText Space() => AddLayer(" ");

    /// <summary>Sets the vertical offset.</summary>
    /// <param name="voffset">The offset value.</param>
    /// <param name="type">The unit type for the offset.</param>
    /// <returns>The builder instance.</returns>
    public RichText VOffset(int voffset, UnitType type = UnitType.Percent) => SurroundWithTag("voffset", voffset.ToString(), type);

    /// <summary>Sets monospacing.</summary>
    /// <param name="mspace">The spacing value.</param>
    /// <param name="type">The unit type for the spacing.</param>
    /// <returns>The builder instance.</returns>
    public RichText MSpace(int mspace, UnitType type = UnitType.Percent) => SurroundWithTag("mspace", mspace.ToString(), type);

    /// <summary>Sets monospacing.</summary>
    /// <param name="mspace">The spacing value.</param>
    /// <param name="type">The unit type for the spacing.</param>
    /// <returns>The builder instance.</returns>
    public RichText MSpace(double mspace, UnitType type = UnitType.Percent) => SurroundWithTag("mspace", mspace.ToString(CultureInfo.InvariantCulture), type);

    /// <summary>Sets the font size.</summary>
    /// <param name="size">The size value.</param>
    /// <param name="type">The unit type for the size.</param>
    /// <returns>The builder instance.</returns>
    public RichText Size(int size, UnitType type = UnitType.Percent) => SurroundWithTag("size", size.ToString(), type);

    /// <summary>Appends text with a specific font size.</summary>
    /// <param name="text">The text to append.</param>
    /// <param name="size">The size value.</param>
    /// <param name="type">The unit type for the size.</param>
    /// <returns>The builder instance.</returns>
    public RichText Size(string text, int size, UnitType type = UnitType.Percent) => Append(text, b => b.Size(size, type));

    /// <summary>Sets the font using the <see cref="FontType" /> enum and optionally the material.</summary>
    /// <param name="font">The font type.</param>
    /// <param name="material">The material name.</param>
    /// <returns>The builder instance.</returns>
    public RichText Font(FontType font, string material = null)
    {
        if (font == FontType.Default)
        {
            return this;
        }

        return Font(FontTypeToString(font), material);
    }

    /// <summary>Sets the font and optionally the material.</summary>
    /// <param name="font">The font name.</param>
    /// <param name="material">The material name.</param>
    /// <returns>The builder instance.</returns>
    public RichText Font(string font, string material = null)
    {
        string open = string.IsNullOrEmpty(material)
            ? $"<font=\"{font}\">"
            : $"<font=\"{font}\" material=\"{material}\">";

        _tags.Add(new Tag { Open = open, Close = "</font>" });
        return this;
    }

    /// <summary>Sets the font weight.</summary>
    /// <param name="weight">The font weight value.</param>
    /// <returns>The builder instance.</returns>
    public RichText FontWeight(int weight) => SurroundWithTag("fontweight", weight.ToString());

    /// <summary>Sets the text alignment.</summary>
    /// <param name="alignment">The alignment type.</param>
    /// <returns>The builder instance.</returns>
    public RichText Align(AlignmentType alignment) => SurroundWithTag("align", AlignmentTypeToString(alignment));

    /// <summary>Sets the margin.</summary>
    /// <param name="margin">The margin value.</param>
    /// <param name="type">The unit type for the margin.</param>
    /// <param name="marginType">The side(s) to apply the margin to.</param>
    /// <returns>The builder instance.</returns>
    public RichText Margin(int margin, UnitType type, MarginType marginType) => SurroundWithTag(MarginTypeToString(marginType), margin.ToString(), type);

    /// <summary>Sets the margin on both sides.</summary>
    /// <param name="margin">The margin value.</param>
    /// <param name="type">The unit type for the margin.</param>
    /// <returns>The builder instance.</returns>
    public RichText Margin(int margin, UnitType type) => Margin(margin, type, MarginType.Both);

    /// <summary>Sets the margin on both sides in percent.</summary>
    /// <param name="margin">The margin value.</param>
    /// <returns>The builder instance.</returns>
    public RichText Margin(int margin) => Margin(margin, UnitType.Percent, MarginType.Both);

    /// <summary>Sets the indentation.</summary>
    /// <param name="indent">The indentation value.</param>
    /// <param name="type">The unit type for the indentation.</param>
    /// <returns>The builder instance.</returns>
    public RichText Indent(string indent, UnitType type) => SurroundWithTag("indent", indent, type);

    /// <summary>Sets the indentation in percent.</summary>
    /// <param name="indent">The indentation value.</param>
    /// <returns>The builder instance.</returns>
    public RichText Indent(string indent) => Indent(indent, UnitType.Percent);

    /// <summary>Sets the line indentation.</summary>
    /// <param name="indent">The indentation value.</param>
    /// <param name="type">The unit type for the indentation.</param>
    /// <returns>The builder instance.</returns>
    public RichText LineIndent(string indent, UnitType type) => SurroundWithTag("line-indent", indent, type);

    /// <summary>Sets the line indentation in percent.</summary>
    /// <param name="indent">The indentation value.</param>
    /// <returns>The builder instance.</returns>
    public RichText LineIndent(string indent) => LineIndent(indent, UnitType.Percent);

    /// <summary>
    ///     Highlights the text with a background color.
    /// </summary>
    /// <param name="color">The color to use for highlighting.</param>
    /// <returns>The builder instance.</returns>
    public RichText Mark(string color)
    {
        string hex = ColorUtils.ParseOrDefault(color).ToMinimizedHex();
        return SurroundWithTag("mark", hex);
    }

    /// <summary>
    ///     Applies a color tag to the text.
    /// </summary>
    /// <param name="color">The color to apply (hex or name).</param>
    /// <returns>The builder instance.</returns>
    /// <example><code>builder.Append("Red").Color("red").Build(); // "&lt;#f00&gt;Red&lt;/color&gt;"</code></example>
    public RichText Color(string color)
    {
        string hex = ColorUtils.ParseOrDefault(color).ToMinimizedHex();
        _tags.Add(new Tag { Open = $"<{hex}>", Close = "</color>" });
        return this;
    }

    /// <summary>Appends colored text to the builder.</summary>
    /// <param name="text">The text to append.</param>
    /// <param name="color">The color to apply.</param>
    /// <returns>The builder instance.</returns>
    /// <example><code>builder.Color("Blue", "#00f").Build(); // "&lt;#00f&gt;Blue&lt;/color&gt;"</code></example>
    public RichText Color(string text, string color) => Append(text, b => b.Color(color));

    /// <summary>
    ///     Applies a color tag using a Unity Color.
    /// </summary>
    /// <param name="color">The color to apply.</param>
    /// <returns>The builder instance.</returns>
    public RichText Color(Color color) => Color(color.ToMinimizedHex());

    /// <summary>Appends colored text using a Unity Color.</summary>
    /// <param name="text">The text to append.</param>
    /// <param name="color">The color to apply.</param>
    /// <returns>The builder instance.</returns>
    public RichText Color(string text, Color color) => Append(text, b => b.Color(color));

    /// <summary>
    ///     Applies a character-by-character gradient across the current content.
    ///     Optimizes output by grouping characters with the same color.
    /// </summary>
    /// <param name="colors">The color stops for the gradient.</param>
    /// <returns>The builder instance.</returns>
    /// <example><code>builder.Append("Rainbow").ColorGradient("red", "green", "blue").Build();</code></example>
    public RichText ColorGradient(params string[] colors)
    {
        if (colors == null || colors.Length == 0)
        {
            return this;
        }

        List<Color32> stops = colors
                              .Select(c => (Color32)ColorUtils.ParseOrDefault(c))
                              .ToList();

        if (stops.Count == 1)
        {
            return Color(((Color)stops[0]).ToMinimizedHex());
        }

        string text = _content.ToString();
        if (string.IsNullOrEmpty(text))
        {
            return this;
        }

        int realChars = CountVisibleCharacters(text);
        if (realChars == 0)
        {
            return this;
        }

        int segments = stops.Count - 1;
        StringBuilder rebuilt = new StringBuilder();
        int visibleIndex = 0;
        string lastHex = null;

        for (int i = 0; i < text.Length; i++)
        {
            char ch = text[i];

            if (ch == '<')
            {
                if (lastHex != null)
                {
                    rebuilt.Append("</color>");
                    lastHex = null;
                }

                while (i < text.Length && text[i] != '>')
                {
                    rebuilt.Append(text[i]);
                    i++;
                }

                rebuilt.Append('>');
                continue;
            }

            float pos = segments == 0 ? 0f : (float)visibleIndex / Math.Max(1, realChars - 1);
            float segFloat = pos * segments;
            int segIndex = Mathf.Clamp((int)Mathf.Floor(segFloat), 0, segments - 1);
            float t = Mathf.Clamp01(segFloat - segIndex);

            Color32 a = stops[segIndex];
            Color32 b = stops[segIndex + 1];

            Color c = new Color32(
                (byte)(a.r + (b.r - a.r) * t),
                (byte)(a.g + (b.g - a.g) * t),
                (byte)(a.b + (b.b - a.b) * t),
                (byte)(a.a + (b.a - a.a) * t)
            );

            string hex = c.ToMinimizedHex();

            if (hex != lastHex)
            {
                if (lastHex != null)
                {
                    rebuilt.Append("</color>");
                }

                rebuilt.Append($"<{hex}>");
                lastHex = hex;
            }

            rebuilt.Append(ch);
            visibleIndex++;
        }

        if (lastHex != null)
        {
            rebuilt.Append("</color>");
        }

        _content.Clear();
        _content.Append(rebuilt);
        return this;
    }

    private static int CountVisibleCharacters(string text)
    {
        int count = 0;
        bool inTag = false;
        foreach (char c in text)
        {
            if (c == '<') inTag = true;
            else if (c == '>') inTag = false;
            else if (!inTag) count++;
        }
        return count;
    }

    /// <summary>
    ///     Builds the final TMP rich text string.
    /// </summary>
    /// <returns>The final rich text string.</returns>
    public string Build()
    {
        if (_tags.Count == 0 && _innerTags.Count == 0)
        {
            return _content.ToString();
        }

        StringBuilder result = new StringBuilder();

        // 1. Add opening tags (Outermost to Innermost)
        foreach (Tag tag in _tags) result.Append(tag.Open);
        foreach (Tag tag in _innerTags) result.Append(tag.Open);

        // 2. Add content
        result.Append(_content);

        // 3. Add closing tags in reverse order (Innermost to Outermost)
        for (int i = _innerTags.Count - 1; i >= 0; i--) result.Append(_innerTags[i].Close);
        for (int i = _tags.Count - 1; i >= 0; i--) result.Append(_tags[i].Close);

        return result.ToString();
    }

    /// <summary>
    ///     Returns the final rich text string. Alias for Build().
    /// </summary>
    /// <returns>The final rich text string.</returns>
    public override string ToString() => Build();

    /// <summary>
    ///     Clears the builder's content and tags.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public RichText Clear()
    {
        _content.Clear();
        _tags.Clear();
        _innerTags.Clear();
        return this;
    }
}
