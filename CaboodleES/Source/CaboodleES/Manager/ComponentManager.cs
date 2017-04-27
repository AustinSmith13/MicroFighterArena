using global::System;
using global::System.Collections.Generic;
using CaboodleES.Interface;


namespace CaboodleES.Manager
{
    /// <summary>
    /// Manages component collections by type.
    /// </summary>
    public sealed class ComponentManager : CManager
    {
        public event Action<int, ComponentInfo> OnRemoved;
        public event Action<int, ComponentInfo> OnAdded;

        private List<IComponentCollection> componentCollections;
        private Dictionary<global::System.Type, IComponentCollection> componentCollectionCache;
        private int _count = 0;

        public ComponentManager(Caboodle caboodle) : base(caboodle)
        {
            componentCollections = new List<IComponentCollection>();
            componentCollectionCache = new Dictionary<global::System.Type, IComponentCollection>();
        }

        public void RegisterComponent<C>() where C : Component, new()
        {
            IComponentCollection collection = null;
            componentCollectionCache.TryGetValue(typeof(C), out collection);
            if (collection == null)
            {
                // Assigns the component collection a filter identifier
                collection = new ComponentCollection<C>(caboodle, _count++);
               // var c = global::System.Activator.CreateInstance(typeof(C), null);
                componentCollections.Add(collection);
                componentCollectionCache.Add(typeof(C), collection);
  
            }
        }

        /// <summary>
        /// Adds/Creates a component associated with the given entity id.
        /// </summary>
        public C AddComponent<C>(int eid)
            where C : Component, new()
        {
            IComponentCollection collection = null;
            componentCollectionCache.TryGetValue(typeof(C), out collection);

            if (collection == null)
            {
                // Assigns the component collection a filter identifier
                collection = new ComponentCollection<C>(caboodle, _count++);
                componentCollections.Add(collection);
                componentCollectionCache.Add(typeof(C), collection);
            }

            var c = collection.Add(eid) as C;

            ComponentInfo info;
            info.id = collection.GetId();
            info.component = c;

            OnAdded?.Invoke(eid, info);
            
            return c;
        }

        /// <summary>
        /// Retrieves the component associated with the given entity id.
        /// </summary>
        public Component[] GetComponents(int eid)
        {
            List<Component> components = new List<Component>();
            for(int i = 0; i < componentCollections.Count; i++)
            {
                if (componentCollections[i].Has(eid))
                    components.Add(componentCollections[i].Get(eid));
            }
            return components.ToArray();
        }

        /// <summary>
        /// Gets the component associated with
        /// </summary>
        public C GetComponent<C>(int eid)
            where C : Component
        {
            return Get(typeof(C)).Get(eid) as C;
        }

        /// <summary>
        /// Removes the component
        /// </summary>
        public C RemoveComponent<C>(int eid)
            where C : Component
        {
            var collection = Get(typeof(C));
            var c = collection.Remove(eid) as C;

            ComponentInfo info;
            info.id = collection.GetId();
            info.component = c;

            OnRemoved?.Invoke(eid, info);

            return c;
        }

        /// <summary>
        /// Removes all of the entity id's components.
        /// </summary>
        /// <param name="eid"></param>
        public void RemoveComponents(int eid)
        {
            for (int i = 0; i < componentCollections.Count; i++)
            {
                componentCollections[i].Remove(eid);
            }
        }

        /// <summary>
        /// Checks if the entity id has the component C.
        /// </summary>
        public bool HasComponent<C>(int eid)
            where C : Component
        {
            IComponentCollection collection = null;
            componentCollectionCache.TryGetValue(typeof(C), out collection);

            if (collection == null)
                return false;

            return collection.Has(eid);
        }

        public IComponentCollection Get(global::System.Type c)
        {
            IComponentCollection cm = null;
            componentCollectionCache.TryGetValue(c, out cm);
            if (cm == null)
                throw new NoSuchComponentException("No such component, " + c.Name, c.Name);

            return cm;
        }

        /// <summary>
        /// Clears all components in the caboodle. Remvoes all components from each and every entity.
        /// </summary>
        public void Clear()
        {
            foreach (var manager in componentCollections)
            {
                manager.Clear();
            }
            componentCollectionCache.Clear();
            componentCollections.Clear();
        }
    }
}
