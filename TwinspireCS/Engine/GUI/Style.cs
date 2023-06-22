using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using TwinspireCS.Engine.Extras;

namespace TwinspireCS.Engine.GUI
{
    public class Style
    {

        public StyleMethod Method;
        public ColorMethod BackgroundColor;
        public Color ForegroundColor;
        public string BackgroundImage;
        public float RadiusCorners;
        public float Opacity;

        public int Spritesheet;
        public int FrameIndex;

        // multiply by 4 for these ones
        public Color[] BorderColors;
        public int[] BorderThicknesses;
        public bool[] Borders;

        public Style()
        {
            BackgroundColor = new ColorMethod();
            ForegroundColor = Color.BLACK;
            BackgroundImage = string.Empty;
            RadiusCorners = 0f;

            BorderColors = new Color[4];
            BorderThicknesses = new int[4];
            Borders = new bool[4];
            Opacity = 1f;
            Method = StyleMethod.Replace;

            Spritesheet = -1;
            FrameIndex = -1;
        }

        public Style SetBackground(ColorMethod color)
        {
            BackgroundColor = color;
            return this;
        }

        public Style SetForeground(Color color)
        {
            ForegroundColor = color;
            return this;
        }

        public Style SetBackground(string imageName)
        {
            BackgroundImage = imageName;
            return this;
        }

        public Style SetBorderColors(Color color)
        {
            BorderColors[0] = color; // top
            BorderColors[1] = color; // left
            BorderColors[2] = color; // right
            BorderColors[3] = color; // bottom
            return this;
        }

        public Style SetBorderTopColors(Color color)
        {
            BorderColors[0] = color; // top
            return this;
        }

        public Style SetBorderLeftColors(Color color)
        {
            BorderColors[1] = color; // left
            return this;
        }

        public Style SetBorderRightColors(Color color)
        {
            BorderColors[2] = color; // right
            return this;
        }

        public Style SetBorderBottomColors(Color color)
        {
            BorderColors[3] = color; // bottom
            return this;
        }

        public Style SetBorderThickness(int lineThickness)
        {
            BorderThicknesses[0] = lineThickness;
            BorderThicknesses[1] = lineThickness;
            BorderThicknesses[2] = lineThickness;
            BorderThicknesses[3] = lineThickness;
            return this;
        }

        public Style SetBorderTopThickness(int lineThickness)
        {
            BorderThicknesses[0] = lineThickness;
            return this;
        }

        public Style SetBorderLeftThickness(int lineThickness)
        {
            BorderThicknesses[1] = lineThickness;
            return this;
        }

        public Style SetBorderRightThickness(int lineThickness)
        {
            BorderThicknesses[2] = lineThickness;
            return this;
        }

        public Style SetBorderBottomThickness(int lineThickness)
        {
            BorderThicknesses[3] = lineThickness;
            return this;
        }

        public Style ApplyBorders(bool yes)
        {
            Borders[0] = yes;
            Borders[1] = yes;
            Borders[2] = yes;
            Borders[3] = yes;
            return this;
        }

        public Style ApplyBordersTop(bool yes)
        {
            Borders[0] = yes;
            return this;
        }

        public Style ApplyBordersLeft(bool yes)
        {
            Borders[1] = yes;
            return this;
        }

        public Style ApplyBordersRight(bool yes)
        {
            Borders[2] = yes;
            return this;
        }

        public Style ApplyBordersBottom(bool yes)
        {
            Borders[3] = yes;
            return this;
        }

        public Style SetRadiusCorners(float radius)
        {
            RadiusCorners = radius;
            return this;
        }

    }
}
