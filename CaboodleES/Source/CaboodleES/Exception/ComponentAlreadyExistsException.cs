using System.Runtime.Serialization;
using System;

namespace CaboodleES
{

    [Serializable]
    public class ComponentAlreadyExistsException : global::System.Exception
    {

        public string component { get; private set; }
        public ulong entity { get; private set; }

        public ComponentAlreadyExistsException() : base() { }

        public ComponentAlreadyExistsException(ulong entity, string component)
            : base(component + " already exists for entity " + entity)
        {
            this.component = component;
            this.entity = entity;
        }

        protected ComponentAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            { 
                this.component = info.GetString("component");
                this.entity = info.GetUInt64("entity");
            }
        }

        // Perform serialization
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("component", component);
                info.AddValue("entity", entity);
            }
        }
    }
}
