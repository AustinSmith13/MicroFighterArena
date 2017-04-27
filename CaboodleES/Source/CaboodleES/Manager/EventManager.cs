using System.Collections.Generic;
using CaboodleES.Interface;
using System;

namespace CaboodleES
{
    public class EventManager : CManager
    {
        private HashSet<global::System.Type> genTypes;

        public void AddHandler<E>(Action<E> handler) where E : IEvent
        {
            if (!genTypes.Contains(typeof(E)))
                genTypes.Add(typeof(E));

            EventsRepository<E>.AddHandler(caboodle.Id, handler);
        }

        public void AddEvent<E>(E @event) where E : IEvent
        {
            if (!genTypes.Contains(typeof(E)))
                genTypes.Add(typeof(E));

            EventsRepository<E>.AddEvent(caboodle.Id, @event);
        }

        internal void Invoke()
        {
            var nonGenType = typeof(EventsRepository<>);

            foreach (var qualType in genTypes)
            {
                var genType = nonGenType.MakeGenericType(qualType);
                var mthd = genType.GetMethod("Invoke");

                mthd.Invoke(null, new object[] { caboodle.Id });
            }
        }

        public EventManager(Caboodle caboodle) : base(caboodle)
        {
            this.genTypes = new HashSet<Type>();
        }
    }

    public static class EventsRepository<E> where E : IEvent
    {
        private static Dictionary<int, EventCollection<E>> eventDictionary =
            new Dictionary<int, EventCollection<E>>();

        public static void AddHandler(int set, Action<E> handler)
        {
            EventCollection<E> events;

            if (!eventDictionary.TryGetValue(set, out events))
                eventDictionary.Add(set, events = new EventCollection<E>());

            events.AddHandler(handler);
        }

        public static void AddEvent(int set, E @event)
        {
            EventCollection<E> events;

            if (!eventDictionary.TryGetValue(set, out events))
                eventDictionary.Add(set, events = new EventCollection<E>());

            events.AddEvent(@event);
        }

        public static void Invoke(int set)
        {
            EventCollection<E> events;

            if (!eventDictionary.TryGetValue(set, out events))
                eventDictionary.Add(set, events = new EventCollection<E>());

            events.Invoke();
        }
    }

    public class EventCollection<E> : IEventCollection
        where E : IEvent
    {
        private List<Action<E>> handlers;
        private List<E> events;

        public EventCollection()
        {
            this.handlers = new List<Action<E>>();
            this.events = new List<E>();
        }

        public void AddHandler(Action<E> handler)
        {
            this.handlers.Add(handler);
        }

        public void AddEvent(E @event)
        {
            events.Add(@event);
        }

        public void Invoke()
        {
            foreach(var @event in events)
                foreach(var handler in handlers)
                    handler(@event);

            events.Clear();
        }
    }
}
