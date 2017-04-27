using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightCyclesAI.Components;
using CaboodleES;

namespace LightCyclesAI.Events
{
    public class CheckBulletCollisionEvent : IEvent
    {
        public Bullet bullet;
        public Transform transform;
        public int sender;

        public CheckBulletCollisionEvent(int sender, Bullet bullet, Transform transform)
        {
            this.bullet = bullet;
            this.transform = transform;
            this.sender = sender;
        }
    }
}
