using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaboodleES;

namespace LightCyclesAI.Components
{
    public class CircleCollider : Component
    {
        public float radius = 1f;

        public override void Reset()
        {
            this.radius = 1f;
        }
    }
}
