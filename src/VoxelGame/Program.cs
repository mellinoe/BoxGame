using EngineCore;
using EngineCore.Entities;
using EngineCore.Graphics;
using EngineCore.Physics;
using GameApplication.Behaviours;
using Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using VoxelGame.World;

namespace VoxelGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Game game = new VoxelGame();
            game.Start();
        }

        public class VoxelGame : Game
        {
            protected override void PerformCustomInitialization()
            {
                CreateFpsTracker();
                AddNoiseWorldStartingStuff();
                CreateNoiseGeneratedWorld();
            }

            private static void CreateFpsTracker()
            {
#if FEATURE_TEXT_RENDERING
                var textRendererObj = new GameObject();
                EngineCore.Graphics.OpenGL.TextRenderer textRenderer = new EngineCore.Graphics.OpenGL.TextRenderer();
                textRendererObj.AddComponent(textRenderer);
                FpsTracker fpsTracker = new FpsTracker();
                fpsTracker.UpdateFrequency = 1.0 / 3.0;
                textRendererObj.AddComponent(fpsTracker);
                fpsTracker.FramesPerSecondUpdated += (value) => textRenderer.DrawText("FPS: " + value.ToString("###.00"), 15, 15);
#endif
            }

            private void CreateNoiseGeneratedWorld()
            {
                VoxelWorldSystem voxelWorldSystem = new VoxelWorldSystem(this);
                AddGameSystem(voxelWorldSystem);
            }

            private static void AddNoiseWorldStartingStuff()
            {
                var camera = new GameObject();
                camera.AddComponent<Camera>();
                camera.AddComponent<FreeFlyMovement>();
            }
        }
    }
}
