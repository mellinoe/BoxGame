using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EngineCore
{
    public class GameSystemCollection : List<GameSystem>
    {
        public T GetSystem<T>() where T : GameSystem
        {
            GameSystem system = this.SingleOrDefault(gs => gs.GetType() == typeof(T));
            if (system == null)
            {
                throw new InvalidOperationException("There is no GameSystem of type " + typeof(T).FullName + ".");
            }

            return (T)system;
        }

        public IEnumerable<GameSystem> GetSystemsByTypes(IEnumerable<Type> types)
        {
            return this.Where(gs => types.Any(t => t.GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo())));
        }
    }
}
