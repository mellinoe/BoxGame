using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics
{
    class OpenTKNativeWindowInfo : IWindowInfo
    {
        private NativeWindow _nativeWindow;
        public OpenTKNativeWindowInfo(NativeWindow window)
        {
            _nativeWindow = window;
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
    }
}
