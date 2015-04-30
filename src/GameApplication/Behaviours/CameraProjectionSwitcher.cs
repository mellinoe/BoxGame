using EngineCore;
using EngineCore.Behaviours;
using EngineCore.Components;
using EngineCore.Graphics;
using EngineCore.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApplication.Behaviours
{
    public class CameraProjectionSwitcher : Behaviour<SharpDxGraphicsSystem>
    {
        private System.Windows.Forms.Keys key = System.Windows.Forms.Keys.P;
        private SharpDxGraphicsSystem system;

        protected override void Initialize(SharpDxGraphicsSystem system)
        {
            this.system = system;
        }

        protected override void Update()
        {
            if (InputSystem.GetKeyDown(key))
            {
                System.Diagnostics.Debug.WriteLine("Detected key.");
                SwitchProjectionType();
            }
        }

        public void SwitchProjectionType()
        {
            if (system.Renderer.MainCamera.ProjectionType == ProjectionType.Orthographic)
            {
                system.Renderer.MainCamera.ProjectionType = ProjectionType.Perspective;
            }
            else
            {
                system.Renderer.MainCamera.ProjectionType = ProjectionType.Orthographic;
            }
        }

        protected override void Uninitialize(SharpDxGraphicsSystem system) { }
    }
}
