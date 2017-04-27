using global::System;

namespace CaboodleES.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ComponentUsageAttribute : Attribute
    {
        public readonly int priority;
        public readonly Type[] types;
        public readonly System.Aspect aspect;
        public readonly LoopType loopType;

        private readonly ExecutionType executionType; // not implemented

        public ComponentUsageAttribute(int priority, LoopType loopType, System.Aspect aspect, params Type[] comps)
        {
            foreach(Type c in comps)
            {
                if(c.BaseType != typeof(Component))
                    throw new ArgumentException("Expected Component base type");
            }

            this.priority = priority;
            this.types = comps;
            this.executionType = ExecutionType.Synchronous;
            this.aspect = aspect;
            this.loopType = loopType;
        }

        public ComponentUsageAttribute(int priority, System.Aspect aspect, params Type[] comps)
        {
            foreach (Type c in comps)
            {
                if (c.BaseType != typeof(Component))
                    throw new ArgumentException("Expected Component base type");
            }

            this.priority = priority;
            this.types = comps;
            this.executionType = ExecutionType.Synchronous;
            this.aspect = aspect;
            this.loopType = LoopType.Update;
        }

        public ComponentUsageAttribute()
        {
            this.priority = 0;
            this.executionType = ExecutionType.Synchronous;
        }
    }

    public enum ExecutionType
    {
        Synchronous,
        Asynchronous
    }

    public enum LoopType
    {
        Update,
        FixedUpdate,
        Once
    }
}
