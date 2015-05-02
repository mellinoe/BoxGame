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
                CreateNoiseGeneratedWorld();
                CreateFpsTracker();
            }

            private static void CreateFpsTracker()
            {
                var textRendererObj = new GameObject();
                EngineCore.Graphics.OpenGL.TextRenderer textRenderer = new EngineCore.Graphics.OpenGL.TextRenderer();
                textRendererObj.AddComponent(textRenderer);
                FpsTracker fpsTracker = new FpsTracker();
                textRendererObj.AddComponent(fpsTracker);
                fpsTracker.FramesPerSecondUpdated += (value) => textRenderer.DrawText("FPS: " + value.ToString("###.00"), 15, 15);
            }

            private static void CreateNoiseGeneratedWorld()
            {
                AddNoiseWorldStartingStuff();

                float xScale = 1f;
                float yScale = 1f;

                NoiseGen noiseGen = new NoiseGen(xScale, yScale, 4);

                int xMax = 60;
                int yMax = 15;
                int zMax = 60;
                float frequency = 1.0f / (float)xMax;

                for (int x = 0; x < xMax; x++)
                {
                    for (int y = 0; y < yMax; y++)
                    {
                        for (int z = 0; z < zMax; z++)
                        {
                            float noiseVal = noiseGen.GetNoise(x * frequency, y * frequency, z * frequency);
                            if (noiseVal > .61f)
                            {
                                GameObject.CreateStaticBox(1f, 1f, 1f).Transform.Position
                                    = new Vector3(x - (xMax / 2f), y - yMax, z - (zMax / 2f));
                            }
                        }
                    }
                }
            }

            private static void AddNoiseWorldStartingStuff()
            {
                var camera = new GameObject();
                camera.AddComponent<Camera>();
                camera.AddComponent<BoxLauncher>();
                camera.AddComponent<FreeFlyMovement>();
            }
        }
    }
}
