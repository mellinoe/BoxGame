using EngineCore.Components;
using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore
{
    public abstract class Behaviour : Component, IUpdateableEntity
    {
        void IUpdateableEntity.Update()
        {
            Update();
        }

        protected abstract void Update();
    }
}
