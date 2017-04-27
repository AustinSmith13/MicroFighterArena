using System;
using System.Collections.Generic;
using CaboodleES.System;
using System.Reflection;
using System.Linq;

namespace CaboodleES.Manager
{
    /// <summary>
    /// Manages systems.
    /// </summary>
    public sealed class SystemManager : CManager
    {
        private readonly List<SystemInfo> all_systems;
        private readonly List<Entity> entitiesToRemove;
        private readonly List<int> entitiesToChange;
        private readonly List<Action> scheduledActions;
        private readonly Utils.Table<Utils.Table<Entity>> entityCache;
        private int updatingSystemsCount = 0, fixedSystemsCount = 0;
        private SystemInfo[] update_systems;
        private SystemInfo[] fixed_systems;
        private int nextSystemId;


        public SystemManager(Caboodle caboodle) : base(caboodle)
        {
            this.caboodle.Entities.OnChange += ScheduleEntityChange;
            this.caboodle.Entities.OnRemoved += ScheduleEntityRemove;
            this.all_systems = new List<SystemInfo>();
            this.entityCache = new Utils.Table<Utils.Table<Entity>>();
            this.entitiesToRemove = new List<Entity>();
            this.entitiesToChange = new List<int>();
            this.scheduledActions = new List<Action>();
            nextSystemId = 0;
        }

        public void ScheduleEntityRemove(Entity entity)
        {
            entitiesToRemove.Add(entity);
        }

        public void ScheduleEntityChange(int eid)
        {
            entitiesToChange.Add(eid);
        }

        public void ScheduleRemoveComponent<C>(int eid) where C : Component
        {
            scheduledActions.Add(() =>
            {
                if(caboodle.Entities.Has(eid))
                    caboodle.Entities.Components.RemoveComponent<C>(eid);
            });
        }

        /// <summary>
        /// Removes the entity from every system.
        /// </summary>
        public void OnEntityRemoved(Entity entity)
        {
            for (int i = 0; i < all_systems.Count; i++)
            {
                if (entityCache.Has(i))
                {
                    if (entityCache.Get(i).Has(entity.Id))
                    {
                        all_systems[i].entities.Remove(entity.Id);
                        entityCache.Get(i).Remove(entity.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Re-catigorize the entity to keep system entity collections up to date and organized.
        /// </summary>
        public void OnEntityChange(int eid)
        {
            Entity entity = this.caboodle.Entities.Get(eid);

            for(int i = 0; i < all_systems.Count; i++)
            {
                var systemMask = all_systems[i].mask;

                // match entity mask and system mask
                if(AspectMatcher.Match(all_systems[i].processor.Aspect, entity.mask, systemMask) && entity.Active)
                {
                    // If the system does not have the entity then add it
                    if (!entityCache.Get(i).Has(entity.Id))
                    {
                        entityCache.Get(i).Set(entity.Id, entity);
                        all_systems[i].entities.Add(entity.Id, entity);
                    }
                }
                // The entity does not belong in its collection of entities
                else
                {
                    // Check if there is an entry
                    if(entityCache.Has(i))
                    {
                        // Remove the entity from the systems collection if it exists
                        if (entityCache.Get(i).Has(entity.Id))
                        {
                            all_systems[i].entities.Remove(entity.Id);
                            entityCache.Get(i).Remove(entity.Id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registers all systems in the assembly.
        /// </summary>
        public void Add(Assembly asm)
        {
            var types = GetTypesWithInterface<object>(asm);

            foreach (Type type in types)
            {
                if (type.BaseType != typeof(Processor)) continue;
                this.Add(type);
            }
        }

        /// <summary>
        /// Instantiates the system-type's class and adds it to the manager along with its attributes.
        /// </summary>
        public void Add<T>() where T : Processor { Add(typeof(T)); }

        /// <summary>
        /// Retrieves a system. Note that systems aka processors should never know about eachother.
        /// Use events for inter system communication.
        /// </summary>
        public T Get<T>() where T : Processor
        {
            foreach(SystemInfo info in all_systems)
                if (info.processor.GetSystemType() == typeof(T)) return info.processor as T;

            return null;
        }

        /// <summary>
        /// Checks if the Processor of the specified type exists within the systems collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Has<T>() where T : Processor
        {
            foreach (SystemInfo info in all_systems)
                if (info.processor.GetSystemType() == typeof(T)) return true;

            return false;
        }

        /// <summary>
        /// Instantiates the system-type's class and adds it to the manager along with its attributes.
        /// </summary>
        public void Add(Type stype)
        {
            // Instantiate system class
            var system = Activator.CreateInstance(stype) as Processor;
            var attr = stype.GetCustomAttributes(typeof(Attributes.ComponentUsageAttribute), true);
            uint priority = 0;
            Aspect aspect = Aspect.Has;
            Attributes.LoopType loop = Attributes.LoopType.Update;
            Type[] comps = null;
            Utils.BitMask systemMask = new Utils.BitMask();

            // Search through attributes
            for (int i = 0; i < attr.Length; i++)
            {
                if (attr[i] is Attributes.ComponentUsageAttribute)
                {
                    Attributes.ComponentUsageAttribute attribute = (Attributes.ComponentUsageAttribute) attr[i];
                    priority = (uint)attribute.priority;
                    aspect = attribute.aspect;
                    comps = attribute.types;
                    loop = attribute.loopType;
                    break;
                }
            }

            if(comps != null)
            {
                for(int i = 0; i < comps.Length; i++)
                {
                    var componentType = comps[i];
                    var classInstance = Activator.CreateInstance(componentType, null);
                    MethodInfo methodInfo = typeof(ComponentManager).GetMethod("RegisterComponent");
                    var m = methodInfo.MakeGenericMethod(componentType);
                    var result = m.Invoke(this.caboodle.Entities.Components, null);
                    int id = this.caboodle.Entities.Components.Get(componentType).GetId();
                    systemMask.Set(id, true);
                }
            }


            SystemInfo info = new SystemInfo(systemMask, system, system.Entities);
            system.SetAttr(caboodle, nextSystemId, priority, aspect, loop, stype);
            entityCache.Set(nextSystemId, new Utils.Table<Entity>());
            all_systems.Add(info);

            nextSystemId += 1;
        }

        private IEnumerable<Type> GetTypesWithInterface<T>(Assembly asm)
        {
            var it = typeof(T);
            return asm.GetLoadableTypes().Where(it.IsAssignableFrom).ToList();
        }

        public void Update()
        {
            if (entitiesToChange.Count > 0)
            {
                for (int i = 0; i < entitiesToChange.Count; i++)
                    OnEntityChange(entitiesToChange[i]);
                entitiesToChange.Clear();
            }
            if (entitiesToRemove.Count > 0)
            {
                for (int i = 0; i < entitiesToRemove.Count; i++)
                {
                    caboodle.Entities.SilentRemove(entitiesToRemove[i].Id);
                    OnEntityRemoved(entitiesToRemove[i]);
                }
                entitiesToRemove.Clear();
            }


            SystemInfo info;
            for (int i = 0; i < updatingSystemsCount; i++)
            {
                info = update_systems[i];
                info.processor.Process(info.entities);
            }

            if (scheduledActions.Count > 0)
            {
                for (int i = 0; i < scheduledActions.Count; i++)
                    scheduledActions[i]();
                scheduledActions.Clear();
            }

            caboodle.Events.Invoke(); 
        }

        public void FixedUpdate()
        {
            /*if (entitiesToRemove.Count > 0)
            {
                for (int i = 0; i < entitiesToRemove.Count; i++)
                    OnEntityRemoved(entitiesToRemove[i]);
                entitiesToRemove.Clear();
            }

            if (entitiesToChange.Count > 0)
            {
                for (int i = 0; i < entitiesToChange.Count; i++)
                    OnEntityChange(entitiesToChange[i]);
                entitiesToChange.Clear();
            }*/

            SystemInfo info;
            for(int i = 0; i < fixedSystemsCount; i++)
            {
                info = fixed_systems[i];
                info.processor.Process(info.entities);
            }
        }

        /// <summary>
        /// Initializes all systems. Should only be called once before any updates are made to 
        /// the systems. Sorts each system based on its priority and loop type
        /// (e.g. FixedUpdate, Update, once).
        /// </summary>
        public void Init()
        {
            all_systems.Sort((x, y) => x.processor.Priority.CompareTo(y.processor.Priority));
            List<SystemInfo> updatingSystems = new List<SystemInfo>();
            List<SystemInfo> fixedSystems = new List<SystemInfo>();

            for (int i = 0; i < all_systems.Count; i++)
            {
                all_systems[i].processor.Start();

                switch (all_systems[i].processor.Loop)
                {
                    case Attributes.LoopType.Once:
                        all_systems[i].processor.Process(all_systems[i].entities);
                        break;
                    case Attributes.LoopType.Update:
                        updatingSystemsCount++;
                        updatingSystems.Add(all_systems[i]);
                        break;
                    case Attributes.LoopType.FixedUpdate:
                        fixedSystemsCount++;
                        fixedSystems.Add(all_systems[i]);
                        break;
                    default:
                        break;
                }
            }
            update_systems = updatingSystems.ToArray();
            fixed_systems = fixedSystems.ToArray();
        }


        struct SystemInfo
        {
            public readonly Utils.BitMask mask;
            public readonly Processor processor;
            public readonly IDictionary<int, Entity> entities;

            public SystemInfo(Utils.BitMask mask,
                Processor processor, Dictionary<int,Entity> entities)
            {
                this.mask = mask;
                this.processor = processor;
                this.entities = entities;
            }
        }
    }

    /// <summary>
    /// Extension
    /// </summary>
    public static class TypeLoaderExtensions
    {

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }

}
