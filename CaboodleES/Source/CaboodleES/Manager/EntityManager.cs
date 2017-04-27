using System.Collections.Generic;
using CaboodleES.Utils;


namespace CaboodleES.Manager
{
    /// <summary>
    /// Manages all entities.
    /// </summary>
    public sealed class EntityManager : CManager
    {
        public event global::System.Action<Entity> OnAdded;
        public event global::System.Action<Entity> OnRemoved;
        public event global::System.Action<int> OnChange;

        #region Properties

        /// <summary>
        /// The number of entities in the collection.
        /// </summary>
        public int Count
        {
            get { return entities.Count; }
        }

        /// <summary>
        /// The component manager.
        /// </summary>
        public ComponentManager Components
        {
            get { return _components; }
        }

        #endregion

        private ComponentManager _components;
        private Table<Entity> entities;

        public EntityManager(Caboodle world) : base(world)
        {
            this._components = new ComponentManager(world);
            this.entities = new Table<Entity>();

            _components.OnAdded += OnEntityAddedComponent;
            _components.OnRemoved += OnEntityRemovedComponent;
        }

        public void OnEntityAddedComponent(int eid, ComponentInfo info)
        {
            Get(eid).mask.Set(info.id, true);
            OnChange?.Invoke(eid);
        }

        public void OnEntityRemovedComponent(int eid, ComponentInfo info)
        {
            Get(eid).mask.Set(info.id, false);
            OnChange?.Invoke(eid);
        }

        /// <summary>
        /// Creates an entity.
        /// </summary>
        /// <returns></returns>
        public Entity Create()
        {
            var entity = caboodle.Pool.CreateEntity();

            entities.Set(entity.Id, entity);
            OnAdded?.Invoke(entity);
            return entity;
        }

        /// <summary>
        /// Gets the entity associated with the eid.
        /// </summary>
        public Entity Get(int eid)
        {
            Entity ent = entities.Get(eid);

            if (ent == null)
                throw new NoSuchEntityException("No such entity, " + eid, eid);

            return ent;
        }

        /// <summary>
        /// Returns true if the entity id exists.
        /// </summary>
        public bool Has(int eid)
        {
            return entities.Get(eid) != null;
        }

        /// <summary>
        /// Returns true if the entity exists.
        /// </summary>
        public bool Has(Entity entity)
        {
            return entities.Get(entity.Id) != null;
        }

        /// <summary>
        /// Removes an entity and its components.
        /// Pools unused entities.
        /// </summary>
        public void Remove(int eid)
        {
            Entity entity = caboodle.Pool.ReleaseEntity(entities.Get(eid));
            OnRemoved?.Invoke(entity);
            _components.RemoveComponents(eid);
            entities.Remove(eid); // Remove the entity after the events finnish
        }

        /// <summary>
        /// Removes an entity and its components without invoking an event. 
        /// [Note: this a temporary work around]
        /// </summary>
        /// <param name="eid"></param>
        internal void SilentRemove(int eid)
        {
            Entity entity = caboodle.Pool.ReleaseEntity(entities.Get(eid));
            _components.RemoveComponents(eid);
            entities.Remove(eid); // Remove the entity after the events finnish
        }

        /// <summary>
        /// Removes an entity and its components.
        /// </summary>
        public void Remove(ref Entity entity)
        {
            this.Remove(entity.Id);
            entity = null;
        }

        /// <summary>
        /// Removes all entities and there associated componenets.
        /// </summary>
        public void Clear()
        {
            entities.Clear();
            _components.Clear();
        }
    }
}