using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Engine.Input
{
    public class HotKeys
    {

        public static readonly string UP = "Up";
        public static readonly string DOWN = "Down";
        public static readonly string LEFT = "Left";
        public static readonly string RIGHT = "Right";
        public static readonly string CONFIRM = "Confirm";
        public static readonly string BACK = "Back";

        public static int MAX_HOTKEY_MAPS = 3;

        private static Dictionary<string, InputMethod[]> hotKeyMappings;
        public static IDictionary<string, InputMethod[]> HotKeyMappings => hotKeyMappings ??= new Dictionary<string, InputMethod[]>();

        private static bool isHotkeysDisabled;

        public static void DisableHotkeys(bool yes)
        {
            isHotkeysDisabled = yes;
        }

        public static void Init()
        {
            hotKeyMappings = new Dictionary<string, InputMethod[]>();
            hotKeyMappings[UP] = new InputMethod[MAX_HOTKEY_MAPS];
            hotKeyMappings[DOWN] = new InputMethod[MAX_HOTKEY_MAPS];
            hotKeyMappings[LEFT] = new InputMethod[MAX_HOTKEY_MAPS];
            hotKeyMappings[RIGHT] = new InputMethod[MAX_HOTKEY_MAPS];
            hotKeyMappings[CONFIRM] = new InputMethod[MAX_HOTKEY_MAPS];
            hotKeyMappings[BACK] = new InputMethod[MAX_HOTKEY_MAPS];

            hotKeyMappings[UP][0] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_UP
            };
            hotKeyMappings[UP][1] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_W
            };
            hotKeyMappings[UP][2] = new InputMethod()
            {
                Type = InputType.Controller,
                Button = (int)GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP
            };

            hotKeyMappings[DOWN][0] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_DOWN
            };
            hotKeyMappings[DOWN][1] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_S
            };
            hotKeyMappings[DOWN][2] = new InputMethod()
            {
                Type = InputType.Controller,
                Button = (int)GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN
            };

            hotKeyMappings[LEFT][0] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_LEFT
            };
            hotKeyMappings[LEFT][1] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_A
            };
            hotKeyMappings[LEFT][2] = new InputMethod()
            {
                Type = InputType.Controller,
                Button = (int)GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT
            };

            hotKeyMappings[RIGHT][0] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_RIGHT
            };
            hotKeyMappings[RIGHT][1] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_D
            };
            hotKeyMappings[RIGHT][2] = new InputMethod()
            {
                Type = InputType.Controller,
                Button = (int)GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT
            };

            hotKeyMappings[CONFIRM][0] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_ENTER
            };
            hotKeyMappings[CONFIRM][1] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_ENTER
            };
            hotKeyMappings[CONFIRM][2] = new InputMethod()
            {
                Type = InputType.Controller,
                Button = (int)GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN
            };

            hotKeyMappings[BACK][0] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_BACK
            };
            hotKeyMappings[BACK][1] = new InputMethod()
            {
                Type = InputType.Keyboard,
                Button = (int)KeyboardKey.KEY_BACK
            };
            hotKeyMappings[BACK][2] = new InputMethod()
            {
                Type = InputType.Controller,
                Button = (int)GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_RIGHT
            };
        }

        public static bool IsHotkeyDown(string hotkey)
        {
            if (!hotKeyMappings.ContainsKey(hotkey))
                return false;

            if (isHotkeysDisabled)
                return false;

            var mappings = hotKeyMappings[hotkey];
            var isDown = false;
            for (int i = 0; i < mappings.Length; i++)
            {
                var mapping = mappings[i];
                if (mapping.Type == InputType.Keyboard)
                {
                    if (Raylib.IsKeyDown((KeyboardKey)mapping.Button))
                    {
                        isDown = true;
                        break;
                    }
                }
                else if (mapping.Type == InputType.Controller)
                {
                    if (Raylib.IsGamepadAvailable(0)
                        && Raylib.IsGamepadButtonDown(0, (GamepadButton)mapping.Button))
                    {
                        isDown = true;
                        break;
                    }
                }
            }

            return isDown;
        }

        public static bool IsHotkeyUp(string hotkey)
        {
            if (!hotKeyMappings.ContainsKey(hotkey))
                return false;

            if (isHotkeysDisabled)
                return false;

            var mappings = hotKeyMappings[hotkey];
            var isUp = false;
            for (int i = 0; i < mappings.Length; i++)
            {
                if (mappings[i].Type == InputType.Keyboard)
                {
                    var requiresControl = mappings[i].Control;
                    var requiresAlt = mappings[i].Alt;
                    var requiresShift = mappings[i].Shift;

                    var hasControl = ((Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) ||
                        Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL)) && requiresControl) ||
                        (Raylib.IsKeyUp(KeyboardKey.KEY_LEFT_CONTROL) ||
                        Raylib.IsKeyUp(KeyboardKey.KEY_RIGHT_CONTROL) && !requiresControl);
                    var hasAlt = ((Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT) ||
                        Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT)) && requiresAlt) ||
                        (Raylib.IsKeyUp(KeyboardKey.KEY_LEFT_ALT) ||
                        Raylib.IsKeyUp(KeyboardKey.KEY_RIGHT_ALT) && !requiresAlt);
                    var hasShift = ((Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) ||
                        Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT)) && requiresShift) ||
                        (Raylib.IsKeyUp(KeyboardKey.KEY_LEFT_SHIFT) ||
                        Raylib.IsKeyUp(KeyboardKey.KEY_RIGHT_SHIFT) && !requiresShift);

                    if (Raylib.IsKeyUp((KeyboardKey)mappings[i].Button) &&
                        hasControl && hasAlt && hasShift)
                    {
                        isUp = true;
                        break;
                    }
                }
                else if (mappings[i].Type == InputType.Controller)
                {
                    var hasHoldButton = mappings[i].HoldButton > -1;
                    var holdingButton = (hasHoldButton && Raylib.IsGamepadButtonDown(0, (GamepadButton)mappings[i].HoldButton))
                        || (!hasHoldButton && Raylib.IsGamepadButtonUp(0, (GamepadButton)mappings[i].HoldButton));

                    if (holdingButton && Raylib.IsGamepadButtonUp(0, (GamepadButton)mappings[i].Button))
                    {
                        isUp = true;
                        break;
                    }
                }
            }

            return isUp;
        }

        public static bool IsHotkeyPressed(string hotkey)
        {
            if (!hotKeyMappings.ContainsKey(hotkey))
                return false;

            if (isHotkeysDisabled)
                return false;

            var mappings = hotKeyMappings[hotkey];
            var isUp = false;
            for (int i = 0; i < mappings.Length; i++)
            {
                if (mappings[i].Type == InputType.Keyboard)
                {
                    var requiresControl = mappings[i].Control;
                    var requiresAlt = mappings[i].Alt;
                    var requiresShift = mappings[i].Shift;

                    var hasControl = ((Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) ||
                        Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL)) && requiresControl) ||
                        (Raylib.IsKeyUp(KeyboardKey.KEY_LEFT_CONTROL) ||
                        Raylib.IsKeyUp(KeyboardKey.KEY_RIGHT_CONTROL) && !requiresControl);
                    var hasAlt = ((Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT) ||
                        Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT)) && requiresAlt) ||
                        (Raylib.IsKeyUp(KeyboardKey.KEY_LEFT_ALT) ||
                        Raylib.IsKeyUp(KeyboardKey.KEY_RIGHT_ALT) && !requiresAlt);
                    var hasShift = ((Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) ||
                        Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT)) && requiresShift) ||
                        (Raylib.IsKeyUp(KeyboardKey.KEY_LEFT_SHIFT) ||
                        Raylib.IsKeyUp(KeyboardKey.KEY_RIGHT_SHIFT) && !requiresShift);

                    if (Raylib.IsKeyPressed((KeyboardKey)mappings[i].Button) &&
                        hasControl && hasAlt && hasShift)
                    {
                        isUp = true;
                        break;
                    }
                }
                else if (mappings[i].Type == InputType.Controller)
                {
                    var hasHoldButton = mappings[i].HoldButton > -1;
                    var holdingButton = (hasHoldButton && Raylib.IsGamepadButtonDown(0, (GamepadButton)mappings[i].HoldButton))
                        || (!hasHoldButton && Raylib.IsGamepadButtonPressed(0, (GamepadButton)mappings[i].HoldButton));

                    if (holdingButton && Raylib.IsGamepadButtonPressed(0, (GamepadButton)mappings[i].Button))
                    {
                        isUp = true;
                        break;
                    }
                }
            }

            return isUp;
        }

        public static bool IsHotkeyReleased(string hotkey)
        {
            if (!hotKeyMappings.ContainsKey(hotkey))
                return false;

            if (isHotkeysDisabled)
                return false;

            var mappings = hotKeyMappings[hotkey];
            var isUp = false;
            for (int i = 0; i < mappings.Length; i++)
            {
                if (mappings[i].Type == InputType.Keyboard)
                {
                    var requiresControl = mappings[i].Control;
                    var requiresAlt = mappings[i].Alt;
                    var requiresShift = mappings[i].Shift;

                    var hasControl = ((Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) ||
                        Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL)) && requiresControl) ||
                        (Raylib.IsKeyUp(KeyboardKey.KEY_LEFT_CONTROL) ||
                        Raylib.IsKeyUp(KeyboardKey.KEY_RIGHT_CONTROL) && !requiresControl);
                    var hasAlt = ((Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT) ||
                        Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT)) && requiresAlt) ||
                        (Raylib.IsKeyUp(KeyboardKey.KEY_LEFT_ALT) ||
                        Raylib.IsKeyUp(KeyboardKey.KEY_RIGHT_ALT) && !requiresAlt);
                    var hasShift = ((Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) ||
                        Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT)) && requiresShift) ||
                        (Raylib.IsKeyUp(KeyboardKey.KEY_LEFT_SHIFT) ||
                        Raylib.IsKeyUp(KeyboardKey.KEY_RIGHT_SHIFT) && !requiresShift);

                    if (Raylib.IsKeyReleased((KeyboardKey)mappings[i].Button) &&
                        hasControl && hasAlt && hasShift)
                    {
                        isUp = true;
                        break;
                    }
                }
                else if (mappings[i].Type == InputType.Controller)
                {
                    var hasHoldButton = mappings[i].HoldButton > -1;
                    var holdingButton = (hasHoldButton && Raylib.IsGamepadButtonDown(0, (GamepadButton)mappings[i].HoldButton))
                        || (!hasHoldButton && Raylib.IsGamepadButtonReleased(0, (GamepadButton)mappings[i].HoldButton));

                    if (holdingButton && Raylib.IsGamepadButtonReleased(0, (GamepadButton)mappings[i].Button))
                    {
                        isUp = true;
                        break;
                    }
                }
            }

            return isUp;
        }

    }
}
