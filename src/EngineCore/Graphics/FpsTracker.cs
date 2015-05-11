using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Matrix4x4 = System.Numerics.Matrix4x4;
using EngineCore.Components;
using EngineCore.Graphics.OpenGL;

namespace EngineCore.Graphics
{
    public class FpsTracker : Behaviour
    {
        public Vector2 Position { get; set; }

        private int numFramesTracked = 50;
        private LinkedList<double> frameTimes;
        private double totalFrameTime;

        private Stopwatch stopwatch;

        private double _updateFrequency = 0f;
        private double _elapsed = 0f;

        public event Action<double> FramesPerSecondUpdated;

        public double UpdateFrequency
        {
            get { return _updateFrequency; }
            set { _updateFrequency = value; }
        }

        protected override void Initialize(Entities.EntityUpdateSystem system)
        {
            base.Initialize(system);
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
            this.frameTimes = new LinkedList<double>();
            for (int g = 0; g < numFramesTracked; g++)
            {
                frameTimes.AddLast(this.stopwatch.ElapsedMilliseconds / 1000.0);
            }
        }

        public double FramesPerSecond
        {
            get
            {
                return 1 / (totalFrameTime / numFramesTracked);
            }
        }

        protected override void Update()
        {
            UpdateFrameCount();
        }

        private void UpdateFrameCount()
        {
            var first = frameTimes.First.Value;
            var second = frameTimes.First.Next.Value;
            var oldDiff = (second - first);

            var last = frameTimes.Last.Value;
            var now = this.stopwatch.ElapsedMilliseconds / 1000.0;
            var newDiff = (now - last);

            var firstNode = frameTimes.First;
            frameTimes.RemoveFirst();
            firstNode.Value = now;
            frameTimes.AddLast(firstNode);
            totalFrameTime += (newDiff - oldDiff);

            _elapsed += (float)newDiff;
            if (_elapsed >= _updateFrequency)
            {
                RaiseFpsUpdated();
                _elapsed -= _updateFrequency;
            }
        }

        private void RaiseFpsUpdated()
        {
            if (FramesPerSecondUpdated != null)
            {
                FramesPerSecondUpdated(FramesPerSecond);
            }
        }
    }
}