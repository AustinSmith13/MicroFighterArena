using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using LightCyclesAI.Graphics;

namespace LightCyclesAI.Scene
{
    class GridObject : DynamicObject
    {
        public int cell_size = 16;
        public int grid_size = 256;

        public override void Render()
        {
            base.Render();
            DrawGrid(_color, 0, 0, 0, cell_size, grid_size);
        }

        public void DrawGrid(OpenTK.Graphics.Color4 color, float X, float Y, float Z, int cell_size = 16, int grid_size = 256)
        {
            int dX = (int)Math.Round(X / cell_size) * cell_size;
            int dZ = (int)Math.Round(Y / cell_size) * cell_size;

            int ratio = grid_size / cell_size;

            GL.PushMatrix();

            GL.Translate(dX - grid_size / 2, dZ - grid_size / 2, 0);

            int i;

            GL.Color4(color);
            GL.Begin(PrimitiveType.Lines);

            for (i = 0; i < ratio + 1; i++)
            {
                int current = i * cell_size;

                GL.Vertex3(current, 0, 0);
                GL.Vertex3(current, grid_size, 0);

                GL.Vertex3(0, current, 0);
                GL.Vertex3(grid_size, current, 0);
            }

            GL.End();

            GL.PopMatrix();
        }
    }
}
