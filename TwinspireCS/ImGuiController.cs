using System;
using System.Collections.Generic;
using System.Buffers;
using System.Numerics;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using ImGuiNET;
using Raylib_cs;
using System.Runtime.InteropServices;

namespace TwinspireCS
{
    public unsafe class ImGuiController
    {

        private Vector2 mousePosition;
        private Vector2 displaySize;
        private float delta;
        private bool isKeyCtrl;
        private bool isKeyShift;
        private bool isKeyAlt;
        private bool isKeySuper;

        static double g_Time = 0.0;
        static bool g_UnloadAtlas = false;
        static uint g_AtlasTexID = 0;

        static List<string> fontFilesToLoad;
        static List<int> fontFilesFontSizes;

        static List<byte[]> fontDataToLoad;
        static List<int> fontDataFontSizes;

        static Dictionary<string, ImFontPtr> fonts;

        public void AddFontFile(string fontFile, int fontSize)
        {
            fontFilesToLoad.Add(fontFile);
            fontFilesFontSizes.Add(fontSize);
        }

        public void AddFontBytes(byte[] data, int fontSize)
        {
            fontDataToLoad.Add(data);
            fontDataFontSizes.Add(fontSize);
        }

        static string GetClipboardText()
        {
            return Raylib.GetClipboardText_();
        }

        static void SetClipboardText(string text)
        {
            Raylib.SetClipboardText(text);
        }

        public ImGuiController()
        {
            fontFilesToLoad = new List<string>();
            fontFilesFontSizes = new List<int>();

            fontDataToLoad = new List<byte[]>();
            fontDataFontSizes = new List<int>();

            fonts = new Dictionary<string, ImFontPtr>();
        }

        public ImFontPtr GetFont(string identifier)
        {
            if (fonts.ContainsKey(identifier))
            {
                return fonts[identifier];
            }

            return null;
        }

        public void Init()
        {
            var io = ImGui.GetIO();

            mousePosition = new Vector2(0, 0);
            io.MousePos = mousePosition;

            for (int i = 0; i < fontFilesToLoad.Count; i++)
            {
                var ptr = io.Fonts.AddFontFromFileTTF(fontFilesToLoad[i], fontFilesFontSizes[i]);
                var filename = Path.GetFileName(fontFilesToLoad[i]);
                fonts.Add(filename + ":" + fontFilesFontSizes[i], ptr);
            }


            for (int i = 0; i < fontDataToLoad.Count; i++)
            {
                Span<byte> dataSpan = fontDataToLoad[i].AsSpan();
                fixed (byte* data = dataSpan)
                {
                    IntPtr buffer = (IntPtr)data;

                    var ptr = io.Fonts.AddFontFromMemoryTTF(buffer, fontDataToLoad[i].Length, fontDataFontSizes[i]);
                    fonts.Add("data_" + i + ":" + fontDataFontSizes[i], ptr);
                }
            }

            LoadDefaultFontAtlas();
        }

        public void Shutdown()
        {
            if (g_UnloadAtlas)
            {
                ImGui.GetIO().Fonts.ClearFonts();
            }
            g_Time = 0.0;
        }

        private void UpdateMousePosAndButtons()
        {
            var io = ImGui.GetIO();

            if (io.WantSetMousePos)
                Raylib.SetMousePosition((int)io.MousePos.X, (int)io.MousePos.Y);

            io.MouseDown[0] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON);
            io.MouseDown[1] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON);
            io.MouseDown[2] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_MIDDLE_BUTTON);

            if (!Raylib.IsWindowMinimized())
                mousePosition = new Vector2(Raylib.GetMouseX(), Raylib.GetMouseY());

            io.MousePos = mousePosition;
        }

        private void UpdateMouseCursor()
        {
            var io = ImGui.GetIO();
            if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.NoMouseCursorChange))
                return;

            var imgui_cursor = ImGui.GetMouseCursor();
            if (io.MouseDrawCursor || imgui_cursor == ImGuiMouseCursor.None)
            {
                Raylib.HideCursor();
            }
            else
            {
                Raylib.ShowCursor();
            }
        }

        public void NewFrame()
        {
            var io = ImGui.GetIO();

            displaySize = new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            io.DisplaySize = displaySize;

            double current_time = Raylib.GetTime();
            delta = g_Time > 0.0 ? (float)(current_time - g_Time) : 1.0f / 60.0f;
            io.DeltaTime = delta;

            isKeyCtrl = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL);
            isKeyShift = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);
            isKeyAlt = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT);
            isKeySuper = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SUPER) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SUPER);

            UpdateMousePosAndButtons();
            UpdateMouseCursor();

            if (Raylib.GetMouseWheelMove() > 0)
            {
                io.MouseWheel += 1;
            }
            else if (Raylib.GetMouseWheelMove() < 0)
            {
                io.MouseWheel -= 1;
            }
        }

        /// <summary>
        /// Gets a value to determine if the Dear ImGui interface is currently being interacted with.
        /// </summary>
        /// <returns></returns>
        public static bool IsImGuiInteracted()
        {
            return ImGui.IsAnyItemActive() || ImGui.IsAnyItemFocused() || ImGui.IsAnyItemHovered() || ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow) || ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow);
        }

        public bool ProcessEvent()
        {
            var io = ImGui.GetIO();

            io.AddKeyEvent(ImGuiKey.Apostrophe, Raylib.IsKeyDown(KeyboardKey.KEY_APOSTROPHE));
            io.AddKeyEvent(ImGuiKey.Comma, Raylib.IsKeyDown(KeyboardKey.KEY_COMMA));
            io.AddKeyEvent(ImGuiKey.Minus, Raylib.IsKeyDown(KeyboardKey.KEY_MINUS));
            io.AddKeyEvent(ImGuiKey.Period, Raylib.IsKeyDown(KeyboardKey.KEY_PERIOD));
            io.AddKeyEvent(ImGuiKey.Tab, Raylib.IsKeyDown(KeyboardKey.KEY_TAB));
            io.AddKeyEvent(ImGuiKey.Slash, Raylib.IsKeyDown(KeyboardKey.KEY_SLASH));
            io.AddKeyEvent(ImGuiKey._0, Raylib.IsKeyDown(KeyboardKey.KEY_ZERO));
            io.AddKeyEvent(ImGuiKey._1, Raylib.IsKeyDown(KeyboardKey.KEY_ONE));
            io.AddKeyEvent(ImGuiKey._2, Raylib.IsKeyDown(KeyboardKey.KEY_TWO));
            io.AddKeyEvent(ImGuiKey._3, Raylib.IsKeyDown(KeyboardKey.KEY_THREE));
            io.AddKeyEvent(ImGuiKey._4, Raylib.IsKeyDown(KeyboardKey.KEY_FOUR));
            io.AddKeyEvent(ImGuiKey._5, Raylib.IsKeyDown(KeyboardKey.KEY_FIVE));
            io.AddKeyEvent(ImGuiKey._6, Raylib.IsKeyDown(KeyboardKey.KEY_SIX));
            io.AddKeyEvent(ImGuiKey._7, Raylib.IsKeyDown(KeyboardKey.KEY_SEVEN));
            io.AddKeyEvent(ImGuiKey._8, Raylib.IsKeyDown(KeyboardKey.KEY_EIGHT));
            io.AddKeyEvent(ImGuiKey._9, Raylib.IsKeyDown(KeyboardKey.KEY_NINE));
            io.AddKeyEvent(ImGuiKey.Semicolon, Raylib.IsKeyDown(KeyboardKey.KEY_SEMICOLON));
            io.AddKeyEvent(ImGuiKey.Equal, Raylib.IsKeyDown(KeyboardKey.KEY_EQUAL));
            io.AddKeyEvent(ImGuiKey.A, Raylib.IsKeyDown(KeyboardKey.KEY_A));
            io.AddKeyEvent(ImGuiKey.B, Raylib.IsKeyDown(KeyboardKey.KEY_B));
            io.AddKeyEvent(ImGuiKey.C, Raylib.IsKeyDown(KeyboardKey.KEY_C));
            io.AddKeyEvent(ImGuiKey.D, Raylib.IsKeyDown(KeyboardKey.KEY_D));
            io.AddKeyEvent(ImGuiKey.E, Raylib.IsKeyDown(KeyboardKey.KEY_E));
            io.AddKeyEvent(ImGuiKey.F, Raylib.IsKeyDown(KeyboardKey.KEY_F));
            io.AddKeyEvent(ImGuiKey.G, Raylib.IsKeyDown(KeyboardKey.KEY_G));
            io.AddKeyEvent(ImGuiKey.H, Raylib.IsKeyDown(KeyboardKey.KEY_H));
            io.AddKeyEvent(ImGuiKey.I, Raylib.IsKeyDown(KeyboardKey.KEY_I));
            io.AddKeyEvent(ImGuiKey.J, Raylib.IsKeyDown(KeyboardKey.KEY_J));
            io.AddKeyEvent(ImGuiKey.K, Raylib.IsKeyDown(KeyboardKey.KEY_K));
            io.AddKeyEvent(ImGuiKey.L, Raylib.IsKeyDown(KeyboardKey.KEY_L));
            io.AddKeyEvent(ImGuiKey.M, Raylib.IsKeyDown(KeyboardKey.KEY_M));
            io.AddKeyEvent(ImGuiKey.N, Raylib.IsKeyDown(KeyboardKey.KEY_N));
            io.AddKeyEvent(ImGuiKey.O, Raylib.IsKeyDown(KeyboardKey.KEY_O));
            io.AddKeyEvent(ImGuiKey.P, Raylib.IsKeyDown(KeyboardKey.KEY_P));
            io.AddKeyEvent(ImGuiKey.Q, Raylib.IsKeyDown(KeyboardKey.KEY_Q));
            io.AddKeyEvent(ImGuiKey.R, Raylib.IsKeyDown(KeyboardKey.KEY_R));
            io.AddKeyEvent(ImGuiKey.S, Raylib.IsKeyDown(KeyboardKey.KEY_S));
            io.AddKeyEvent(ImGuiKey.T, Raylib.IsKeyDown(KeyboardKey.KEY_T));
            io.AddKeyEvent(ImGuiKey.U, Raylib.IsKeyDown(KeyboardKey.KEY_U));
            io.AddKeyEvent(ImGuiKey.V, Raylib.IsKeyDown(KeyboardKey.KEY_V));
            io.AddKeyEvent(ImGuiKey.W, Raylib.IsKeyDown(KeyboardKey.KEY_W));
            io.AddKeyEvent(ImGuiKey.X, Raylib.IsKeyDown(KeyboardKey.KEY_X));
            io.AddKeyEvent(ImGuiKey.Y, Raylib.IsKeyDown(KeyboardKey.KEY_Y));
            io.AddKeyEvent(ImGuiKey.Z, Raylib.IsKeyDown(KeyboardKey.KEY_Z));
            io.AddKeyEvent(ImGuiKey.Space, Raylib.IsKeyDown(KeyboardKey.KEY_SPACE));
            io.AddKeyEvent(ImGuiKey.Escape, Raylib.IsKeyDown(KeyboardKey.KEY_ESCAPE));
            io.AddKeyEvent(ImGuiKey.Enter, Raylib.IsKeyDown(KeyboardKey.KEY_ENTER));
            io.AddKeyEvent(ImGuiKey.Backspace, Raylib.IsKeyDown(KeyboardKey.KEY_BACKSPACE));
            io.AddKeyEvent(ImGuiKey.Insert, Raylib.IsKeyDown(KeyboardKey.KEY_INSERT));
            io.AddKeyEvent(ImGuiKey.Delete, Raylib.IsKeyDown(KeyboardKey.KEY_DELETE));
            io.AddKeyEvent(ImGuiKey.RightArrow, Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT));
            io.AddKeyEvent(ImGuiKey.LeftArrow, Raylib.IsKeyDown(KeyboardKey.KEY_LEFT));
            io.AddKeyEvent(ImGuiKey.DownArrow, Raylib.IsKeyDown(KeyboardKey.KEY_DOWN));
            io.AddKeyEvent(ImGuiKey.UpArrow, Raylib.IsKeyDown(KeyboardKey.KEY_UP));
            io.AddKeyEvent(ImGuiKey.PageUp, Raylib.IsKeyDown(KeyboardKey.KEY_PAGE_UP));
            io.AddKeyEvent(ImGuiKey.PageDown, Raylib.IsKeyDown(KeyboardKey.KEY_PAGE_DOWN));
            io.AddKeyEvent(ImGuiKey.Home, Raylib.IsKeyDown(KeyboardKey.KEY_HOME));
            io.AddKeyEvent(ImGuiKey.End, Raylib.IsKeyDown(KeyboardKey.KEY_END));
            io.AddKeyEvent(ImGuiKey.CapsLock, Raylib.IsKeyDown(KeyboardKey.KEY_CAPS_LOCK));
            io.AddKeyEvent(ImGuiKey.ScrollLock, Raylib.IsKeyDown(KeyboardKey.KEY_SCROLL_LOCK));
            io.AddKeyEvent(ImGuiKey.NumLock, Raylib.IsKeyDown(KeyboardKey.KEY_NUM_LOCK));
            io.AddKeyEvent(ImGuiKey.PrintScreen, Raylib.IsKeyDown(KeyboardKey.KEY_PRINT_SCREEN));
            io.AddKeyEvent(ImGuiKey.Pause, Raylib.IsKeyDown(KeyboardKey.KEY_PAUSE));
            io.AddKeyEvent(ImGuiKey.F1, Raylib.IsKeyDown(KeyboardKey.KEY_F1));
            io.AddKeyEvent(ImGuiKey.F2, Raylib.IsKeyDown(KeyboardKey.KEY_F2));
            io.AddKeyEvent(ImGuiKey.F3, Raylib.IsKeyDown(KeyboardKey.KEY_F3));
            io.AddKeyEvent(ImGuiKey.F4, Raylib.IsKeyDown(KeyboardKey.KEY_F4));
            io.AddKeyEvent(ImGuiKey.F5, Raylib.IsKeyDown(KeyboardKey.KEY_F5));
            io.AddKeyEvent(ImGuiKey.F6, Raylib.IsKeyDown(KeyboardKey.KEY_F6));
            io.AddKeyEvent(ImGuiKey.F7, Raylib.IsKeyDown(KeyboardKey.KEY_F7));
            io.AddKeyEvent(ImGuiKey.F8, Raylib.IsKeyDown(KeyboardKey.KEY_F8));
            io.AddKeyEvent(ImGuiKey.F9, Raylib.IsKeyDown(KeyboardKey.KEY_F9));
            io.AddKeyEvent(ImGuiKey.F10, Raylib.IsKeyDown(KeyboardKey.KEY_F10));
            io.AddKeyEvent(ImGuiKey.F11, Raylib.IsKeyDown(KeyboardKey.KEY_F11));
            io.AddKeyEvent(ImGuiKey.F12, Raylib.IsKeyDown(KeyboardKey.KEY_F12));
            io.AddKeyEvent(ImGuiKey.LeftShift, Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT));
            io.AddKeyEvent(ImGuiKey.LeftCtrl, Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL));
            io.AddKeyEvent(ImGuiKey.LeftAlt, Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT));
            io.AddKeyEvent(ImGuiKey.LeftSuper, Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SUPER));
            io.AddKeyEvent(ImGuiKey.RightShift, Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT));
            io.AddKeyEvent(ImGuiKey.RightCtrl, Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL));
            io.AddKeyEvent(ImGuiKey.RightAlt, Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT));
            io.AddKeyEvent(ImGuiKey.RightSuper, Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SUPER));
            io.AddKeyEvent(ImGuiKey.Menu, Raylib.IsKeyDown(KeyboardKey.KEY_MENU));
            io.AddKeyEvent(ImGuiKey.LeftBracket, Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_BRACKET));
            io.AddKeyEvent(ImGuiKey.Backslash, Raylib.IsKeyDown(KeyboardKey.KEY_BACKSLASH));
            io.AddKeyEvent(ImGuiKey.RightBracket, Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_BRACKET));
            io.AddKeyEvent(ImGuiKey.GraveAccent, Raylib.IsKeyDown(KeyboardKey.KEY_GRAVE));
            io.AddKeyEvent(ImGuiKey.Keypad1, Raylib.IsKeyDown(KeyboardKey.KEY_KP_1));
            io.AddKeyEvent(ImGuiKey.Keypad2, Raylib.IsKeyDown(KeyboardKey.KEY_KP_2));
            io.AddKeyEvent(ImGuiKey.Keypad3, Raylib.IsKeyDown(KeyboardKey.KEY_KP_3));
            io.AddKeyEvent(ImGuiKey.Keypad4, Raylib.IsKeyDown(KeyboardKey.KEY_KP_4));
            io.AddKeyEvent(ImGuiKey.Keypad5, Raylib.IsKeyDown(KeyboardKey.KEY_KP_5));
            io.AddKeyEvent(ImGuiKey.Keypad6, Raylib.IsKeyDown(KeyboardKey.KEY_KP_6));
            io.AddKeyEvent(ImGuiKey.Keypad7, Raylib.IsKeyDown(KeyboardKey.KEY_KP_7));
            io.AddKeyEvent(ImGuiKey.Keypad8, Raylib.IsKeyDown(KeyboardKey.KEY_KP_8));
            io.AddKeyEvent(ImGuiKey.Keypad9, Raylib.IsKeyDown(KeyboardKey.KEY_KP_9));
            io.AddKeyEvent(ImGuiKey.Keypad0, Raylib.IsKeyDown(KeyboardKey.KEY_KP_0));
            io.AddKeyEvent(ImGuiKey.KeypadDecimal, Raylib.IsKeyDown(KeyboardKey.KEY_KP_DECIMAL));
            io.AddKeyEvent(ImGuiKey.KeypadDivide, Raylib.IsKeyDown(KeyboardKey.KEY_KP_DIVIDE));
            io.AddKeyEvent(ImGuiKey.KeypadMultiply, Raylib.IsKeyDown(KeyboardKey.KEY_KP_MULTIPLY));
            io.AddKeyEvent(ImGuiKey.KeypadSubtract, Raylib.IsKeyDown(KeyboardKey.KEY_KP_SUBTRACT));
            io.AddKeyEvent(ImGuiKey.KeypadAdd, Raylib.IsKeyDown(KeyboardKey.KEY_KP_ADD));
            io.AddKeyEvent(ImGuiKey.KeypadEnter, Raylib.IsKeyDown(KeyboardKey.KEY_KP_ENTER));
            io.AddKeyEvent(ImGuiKey.KeypadEqual, Raylib.IsKeyDown(KeyboardKey.KEY_KP_EQUAL));
            io.AddKeyEvent(ImGuiKey.AppBack, Raylib.IsKeyDown(KeyboardKey.KEY_BACK));           

            int length = 0;
            io.AddInputCharactersUTF8(Raylib.CodepointToUTF8(Raylib.GetCharPressed(), ref length));

            return true;
        }

        void LoadDefaultFontAtlas()
        {
            if (!g_UnloadAtlas)
            {
                var io = ImGui.GetIO();
                byte* pixels;
                int width, height, bpp;
                Image image = new Image();

                io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bpp);
                var size = Raylib.GetPixelDataSize(width, height, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
                image.Data = (void*)Marshal.AllocHGlobal(size);
                Buffer.MemoryCopy(pixels, image.Data, size, size);
                image.Width = width;
                image.Height = height;
                image.Mipmaps = 1;
                image.Format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8;
                var tex = Raylib.LoadTextureFromImage(image);
                g_AtlasTexID = tex.Id;
                io.Fonts.TexID = (IntPtr)g_AtlasTexID;
                Marshal.FreeHGlobal((IntPtr)pixels);
                Marshal.FreeHGlobal((IntPtr)image.Data);
                g_UnloadAtlas = true;
            }
        }

        public void Render(ImDrawDataPtr draw_data)
        {
            Rlgl.DisableBackfaceCulling();
            for (int n = 0; n < draw_data.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdLists[n];
                uint idx_index = 0;
                for (int i = 0; i < cmd_list.CmdBuffer.Size; i++)
                {
                    var pcmd = cmd_list.CmdBuffer[i];
                    var pos = draw_data.DisplayPos;
                    var rectX = (int)(pcmd.ClipRect.X - pos.X);
                    var rectY = (int)(pcmd.ClipRect.Y - pos.Y);
                    var rectW = (int)(pcmd.ClipRect.Z - rectX);
                    var rectH = (int)(pcmd.ClipRect.W - rectY);
                    Raylib.BeginScissorMode(rectX, rectY, rectW, rectH);
                    {
                        var ti = pcmd.TextureId;
                        for (int j = 0; j <= (pcmd.ElemCount - 3); j += 3)
                        {
                            if (pcmd.ElemCount == 0)
                            {
                                break;
                            }

                            Rlgl.PushMatrix();
                            Rlgl.Begin(DrawMode.TRIANGLES);
                            Rlgl.SetTexture((uint)ti.ToInt32());

                            ImDrawVertPtr vertex;
                            ushort index;

                            index = cmd_list.IdxBuffer[(int)(j + idx_index)];
                            vertex = cmd_list.VtxBuffer[index];
                            DrawTriangleVertex(vertex);

                            index = cmd_list.IdxBuffer[(int)(j + 2 + idx_index)];
                            vertex = cmd_list.VtxBuffer[index];
                            DrawTriangleVertex(vertex);

                            index = cmd_list.IdxBuffer[(int)(j + 1 + idx_index)];
                            vertex = cmd_list.VtxBuffer[index];
                            DrawTriangleVertex(vertex);

                            Rlgl.DisableTexture();
                            Rlgl.End();
                            Rlgl.PopMatrix();
                        }
                    }

                    idx_index += pcmd.ElemCount;
                }
            }

            Raylib.EndScissorMode();
            Rlgl.EnableBackfaceCulling();
        }

        void DrawTriangleVertex(ImDrawVertPtr idx_vert)
        {
            Color c = new Color((byte)(idx_vert.col >> 0), (byte)(idx_vert.col >> 8), (byte)(idx_vert.col >> 16), (byte)(idx_vert.col >> 24));
            Rlgl.Color4ub(c.R, c.G, c.B, c.A);
            Rlgl.TexCoord2f(idx_vert.uv.X, idx_vert.uv.Y);
            Rlgl.Vertex2f(idx_vert.pos.X, idx_vert.pos.Y);
        }

    }

}
