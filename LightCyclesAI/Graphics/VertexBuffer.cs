using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace LightCyclesAI.Graphics
{
    /// <summary>
    /// Data structure thats fed into the GPU.
    /// </summary>
    public struct Vertex
    {
        public Vector3 Position;//, Normal;
        public float R, B, G, A;
        public Vector2 TexCoord;
        public static readonly int Stride = System.Runtime.InteropServices.Marshal.SizeOf(default(Vertex)); // used for offset

        public Vertex(Vector3 position, float r, float g, float b, float a, Vector2 texcoord)
        {
            this.Position = position;
            this.TexCoord = texcoord;
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public Vertex(Vector3 position, float r, float g, float b, float a)
        {
            this.Position = position;
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
            this.TexCoord = new Vector2(0, 0);
        }

        public Vertex(Vector3 position)
        {
            this.Position = position;
            this.R = 1;
            this.G = 1;
            this.B = 1;
            this.A = 1;
            this.TexCoord = new Vector2(0, 0);
        }
    }

    /// <summary>
    /// VBO - Vertex Buffer Object, contains information for drawing geometry.
    /// </summary>
    public sealed class VertexBuffer
    {
        int id;
        Vertex[] data;

        int Id
        {
            get
            {
                // Create an id on first use.
                if (id == 0)
                {
                    GraphicsContext.Assert();

                    GL.GenBuffers(1, out id); // gives us our vbo id (handle)
                    if (id == 0)
                        throw new Exception("Could not create VBO.");
                }

                return id;
            }
        }

        public VertexBuffer() { }

        public void SetData(Vertex[] data)
        {
            // expects data to not be null
            if (data == null)
                throw new ArgumentNullException("data");

            this.data = data;

            // Binds the data to gpu memory, Id is the handle
            GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(data.Length * Vertex.Stride), data, BufferUsageHint.StaticDraw);
        }

        public void Render(RenderState renderState)
        {
            // Is there any data to send to the gpu?
            if (data == null)
                return;

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.Disable(EnableCap.Texture2D);
            GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
            GL.VertexPointer(3, VertexPointerType.Float, Vertex.Stride, 0);
            GL.ColorPointer(3, ColorPointerType.Float, Vertex.Stride, new IntPtr(Vector3.SizeInBytes));
            //GL.NormalPointer(NormalPointerType.Float, Vertex.Stride, new IntPtr(Vector3.SizeInBytes));
            //GL.NormalPointer(3, NormalPointerType.Float, Vertex.Stride, new IntPtr(Vector3.SizeInBytes));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Stride, 4 * Vector3.SizeInBytes);
            GL.PolygonMode(MaterialFace.Front, renderState.polygonMode);
            GL.DrawArrays(PrimitiveType.Triangles, 0, data.Length);
            //GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            //GL.ColorPointer(1, ColorPointerType.Float, Vertex.Stride, new IntPtr(Vector3.SizeInBytes * 4));
        }

        [Obsolete("RenderWireFrame is deprecated, use Render(RenderState) with the appropriate polygonmode.")]
        public void RenderWireFrame()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.Disable(EnableCap.Texture2D);
            GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
            GL.VertexPointer(3, VertexPointerType.Float, Vertex.Stride, 0);
            GL.ColorPointer(4, ColorPointerType.Float, Vertex.Stride, new IntPtr(Vector3.SizeInBytes));
            //GL.NormalPointer(NormalPointerType.Float, Vertex.Stride, new IntPtr(Vector3.SizeInBytes));
            //GL.NormalPointer(3, NormalPointerType.Float, Vertex.Stride, new IntPtr(Vector3.SizeInBytes));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Stride, 4 * Vector3.SizeInBytes);
            GL.Color4(Color4.White);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.DrawArrays(PrimitiveType.Lines, 0, data.Length);
        } 
    }
}
