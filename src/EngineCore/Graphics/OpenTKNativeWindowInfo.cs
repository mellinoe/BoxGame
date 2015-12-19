using OpenTK;

namespace EngineCore.Graphics
{
    class OpenTKNativeWindowInfo : IWindowInfo
    {
        private NativeWindow _nativeWindow;
        private float _scaleFactor;

        public OpenTKNativeWindowInfo(NativeWindow window, float scaleFactor)
        {
            _nativeWindow = window;
            _scaleFactor = scaleFactor;
        }

        public string Title
        {
            get
            {
                return _nativeWindow.Title;
            }

            set
            {
                _nativeWindow.Title = value;
            }
        }

        public Size Size
        {
            get
            {
                return _nativeWindow.ClientSize;
            }
            set
            {
                _nativeWindow.ClientSize = value;
            }
        }

        public bool IsFullscreen
        {
            get
            {
                return _nativeWindow.WindowState == WindowState.Fullscreen;
            }

            set
            {
                _nativeWindow.WindowState = value ? WindowState.Fullscreen : WindowState.Normal;
            }
        }

        public int Width
        {
            get
            {
                return _nativeWindow.Width;
            }

            set
            {
                _nativeWindow.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return _nativeWindow.Height;
            }

            set
            {
                _nativeWindow.Height = value;
            }
        }

        public float ScaleFactor => _scaleFactor;
    }
}
