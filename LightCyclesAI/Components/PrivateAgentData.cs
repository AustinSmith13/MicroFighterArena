using CaboodleES;

namespace LightCyclesAI.Components
{
    /// <summary>
    /// This data is inaccessable to user agents.
    /// Keeps track of agent information such as ammo left or steps taken.
    /// </summary>
    public class PrivateAgentData : Component
    {
        public int kills = 0;                          // The number of kills an agent has
        public int fireTimeout = 100;                  // The number of steps until another bullet can be fired
        public int ammo = 1000;                        // The amount of ammo an agent has left
        public long stepsTaken = 0;                    // The number of steps taken since the begining of the match

        public override void Reset()
        {
            stepsTaken = 0;
            ammo = 1000;
            fireTimeout = 100;
        }
    }
}
