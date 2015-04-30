using EngineCore;
using EngineCore.Behaviours;
using EngineCore.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameApplication.Behaviours
{
    public class LightRotator : Behaviour
    {
        private DirectionalLight light;

        private Vector3 startingRotation;
        private Vector3 endingRotation;
        private float duration;
        private float elapsed;

        int direction = 1;

        public LightRotator(DirectionalLight light)
        {
            this.light = light;
            this.startingRotation = new Vector3(-1f, -.2f, 0f);
            this.endingRotation = new Vector3(1f, -.35f, 0f);
            this.duration = 4.0f;
            this.elapsed = 0.0f;
        }

        protected override void Update()
        {
            elapsed += Time.DeltaTime * direction;
            if (elapsed > duration)
            {
                direction = -1;
            }
            else if (elapsed <= 0)
            {
                direction = 1;
                elapsed = 0f;
            }
            var currentDirection = Vector3.Lerp(startingRotation, endingRotation, elapsed / duration);
            this.light.Direction = currentDirection;
        }
    }
}
