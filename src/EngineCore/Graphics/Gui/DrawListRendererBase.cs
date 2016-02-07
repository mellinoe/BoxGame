using ImGuiNET;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace EngineCore.Graphics.Gui
{
    internal abstract class DrawListRendererBase : IDrawListRenderer
    {
        internal NativeWindow _nativeWindow;
        internal IWindowInfo _windowInfo;
        private float _wheelPosition;

        public DrawListRendererBase(NativeWindow nativeWindow, IWindowInfo windowInfo)
        {
            _nativeWindow = nativeWindow;
            _windowInfo = windowInfo;

            _nativeWindow.KeyDown += OnKeyDown;
            _nativeWindow.KeyUp += OnKeyUp;
            _nativeWindow.KeyPress += OnKeyPress;

            ImGui.LoadDefaultFont();
            SetOpenTKKeyMappings();
            SetPerFrameImGuiData();
        }

        public unsafe void Render()
        {
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData());
            SetPerFrameImGuiData();
            UpdateImGuiInput(ImGui.GetIO());
            ImGui.NewFrame();
        }

        internal abstract unsafe void RenderImDrawData(DrawData* drawData);

        private static unsafe void SetOpenTKKeyMappings()
        {
            IO io = ImGui.GetIO();
            io.KeyMap[GuiKey.Tab] = (int)Key.Tab;
            io.KeyMap[GuiKey.LeftArrow] = (int)Key.Left;
            io.KeyMap[GuiKey.RightArrow] = (int)Key.Right;
            io.KeyMap[GuiKey.UpArrow] = (int)Key.Up;
            io.KeyMap[GuiKey.DownArrow] = (int)Key.Down;
            io.KeyMap[GuiKey.PageUp] = (int)Key.PageUp;
            io.KeyMap[GuiKey.PageDown] = (int)Key.PageDown;
            io.KeyMap[GuiKey.Home] = (int)Key.Home;
            io.KeyMap[GuiKey.End] = (int)Key.End;
            io.KeyMap[GuiKey.Delete] = (int)Key.Delete;
            io.KeyMap[GuiKey.Backspace] = (int)Key.BackSpace;
            io.KeyMap[GuiKey.Enter] = (int)Key.Enter;
            io.KeyMap[GuiKey.Escape] = (int)Key.Escape;
            io.KeyMap[GuiKey.A] = (int)Key.A;
            io.KeyMap[GuiKey.C] = (int)Key.C;
            io.KeyMap[GuiKey.V] = (int)Key.V;
            io.KeyMap[GuiKey.X] = (int)Key.X;
            io.KeyMap[GuiKey.Y] = (int)Key.Y;
            io.KeyMap[GuiKey.Z] = (int)Key.Z;
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            ImGui.AddInputCharacter(e.KeyChar);
        }

        private unsafe void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            ImGui.GetIO().KeysDown[(int)e.Key] = true;
            UpdateModifiers(e);
        }

        private unsafe void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            ImGui.GetIO().KeysDown[(int)e.Key] = false;
            UpdateModifiers(e);
        }

        private static unsafe void UpdateModifiers(KeyboardKeyEventArgs e)
        {
            IO io = ImGui.GetIO();
            io.AltPressed = e.Alt;
            io.CtrlPressed = e.Control;
            io.ShiftPressed = e.Shift;
        }

        private unsafe void SetPerFrameImGuiData()
        {
            IO io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(
                _windowInfo.Width,
                _windowInfo.Height);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(_windowInfo.ScaleFactor);
            io.DeltaTime = Time.DeltaTime;
        }

        private unsafe void UpdateImGuiInput(IO io)
        {
            MouseState cursorState = Mouse.GetCursorState();
            MouseState mouseState = Mouse.GetState();

            if (_nativeWindow.Bounds.Contains(cursorState.X, cursorState.Y))
            {
                Point windowPoint = _nativeWindow.PointToClient(new Point(cursorState.X, cursorState.Y));
                io.MousePosition = new System.Numerics.Vector2(
                    windowPoint.X / _windowInfo.ScaleFactor,
                    windowPoint.Y / _windowInfo.ScaleFactor);
            }
            else
            {
                io.MousePosition = new System.Numerics.Vector2(-1f, -1f);
            }

            io.MouseDown[0] = mouseState.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouseState.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouseState.MiddleButton == ButtonState.Pressed;

            float newWheelPos = mouseState.WheelPrecise;
            float delta = newWheelPos - _wheelPosition;
            _wheelPosition = newWheelPos;
            io.MouseWheel = delta;
        }

        public abstract void Dispose();
    }
}
