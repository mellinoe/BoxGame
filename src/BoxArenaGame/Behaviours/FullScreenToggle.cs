using EngineCore;
using EngineCore.Components;
using EngineCore.Entities;
using EngineCore.Graphics;
using EngineCore.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxArenaGame.Behaviours
{
    public class FullScreenToggle : Behaviour<InputSystem, GraphicsSystem>
    {
        private GraphicsSystem _graphics;
        private InputSystem _input;

        protected override void Update()
        {
            if (_input.GetKeyDown(KeyCode.F12))
            {
                ToggleFullscreen();
            }
        }

        private void ToggleFullscreen()
        {
            _graphics.WindowInfo.IsFullscreen = !_graphics.WindowInfo.IsFullscreen;
        }

        protected override void Initialize(InputSystem system1, GraphicsSystem system2)
        {
            _input = system1;
            _graphics = system2;
        }

        protected override void Uninitialize(InputSystem system1, GraphicsSystem system2)
        {
            _input = null;
            _graphics = null;
        }
    }
}
