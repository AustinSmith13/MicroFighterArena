

namespace CaboodleES
{
    /// <summary>
    /// Entity handle.
    /// </summary>
    public sealed class Entity
    {
        public int Id { get { return _id; } }
        public bool Active { get { return _active; } set { _active = value; } }
        public Caboodle World { get { return _world; } }

        private readonly int _id;
        private readonly Caboodle _world;
        private bool _active;
        internal readonly Utils.BitMask mask;


        internal Entity(Caboodle entityWorld, int id)
        {
            this._world = entityWorld;
            this._id = id;
            this._active = true;
            this.mask = new Utils.BitMask();
        }

        /// <summary>
        /// Retrieves all components.
        /// </summary>
        public Component[] GetComponents()
        {
            return _world.Entities.Components.GetComponents(_id);
        }

        /// <summary>
        /// Gets the component.
        /// </summary>
        public C GetComponent<C>() where C : Component
        {
            return _world.Entities.Components.GetComponent<C>(_id);
        }

        /// <summary>
        /// Adds the component.
        /// </summary>
        public C AddComponent<C>() where C : Component, new()
        {
            return _world.Entities.Components.AddComponent<C>(_id);
        }

        /// <summary>
        /// Removes the component.
        /// </summary>
        public C RemoveComponent<C>() where C : Component
        {
            _world.Systems.ScheduleRemoveComponent<C>(_id);
            return GetComponent<C>();
            //return _world.Entities.Components.RemoveComponent<C>(_id);
        }

        /// <summary>
        /// Checks if the entity has a component of type {C}.
        /// </summary>
        public bool HasComponent<C>() where C : Component
        {
            return _world.Entities.Components.HasComponent<C>(_id);
        }

        /// <summary>
        /// Deletes all components associated with the entity and expires the id.
        /// </summary>
        public void Destroy()
        {
            _world.Entities.Remove(_id);
        }

        public override string ToString()
        {
            return string.Format("Entity({0})", this._id);
        }
    }
}
