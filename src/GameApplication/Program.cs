﻿using EngineCore;
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

namespace GameApplication
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Game game = new BoxGame();
            game.Start();
        }

        public class BoxGame : Game
        {
            protected override void PerformCustomInitialization()
            {
                AddStartingStuff();
            }

            private void AddStartingStuff()
            {
                var character = new GameObject();
                character.AddComponent<CharacterController>();
                character.AddComponent<SimpleFpsController>();
                character.Transform.Position = new Vector3(0, 2f, 5f);

                var camera = new GameObject();
                camera.AddComponent<Camera>();
                camera.AddComponent<BoxLauncher>();
                var fpsLookController = camera.AddComponent<FpsLookController>();
                fpsLookController.Tracked = character.Transform;

                //CreateStandardBoxArena();
                CreateNoiseGeneratedWorld();

                var textRendererObj = new GameObject();
                EngineCore.Graphics.OpenGL.TextRenderer textRenderer = new EngineCore.Graphics.OpenGL.TextRenderer();
                textRendererObj.AddComponent(textRenderer);
                FpsTracker fpsTracker = new FpsTracker();
                textRendererObj.AddComponent(fpsTracker);
                fpsTracker.FramesPerSecondUpdated += (value) => textRenderer.DrawText("FPS: " + value.ToString("###.00"), 15, 15);

            }

            private void CreateNoiseGeneratedWorld()
            {
                float xScale = 1.5f;
                float yScale = 1.5f;

                NoiseGen noiseGen = new NoiseGen(xScale, yScale, 6);

                for (int x = -20; x < 20; x++)
                {
                    for (int z = -20; z < 20; z++)
                    {
                        for (int y = -15; y < -3; y++)
                        {
                            float generatedNoise = noiseGen.GetNoise(x, y, z);
                            //Console.WriteLine(string.Format("Generated noise at <{0}, {1}, {2}>: {3}", x, y, z, generatedNoise));
                            if (generatedNoise > .6f)
                            {
                                GameObject.CreateStaticBox(1.5f, 1.5f, 1.5f).Transform.Position = new Vector3(x, y, z) * 1.5f;
                            }
                        }
                    }
                }
            }

            private static void CreateStandardBoxArena()
            {
                var box = GameObject.CreateBox(3.0f, 3.0f, 3.0f, 6f);
                box.Transform.Position = new Vector3(0, 5, 15);

                var box2 = GameObject.CreateBox(3.0f, 1.0f, 3.0f, 4.0f);
                box2.Transform.Position = new Vector3(1.5f, 10, 15);

                var box3 = GameObject.CreateBox(18.0f, 0.33f, 0.5f, 2.0f);
                box3.Transform.Position = new Vector3(0, 18, 15.5f);

                box = GameObject.CreateBox(2.0f, .15f, 2.0f, 0.666f);
                box.Transform.Position = new Vector3(-6.5f, 19, 15.5f);

                box = GameObject.CreateBox(0.5f, 0.5f, 0.5f, 0.1f);
                box.Transform.Position = new Vector3(6.5f, 32, 15.5f);

                box = GameObject.CreateBox(1.0f, 1.0f, 1.0f);
                box.Transform.Position = new Vector3(4, 5, -10);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = new Vector3(0, 2, 15);

                box = GameObject.CreateBox(1.0f, 1.0f, 1.0f);
                box.Transform.Position = new Vector3(4.9f, 5, 30);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = new Vector3(0, 2, -15);

                var floor = GameObject.CreateStaticBox(50.0f, 1.0f, 50.0f);
                floor.Transform.Position = new Vector3(0, 0f, 0);

                var wall1 = GameObject.CreateStaticBox(50.0f, 11.0f, 0.5f);
                wall1.Transform.Position = new Vector3(0, 5.5f, 25f);

                var wall2 = GameObject.CreateStaticBox(50.0f, 11.0f, 0.5f);
                wall2.Transform.Position = new Vector3(0, 5.5f, -25f);

                var wall3 = GameObject.CreateStaticBox(0.5f, 11.0f, 50f);
                wall3.Transform.Position = new Vector3(25f, 5.5f, 0f);

                var wall4 = GameObject.CreateStaticBox(0.5f, 11.0f, 50f);
                wall4.Transform.Position = new Vector3(-25f, 5.5f, 0f);
            }
        }
    }
}
