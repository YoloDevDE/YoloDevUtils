using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YoloDev.Text;

/// <summary>
///     A fluent builder for creating TMP rich text strings with tags, styling and inline formatting.
/// </summary>
public class TMPRichTextBuilder
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

    private readonly List<string> _closingTags = [];
    private readonly StringBuilder _content = new StringBuilder();
    private readonly List<string> _innerClosingTags = [];
    private readonly List<string> _innerOpeningTags = [];
    private readonly List<string> _openingTags = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="TMPRichTextBuilder"/> class.
    /// </summary>
    public TMPRichTextBuilder() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TMPRichTextBuilder"/> class with initial text.
    /// </summary>
    /// <param name="text">The initial text to add.</param>
    public TMPRichTextBuilder(string text)
    {
        AddLayer(text);
    }

    private static string AlignmentTypeToString(AlignmentType align)
    {
        return align switch
        {
            AlignmentType.Left => "left",
            AlignmentType.Center => "center",
            AlignmentType.Right => "right",
            AlignmentType.Justify => "justify",
            AlignmentType.Flush => "flush",
            _ => throw new ArgumentOutOfRangeException(nameof(align), align, null)
        };
    }

    private static string MarginTypeToString(MarginType marginType)
    {
        return marginType switch
        {
            MarginType.Left => "margin-left",
            MarginType.Right => "margin-right",
            MarginType.Both => "margin",
            _ => throw new ArgumentOutOfRangeException(nameof(marginType), marginType, null)
        };
    }

    private static string WrapWithUnitType(string value, UnitType unitType)
    {
        return unitType switch
        {
            UnitType.Plus => "+" + value,
            UnitType.Percent => value + "%",
            UnitType.Pixels => value + "px",
            UnitType.Em => value + "em",
            _ => value
        };
    }

    /// <summary>Appends raw text to the builder.</summary>
    /// <param name="text">The text to append.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder AddLayer(string text)
    {
        _content.Append(text);
        return this;
    }

    /// <summary>Appends a custom sub-builder with text.</summary>
    /// <param name="text">The initial text for the sub-builder.</param>
    /// <param name="customizer">An action to customize the sub-builder.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder AddLayer(string text, Action<TMPRichTextBuilder> customizer)
    {
        TMPRichTextBuilder builder = new TMPRichTextBuilder().AddLayer(text);
        customizer(builder);
        _content.Append(builder.Build());
        return this;
    }

    /// <summary>Appends a custom sub-builder.</summary>
    /// <param name="customizer">An action to customize the sub-builder.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder AddLayer(Action<TMPRichTextBuilder> customizer)
    {
        TMPRichTextBuilder builder = new TMPRichTextBuilder();
        customizer(builder);
        _content.Append(builder.Build());
        return this;
    }

    /// <summary>Appends an existing builder's content.</summary>
    /// <param name="builder">The builder whose content to append.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder AddLayer(TMPRichTextBuilder builder)
    {
        _content.Append(builder.Build());
        return this;
    }

    /// <summary>Makes the content bold.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Bold() => SurroundWithTag("b");

    /// <summary>Makes the content italic.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Italic() => SurroundWithTag("i");

    /// <summary>Prevents line breaks within the content.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder NoBreak() => SurroundWithTag("nobr");

    /// <summary>Adds a manual line break.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Break() => AddLayer("<br>");

    /// <summary>Prevents TMP from parsing tags within the content.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder NoParse() => SurroundWithInnerTag("noparse");

    /// <summary>Underlines the content.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Underline() => SurroundWithTag("u");

    /// <summary>Strikes through the content.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder StrikeThrough() => SurroundWithTag("s");

    /// <summary>Converts the content to all caps.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder AllCaps() => SurroundWithTag("allcaps");

    /// <summary>Converts the content to upper case.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder UpperCase() => AllCaps();

    /// <summary>Converts the content to small caps.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder SmallCaps() => SurroundWithTag("smallcaps");

    /// <summary>Converts the content to lower case.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder LowerCase() => SurroundWithTag("lowercase");

    /// <summary>
    ///     Sets the global line height.
    /// </summary>
    /// <param name="height">The height value.</param>
    /// <param name="type">The unit type for the height.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder LineHeight(int height, UnitType type = UnitType.Percent)
    {
        _openingTags.Insert(0, $"<line-height={WrapWithUnitType(height.ToString(), type)}>");
        return this;
    }

    /// <summary>
    ///     Sets the position offset for the next character.
    /// </summary>
    /// <param name="pos">The position value.</param>
    /// <param name="type">The unit type for the position.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Pos(int pos, UnitType type = UnitType.Percent)
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
    public TMPRichTextBuilder Indent(int indent, UnitType type = UnitType.Percent)
    {
        _content.Append($"<indent={WrapWithUnitType(indent.ToString(), type)}>");
        return this;
    }

    private TMPRichTextBuilder SurroundWithTag(string tag)
    {
        _openingTags.Add($"<{tag}>");
        _closingTags.Add($"</{tag}>");
        return this;
    }

    private TMPRichTextBuilder SurroundWithInnerTag(string tag)
    {
        _innerOpeningTags.Add($"<{tag}>");
        _innerClosingTags.Add($"</{tag}>");
        return this;
    }

    private TMPRichTextBuilder SurroundWithTag(string tag, string value, UnitType type)
    {
        _openingTags.Add($"<{tag}={WrapWithUnitType(value.Trim(), type)}>");
        _closingTags.Add($"</{tag}>");
        return this;
    }

    private TMPRichTextBuilder SurroundWithTag(string tag, string value) => SurroundWithTag(tag, value, UnitType.None);

    /// <summary>Adds character spacing.</summary>
    /// <param name="cspace">The spacing value.</param>
    /// <param name="type">The unit type for the spacing.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder CSpace(int cspace, UnitType type) => SurroundWithTag("cspace", cspace.ToString(), type);

    /// <summary>Adds character spacing in percent.</summary>
    /// <param name="cspace">The spacing value.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder CSpace(int cspace) => CSpace(cspace, UnitType.Percent);

    /// <summary>Adds horizontal space.</summary>
    /// <param name="space">The space value.</param>
    /// <param name="type">The unit type for the space.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Space(int space, UnitType type) => AddLayer($"<space={WrapWithUnitType(space.ToString(), type)}>");

    /// <summary>Adds horizontal space in percent.</summary>
    /// <param name="space">The space value.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Space(int space) => Space(space, UnitType.Percent);

    /// <summary>Adds a single space character.</summary>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Space() => AddLayer(" ");

    /// <summary>Sets the vertical offset.</summary>
    /// <param name="voffset">The offset value.</param>
    /// <param name="type">The unit type for the offset.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder VOffset(int voffset, UnitType type = UnitType.Percent) => SurroundWithTag("voffset", voffset.ToString(), type);

    /// <summary>Sets monospacing.</summary>
    /// <param name="mspace">The spacing value.</param>
    /// <param name="type">The unit type for the spacing.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder MSpace(int mspace, UnitType type = UnitType.Percent) => SurroundWithTag("mspace", mspace.ToString(), type);

    /// <summary>Sets monospacing.</summary>
    /// <param name="mspace">The spacing value.</param>
    /// <param name="type">The unit type for the spacing.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder MSpace(double mspace, UnitType type = UnitType.Percent) => SurroundWithTag("mspace", mspace.ToString(CultureInfo.InvariantCulture), type);

    /// <summary>Sets the font size.</summary>
    /// <param name="size">The size value.</param>
    /// <param name="type">The unit type for the size.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Size(int size, UnitType type = UnitType.Percent) => SurroundWithTag("size", size.ToString(), type);

    /// <summary>Sets the font and optionally the material.</summary>
    /// <param name="font">The font name.</param>
    /// <param name="material">The material name.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Font(string font, string material = null)
    {
        if (string.IsNullOrEmpty(material))
        {
            _openingTags.Add("<font=\"" + font + "\">");
            _closingTags.Add("</font>");
            return this;
        }

        _openingTags.Add($"<font=\"{font}\" material=\"{material}\">");
        _closingTags.Add("</font>");
        return this;
    }

    /// <summary>Sets the font weight.</summary>
    /// <param name="weight">The font weight value.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder FontWeight(int weight) => SurroundWithTag("fontweight", weight.ToString());

    /// <summary>Sets the text alignment.</summary>
    /// <param name="alignment">The alignment type.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Align(AlignmentType alignment) => SurroundWithTag("align", AlignmentTypeToString(alignment));

    /// <summary>Sets the margin.</summary>
    /// <param name="margin">The margin value.</param>
    /// <param name="type">The unit type for the margin.</param>
    /// <param name="marginType">The side(s) to apply the margin to.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Margin(int margin, UnitType type, MarginType marginType) => SurroundWithTag(MarginTypeToString(marginType), margin.ToString(), type);

    /// <summary>Sets the margin on both sides.</summary>
    /// <param name="margin">The margin value.</param>
    /// <param name="type">The unit type for the margin.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Margin(int margin, UnitType type) => Margin(margin, type, MarginType.Both);

    /// <summary>Sets the margin on both sides in percent.</summary>
    /// <param name="margin">The margin value.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Margin(int margin) => Margin(margin, UnitType.Percent, MarginType.Both);

    /// <summary>Sets the indentation.</summary>
    /// <param name="indent">The indentation value.</param>
    /// <param name="type">The unit type for the indentation.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Indent(string indent, UnitType type) => SurroundWithTag("indent", indent, type);

    /// <summary>Sets the indentation in percent.</summary>
    /// <param name="indent">The indentation value.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Indent(string indent) => Indent(indent, UnitType.Percent);

    /// <summary>Sets the line indentation.</summary>
    /// <param name="indent">The indentation value.</param>
    /// <param name="type">The unit type for the indentation.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder LineIndent(string indent, UnitType type) => SurroundWithTag("line-indent", indent, type);

    /// <summary>Sets the line indentation in percent.</summary>
    /// <param name="indent">The indentation value.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder LineIndent(string indent) => LineIndent(indent, UnitType.Percent);

    /// <summary>
    ///     Highlights the text with a background color.
    /// </summary>
    /// <param name="color">The color to use for highlighting.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Mark(string color)
    {
        string hex = ColorUtils.ParseOrDefault(color).ToMinimizedHex();
        return SurroundWithTag("mark", hex);
    }

    /// <summary>
    ///     Applies a color tag to the text.
    /// </summary>
    /// <param name="color">The color to apply.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Color(string color)
    {
        string hex = ColorUtils.ParseOrDefault(color).ToMinimizedHex();
        _openingTags.Add($"<{hex}>");
        _closingTags.Add("</color>");
        return this;
    }

    /// <summary>
    ///     Applies a color tag using a Unity Color.
    /// </summary>
    /// <param name="color">The color to apply.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder Color(Color color) => Color(color.ToMinimizedHex());

    /// <summary>
    ///     Applies a character-by-character gradient across the current content.
    /// </summary>
    /// <param name="colors">The color stops for the gradient.</param>
    /// <returns>The builder instance.</returns>
    public TMPRichTextBuilder ColorGradient(params string[] colors)
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

        int realChars = text.Count(c => c != '<');
        if (realChars == 0)
        {
            return this;
        }

        int segments = stops.Count - 1;

        StringBuilder rebuilt = new StringBuilder();
        int visibleIndex = 0;

        for (int i = 0; i < text.Length; i++)
        {
            char ch = text[i];

            if (ch == '<')
            {
                while (i < text.Length && text[i] != '>')
                {
                    rebuilt.Append(text[i]);
                    i++;
                }

                rebuilt.Append('>');
                continue;
            }

            if (char.IsWhiteSpace(ch))
            {
                rebuilt.Append(ch);
                visibleIndex++;
                continue;
            }

            float pos = segments == 0 ? 0f : (float)visibleIndex / Mathf.Max(1, realChars - 1);

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

            rebuilt.Append($"<{hex}>{ch}</color>");

            visibleIndex++;
        }

        _content.Clear();
        _content.Append(rebuilt);
        return this;
    }

    /// <summary>
    ///     Builds the final TMP rich text string.
    /// </summary>
    /// <returns>The final rich text string.</returns>
    public string Build()
    {
        StringBuilder result = new StringBuilder();

        foreach (string tag in _innerOpeningTags)
        {
            result.Insert(0, tag);
        }

        foreach (string tag in _openingTags)
        {
            result.Insert(0, tag);
        }

        result.Append(_content);

        foreach (string tag in _innerClosingTags)
        {
            result.Append(tag);
        }

        foreach (string tag in _closingTags)
        {
            result.Append(tag);
        }

        Debug.Log(result);
        return result.ToString();
    }
}