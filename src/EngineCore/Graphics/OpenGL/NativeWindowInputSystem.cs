using EngineCore.Input;
using System.Collections.Generic;
using System.Numerics;

namespace EngineCore.Graphics.OpenGL
{
    internal class NativeWindowInputSystem : InputSystem
    {
        private OpenTK.NativeWindow _window;

        private HashSet<KeyCode> currentlyPressedKeys = new HashSet<KeyCode>();
        private HashSet<KeyCode> newKeysDownThisFrame = new HashSet<KeyCode>();
        private HashSet<KeyCode> newlyQueuedKeys = new HashSet<KeyCode>();

        private HashSet<MouseButton> currentlyPressedMouseButtons = new HashSet<MouseButton>();
        private HashSet<MouseButton> newMouseButtonsDownThisFrame = new HashSet<MouseButton>();
        private HashSet<MouseButton> newlyQueuedMouseButtons = new HashSet<MouseButton>();
        private Vector2 _mousePosition;

        public override Vector2 MousePosition { get { return _mousePosition; } }

        public NativeWindowInputSystem(Game game, OpenTK.NativeWindow nativeWindow)
            : base(game)
        {
            _window = nativeWindow;

            _window.KeyDown += OnKeyDown;
            _window.KeyUp += OnKeyUp;

            _window.MouseDown += OnMouseDown;
            _window.MouseUp += OnMouseUp;
            _window.MouseMove += OnMouseMoved;
        }

        private void OnMouseMoved(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            _mousePosition = new Vector2(e.X, e.Y);
        }

        private void OnMouseUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            currentlyPressedMouseButtons.Remove((MouseButton)e.Button);
            newlyQueuedMouseButtons.Remove((MouseButton)e.Button);
            newMouseButtonsDownThisFrame.Remove((MouseButton)e.Button);
        }

        private void OnMouseDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            if (currentlyPressedMouseButtons.Add((MouseButton)e.Button))
            {
                newlyQueuedMouseButtons.Add((MouseButton)e.Button);
            }
        }

        private void OnKeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            currentlyPressedKeys.Remove((KeyCode)e.Key);
            newlyQueuedKeys.Remove((KeyCode)e.Key);
            newKeysDownThisFrame.Remove((KeyCode)e.Key);
        }

        private void OnKeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (currentlyPressedKeys.Add((KeyCode)e.Key))
            {
                newlyQueuedKeys.Add((KeyCode)e.Key);
            }
        }

        public override bool GetKeyDown(KeyCode key)
        {
            return newKeysDownThisFrame.Contains(key);
        }

        public override bool GetKey(KeyCode key)
        {
            return currentlyPressedKeys.Contains(key);
        }

        public override bool GetMouseButtonDown(MouseButton button)
        {
            return newMouseButtonsDownThisFrame.Contains(button);
        }

        public override bool GetMouseButton(MouseButton button)
        {
            return currentlyPressedMouseButtons.Contains(button);
        }

        public override void Start()
        {

        }

        public override void Stop()
        {

        }

        public override void Update()
        {
            HashSet<KeyCode> temp = newKeysDownThisFrame;

            newKeysDownThisFrame = newlyQueuedKeys;
            newlyQueuedKeys = temp;

            newlyQueuedKeys.Clear();
        }
    }
}
