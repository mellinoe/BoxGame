using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics
{
    public interface ILightInfo
    {
        LightKind Kind { get; }

        Vector3 Direction { get; }

        Color4f Color { get; }
    }
}
