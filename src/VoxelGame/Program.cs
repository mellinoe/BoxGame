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
                AddNoiseWorldStartingStuff();
                CreateNoiseGeneratedWorld();
            }

            private void CreateNoiseGeneratedWorld()
            {
                VoxelWorldSystem voxelWorldSystem = new VoxelWorldSystem(this);
                AddGameSystem(voxelWorldSystem);
            }

            private static void AddNoiseWorldStartingStuff()
            {
                var character = new GameObject();
                character.AddComponent<CharacterController>().BepuController.BodyRadius = .3f;
                character.AddComponent<SimpleFpsController>();
                character.Transform.Position = new Vector3(-20, 250f, -20f);
                character.GetComponent<CharacterController>().BepuController.Body.Position = new Vector3(50f, 250f, 50f);

                var camera = new GameObject();
                camera.AddComponent<Camera>();
                camera.AddComponent<BoxLauncher>();
                var fpsLookController = camera.AddComponent(new FpsLookController(character.Transform));
                camera.AddComponent<BoxArenaGame.Behaviours.FullScreenToggle>();
            }
        }
    }
}
