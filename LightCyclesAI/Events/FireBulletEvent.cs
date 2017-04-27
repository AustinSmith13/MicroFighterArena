using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightCyclesAI.Components;
using CaboodleES;

namespace LightCyclesAI.Events
{
    public class FireBulletEvent : IEvent
    {
        public Transform start;
        public int owner;

        public FireBulletEvent(int owner, Transform start)
        {
            this.start = start;
            this.owner = owner;
        }
    }
}
