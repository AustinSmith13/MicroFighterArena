using System;

namespace CaboodleES
{
    public interface IEvent
    {
    }
}

namespace CaboodleES.Interface
{
    public interface IEventCollection
    {
        void Invoke();
    }
}
