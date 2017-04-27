using System;
using System.Collections.Generic;
using LightCyclesAI.Events;
using LightCyclesAI.Components;
using CaboodleES.Attributes;
using CaboodleES.System;
using CaboodleES;

namespace LightCyclesAI.Systems
{
    [ComponentUsage(1, CaboodleES.System.Aspect.Has, 
        typeof(Transform), typeof(AgentData), typeof(PrivateAgentData))]
    public class AgentMovementSystem : Processor
    {
        public override void Process(IDictionary<int, Entity> entities)
        {

            if (entities.Count == 0)
                Console.WriteLine("Round over!");

            foreach(Entity entity in entities.Values)
            {
                Transform transform = entity.GetComponent<Transform>();
                AgentData data = entity.GetComponent<AgentData>();
                PrivateAgentData privateData = entity.GetComponent<PrivateAgentData>();

                privateData.stepsTaken++;

                if (privateData.ammo == 0)
                    RemoveEntity(entity);

                if (data.nextMove == AgentData.Move.Left)
                    transform.SetAngles(transform.GetAngles() + new OpenTK.Vector3(0, 0, 0.1f));

                else if (data.nextMove == AgentData.Move.Right)
                    transform.SetAngles(transform.GetAngles() + new OpenTK.Vector3(0, 0, -0.0f));

                else if (data.nextMove == AgentData.Move.Fire && privateData.fireTimeout <= 0
                   && privateData.ammo > 0)
                {
                    AddEvent<FireBulletEvent>(new FireBulletEvent(entity.Id, transform));
                    privateData.ammo--;
                    privateData.fireTimeout = 100;
                }

                if (privateData.fireTimeout > 0)
                    privateData.fireTimeout--;

                data.nextMove = AgentData.Move.Forward;

                transform.Position = transform.Position + (transform.Up / 4f);
            }
        }

        public override void Start()
        {
            AddHandler<CheckBulletCollisionEvent>((e) =>
            {
                foreach(Entity entity in entities.Values)
                {
                    Transform transform = entity.GetComponent<Transform>();
                    if ((transform.Position - e.transform.Position).LengthSquared < 10f)
                    {
                        RemoveEntity(entity);
                        RemoveEntity(Caboodle.Entities.Get(e.sender));
                    }
                }
            });
        }
    }
}
