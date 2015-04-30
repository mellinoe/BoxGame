using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore
{
    public abstract class GameSystem
    {
        public Game Game { get; set; }
        public GameSystem(Game game)
        {
            this.Game = game;
        }
        public abstract void Start();
        public abstract void Stop();
        public abstract void Update();
    }
}
