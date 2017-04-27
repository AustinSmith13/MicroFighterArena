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
    [ComponentUsage(3, CaboodleES.System.Aspect.Has, 
        typeof(Transform), typeof(CircleCollider))]
    public class CollisionSystem : Processor
    {
        public override void Process(IDictionary<int, Entity> entities)
        {
        }

        public override void Start()
        {
        }
    }
}
