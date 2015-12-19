using BEPUphysics.CollisionShapes.ConvexShapes;
using BoxArenaGame.Behaviours;
using EngineCore;
using EngineCore.Graphics;
using EngineCore.Graphics.Formats;
using EngineCore.Physics;
using GameApplication.Behaviours;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace GameApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            try
            {
                Game game = new BoxGame();
                game.Start();
            }
            catch (Exception e) when (!Debugger.IsAttached)
            {
                Console.WriteLine("Unexpected exception encountered:" + Environment.NewLine + e);
                Console.WriteLine("Enter D to attach debugger.");

                int input = Console.Read();
                if (input == 100 || input == 68)
                {
                    Debugger.Launch();
                }
            }
        }

        public class BoxGame : Game
        {
            private static readonly string s_windowTitle = "Hold F to Fire Boxes, Y to Place, +/- to Resize";

            protected override void PerformCustomInitialization()
            {
                CreateStandardBoxArena();
            }

            protected override void PostSystemsStart()
            {
                GraphicsSystem.WindowInfo.Title = s_windowTitle;
                // Fix when the text renderer isn't OpenGL-specific
                if (GraphicsSystem is EngineCore.Graphics.OpenGL.OpenGLGraphicsSystem)
                {
                    CreateFpsTracker();
                }
            }

            private void CreateFpsTracker()
            {
                GameObject fpsTrackerObj = new GameObject();
                FpsTracker fpsTracker = new FpsTracker();
                fpsTracker.UpdateFrequency = 100;
                fpsTrackerObj.AddComponent(fpsTracker);
#if FEATURE_TEXT_RENDERING
                EngineCore.Graphics.OpenGL.TextRenderer textRenderer = new EngineCore.Graphics.OpenGL.TextRenderer();
                fpsTrackerObj.AddComponent(textRenderer);                
                fpsTracker.FramesPerSecondUpdated += (value) => textRenderer.DrawText("FPS: " + value.ToString("###.00"), 15, 15);
#else
                fpsTracker.FramesPerSecondUpdated += (value) => GraphicsSystem.WindowInfo.Title = $"{s_windowTitle} {value.ToString("###.00")} FPS";
#endif

            }

            private static void AddArenaStartingStuff()
            {
                var character = new GameObject();
                character.AddComponent<CharacterController>().BepuController.BodyRadius = .3f;
                character.AddComponent<SimpleFpsController>();
                character.Transform.Position = new Vector3(0, 10f, 0f);

                var camera = new GameObject();
                camera.AddComponent<Camera>();
                camera.AddComponent<BoxLauncher>();
                var fpsLookController = camera.AddComponent(new FpsLookController(character.Transform));

                new GameObject().AddComponent<FullScreenToggle>();
                new GameObject().AddComponent<GravityModifier>();

                var sphere = new GameObject();
                Texture2D stoneTex = Texture2D.CreateFromFile(Path.Combine("Textures", "StoneTile.png"));
                sphere.AddComponent(new MeshRenderer(Primitives.Sphere, stoneTex));
                sphere.AddComponent<SphereCollider>();
                sphere.Transform.Position = new Vector3(0, 5, 10);
                sphere.Transform.Scale = new Vector3(1);

                //Vector3 center;
                //var convexShape = new ConvexHullShape(Primitives.Teapot.Vertices.Select(sv => sv.Position).ToArray(), out center);
                //sphere.AddComponent(new ConvexHullCollider(Primitives.Teapot));
                //sphere.GetComponent<MeshRenderer>().SetRenderOffset(-center);
                //sphere.GetComponent<ConvexHullCollider>().Mass = 50.0f;
            }

            private void CreateSimpleWorld()
            {
                Random rand = new Random();

                for (int x = -20; x < 20; x++)
                {
                    for (int z = -20; z < 20; z++)
                    {
                        GameObject.CreateStaticBox(1.5f, 1.5f, 1.5f).Transform.Position = new Vector3(x, x + z, z) * 1.5f;
                    }
                }
            }

            private static void CreateStandardBoxArena()
            {
                AddArenaStartingStuff();

                var light = new GameObject();
                LightComponent lightComponent = light.AddComponent(new LightComponent(LightKind.Directional, new Vector3(.5f, -1f, .5f), Color4f.White));

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
