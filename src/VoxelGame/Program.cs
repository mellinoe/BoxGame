using EngineCore;
using EngineCore.Graphics;
using GameApplication.Behaviours;
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
                //var character = new GameObject();
                //character.AddComponent<CharacterController>().BepuController.BodyRadius = .3f;
                //character.AddComponent<SimpleFpsController>();
                //character.Transform.Position = new Vector3(8f, 250f, 8f);

                var camera = new GameObject();
                camera.AddComponent<Camera>();
                camera.AddComponent<BoxLauncher>();
                camera.AddComponent<BoxArenaGame.Behaviours.FullScreenToggle>();
                camera.AddComponent<FreeFlyMovement>();

                //var fpsLookController = camera.AddComponent(new FpsLookController(character.Transform));
            }
        }
    }
}
