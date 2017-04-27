using System.Collections.Generic;
using LightCyclesAI.Components;
using CaboodleES.Attributes;
using CaboodleES.System;
using CaboodleES;
using System;

namespace LightCyclesAI.Systems
{
    /// <summary>
    /// Governs behavior for all instances (entities) of AgentAvoid.
    /// </summary>
    [ComponentUsage(10, CaboodleES.System.Aspect.Has,
        typeof(AgentData), typeof(AgentRandom))]
    public class AgentRandomSystem : Processor
    {
        // Every entity is considered an AgentAvoid
        public override void Process(IDictionary<int, Entity> entities)
        {
            foreach(Entity entity in entities.Values)
            {
                // Get the relavent data
                Transform transform = entity.GetComponent<Transform>();
                AgentData data = entity.GetComponent<AgentData>();
                var rand = rnd.NextDouble();
                if (rand > 0.95f)
                    data.nextMove = AgentData.Move.Forward;
                else if (rand > 0.9f)
                    data.nextMove = AgentData.Move.Left;
                else if (rand > 0.8f)
                    data.nextMove = AgentData.Move.Right;
                else
                    data.nextMove = AgentData.Move.Fire;
            }
        }
        Random rnd = new Random();
        public override void Start()
        {

        }
    }
}
