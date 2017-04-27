using System.Runtime.Serialization;
using System;

namespace CaboodleES
{

    [Serializable]
    public class NoSuchEntityException : Exception
    {

        public int entity { get; private set; }

        public NoSuchEntityException() : base() { }

        public NoSuchEntityException(string message, int entity)
            : base(message)
        {
            this.entity = entity;
        }

        protected NoSuchEntityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
                this.entity = info.GetInt32("entity");
        }

        // Perform serialization
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
                info.AddValue("entity", entity);
        }
    }
}
