using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using LightCyclesAI.Utils;

namespace LightCyclesAI.Graphics
{
    /// <summary>
    /// Perspective camera with viewport controls.
    /// </summary>
    public class ViewPortCamera : Camera
    {
        public override void Start()
        {
            // Tell the scene that this is a perspective camera.
            RenderConfig.mode = 0x00;
        }

        public override void Resize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            Matrix4 mat = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, width / (float)height, Znear, Zfar);
            Transform.Matrix = mat;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref mat);
        }

        int xLast = 0, yLast = 0;

        public override void Update(float deltatime)
        {
            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.W))
                Transform.Position += (Transform.Dir * 15f * deltatime);

            if (input.IsKeyDown(Key.S))
                Transform.Position += (Transform.Dir * -15f * deltatime);

            if (input.IsKeyDown(Key.A))
                Transform.Position += (Transform.Right * -15f * deltatime);

            if (input.IsKeyDown(Key.D))
                Transform.Position += (Transform.Right * 15f * deltatime);

            var m = Mouse.GetState();

            var XDelta = ((float)m.X) / 10f - ((float)xLast) / 10f;
            var YDelta = ((float)m.Y) / 10f - ((float)yLast) / 10f;
            xLast = m.X;
            yLast = m.Y;

            if (pitchAngle > (float)Math.PI / 2f && pitchAngle < 1.5f * (float)Math.PI)
            {
                // upside down
                XDelta *= (int)-1f;
            }

            pitchAngle += Helper.DegreeToRadian(YDelta);
            yawAngle += Helper.DegreeToRadian(XDelta);

            const float twoPi = (float)(2.0 * Math.PI);
            if (pitchAngle < 0f)
            {
                pitchAngle += twoPi;
            }
            else if (pitchAngle > twoPi)
            {
                pitchAngle -= twoPi;
            }

            this.Transform.SetAngles(new Vector3(pitchAngle, yawAngle, 0f));
        }
    }

    /// <summary>
    /// Perspective camera.
    /// </summary>
    public class PerspectiveCamera : Camera
    {
        public override void Start()
        {
            // Tell the scene that this is a perspective camera.
            RenderConfig.mode = 0x00;
        }

        public override void Resize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            var mat = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, width / (float)height, Znear, Zfar);
            Transform.Matrix = mat;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref mat);
        }
    }

    /// <summary>
    /// Orthographic camera.
    /// </summary>
    public class OrthographicCamera : Camera
    {
        int width, height;
        public override void Start()
        {
            // Tell the scene that this is a orthographic camera.
            RenderConfig.mode = 0x01;
        }

        public override void Resize(int width, int height)
        {
            this.width = width;
            this.height = height;

            GL.Viewport(0, 0, width, height);

            float aspect = ((float)width / (float)height);

            var mat = Matrix4.CreateOrthographic(((float)width * aspect) / Size, ((float)height * aspect) / Size, Znear, Zfar);
            Transform.Matrix = mat;
            Transform.SetPosition(lastPos);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref mat);
        }

        float xLast = 0f, yLast = 0f, XDelta = 0f, YDelta = 0f, lastScroll;
        Vector3 lastPos;
        public override void Update(float deltatime)
        {
            var input = Keyboard.GetState();
            var m = Mouse.GetState();

            if(m.Scroll.Y != lastScroll)
            {
                Size = ((m.Scroll.Y+32)*(m.Scroll.Y+32))/100f;
                Resize(width, height);
            }
            lastScroll = m.Scroll.Y;
            if(m.MiddleButton == ButtonState.Pressed)
            {

                XDelta = (((float)m.X) - ((float)xLast)) / Size * 2;
                YDelta = (((float)m.Y) - ((float)yLast)) / Size * 2;

                this.Transform.SetPosition((this.Transform.Right * -XDelta) + (this.Transform.Up * YDelta) + lastPos);
            } else
            {
                xLast = m.X;
                yLast = m.Y;
                lastPos = Transform.Position;
            }

        }
    }

    /// <summary>
    /// Scene Camera.
    /// </summary>
    public abstract class Camera : SceneObject, IStartable, IUpdateable
    {
        public float Size
        {
            get { return _size; }
            set { _size = value; }
        }
        protected float _size = 18f;
        protected float pitchAngle = 0f;
        protected float yawAngle = 0f;

        protected static readonly float Znear = 1f, Zfar = 500f;

        #region Properties

        public Matrix4 ViewMatrix { get { return Transform.worldMat.Inverted(); } }

        #endregion

        #region Transformation

        public virtual void SetPosition(Vector3 pos)
        {
            Transform.Position = pos;
        }

        public virtual void SetPosition(float x, float y, float z)
        {
            Transform.Position = new Vector3(x, y, z);
        }

        public virtual void AddPosition(Vector3 delta)
        {
            Transform.Position += delta;
        }

        public virtual void AddPosition(float dx, float dy, float dz)
        {
            Transform.Position += new Vector3(dx, dy, dz);
        }

        #endregion

        // Only needed for when the variable "_size" changes. (Temporary fix)
        public void Update()
        {
            this.Transform.CalcMatFromState();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref Transform.worldMat);
        }

        public abstract void Resize(int width, int height);

        public virtual void Start() { }

        public virtual void Update(float deletatime) { }
    }
}
