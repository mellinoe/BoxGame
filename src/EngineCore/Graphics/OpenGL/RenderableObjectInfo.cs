﻿using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics.OpenGL
{
    public interface IRenderableObjectInfo
    {
        void Render(ref Matrix4x4 lookatMatrix, GraphicsContext graphicsContext);
    }
}
