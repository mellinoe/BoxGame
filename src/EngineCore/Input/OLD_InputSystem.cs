using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineCore.Input
{
    public class InputSystem : GameSystem
    {
        public InputSystem(Game game) : base(game) { }

        public static bool GetMouseButtonDown(MouseButtons button)
        {
            return newMouseButtonsDownThisFrame.Contains(button);
        }
        public static bool GetMouseButton(MouseButtons button)
        {
            return currentlyPressedMouseButtons.Contains(button);
        }

        public static bool GetKeyDown(Keys key)
        {
            return newKeysDownThisFrame.Contains(key);
        }
        public static bool GetKey(Keys key)
        {
            return currentlyPressedKeys.Contains(key);
        }

        public static Vector2 MousePosition { get; private set; }
        public static float MouseWheelDelta { get; private set; }

        private RenderForm gameWindow;

        private static HashSet<Keys> currentlyPressedKeys = new HashSet<Keys>();
        private static HashSet<Keys> newKeysDownThisFrame = new HashSet<Keys>();

        private static HashSet<Keys> newlyQueuedKeys = new HashSet<Keys>();
        private static HashSet<MouseButtons> newlyQueuedMouseButtons = new HashSet<MouseButtons>();

        private static HashSet<MouseButtons> currentlyPressedMouseButtons = new HashSet<MouseButtons>();
        private static HashSet<MouseButtons> newMouseButtonsDownThisFrame = new HashSet<MouseButtons>();

        public override void Start()
        {
            this.gameWindow = Game.GraphicsSystem.Renderer.Form;
            gameWindow.MouseDown += OnMouseDown;
            gameWindow.MouseUp += OnMouseUp;
            gameWindow.MouseMove += OnMouseMoved;
            gameWindow.KeyDown += OnKeyDown;
            gameWindow.KeyUp += OnKeyUp;
            gameWindow.MouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            MouseWheelDelta = e.Delta;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            currentlyPressedKeys.Remove(e.KeyCode);
            newlyQueuedKeys.Remove(e.KeyCode);
            newKeysDownThisFrame.Remove(e.KeyCode);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (currentlyPressedKeys.Add(e.KeyCode))
            {
                newlyQueuedKeys.Add(e.KeyCode);
            }
        }

        private void OnMouseMoved(object sender, MouseEventArgs e)
        {
            MousePosition = new Vector2(e.X, e.Y);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            currentlyPressedMouseButtons.Remove(e.Button);
            newlyQueuedMouseButtons.Remove(e.Button);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (currentlyPressedMouseButtons.Add(e.Button))
            {
                newlyQueuedMouseButtons.Add(e.Button);
            }
        }

        public override void Stop()
        {

        }

        public override void Update()
        {
            MouseWheelDelta = 0f;

            newMouseButtonsDownThisFrame = newlyQueuedMouseButtons;
            newlyQueuedMouseButtons = new HashSet<MouseButtons>();

            newKeysDownThisFrame = newlyQueuedKeys;
            newlyQueuedKeys = new HashSet<Keys>();
        }
    }
}
