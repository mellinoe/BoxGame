using EngineCore.Components;
using System.Numerics;
namespace EngineCore.Graphics
{
    // ** LEGACY IMPL **
    //public interface IRenderable
    //{
    //    void Render(SimpleRenderer renderer);
    //    Matrix4x4 WorldMatrix { get; }
    //}

    public interface IRenderable
    {
        Matrix4x4 WorldMatrix { get; }

        Transform Transform { get; }
    }
}
