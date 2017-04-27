using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaboodleES;

namespace LightCyclesAI.Components
{
    public class Bullet : Component
    {
        public int life = 256;
        public int owner = -1;
        public OpenTK.Vector3 dir = OpenTK.Vector3.Zero;

        public override void Reset() { life = 256; dir = OpenTK.Vector3.Zero; owner = -1; }
    }
}
