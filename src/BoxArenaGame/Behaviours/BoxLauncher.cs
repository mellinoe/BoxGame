using EngineCore;
using EngineCore.Components;
using EngineCore.Input;
using EngineCore.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameApplication.Behaviours
{
    public class BoxLauncher : Behaviour<InputSystem>
    {
        private GameObject _heldBox;
        private Tracker _tracker;
        private InputSystem _input;
        private bool _launchingTonsOfBoxes;

        protected override void Update()
        {
            if (_input.GetKey(KeyCode.F))
            {
                FireBoxForward();
            }
            if (_input.GetKeyDown(KeyCode.Y))
            {
                StartStopHoldingBoxPressed();
            }
            if (_input.GetKeyDown(KeyCode.KeypadPlus))
            {
                PlusButtonPressed();
            }
            if (_input.GetKeyDown(KeyCode.KeypadMinus))
            {
                MinusButtonPressed();
            }
            if (_input.GetKeyDown(KeyCode.F6))
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
            this._heldBox.RemoveComponent<Tracker>();
            this._heldBox.GetComponent<BoxCollider>().PhysicsEntity.Mass = 10.0f;
            this._heldBox = null;
        }

        private void StartHoldingBox()
        {
            this._heldBox = GameObject.CreateStaticBox(2.0f, 2.0f, 2.0f);
            _tracker = this._heldBox.AddComponent<Tracker>();
            _tracker.TrackedObject = this.Transform;
            _tracker.Offset = new Vector3(0, 0, 3.0f);
        }

        private void FireBoxForward()
        {
            GameObject box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
            box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (this.Transform.Forward * 25.0f);
            box.Transform.Position = this.Transform.Position + this.Transform.Forward * 1.5f;

            if (_launchingTonsOfBoxes)
            {
                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (this.Transform.Forward * 25.0f);
                box.Transform.Position = this.Transform.Position + this.Transform.Forward * 1.5f + this.Transform.Right * .5f;

                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (this.Transform.Forward * 25.0f);
                box.Transform.Position = this.Transform.Position + this.Transform.Forward * 1.5f - this.Transform.Right * .5f;

                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (this.Transform.Forward * 25.0f);
                box.Transform.Position = this.Transform.Position + this.Transform.Forward * 1.5f + this.Transform.Up * .5f;

                box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (this.Transform.Forward * 25.0f);
                box.Transform.Position = this.Transform.Position + this.Transform.Forward * 1.5f - this.Transform.Up * .5f;
            }
        }

        protected override void Initialize(InputSystem system)
        {
            _input = system;
        }

        protected override void Uninitialize(InputSystem system)
        {
            _input = null;
        }
    }

    public class Tracker : Behaviour
    {
        public Transform TrackedObject { get; set; }
        public Vector3 Offset { get; set; }

        protected override void Update()
        {
            this.Transform.Position = TrackedObject.Position + Vector3.Transform(Offset, TrackedObject.Rotation);
        }
    }

}
