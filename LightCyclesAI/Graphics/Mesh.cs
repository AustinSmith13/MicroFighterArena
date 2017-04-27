using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Platform.Windows;

namespace LightCyclesAI.Graphics
{
    public sealed class Mesh
    {

        #region Properties

        public Vertex[] VertexData
        {
            get { return _vertexData; }
            set { _vertexData = value; }
        }

        public VertexBuffer VBO
        {
            get { return _vbo; }
        }

        #endregion

        private Vertex[] _vertexData;
        private VertexBuffer _vbo;

        public Mesh() { _vbo = new VertexBuffer(); }

        public Mesh(Vertex[] data)
        {
            this._vertexData = data;
            _vbo = new VertexBuffer();
            _vbo.SetData(_vertexData);
        }

        public void SetData(Vertex[] data)
        {
            this._vertexData = data;
            _vbo = new VertexBuffer();
            _vbo.SetData(_vertexData);
        }
    }
}
