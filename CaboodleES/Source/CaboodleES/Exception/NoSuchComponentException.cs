using System.Runtime.Serialization;
using System;

namespace CaboodleES
{

    [Serializable]
    public class NoSuchComponentException : Exception
    {

        public string component { get; private set; }

        public NoSuchComponentException() : base() { }

        public NoSuchComponentException(string message, string component)
            : base(message)
        {
            this.component = component;
        }

        protected NoSuchComponentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
                this.component = info.GetString("component");
        }

        // Perform serialization
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
                info.AddValue("component", component);
        }
    }
}
