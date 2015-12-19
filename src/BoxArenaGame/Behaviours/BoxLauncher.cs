using EngineCore;
using EngineCore.Components;
using EngineCore.Input;
using EngineCore.Physics;
using EngineCore.Services;
using System.Numerics;
using ImGuiNET;

namespace GameApplication.Behaviours
{
    public class BoxLauncher : Behaviour
    {
        [AutoInject]
        public IInputService InputService { get; set; }

        private GameObject _heldBox;
        private Tracker _tracker;
        private bool _launchingTonsOfBoxes;

        private float _width = 1f;
        private float _height = 1f;
        private float _numPadChangeScale = (1f / 3f);

        private int _numBoxesLaunched = 0;
        private int _numBoxesPlaced;

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
            if (InputService.GetKeyDown(KeyCode.Keypad6))
            {
                if (_heldBox != null)
                {
                    ModifyWidth(+1);
                    SetBoxDimensions();
                    FixHoldOffset();
                }
            }
            if (InputService.GetKeyDown(KeyCode.Keypad4))
            {
                if (_heldBox != null)
                {
                    ModifyWidth(-1);
                    SetBoxDimensions();
                    FixHoldOffset();
                }
            }
            if (InputService.GetKeyDown(KeyCode.Keypad8))
            {
                if (_heldBox != null)
                {
                    ModifyHeight(+1);
                    SetBoxDimensions();
                    FixHoldOffset();
                }
            }
            if (InputService.GetKeyDown(KeyCode.Keypad2))
            {
                if (_heldBox != null)
                {
                    ModifyHeight(-1);
                    SetBoxDimensions();
                    FixHoldOffset();
                }
            }
            if (InputService.GetKeyDown(KeyCode.F6))
            {
                ToggleLaunchAmount();
            }

            ImGui.Text("Total boxes fired: " + _numBoxesLaunched);
            ImGui.Text("Total boxes placed: " + _numBoxesPlaced);

        }

        private void ModifyWidth(int direction)
        {
            _width += direction * _numPadChangeScale;
        }

        private void ModifyHeight(int direction)
        {
            _height += direction * _numPadChangeScale;

        }

        private void ToggleLaunchAmount()
        {
            _launchingTonsOfBoxes = !_launchingTonsOfBoxes;
        }

        private void SetBoxDimensions()
        {
            _heldBox.Transform.Scale = new Vector3(_width, _height, _width);
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
            _numBoxesPlaced++;
        }

        private void StartHoldingBox()
        {
            _heldBox = GameObject.CreateStaticBox(1.0f, 1.0f, 1.0f);
            _tracker = _heldBox.AddComponent<Tracker>();
            _tracker.TrackedObject = Transform;
            SetBoxDimensions();
            FixHoldOffset();
        }

        private void FireBoxForward()
        {
            GameObject box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
            box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (Transform.Forward * 25.0f);
            box.Transform.Position = Transform.Position + Transform.Forward * 1.5f;
            _numBoxesLaunched++;

            if (_launchingTonsOfBoxes)
            {
                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (Transform.Forward * 25.0f);
                box.Transform.Position = Transform.Position + Transform.Forward * 1.5f + Transform.Right * .5f;
                _numBoxesLaunched++;

                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (Transform.Forward * 25.0f);
                box.Transform.Position = Transform.Position + Transform.Forward * 1.5f - Transform.Right * .5f;
                _numBoxesLaunched++;

                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (Transform.Forward * 25.0f);
                box.Transform.Position = Transform.Position + Transform.Forward * 1.5f + Transform.Up * .5f;
                _numBoxesLaunched++;

                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (Transform.Forward * 25.0f);
                box.Transform.Position = Transform.Position + Transform.Forward * 1.5f - Transform.Up * .5f;
                _numBoxesLaunched++;
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
