using EngineCore.Components;
using EngineCore.Systems;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EngineCore
{
    public class EcsGame
    {
        private double _desiredFrameLength = 1.0 / 60.0;
        private bool _running = false;
        private List<GameSystem2> _gameSystems = new List<GameSystem2>();
        private ComponentRegistry _registry = new ComponentRegistry();
        private DateTime previousFrameStartTime;

        public List<GameSystem2> Systems => _gameSystems;

        public void AddSystem(GameSystem2 system)
        {
            system.Registry = _registry;
            _gameSystems.Add(system);
        }

        private void RunMainLoop()
        {
            while (_running)
            {
                RunSingleFrame();
            }
        }

        private void RunSingleFrame()
        {
            DateTime beforeFrameTime = DateTime.UtcNow;
            float elapsedSinceLastFrame = (float)(beforeFrameTime - previousFrameStartTime).TotalSeconds;
            Time.SetDeltaTime(elapsedSinceLastFrame);
            previousFrameStartTime = beforeFrameTime;

            foreach (GameSystem2 system in _gameSystems)
            {
                system.Update();
            }

            DateTime afterFrameTime = DateTime.UtcNow;
            double elapsed = (afterFrameTime - beforeFrameTime).TotalSeconds;
            double sleepTime = _desiredFrameLength - elapsed;
            if (sleepTime > 0.0)
            {
#if USE_THREADYIELD || USE_SLEEP0
                DateTime finishTime = afterFrameTime + TimeSpan.FromSeconds(sleepTime);
                while (DateTime.UtcNow < finishTime)
                {
#if USE_THREADYIELD
                    Thread.Yield();
#elif USE_SLEEP0
                    Thread.Sleep(0);
#endif
                }
#else
                Thread.Sleep((int)(sleepTime * 1000));
#endif
            }
#if MONITOR_SLOWRUNNING
            else
            {
                Console.WriteLine("Running slowly, no sleep time.");
            }
#endif
        }

        internal void Exit()
        {
            _running = false;
        }
    }
}
