using System.Collections.Immutable;

namespace EngineCore.Entities
{
    public class EntityUpdateSystem : GameSystem
    {
        ImmutableArray<IUpdateableEntity> _entities = ImmutableArray<IUpdateableEntity>.Empty;

        public void AddEntity(IUpdateableEntity entity)
        {
            _entities = _entities.Add(entity);
        }

        public void RemoveEntity(IUpdateableEntity entity)
        {
            _entities = _entities.Remove(entity);
        }

        public EntityUpdateSystem(Game game) : base(game)
        {
            game.ComponentRegistry.AddComponentRegistration<IUpdateableEntity>(
                entity => AddEntity(entity),
                entity => RemoveEntity(entity));
        }

        public override void Update()
        {
            foreach (IUpdateableEntity entity in _entities)
            {
                entity.Update();
            }
        }

        public override void Start() { }
        public override void Stop() { }
    }
}
