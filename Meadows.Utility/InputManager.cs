using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

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

        public static bool IsKeyDown(Keys key) {
            return currentKeyState.IsKeyDown(key);
        }

        public static bool IsKeyReleased(Keys key) {
            return !currentKeyState.IsKeyDown(key) && previousKeyState.IsKeyDown(key);
        }

        public static Vector2 MousePosition => new Vector2(currentMouseState.X, currentMouseState.Y);
        public static float MouseX => currentMouseState.X;
        public static float MouseY => currentMouseState.Y;


        public static void SetMousePosition(int x, int y) {
            Mouse.SetPosition(x, y);
        }
    }
}
