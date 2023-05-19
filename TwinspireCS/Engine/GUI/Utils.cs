using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Raylib_cs;

namespace TwinspireCS.Engine.GUI
{
    internal class Utils
    {

        public static TextDim MeasureTextWrapping(Font font, int fontSize, int spacing, int maxWidth, string text)
        {
            TextDim result = new TextDim();
            int lastChance = -1;
            int lastBreak = 0;
            var breaks = new List<int>();
            result.Characters = text.ToCharArray();
            int i = 0;

            var sizeRatio = font.baseSize / fontSize;
            var currentLineWidth = 0f;
            while (i < text.Length)
            {
                var glyph = Raylib.GetGlyphInfo(font, text[i]);
                var width = glyph.image.width * sizeRatio + spacing;
                if (width + currentLineWidth >= maxWidth)
                {
                    if (lastChance < 0)
                    {
                        lastChance = i - 1;
                    }
                    breaks.Add(lastChance + 1);
                    lastBreak = lastChance + 1;
                    i = lastBreak;
                    lastChance = -1;
                }
                else
                {
                    currentLineWidth += width;
                }

                if (text[i] == ' ')
                {
                    lastChance = i;
                }
                else if (text[i] == '\n' || text[i] == '\r')
                {
                    breaks.Add(i + 1);
                    currentLineWidth = 0;
                    lastBreak = i + 1;
                    lastChance = -1;
                }

                i += 1;
            }
            result.Breaks = breaks.ToArray();
            result.ContentSize = new Vector2(maxWidth, result.Breaks.Length * fontSize);
            return result;
        }

    }
}
