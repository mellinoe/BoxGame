using EngineCore.Components;
using EngineCore.Entities;
using System;
using System.Collections.Generic;

namespace EngineCore.Systems
{
    public abstract class GameSystem2
    {
        protected internal ComponentRegistry Registry { get; internal set; }

        internal void Update()
        {
            var entities = Registry.GetEntitiesWithComponents(Mask);
            PerFrameUpdate(entities);
            ProcessEntities(entities);
        }

        internal abstract ComponentMask Mask { get; }

        protected abstract void PerFrameUpdate(List<EntityID> entities);

        public abstract void ProcessEntities(List<EntityID> entities);
    }

    public abstract class GameSystem2<TComponent1, TComponent2> : GameSystem2
    {
        public override void ProcessEntities(List<EntityID> entities)
        {
            var view1 = Registry.GetIndexedComponentView<TComponent1>(entities);
            var view2 = Registry.GetIndexedComponentView<TComponent2>(entities);
            ProcessEntities(entities, view1, view2);
        }

        public virtual void ProcessEntities(
            List<EntityID> entities,
            IndexedComponentView<TComponent1> view1,
            IndexedComponentView<TComponent2> view2)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                ProcessEntity(entities[i], ref view1.Components[view1.Indices[i]], ref view2.Components[view2.Indices[i]]);
            }
        }

        internal override ComponentMask Mask
        {
            get
            {
                return ComponentRegistry.GetComponentMask(typeof(TComponent1)) | ComponentRegistry.GetComponentMask(typeof(TComponent2));
            }
        }

        protected abstract void ProcessEntity(EntityID entityID, ref TComponent1 component1, ref TComponent2 component2);
    }
}
