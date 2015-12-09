using System;
using System.Diagnostics;
using System.Numerics;

namespace EngineCore.Graphics
{
    public class FpsTracker : Behaviour
    {
        public Vector2 Position { get; set; }

        private int numFramesTracked = 0;
        private int maxFrameHistory = 50;
        private long[] samples;
        private int sampleCursor = 0;
        private double totalFrameTime;

        private Stopwatch _stopwatch;

        private double _updateFrequency = 0f;
        private double _elapsed = 0f;
        private long lastSampleMs;

        public event Action<double> FramesPerSecondUpdated;

        /// <summary>
        /// The update frequency, in seconds.
        /// </summary>
        public double UpdateFrequency
        {
            get { return _updateFrequency; }
            set { _updateFrequency = value; }
        }

        protected override void Start()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            samples = new long[maxFrameHistory];

            lastSampleMs = _stopwatch.ElapsedMilliseconds;
            AddSample(_stopwatch.ElapsedMilliseconds);
        }

        private void AddSample(long ms)
        {
            long diff = ms - lastSampleMs;
            long previousValue = samples[sampleCursor];
            samples[sampleCursor] = diff;
            lastSampleMs = ms;

            long cursorPosDiff = diff - previousValue;

            totalFrameTime += cursorPosDiff;

            sampleCursor = (sampleCursor + 1) % maxFrameHistory;
            numFramesTracked = Math.Min(maxFrameHistory, numFramesTracked + 1);
        }

        public double FramesPerSecond
        {
            get
            {
                return 1000.0 / (totalFrameTime / numFramesTracked);
            }
        }

        protected override void Update()
        {
            UpdateFrameCount();
        }

        private void UpdateFrameCount()
        {
            var now = _stopwatch.ElapsedMilliseconds;
            var diff = now - lastSampleMs;
            AddSample(now);

            _elapsed += (float)diff;
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