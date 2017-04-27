using System.Collections.Generic;


namespace CaboodleES.Manager
{
    internal sealed class PoolManager : CManager
    {
        private Dictionary<global::System.Type, Stack<Component>> reusableComponents =
            new Dictionary<global::System.Type, Stack<Component>>();
        private Stack<Entity> entityPool = new Stack<Entity>();
        private int nextEntityId = 0;

        // Default 
        private readonly int max = 128;

        public PoolManager(Caboodle caboodle) : base(caboodle) { }
        
        public PoolManager(Caboodle caboodle, int maxComps) : base (caboodle)
        {
            this.max = maxComps;
        }

        public Entity CreateEntity()
        {
            if (entityPool.Count > 0)
                return entityPool.Pop();
            else
                return new CaboodleES.Entity(caboodle, nextEntityId++);
        }

        public Entity ReleaseEntity(Entity entity)
        {
            if (entityPool.Count > max) return entity;
            if(entity == null)
            {
                return null;
            }
            entity.Active = true;
            entity.mask.Clear();
            entityPool.Push(entity);
            return entity;
        }

        public Component ReleaseComponent(Component component)
        {
            Stack<Component> cs = null;

            component.Reset();

            if(!reusableComponents.TryGetValue(component.GetType(), out cs))
            {
                cs = new Stack<Component>();
                reusableComponents.Add(component.GetType(), cs);
            }

            if (cs.Count > max)
                return component;

            cs.Push(component);

            return component;
        }

        public T CreateComponent<T>() where T : Component, new()
        {
            Stack<Component> cstack = null;
            if(!reusableComponents.TryGetValue(typeof(T), out cstack))
            {
                cstack = new Stack<Component>();
                for(int i = 0; i < max; i++)
                {
                    cstack.Push(new T());
                }

                reusableComponents.Add(typeof(T), cstack);
            }
            else if(cstack.Count == 0)
            {
                for (int i = 0; i < max; i++)
                {
                    cstack.Push(new T());
                }
            }

            return (T) cstack.Pop();
        }

        public void Clear()
        {
            reusableComponents.Clear();
        }
    }
}