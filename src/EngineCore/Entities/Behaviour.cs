using EngineCore.Components;
using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore
{
    public abstract class Behaviour : Component<EntityUpdateSystem>, IUpdateableEntity
    {
        protected override void Initialize(EntityUpdateSystem system)
        {
            system.AddEntity(this);
        }

        protected override void Uninitialize(EntityUpdateSystem system)
        {
            system.RemoveEntity(this);
        }

        void IUpdateableEntity.Update()
        {
            this.Update();
        }

        protected abstract void Update();

    }

    public abstract class Behaviour<TSystem> : Component<EntityUpdateSystem, TSystem>, IUpdateableEntity
        where TSystem : GameSystem
    {

        protected override void Initialize(EntityUpdateSystem system1, TSystem system2)
        {
            system1.AddEntity(this);
            this.Initialize(system2);
        }

        protected override void Uninitialize(EntityUpdateSystem system1, TSystem system2)
        {
            system1.RemoveEntity(this);
            this.Uninitialize(system2);
        }

        void IUpdateableEntity.Update()
        {
            this.Update();
        }

        protected abstract void Update();
        protected abstract void Initialize(TSystem system);
        protected abstract void Uninitialize(TSystem system);
    }

    public abstract class Behaviour<TSystem1, TSystem2> : Component<EntityUpdateSystem, TSystem1, TSystem2>, IUpdateableEntity
        where TSystem1 : GameSystem
        where TSystem2 : GameSystem
    {

        protected override void Initialize(EntityUpdateSystem updateSystem, TSystem1 system1, TSystem2 system2)
        {
            updateSystem.AddEntity(this);
            this.Initialize(system1, system2);
        }

        protected override void Uninitialize(EntityUpdateSystem updateSystem, TSystem1 system1, TSystem2 system2)
        {
            updateSystem.RemoveEntity(this);
            this.Uninitialize(system1, system2);
        }

        void IUpdateableEntity.Update()
        {
            this.Update();
        }

        protected abstract void Update();
        protected abstract void Initialize(TSystem1 system1, TSystem2 system2);
        protected abstract void Uninitialize(TSystem1 system1, TSystem2 system2);
    }
}
