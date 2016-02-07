using EngineCore;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace BoxArenaGame.Behaviours
{
    public class RotatorDisplay : Behaviour
    {
        protected override void Update()
        {
            //ImGuiNative.igDragFloat3("Rotation", new Vector3(), 1, -360, 360, "%f", 1);
        }
    }
}
