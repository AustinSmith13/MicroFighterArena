using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightCyclesAI.Graphics;
using LightCyclesAI.Components;
using CaboodleES.Attributes;
using CaboodleES.System;
using CaboodleES;
using OpenTK;

namespace LightCyclesAI.Systems
{
    /// <summary>
    /// Governs the behavior of all bullets in the system.
    /// </summary>
    [ComponentUsage(4, CaboodleES.System.Aspect.Has, 
        typeof(Bullet), typeof(Transform))]
    public class BulletSystem : Processor
    {
        public override void Process(IDictionary<int, Entity> entities)
        {
            foreach(Entity entity in entities.Values)
            {
                // Get the required components from the entity
                Transform transform = entity.GetComponent<Transform>();
                Bullet bullet = entity.GetComponent<Bullet>();

                // Remove a bullet if it is out of life
                if(bullet.life <= 0)
                {
                    RemoveEntity(entity); // This method will schedule a proper entity deletion
                }

                // Move the bullet in the direction of where it is facing by one unit
                transform.Position = transform.Position + bullet.dir;

                // Let other systems check to see if there has been a bullet collision with another agent
                AddEvent(new Events.CheckBulletCollisionEvent(entity.Id, bullet, transform));

                // Decrement the life by one
                bullet.life--;
            }
        }

        public override void Start()
        {
            var mesh = new Graphics.Mesh();
                mesh.SetData(new Vertex[] {
                    new Vertex(new Vector3(0.5f, 0.5f, 0), 1f, 0.2f, 0, 1f),
                    new Vertex(new Vector3(-0.5f, 0.5f, 0), 1f, 0.2f, 0, 1f),
                    new Vertex(new Vector3(-0.5f, -0.5f, 0), 1f, 0.5f, 0, 1f),
                    new Vertex(new Vector3(-0.5f, 0.5f, 0), 1f, 0.5f, 0, 1f),
                    new Vertex(new Vector3(0.5f, -0.5f, 0), 1f, 0.5f, 0, 1f),
                    new Vertex(new Vector3(0.5f, 0.5f, 0), 1f, 0.5f, 0, 1f)
                });
            // Creates a new bullet that is deadly to all agents including the owner
            AddHandler<Events.FireBulletEvent>((e) =>
            {
                Entity bullet = Caboodle.Entities.Create();
                bullet.AddComponent<MeshRenderer>().mesh = mesh;
                Transform transform = bullet.AddComponent<Transform>();
                transform.SetAngles(e.start.GetAngles());
                transform.Position = e.start.Position + e.start.Up*10;
                Bullet bulletComponent = bullet.AddComponent<Bullet>();
                bulletComponent.dir = e.start.Up;
                bulletComponent.owner = e.owner;
            });
        }
    }
}
