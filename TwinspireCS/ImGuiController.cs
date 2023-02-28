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
    internal unsafe class ImGuiController
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

        public void AddFontFile(string fontFile, int fontSize)
        {
            fontFilesToLoad.Add(fontFile);
            fontFilesFontSizes.Add(fontSize);
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
        }

        public void Init()
        {
            var io = ImGui.GetIO();

            io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyboardKey.KEY_TAB;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyboardKey.KEY_LEFT;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyboardKey.KEY_RIGHT;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyboardKey.KEY_UP;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyboardKey.KEY_DOWN;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)KeyboardKey.KEY_PAGE_UP;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)KeyboardKey.KEY_PAGE_DOWN;
            io.KeyMap[(int)ImGuiKey.Home] = (int)KeyboardKey.KEY_HOME;
            io.KeyMap[(int)ImGuiKey.End] = (int)KeyboardKey.KEY_END;
            io.KeyMap[(int)ImGuiKey.Insert] = (int)KeyboardKey.KEY_INSERT;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyboardKey.KEY_DELETE;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyboardKey.KEY_BACKSPACE;
            io.KeyMap[(int)ImGuiKey.Space] = (int)KeyboardKey.KEY_SPACE;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyboardKey.KEY_ENTER;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)KeyboardKey.KEY_ESCAPE;
            io.KeyMap[(int)ImGuiKey.KeypadEnter] = (int)KeyboardKey.KEY_KP_ENTER;
            io.KeyMap[(int)ImGuiKey.A] = (int)KeyboardKey.KEY_A;
            io.KeyMap[(int)ImGuiKey.C] = (int)KeyboardKey.KEY_C;
            io.KeyMap[(int)ImGuiKey.V] = (int)KeyboardKey.KEY_V;
            io.KeyMap[(int)ImGuiKey.X] = (int)KeyboardKey.KEY_X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)KeyboardKey.KEY_Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)KeyboardKey.KEY_Z;

            mousePosition = new Vector2(0, 0);
            io.MousePos = mousePosition;

            for (int i = 0; i < fontFilesToLoad.Count; i++)
            {
                io.Fonts.AddFontFromFileTTF(fontFilesToLoad[i], fontFilesFontSizes[i]);
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

            io.KeyCtrl = isKeyCtrl;
            io.KeyAlt = isKeyAlt;
            io.KeyShift = isKeyShift;
            io.KeySuper = isKeySuper;

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

        public bool ProcessEvent()
        {
            var io = ImGui.GetIO();

            io.KeysDown[(int)KeyboardKey.KEY_APOSTROPHE] = Raylib.IsKeyDown(KeyboardKey.KEY_APOSTROPHE);
            io.KeysDown[(int)KeyboardKey.KEY_COMMA] = Raylib.IsKeyDown(KeyboardKey.KEY_COMMA);
            io.KeysDown[(int)KeyboardKey.KEY_MINUS] = Raylib.IsKeyDown(KeyboardKey.KEY_MINUS);
            io.KeysDown[(int)KeyboardKey.KEY_PERIOD] = Raylib.IsKeyDown(KeyboardKey.KEY_PERIOD);
            io.KeysDown[(int)KeyboardKey.KEY_SLASH] = Raylib.IsKeyDown(KeyboardKey.KEY_SLASH);
            io.KeysDown[(int)KeyboardKey.KEY_ZERO] = Raylib.IsKeyDown(KeyboardKey.KEY_ZERO);
            io.KeysDown[(int)KeyboardKey.KEY_ONE] = Raylib.IsKeyDown(KeyboardKey.KEY_ONE);
            io.KeysDown[(int)KeyboardKey.KEY_TWO] = Raylib.IsKeyDown(KeyboardKey.KEY_TWO);
            io.KeysDown[(int)KeyboardKey.KEY_THREE] = Raylib.IsKeyDown(KeyboardKey.KEY_THREE);
            io.KeysDown[(int)KeyboardKey.KEY_FOUR] = Raylib.IsKeyDown(KeyboardKey.KEY_FOUR);
            io.KeysDown[(int)KeyboardKey.KEY_FIVE] = Raylib.IsKeyDown(KeyboardKey.KEY_FIVE);
            io.KeysDown[(int)KeyboardKey.KEY_SIX] = Raylib.IsKeyDown(KeyboardKey.KEY_SIX);
            io.KeysDown[(int)KeyboardKey.KEY_SEVEN] = Raylib.IsKeyDown(KeyboardKey.KEY_SEVEN);
            io.KeysDown[(int)KeyboardKey.KEY_EIGHT] = Raylib.IsKeyDown(KeyboardKey.KEY_EIGHT);
            io.KeysDown[(int)KeyboardKey.KEY_NINE] = Raylib.IsKeyDown(KeyboardKey.KEY_NINE);
            io.KeysDown[(int)KeyboardKey.KEY_SEMICOLON] = Raylib.IsKeyDown(KeyboardKey.KEY_SEMICOLON);
            io.KeysDown[(int)KeyboardKey.KEY_EQUAL] = Raylib.IsKeyDown(KeyboardKey.KEY_EQUAL);
            io.KeysDown[(int)KeyboardKey.KEY_A] = Raylib.IsKeyDown(KeyboardKey.KEY_A);
            io.KeysDown[(int)KeyboardKey.KEY_B] = Raylib.IsKeyDown(KeyboardKey.KEY_B);
            io.KeysDown[(int)KeyboardKey.KEY_C] = Raylib.IsKeyDown(KeyboardKey.KEY_C);
            io.KeysDown[(int)KeyboardKey.KEY_D] = Raylib.IsKeyDown(KeyboardKey.KEY_D);
            io.KeysDown[(int)KeyboardKey.KEY_E] = Raylib.IsKeyDown(KeyboardKey.KEY_E);
            io.KeysDown[(int)KeyboardKey.KEY_F] = Raylib.IsKeyDown(KeyboardKey.KEY_F);
            io.KeysDown[(int)KeyboardKey.KEY_G] = Raylib.IsKeyDown(KeyboardKey.KEY_G);
            io.KeysDown[(int)KeyboardKey.KEY_H] = Raylib.IsKeyDown(KeyboardKey.KEY_H);
            io.KeysDown[(int)KeyboardKey.KEY_I] = Raylib.IsKeyDown(KeyboardKey.KEY_I);
            io.KeysDown[(int)KeyboardKey.KEY_J] = Raylib.IsKeyDown(KeyboardKey.KEY_J);
            io.KeysDown[(int)KeyboardKey.KEY_K] = Raylib.IsKeyDown(KeyboardKey.KEY_K);
            io.KeysDown[(int)KeyboardKey.KEY_L] = Raylib.IsKeyDown(KeyboardKey.KEY_L);
            io.KeysDown[(int)KeyboardKey.KEY_M] = Raylib.IsKeyDown(KeyboardKey.KEY_M);
            io.KeysDown[(int)KeyboardKey.KEY_N] = Raylib.IsKeyDown(KeyboardKey.KEY_N);
            io.KeysDown[(int)KeyboardKey.KEY_O] = Raylib.IsKeyDown(KeyboardKey.KEY_O);
            io.KeysDown[(int)KeyboardKey.KEY_P] = Raylib.IsKeyDown(KeyboardKey.KEY_P);
            io.KeysDown[(int)KeyboardKey.KEY_Q] = Raylib.IsKeyDown(KeyboardKey.KEY_Q);
            io.KeysDown[(int)KeyboardKey.KEY_R] = Raylib.IsKeyDown(KeyboardKey.KEY_R);
            io.KeysDown[(int)KeyboardKey.KEY_S] = Raylib.IsKeyDown(KeyboardKey.KEY_S);
            io.KeysDown[(int)KeyboardKey.KEY_T] = Raylib.IsKeyDown(KeyboardKey.KEY_T);
            io.KeysDown[(int)KeyboardKey.KEY_U] = Raylib.IsKeyDown(KeyboardKey.KEY_U);
            io.KeysDown[(int)KeyboardKey.KEY_V] = Raylib.IsKeyDown(KeyboardKey.KEY_V);
            io.KeysDown[(int)KeyboardKey.KEY_W] = Raylib.IsKeyDown(KeyboardKey.KEY_W);
            io.KeysDown[(int)KeyboardKey.KEY_X] = Raylib.IsKeyDown(KeyboardKey.KEY_X);
            io.KeysDown[(int)KeyboardKey.KEY_Y] = Raylib.IsKeyDown(KeyboardKey.KEY_Y);
            io.KeysDown[(int)KeyboardKey.KEY_Z] = Raylib.IsKeyDown(KeyboardKey.KEY_Z);
            io.KeysDown[(int)KeyboardKey.KEY_SPACE] = Raylib.IsKeyDown(KeyboardKey.KEY_SPACE);
            io.KeysDown[(int)KeyboardKey.KEY_ESCAPE] = Raylib.IsKeyDown(KeyboardKey.KEY_ESCAPE);
            io.KeysDown[(int)KeyboardKey.KEY_ENTER] = Raylib.IsKeyDown(KeyboardKey.KEY_ENTER);
            io.KeysDown[(int)KeyboardKey.KEY_TAB] = Raylib.IsKeyDown(KeyboardKey.KEY_TAB);
            io.KeysDown[(int)KeyboardKey.KEY_BACKSPACE] = Raylib.IsKeyDown(KeyboardKey.KEY_BACKSPACE);
            io.KeysDown[(int)KeyboardKey.KEY_INSERT] = Raylib.IsKeyDown(KeyboardKey.KEY_INSERT);
            io.KeysDown[(int)KeyboardKey.KEY_DELETE] = Raylib.IsKeyDown(KeyboardKey.KEY_DELETE);
            io.KeysDown[(int)KeyboardKey.KEY_RIGHT] = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT);
            io.KeysDown[(int)KeyboardKey.KEY_LEFT] = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT);
            io.KeysDown[(int)KeyboardKey.KEY_DOWN] = Raylib.IsKeyDown(KeyboardKey.KEY_DOWN);
            io.KeysDown[(int)KeyboardKey.KEY_UP] = Raylib.IsKeyDown(KeyboardKey.KEY_UP);
            io.KeysDown[(int)KeyboardKey.KEY_PAGE_UP] = Raylib.IsKeyDown(KeyboardKey.KEY_PAGE_UP);
            io.KeysDown[(int)KeyboardKey.KEY_PAGE_DOWN] = Raylib.IsKeyDown(KeyboardKey.KEY_PAGE_DOWN);
            io.KeysDown[(int)KeyboardKey.KEY_HOME] = Raylib.IsKeyDown(KeyboardKey.KEY_HOME);
            io.KeysDown[(int)KeyboardKey.KEY_END] = Raylib.IsKeyDown(KeyboardKey.KEY_END);
            io.KeysDown[(int)KeyboardKey.KEY_CAPS_LOCK] = Raylib.IsKeyDown(KeyboardKey.KEY_CAPS_LOCK);
            io.KeysDown[(int)KeyboardKey.KEY_SCROLL_LOCK] = Raylib.IsKeyDown(KeyboardKey.KEY_SCROLL_LOCK);
            io.KeysDown[(int)KeyboardKey.KEY_NUM_LOCK] = Raylib.IsKeyDown(KeyboardKey.KEY_NUM_LOCK);
            io.KeysDown[(int)KeyboardKey.KEY_PRINT_SCREEN] = Raylib.IsKeyDown(KeyboardKey.KEY_PRINT_SCREEN);
            io.KeysDown[(int)KeyboardKey.KEY_PAUSE] = Raylib.IsKeyDown(KeyboardKey.KEY_PAUSE);
            io.KeysDown[(int)KeyboardKey.KEY_F1] = Raylib.IsKeyDown(KeyboardKey.KEY_F1);
            io.KeysDown[(int)KeyboardKey.KEY_F2] = Raylib.IsKeyDown(KeyboardKey.KEY_F2);
            io.KeysDown[(int)KeyboardKey.KEY_F3] = Raylib.IsKeyDown(KeyboardKey.KEY_F3);
            io.KeysDown[(int)KeyboardKey.KEY_F4] = Raylib.IsKeyDown(KeyboardKey.KEY_F4);
            io.KeysDown[(int)KeyboardKey.KEY_F5] = Raylib.IsKeyDown(KeyboardKey.KEY_F5);
            io.KeysDown[(int)KeyboardKey.KEY_F6] = Raylib.IsKeyDown(KeyboardKey.KEY_F6);
            io.KeysDown[(int)KeyboardKey.KEY_F7] = Raylib.IsKeyDown(KeyboardKey.KEY_F7);
            io.KeysDown[(int)KeyboardKey.KEY_F8] = Raylib.IsKeyDown(KeyboardKey.KEY_F8);
            io.KeysDown[(int)KeyboardKey.KEY_F9] = Raylib.IsKeyDown(KeyboardKey.KEY_F9);
            io.KeysDown[(int)KeyboardKey.KEY_F10] = Raylib.IsKeyDown(KeyboardKey.KEY_F10);
            io.KeysDown[(int)KeyboardKey.KEY_F11] = Raylib.IsKeyDown(KeyboardKey.KEY_F11);
            io.KeysDown[(int)KeyboardKey.KEY_F12] = Raylib.IsKeyDown(KeyboardKey.KEY_F12);
            io.KeysDown[(int)KeyboardKey.KEY_LEFT_SHIFT] = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);
            io.KeysDown[(int)KeyboardKey.KEY_LEFT_CONTROL] = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL);
            io.KeysDown[(int)KeyboardKey.KEY_LEFT_ALT] = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT);
            io.KeysDown[(int)KeyboardKey.KEY_LEFT_SUPER] = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SUPER);
            io.KeysDown[(int)KeyboardKey.KEY_RIGHT_SHIFT] = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT);
            io.KeysDown[(int)KeyboardKey.KEY_RIGHT_CONTROL] = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL);
            io.KeysDown[(int)KeyboardKey.KEY_RIGHT_ALT] = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT);
            io.KeysDown[(int)KeyboardKey.KEY_RIGHT_SUPER] = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SUPER);
            io.KeysDown[(int)KeyboardKey.KEY_KB_MENU] = Raylib.IsKeyDown(KeyboardKey.KEY_KB_MENU);
            io.KeysDown[(int)KeyboardKey.KEY_LEFT_BRACKET] = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_BRACKET);
            io.KeysDown[(int)KeyboardKey.KEY_BACKSLASH] = Raylib.IsKeyDown(KeyboardKey.KEY_BACKSLASH);
            io.KeysDown[(int)KeyboardKey.KEY_RIGHT_BRACKET] = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_BRACKET);
            io.KeysDown[(int)KeyboardKey.KEY_GRAVE] = Raylib.IsKeyDown(KeyboardKey.KEY_GRAVE);
            io.KeysDown[(int)KeyboardKey.KEY_KP_0] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_0);
            io.KeysDown[(int)KeyboardKey.KEY_KP_1] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_1);
            io.KeysDown[(int)KeyboardKey.KEY_KP_2] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_2);
            io.KeysDown[(int)KeyboardKey.KEY_KP_3] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_3);
            io.KeysDown[(int)KeyboardKey.KEY_KP_4] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_4);
            io.KeysDown[(int)KeyboardKey.KEY_KP_5] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_5);
            io.KeysDown[(int)KeyboardKey.KEY_KP_6] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_6);
            io.KeysDown[(int)KeyboardKey.KEY_KP_7] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_7);
            io.KeysDown[(int)KeyboardKey.KEY_KP_8] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_8);
            io.KeysDown[(int)KeyboardKey.KEY_KP_9] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_9);
            io.KeysDown[(int)KeyboardKey.KEY_KP_DECIMAL] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_DECIMAL);
            io.KeysDown[(int)KeyboardKey.KEY_KP_DIVIDE] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_DIVIDE);
            io.KeysDown[(int)KeyboardKey.KEY_KP_MULTIPLY] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_MULTIPLY);
            io.KeysDown[(int)KeyboardKey.KEY_KP_SUBTRACT] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_SUBTRACT);
            io.KeysDown[(int)KeyboardKey.KEY_KP_ADD] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_ADD);
            io.KeysDown[(int)KeyboardKey.KEY_KP_ENTER] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_ENTER);
            io.KeysDown[(int)KeyboardKey.KEY_KP_EQUAL] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_EQUAL);
            io.KeysDown[(int)KeyboardKey.KEY_BACK] = Raylib.IsKeyDown(KeyboardKey.KEY_BACK);
            io.KeysDown[(int)KeyboardKey.KEY_MENU] = Raylib.IsKeyDown(KeyboardKey.KEY_MENU);
            io.KeysDown[(int)KeyboardKey.KEY_VOLUME_UP] = Raylib.IsKeyDown(KeyboardKey.KEY_VOLUME_UP);
            io.KeysDown[(int)KeyboardKey.KEY_VOLUME_DOWN] = Raylib.IsKeyDown(KeyboardKey.KEY_VOLUME_DOWN);

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
                Image image;

                io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bpp);
                var size = Raylib.GetPixelDataSize(width, height, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
                image.data = (void*)Marshal.AllocHGlobal(size);
                Buffer.MemoryCopy(pixels, image.data, size, size);
                image.width = width;
                image.height = height;
                image.mipmaps = 1;
                image.format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8;
                var tex = Raylib.LoadTextureFromImage(image);
                g_AtlasTexID = tex.id;
                io.Fonts.TexID = (IntPtr)g_AtlasTexID;
                Marshal.FreeHGlobal((IntPtr)pixels);
                Marshal.FreeHGlobal((IntPtr)image.data);
                g_UnloadAtlas = true;
            }
        }

        public void Render(ImDrawDataPtr draw_data)
        {
            Rlgl.rlDisableBackfaceCulling();
            for (int n = 0; n < draw_data.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdListsRange[n];
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

                            Rlgl.rlPushMatrix();
                            Rlgl.rlBegin(DrawMode.TRIANGLES);
                            Rlgl.rlSetTexture((uint)ti.ToInt32());

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

                            Rlgl.rlDisableTexture();
                            Rlgl.rlEnd();
                            Rlgl.rlPopMatrix();
                        }
                    }

                    idx_index += pcmd.ElemCount;
                }
            }

            Raylib.EndScissorMode();
            Rlgl.rlEnableBackfaceCulling();
        }

        void DrawTriangleVertex(ImDrawVertPtr idx_vert)
        {
            Color c = new Color((byte)(idx_vert.col >> 0), (byte)(idx_vert.col >> 8), (byte)(idx_vert.col >> 16), (byte)(idx_vert.col >> 24));
            Rlgl.rlColor4ub(c.r, c.g, c.b, c.a);
            Rlgl.rlTexCoord2f(idx_vert.uv.X, idx_vert.uv.Y);
            Rlgl.rlVertex2f(idx_vert.pos.X, idx_vert.pos.Y);
        }

    }

}
