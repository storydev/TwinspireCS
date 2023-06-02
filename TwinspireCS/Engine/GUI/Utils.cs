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

        public static unsafe TextDim MeasureTextWrapping(Font font, int fontSize, int spacing, int maxWidth, string text)
        {
            TextDim result = new TextDim();
            int lastChance = -1;
            int lastBreak = 0;
            var breaks = new List<int>();
            result.Characters = text.ToCharArray();
            
            int tempByteCounter = 0;
            int byteCounter = 0;

            var currentLineWidth = 0f;
            var tempLineWidth = 0.0f;

            float textHeight = font.baseSize * 1.5f;
            float scaleFactor = (float)fontSize / (float)font.baseSize;

            int letter = 0;
            int index = 0;

            int i = 0;

            while (i < text.Length)
            {
                byteCounter++;

                int next = 0;
                letter = Raylib.GetCodepoint(text, ref next);
                index = Raylib.GetGlyphIndex(font, letter);

                if (font.glyphs[index].advanceX != 0) currentLineWidth += font.glyphs[index].advanceX * scaleFactor;
                else currentLineWidth += (font.recs[index].width + font.glyphs[index].offsetX) * scaleFactor;

                if (currentLineWidth >= maxWidth)
                {
                    if (lastChance < 0)
                    {
                        lastChance = i - 1;
                    }
                    breaks.Add(lastChance + 1);
                    lastBreak = lastChance + 1;
                    i = lastBreak;
                    lastChance = -1;
                    currentLineWidth = 0f;
                    byteCounter = 0;
                    tempLineWidth = 0.0f;
                }

                if (text[i] == ' ')
                {
                    lastChance = i;
                }
                else if (text[i] == '\n' || text[i] == '\r')
                {
                    breaks.Add(i + 1);
                    byteCounter = 0;
                    currentLineWidth = 0;
                    lastBreak = i + 1;
                    lastChance = -1;
                }

                i += 1;

                if (tempByteCounter < byteCounter) tempByteCounter = byteCounter;
            }

            result.Breaks = breaks.ToArray();
            var actualWidth = maxWidth;
            if (currentLineWidth < maxWidth && breaks.Count == 0)
                actualWidth = (int)currentLineWidth;

            result.ContentSize = new Vector2(actualWidth, (result.Breaks.Length + 1) * (textHeight * scaleFactor));
            return result;
        }

        public static void RenderMultilineText(Font font, float fontSize, Vector2 pos, int spacing, TextDim textDim, Color color, ContentAlignment alignment = ContentAlignment.Center)
        {
            var startY = pos.Y;
            Raylib.SetTextureFilter(font.texture, TextureFilter.TEXTURE_FILTER_BILINEAR);
            if (textDim.Breaks.Length > 0)
            {
                for (int i = 0; i < textDim.Breaks.Length + 1; i++)
                {
                    var x = pos.X;
                    var start = i == 0 ? 0 : textDim.Breaks[i - 1];
                    var end = i == textDim.Breaks.Length ? textDim.Characters.Length : textDim.Breaks[i];
                    string lineText = textDim.Characters.GetCharsAsString(start, end);
                    var lineSize = Raylib.MeasureTextEx(font, lineText, fontSize, spacing);

                    if (alignment == ContentAlignment.Bottom || alignment == ContentAlignment.Center || alignment == ContentAlignment.Top)
                    {
                        x = ((textDim.ContentSize.X - lineSize.X) / 2) + pos.X;
                    }
                    else if (alignment == ContentAlignment.BottomRight || alignment == ContentAlignment.Right || alignment == ContentAlignment.TopRight)
                    {
                        x = (textDim.ContentSize.X - lineSize.Y) + pos.X;
                    }

                    Raylib.DrawTextEx(font, lineText, new Vector2(x, startY), fontSize, spacing, color);
                    startY += lineSize.Y;
                }
            }
            else
            {
                var text = textDim.Characters.GetCharsAsString(0, textDim.Characters.Length);
                var textSize = Raylib.MeasureTextEx(font, text, fontSize, spacing);
                var x = pos.X;
                if (alignment == ContentAlignment.Bottom || alignment == ContentAlignment.Center || alignment == ContentAlignment.Top)
                {
                    x = ((textDim.ContentSize.X - textSize.X) / 2) + pos.X;
                }
                else if (alignment == ContentAlignment.BottomRight || alignment == ContentAlignment.Right || alignment == ContentAlignment.TopRight)
                {
                    x = (textDim.ContentSize.X - textSize.X) + pos.X;
                }

                Raylib.DrawTextEx(font, text, new Vector2(x, startY), fontSize, spacing, color);
            }
        }

        public static Color ChangeColour(Color from, Color to, float ratio)
        {
            var fromHSV = Raylib.ColorToHSV(from);
            var toHSV = Raylib.ColorToHSV(to);
            var newHSV = new Vector3(((toHSV.X - fromHSV.X) * ratio) + fromHSV.X, ((toHSV.Y - fromHSV.Y) * ratio) + fromHSV.Y, ((toHSV.Z - fromHSV.Z) * ratio) + fromHSV.Z);
            return Raylib.ColorFromHSV(newHSV.X, newHSV.Y, newHSV.Z);
        }

    }
}
