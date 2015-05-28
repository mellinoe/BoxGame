using EngineCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics
{
    public class LightComponent : Component<GraphicsSystem>, ILightInfo
    {
        private LightKind _kind;
        private Vector3 _direction;
        private Color4f _color;

        public LightComponent(LightKind kind, Vector3 direction, Color4f color)
        {
            _kind = kind;
            _direction = direction;
            _color = color;
        }

        protected override void Initialize(GraphicsSystem system)
        {
            system.RegisterLight(this);
        }

        protected override void Uninitialize(GraphicsSystem system)
        {
            throw new NotImplementedException();
        }

        public LightKind Kind
        {
            get { return _kind; }
        }

        public System.Numerics.Vector3 Direction
        {
            get { return _direction; }
        }

        public Color4f Color
        {
            get { return _color; }
        }
    }
}
