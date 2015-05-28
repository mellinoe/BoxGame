using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics.DirectX
{
    public abstract class RenderableObjectBase
    {
        public SimpleRenderer SimpleRenderer { get; private set; }

        public RenderableObjectBase(SimpleRenderer simpleRenderer)
        {
            SimpleRenderer = simpleRenderer;
        }
    }
}
