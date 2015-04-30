using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace EngineCore.Entities
{
    public class EntityUpdateSystem : GameSystem
    {
        ImmutableArray<IUpdateableEntity> entities = ImmutableArray<IUpdateableEntity>.Empty;
        public ImmutableArray<IUpdateableEntity> Entities
        {
            get { return entities; }
        }

        public void AddEntity(IUpdateableEntity entity)
        {
            this.entities = this.entities.Add(entity);
        }

        public void RemoveEntity(IUpdateableEntity entity)
        {
            this.entities = this.entities.Remove(entity);
        }

        public EntityUpdateSystem(Game game) : base(game) { }

        public override void Update()
        {
            foreach (IUpdateableEntity entity in entities)
            {
                entity.Update();
            }
        }

        public override void Start() { }
        public override void Stop() { }
    }
}
