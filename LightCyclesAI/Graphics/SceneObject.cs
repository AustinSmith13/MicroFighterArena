using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;

namespace LightCyclesAI.Graphics
{
    public abstract class SceneObject : IDisposable
    {
        static int ctid = 0;

        protected readonly int _id;
        protected int _zindex;
        protected Transform2 _transform;
        protected RenderState _renderState;
        protected Color4 _color;

        public SceneObject()
        {
            _id = ctid++;
            _transform = new Transform2();
            _renderState = new RenderState();
            _color = Color4.White;
        }

        public virtual void Dispose() { }

        #region Properties

        /// <summary>
        /// Gets the Id of an object.
        /// </summary>
        public int Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the Z-Index of an object.
        /// </summary>
        public int Z_Index
        {
            get { return _zindex; }
            set { _zindex = value; }
        }

        /// <summary>
        /// Gets the Transform of an object.
        /// </summary>
        public Transform2 Transform
        {
            get { return _transform; }
        }

        /// <summary>
        /// Gets the RenderState of an object.
        /// </summary>
        public RenderState RenderState
        {
            get { return _renderState; }
        }

        /// <summary>
        /// Gets the color of an object.
        /// </summary>
        public Color4 Color
        {
            get { return _color; }
            set { _color = value; }
        }

        #endregion

    }
}
