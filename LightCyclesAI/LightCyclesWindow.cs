using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using CaboodleES;
using System.Reflection;
using LightCyclesAI.Scene;
using LightCyclesAI.Graphics;
using LightCyclesAI.Components;

namespace LightCyclesAI
{
    class LightCyclesWindow : GameWindow
    {
        private Caboodle world = new Caboodle();

        public LightCyclesWindow()
            :base(800, 600, OpenTK.Graphics.GraphicsMode.Default, "Light Cycles AI")
        {
            VSync = VSyncMode.On;
            Console.WriteLine("[Options]");
            Console.WriteLine("     esc - exits application.");
            Console.WriteLine("     p   - Play a round of light cycles.");
            Console.WriteLine("     t   - Trains neural network to play light cycles.");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Find every processor (System) in the assembly
            world.Systems.Add(Assembly.GetExecutingAssembly());

            // Initialize the world
            world.Systems.Init();

            RenderConfig.camera.SetPosition(0, 0, 0);

            Entity gridEntity = world.Entities.Create();
            gridEntity.AddComponent<Transform>();
            var meshRenderer = gridEntity.AddComponent<MeshRenderer>();
            meshRenderer.render = () =>
            {
                int dX = (int)Math.Round(0 / 16d) * 16;
                int dZ = (int)Math.Round(0 / 16d) * 16;

                int ratio = 256 / 16;

                GL.PushMatrix();

                GL.Translate(dX - 256f / 2, dZ - 256f / 2, 0);

                int i;

                GL.Color4(Color4.DarkGray);
                GL.Begin(PrimitiveType.Lines);

                for (i = 0; i < ratio + 1; i++)
                {
                    int current = i * 16;

                    if (i == ratio / 2)
                        GL.Color4(Color4.DarkRed);
                    else
                        GL.Color4(Color4.DarkGray);

                    GL.Vertex3(current, 0, 0);
                    GL.Vertex3(current, 256, 0);

                    GL.Vertex3(0, current, 0);
                    GL.Vertex3(256, current, 0);
                }

                GL.End();

                GL.PopMatrix();
            };

            Mesh mesh = new Mesh();
            mesh.SetData(new Vertex[]{
                new Vertex( new Vector3(-0.8f, -0.8f, 0f)),
                new Vertex( new Vector3(0.8f, -0.8f, 0f)),
                new Vertex( new Vector3(0f, 0.8f, 0f))
            });

            Entity agent1 = world.Entities.Create();
            agent1.AddComponent<PrivateAgentData>();
            agent1.AddComponent<Transform>();
            agent1.AddComponent<AgentData>();
            agent1.AddComponent<AgentRandom>();
            agent1.AddComponent<MeshRenderer>().mesh = mesh;

            for (int i = 0; i < 500; i++)
            {
                var x = (float)Math.Sin(i / 10f) * 100f;
                var y = (float)Math.Cos(i / 10f) * 100f;
                Entity agent2 = world.Entities.Create();
                agent2.AddComponent<PrivateAgentData>();
                var transform2 = agent2.AddComponent<Transform>();
                agent2.AddComponent<AgentData>();
                agent2.AddComponent<AgentRandom>();
                agent2.AddComponent<MeshRenderer>().mesh = mesh;
                transform2.Position = new Vector3(x, y, 0);
            }



            // Forward any key events to the other systems
            Keyboard.KeyDown += (sender, ev) =>
            {
                world.Events.AddEvent<Events.KeyboardKeyEvent>(new Events.KeyboardKeyEvent(ev));

                if(ev.Key == Key.V)
                {
                    if (VSync == VSyncMode.Off)
                        VSync = VSyncMode.On;
                    else
                        VSync = VSyncMode.Off;
                }

                if(ev.Key ==  Key.C)
                {
                    for (int i = 0; i < 500; i++)
                    {
                        var x = (float)Math.Sin(i / 10f) * 100f;
                        var y = (float)Math.Cos(i / 10f) * 100f;
                        Entity agent2 = world.Entities.Create();
                        agent2.AddComponent<PrivateAgentData>();
                        var transform2 = agent2.AddComponent<Transform>();
                        agent2.AddComponent<AgentData>();
                        agent2.AddComponent<AgentRandom>();
                        agent2.AddComponent<MeshRenderer>().mesh = mesh;
                        transform2.Position = new Vector3(x, y, 0);
                    }

                }
            };

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        /// <summary>
        /// Defines behavior of the window when it is resized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RenderConfig.camera.Resize(Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Updates all systems
            world.Systems.Update();

            // Update camera
            RenderConfig.camera.Update(1f); 

            SwapBuffers();
        }

        [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (LightCyclesWindow game = new LightCyclesWindow())
            {
                game.Run(30.0);
            }
        }
    }
}
