using EngineCore;
using EngineCore.Graphics;
using EngineCore.Input;
using EngineCore.Services;

namespace BoxArenaGame.Behaviours
{
    public class FullScreenToggle : Behaviour
    {
        [AutoInject]
        public IInputService InputService { get; set; }

        [AutoInject]
        private IGraphicsService GraphicsService { get; set; }

        protected override void Update()
        {
            if (InputService.GetKeyDown(KeyCode.F12))
            {
                ToggleFullscreen();
            }
        }

        private void ToggleFullscreen()
        {
            GraphicsService.WindowInfo.IsFullscreen = !GraphicsService.WindowInfo.IsFullscreen;
        }
    }
}
