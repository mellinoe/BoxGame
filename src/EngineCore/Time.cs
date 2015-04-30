using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore
{
    public static class Time
    {
        private static float _deltaTime;
        public static float DeltaTime
        {
            get
            {
                return _deltaTime;
            }
        }

        internal static void SetDeltaTime(float time)
        {
            _deltaTime = time;
        }
    }
}
