using Microsoft.Xna.Framework.Input;
using System;

namespace Meadows.Utility {
    public static class InputManager {
        private static KeyboardState currentKeyState;
        private static KeyboardState previousKeyState;
        private static MouseState currentMouseState;
        private static MouseState previousMouseState;
        public static int previousScroll;
        public static int currentScroll;

        public static void Update() {
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            previousScroll = currentScroll;
            currentScroll = currentMouseState.ScrollWheelValue;
        }

        public static bool IsKeyPressed(Keys key) {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }
    }
}
