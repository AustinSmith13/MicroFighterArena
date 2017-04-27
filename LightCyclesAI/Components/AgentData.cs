using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaboodleES;

namespace LightCyclesAI.Components
{
    public class AgentData : Component
    {
        public enum Move
        {
            Forward,
            Left,
            Right,
            Fire
        }

        // The next move the light cycle will make
        public Move nextMove;

        // Sensors
        public float forward, left, right, diagnalLeft, diangnalRight;

        public override void Reset()
        {
            nextMove = Move.Forward;
            forward = 0;
            left = 0;
            right = 0;
            diangnalRight = 0;
            diagnalLeft = 0;
        }
    }
}
