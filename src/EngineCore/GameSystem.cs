namespace EngineCore
{
    public abstract class GameSystem
    {
        public Game Game { get; set; }

        public GameSystem(Game game)
        {
            Game = game;
        }

        public abstract void Start();

        public abstract void Stop();

        public abstract void Update();
    }
}
