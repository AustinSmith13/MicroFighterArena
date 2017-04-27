using CaboodleES.Interface;

namespace CaboodleES
{
    public abstract class Component : ICloneable
    {
        public abstract void Reset();

        public virtual object Clone()
        {
            return this;
        }
    }
}
