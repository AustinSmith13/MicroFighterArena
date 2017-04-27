using global::System.Collections.Generic;
using global::System;
using CaboodleES.Interface;
using CaboodleES.Utils;


namespace CaboodleES.Manager
{
    /// <summary>
    /// Keeps track of a list of components of type T.
    /// </summary>
    public sealed class ComponentCollection<T> : IComponentCollection
        where T : Component, new()
    {
        public int Id
        {
            get { return id; }
        }

      
        //private Table<T> components; // Was giving issues...
        private Dictionary<int, T> components;
       
        //private Bag<T> components;
        private Caboodle caboodle;
        private int id;

        public IEnumerable<T> Components
        {
            get { return null; }
        }

        public ComponentCollection(Caboodle world, int id)
        {
            this.caboodle = world;
            this.id = id;
            this.components = new Dictionary<int, T>();
            //this.components = new Table<T>();
        }

        public int GetId()
        {
            return id;
        }

        /// <summary>
        /// Adds a component to an entity. If the component already exists a new one will be created.
        /// </summary>
        public Component Add(int eid)
        {
            if (this.Has(eid)) return this.Get(eid);
            var c = caboodle.Pool.CreateComponent<T>();

            components.Add(eid, c);
            
            return c;
        }

        /// <summary>
        /// Retrives a component associated with the given Entity Id.
        /// </summary>
        public Component Get(int eid)
        {
            return components[eid];
        }

        public bool Has(int eid)
        {
            T c = null;
            return components.TryGetValue(eid, out c);
        }

        /// <summary>
        /// Removes a component associated with the Entity Id.
        /// </summary>
        public Component Remove(int eid)
        {
            T removed = null;
            components.TryGetValue(eid, out removed);
            components.Remove(eid);
            if (removed == null)
                return null;
            return caboodle.Pool.ReleaseComponent(removed);
        }

        /// <summary>
        /// Removes all components from the collection.
        /// </summary>
        public void Clear()
        {
            components.Clear();
        }

        /// <summary>
        /// Component Type manager manages. [DEPRECATED]
        /// </summary>
        /// <returns>The type of component the manager is interested in</returns>
        [Obsolete("This method is deprecated, Please use its type instead.")]
        public global::System.Type GetCType()
        {
            return typeof(T);
        }
    }
}
