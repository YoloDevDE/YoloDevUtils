using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YoloDev.Utils
{
    /// <summary>
    /// A fluent builder for creating TMP rich text strings with tags, styling and inline formatting.
    /// </summary>
    public class TMPRichTextBuilder
    {
        public enum AlignmentType
        {
            Left,
            Center,
            Right,
            Justify,
            Flush
        }

        public enum MarginType
        {
            Left,
            Right,
            Both
        }

        public enum UnitType
        {
            Plus,
            Percent,
            Pixels,
            Em,
            None
        }

        private readonly List<string> _closingTags = new();
        private readonly StringBuilder _content = new();
        private readonly List<string> _innerClosingTags = new();
        private readonly List<string> _innerOpeningTags = new();
        private readonly List<string> _openingTags = new();

        public TMPRichTextBuilder() { }

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

        /// <summary>
        /// Appends raw text to the builder.
        /// </summary>
        public TMPRichTextBuilder AddLayer(string text)
        {
            _content.Append(text);
            return this;
        }

        /// <summary>
        /// Appends a custom sub-builder with text.
        /// </summary>
        public TMPRichTextBuilder AddLayer(string text, Action<TMPRichTextBuilder> customizer)
        {
            TMPRichTextBuilder builder = new TMPRichTextBuilder().AddLayer(text);
            customizer(builder);
            _content.Append(builder.Build());
            return this;
        }

        /// <summary>
        /// Appends a custom sub-builder.
        /// </summary>
        public TMPRichTextBuilder AddLayer(Action<TMPRichTextBuilder> customizer)
        {
            TMPRichTextBuilder builder = new TMPRichTextBuilder();
            customizer(builder);
            _content.Append(builder.Build());
            return this;
        }

        /// <summary>
        /// Appends an existing builder.
        /// </summary>
        public TMPRichTextBuilder AddLayer(TMPRichTextBuilder builder)
        {
            _content.Append(builder.Build());
            return this;
        }

        public TMPRichTextBuilder Bold() => SurroundWithTag("b");
        public TMPRichTextBuilder Italic() => SurroundWithTag("i");
        public TMPRichTextBuilder NoBreak() => SurroundWithTag("nobr");
        public TMPRichTextBuilder Break() => AddLayer("<br>");
        public TMPRichTextBuilder NoParse() => SurroundWithInnerTag("noparse");
        public TMPRichTextBuilder Underline() => SurroundWithTag("u");
        public TMPRichTextBuilder StrikeThrough() => SurroundWithTag("s");
        public TMPRichTextBuilder AllCaps() => SurroundWithTag("allcaps");
        public TMPRichTextBuilder UpperCase() => AllCaps();
        public TMPRichTextBuilder SmallCaps() => SurroundWithTag("smallcaps");
        public TMPRichTextBuilder LowerCase() => SurroundWithTag("lowercase");

        /// <summary>
        /// Sets the global line height.
        /// </summary>
        public TMPRichTextBuilder LineHeight(int height, UnitType type = UnitType.Percent)
        {
            _openingTags.Insert(0, $"<line-height={WrapWithUnitType(height.ToString(), type)}>");
            return this;
        }

        /// <summary>
        /// Sets the position offset for the next character.
        /// </summary>
        public TMPRichTextBuilder Pos(int pos, UnitType type = UnitType.Percent)
        {
            _content.Append($"<pos={WrapWithUnitType(pos.ToString(), type)}>");
            return this;
        }

        /// <summary>
        /// Sets indentation for the content.
        /// </summary>
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

        private TMPRichTextBuilder SurroundWithTag(string tag, string value) =>
            SurroundWithTag(tag, value, UnitType.None);

        public TMPRichTextBuilder CSpace(int cspace, UnitType type) => SurroundWithTag("cspace", cspace.ToString(), type);
        public TMPRichTextBuilder CSpace(int cspace) => CSpace(cspace, UnitType.Percent);

        public TMPRichTextBuilder Space(int space, UnitType type) =>
            AddLayer($"<space={WrapWithUnitType(space.ToString(), type)}>");

        public TMPRichTextBuilder Space(int space) => Space(space, UnitType.Percent);
        public TMPRichTextBuilder Space() => AddLayer(" ");

        public TMPRichTextBuilder VOffset(int voffset, UnitType type = UnitType.Percent) =>
            SurroundWithTag("voffset", voffset.ToString(), type);

        public TMPRichTextBuilder MSpace(int mspace, UnitType type = UnitType.Percent) =>
            SurroundWithTag("mspace", mspace.ToString(), type);

        public TMPRichTextBuilder MSpace(double mspace, UnitType type) =>
            SurroundWithTag("mspace", mspace.ToString(CultureInfo.InvariantCulture), type);

        public TMPRichTextBuilder MSpace(double mspace) => MSpace(mspace, UnitType.Percent);

        public TMPRichTextBuilder Size(int size, UnitType type = UnitType.Percent) =>
            SurroundWithTag("size", size.ToString(), type);

        public TMPRichTextBuilder Font(string font) => Font(font, null);

        public TMPRichTextBuilder Font(string font, string material)
        {
            if (string.IsNullOrEmpty(material))
                return SurroundWithTag("font", font);

            _openingTags.Add($"<font=\"{font}\" material=\"{material}\">");
            _closingTags.Add("</font>");
            return this;
        }

        public TMPRichTextBuilder FontWeight(int weight) =>
            SurroundWithTag("fontweight", weight.ToString());

        public TMPRichTextBuilder Align(AlignmentType alignment) =>
            SurroundWithTag("align", AlignmentTypeToString(alignment));

        public TMPRichTextBuilder Margin(int margin, UnitType type, MarginType marginType) =>
            SurroundWithTag(MarginTypeToString(marginType), margin.ToString(), type);

        public TMPRichTextBuilder Margin(int margin, UnitType type) =>
            Margin(margin, type, MarginType.Both);

        public TMPRichTextBuilder Margin(int margin) =>
            Margin(margin, UnitType.Percent, MarginType.Both);

        public TMPRichTextBuilder Indent(string indent, UnitType type) =>
            SurroundWithTag("indent", indent, type);

        public TMPRichTextBuilder Indent(string indent) =>
            Indent(indent, UnitType.Percent);

        public TMPRichTextBuilder LineIndent(string indent, UnitType type) =>
            SurroundWithTag("line-indent", indent, type);

        public TMPRichTextBuilder LineIndent(string indent) =>
            LineIndent(indent, UnitType.Percent);

        /// <summary>
        /// Highlights the text with a background color.
        /// </summary>
        public TMPRichTextBuilder Mark(string color)
        {
            string hex = ColorUtils.ParseOrDefault(color).ToMinimizedHex();
            return SurroundWithTag("mark", hex);
        }

        /// <summary>
        /// Applies a color tag to the text.
        /// </summary>
        public TMPRichTextBuilder Color(string color)
        {
            string hex = ColorUtils.ParseOrDefault(color).ToMinimizedHex();
            _openingTags.Add($"<{hex}>");
            _closingTags.Add("</color>");
            return this;
        }

        /// <summary>
        /// Applies a color tag using a Unity Color.
        /// </summary>
        public TMPRichTextBuilder Color(Color color)
        {
            return Color(color.ToMinimizedHex());
        }

        /// <summary>
        /// Applies a character-by-character gradient across the current content.
        /// </summary>
        public TMPRichTextBuilder ColorGradient(params string[] colors)
        {
            if (colors == null || colors.Length == 0)
                return this;

            List<Color32> stops = colors
                .Select(c => (Color32)ColorUtils.ParseOrDefault(c))
                .ToList();

            if (stops.Count == 1)
                return Color(((Color)stops[0]).ToMinimizedHex());

            string text = _content.ToString();
            if (string.IsNullOrEmpty(text))
                return this;

            int realChars = text.Count(c => c != '<');
            if (realChars == 0)
                return this;

            int segments = stops.Count - 1;

            StringBuilder rebuilt = new();
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

                float pos = segments == 0 ? 0f :
                    (float)visibleIndex / Mathf.Max(1, realChars - 1);

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
        /// Builds the final TMP rich text string.
        /// </summary>
        public string Build()
        {
            StringBuilder result = new();

            foreach (string tag in _innerOpeningTags)
                result.Insert(0, tag);

            foreach (string tag in _openingTags)
                result.Insert(0, tag);

            result.Append(_content);

            foreach (string tag in _innerClosingTags)
                result.Append(tag);

            foreach (string tag in _closingTags)
                result.Append(tag);

            return result.ToString();
        }
    }
}
