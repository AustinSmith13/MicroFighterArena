using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;

namespace LightCyclesAI.Graphics
{

    public class DynamicObject : SceneObject, IRenderable, IUpdateable, IStartable
    {
        protected Mesh _mesh = new Mesh();

        public DynamicObject() : base() { }

        public Mesh Mesh
        {
            get { return _mesh; }
        }

        public virtual void Start()
        {

        }

        public virtual void Render()
        {
            Matrix4 modelViewMat = _transform.worldMat * RenderConfig.camera.ViewMatrix;

            if (_renderState.doClipping)
            {
                if ((_transform.AbsolutePosition() - RenderConfig.camera.Transform.Position).Length > _renderState.clipDistance)
                    _renderState.visible = false;
                else
                    _renderState.visible = true;
            }

            if (_renderState.doBillBoarding)
                modelViewMat = Utils.Helper.BillboardMatrix(ref modelViewMat);

            GL.FrontFace(_renderState.frontFaceMode);
            GL.PolygonMode(MaterialFace.Front, _renderState.polygonMode);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelViewMat);
        }

        public virtual void Update(float deltatime)
        {

        }
    }
}
