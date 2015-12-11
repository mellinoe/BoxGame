using EngineCore;
using EngineCore.Components;
using EngineCore.Input;
using EngineCore.Physics;
using EngineCore.Services;
using System.Numerics;

namespace GameApplication.Behaviours
{
    public class BoxLauncher : Behaviour
    {
        [AutoInject]
        public IInputService InputService { get; set; }

        private GameObject _heldBox;
        private Tracker _tracker;
        private bool _launchingTonsOfBoxes;

        protected override void Update()
        {
            if (InputService.GetKey(KeyCode.F))
            {
                FireBoxForward();
            }
            if (InputService.GetKeyDown(KeyCode.Y))
            {
                StartStopHoldingBoxPressed();
            }
            if (InputService.GetKeyDown(KeyCode.KeypadPlus))
            {
                PlusButtonPressed();
            }
            if (InputService.GetKeyDown(KeyCode.KeypadMinus))
            {
                MinusButtonPressed();
            }
            if (InputService.GetKeyDown(KeyCode.F6))
            {
                ToggleLaunchAmount();
            }
        }

        private void ToggleLaunchAmount()
        {
            _launchingTonsOfBoxes = !_launchingTonsOfBoxes;
        }

        private void MinusButtonPressed()
        {
            if (_heldBox != null)
            {
                _heldBox.Transform.Scale -= new Vector3(.2f, 0, .2f);
                FixHoldOffset();
            }
        }

        private void PlusButtonPressed()
        {
            if (_heldBox != null)
            {
                _heldBox.Transform.Scale += new Vector3(.2f, 0, .2f);
                FixHoldOffset();
            }
        }

        private void FixHoldOffset()
        {
            _tracker.Offset = new Vector3(0, 0, _heldBox.Transform.Scale.Z + 2.0f);
        }

        private void StartStopHoldingBoxPressed()
        {
            if (_heldBox != null)
            {
                StopHoldingBox();
            }
            else
            {
                StartHoldingBox();
            }
        }

        private void StopHoldingBox()
        {
            _heldBox.RemoveComponent<Tracker>();
            _heldBox.GetComponent<BoxCollider>().PhysicsEntity.Mass = 10.0f;
            _heldBox = null;
        }

        private void StartHoldingBox()
        {
            _heldBox = GameObject.CreateStaticBox(2.0f, 2.0f, 2.0f);
            _tracker = _heldBox.AddComponent<Tracker>();
            _tracker.TrackedObject = Transform;
            _tracker.Offset = new Vector3(0, 0, 3.0f);
        }

        private void FireBoxForward()
        {
            GameObject box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
            box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (Transform.Forward * 25.0f);
            box.Transform.Position = Transform.Position + Transform.Forward * 1.5f;

            if (_launchingTonsOfBoxes)
            {
                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (Transform.Forward * 25.0f);
                box.Transform.Position = Transform.Position + Transform.Forward * 1.5f + Transform.Right * .5f;

                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (Transform.Forward * 25.0f);
                box.Transform.Position = Transform.Position + Transform.Forward * 1.5f - Transform.Right * .5f;

                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (Transform.Forward * 25.0f);
                box.Transform.Position = Transform.Position + Transform.Forward * 1.5f + Transform.Up * .5f;

                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (Transform.Forward * 25.0f);
                box.Transform.Position = Transform.Position + Transform.Forward * 1.5f - Transform.Up * .5f;
            }
        }
    }

    public class Tracker : Behaviour
    {
        public Transform TrackedObject { get; set; }
        public Vector3 Offset { get; set; }

        protected override void Update()
        {
            Transform.Position = TrackedObject.Position + Vector3.Transform(Offset, TrackedObject.Rotation);
        }
    }

}
