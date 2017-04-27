using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightCyclesAI.Components;
using CaboodleES.Attributes;
using CaboodleES.System;
using CaboodleES;

namespace LightCyclesAI.Systems
{
    /// <summary>
    /// Updates the AgentData sensor.
    /// </summary>
    [ComponentUsage(2, CaboodleES.System.Aspect.Has, 
        typeof(Transform), typeof(AgentData))]
    public class AgentSensorSystem : Processor
    {
        public override void Process(IDictionary<int, Entity> entities)
        {
            foreach(Entity entity in entities.Values)
            {
                Transform transform = entity.GetComponent<Transform>();
                AgentData data = entity.GetComponent<AgentData>();
                data.forward = 10 - transform.Position.Y;
            }
        }

        public override void Start()
        {
        }
    }
}
