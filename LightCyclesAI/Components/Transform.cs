using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaboodleES;
using OpenTK;
using LightCyclesAI.Graphics;

namespace LightCyclesAI.Components
{
    /// <summary>
    /// Represents a transformation in 3D space that can inherent from other transformations (3D spaces).
    /// </summary>
    public class Transform : Component
    {

        public override void Reset()
        {
            _orientation = Quaternion.Identity;
            _rotation = Vector3.Zero;
            Children = new List<Transform>();
            _flip = new Vector3(1);
            _pos = new Vector3(0f, 0f, 0f);
            _dir = new Vector3(0f, 0f, 1f);
            _up = new Vector3(0f, 1f, 0f);
            _right = new Vector3(1f, 0f, 0f);
            _scale = Vector3.One;
        }

        #region State

        private Quaternion _orientation;                        // Quaternions still needs to be re-implemented
        public Vector3 _rotation = new Vector3(0, 0, 0);
        private Vector3 _flip;
        private Vector3 _pos;
        private Vector3 _scale = new Vector3(1.0f);
        private Vector3 _dir;
        private Vector3 _right;
        private Vector3 _up;
        private Transform parent;
        private IList<Transform> children;

        // transform matricies
        public Matrix4 localMat;
        public Matrix4 worldMat;

        #endregion

        public Transform()
        {
            this.Children = new List<Transform>();
            this._flip = new Vector3(1);
            this._pos = new Vector3(0f, 0f, 0f);
            this._dir = new Vector3(0f, 0f, 1f);
            this._up = new Vector3(0f, 1f, 0f);
            this._right = new Vector3(1f, 0f, 0f);
            this.CalcMatFromState();
        }

        #region Properties

        /// <summary>
        /// Gets the world matrix of a transform.
        /// </summary>
        public Matrix4 Matrix
        {
            get { return worldMat; }
            set { worldMat = value; }
        }

        /// <summary>
        /// Gets or sets the euler angles of a transform.
        /// </summary>
        public Vector3 Rotation
        {
            get { return _rotation; }
            set { _rotation = value; CalcMatFromState(); }
        }

        public Quaternion Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        /// <summary>
        /// Gets or sets the parent of the transform.
        /// </summary>
        public Transform Parent
        {
            get { return parent; }
            set
            {
                // Corrects child parent pointers
                if (value != this)
                {
                    parent = value; this.CalcMatFromState(); if (parent != null) parent.Children.Add(this);
                }
            }
        }

        // Has not been tested! (This may or may not work)
        /// <summary>
        /// Gets the children of the transform.
        /// </summary>
        public IList<Transform> Children
        {
            get { return children; }
            set
            {
                children = value;
                foreach (var child in children)
                    if (child.Parent != this)
                        child.Parent = this;
            }
        }

        /// <summary>
        /// Sets the transforms scale.
        /// </summary>
        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; CalcMatFromState(); }
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        public Vector3 Position
        {
            get { return _pos; }
            set { _pos = value; CalcMatFromState(); }
        }

        /// <summary>
        /// Sets the scale to a uniform value.
        /// </summary>
        public float Size
        {
            set { Scale = new Vector3(value); }
        }

        /// <summary>
        /// Gets the forward directional vector.
        /// </summary>
        public Vector3 Dir
        {
            get { return _dir; }
        }

        /// <summary>
        /// Gets the right directional vector.
        /// </summary>
        public Vector3 Right
        {
            get { return _right; }
        }

        /// <summary>
        /// Gets the up directional vector.
        /// </summary>
        public Vector3 Up
        {
            get { return _up; }
        }

        #endregion

        #region Broken

        /// <summary>
        /// This is broken do not use.
        /// </summary>
        public void Orient(Vector3 dir, Vector3 up)
        {
            this._dir = dir;
            this._up = up;
            this._right = Vector3.Cross(this._up, this._dir).Normalized();
            this.CalcMatFromState();
        }

        /// <summary>
        /// Mostly borken dont use.
        /// </summary>
        public void Orient(Quaternion o)
        {
            Matrix4 newOrientation = Matrix4.CreateFromQuaternion(o);

            // Still a work in progress
            float ex = (float)Math.Atan((double)(2 * (o.W * o.X + o.Y * o.Z) / (1 - 2 * (o.X * o.X + o.Y * o.Y))));
            float ey = (float)Math.Asin((double)(2 * (o.W * o.Y - o.Z * o.X)));
            float ez = (float)Math.Atan((double)(2 * (o.W * o.Z + o.X * o.Y) / (1 - 2 * (o.Y * o.Y + o.Z * o.Z))));
            _rotation = new Vector3(ex, ey, ez);
            _orientation = o;

            // Possibly calculate euler from directional components
            this._dir = new Vector3(newOrientation.M31, newOrientation.M32, newOrientation.M33);
            this._up = new Vector3(newOrientation.M21, newOrientation.M22, newOrientation.M23);
            this._right = Vector3.Cross(this._up, this._dir).Normalized();
            this.CalcMatFromState();
        }

        #endregion

        #region Transform Methods

        /// <summary>
        /// Sets the transforms orientation in radians.
        /// </summary>
        public void SetAngles(Vector3 angle)
        {
            _rotation = angle;
            CalcMatFromState();
        }

        /// <summary>
        /// Sets the transforms position.
        /// </summary>
        public void SetPosition(Vector3 pos)
        {
            _pos = pos;
            CalcMatFromState();
        }

        /// <summary>
        /// Sets the transforms scale.
        /// </summary>
        public void SetScale(Vector3 scale)
        {
            _scale = scale;
            CalcMatFromState();
        }

        /// <summary>
        /// Inverts the X-axis.
        /// </summary>
        public void FlipX()
        {
            _flip = _flip * new Vector3(-1, 1, 1);
            CalcMatFromState();
        }

        /// <summary>
        /// Inverts the Y-axis.
        /// </summary>
        public void FlipY()
        {
            _flip = _flip * new Vector3(1, -1, 1);
            CalcMatFromState();
        }

        /// <summary>
        /// Inverts the Z-axis.
        /// </summary>
        public void FlipZ()
        {
            _flip = _flip * new Vector3(1, 1, -1);
            CalcMatFromState();
        }

        /// <summary>
        /// Gets the transforms position.
        /// </summary>
        public Vector3 GetPosition()
        {
            return _pos;
        }

        /// <summary>
        /// Gets the transforms rotation.
        /// </summary>
        public Vector3 GetAngles()
        {
            return _rotation;
        }

        /// <summary>
        /// Gets the transforms scale.
        /// </summary>
        public Vector3 GetScale()
        {
            return _scale;
        }

        /// <summary>
        /// Gets the position relative to the global coordinate system.
        /// </summary>
        public Vector3 AbsolutePosition()
        {
            return new Vector3(worldMat.M41, worldMat.M42, worldMat.M43);
        }

        /// <summary>
        /// Gets the rotation matrix relative to the global coordinate system.
        /// </summary>
        public Matrix4 AbsoluteRotationMatrix()
        {
            if (parent != null)
                return RotationMatrix() * parent.AbsoluteRotationMatrix();

            return RotationMatrix();
        }

        #endregion

        #region Internal Logic

        /// <summary>
        /// Gets the rotation matrix.
        /// </summary>
        /// <returns></returns>
        private Matrix4 RotationMatrix()
        {
            var rotMat = Matrix4.Identity;

            rotMat *= Matrix4.CreateRotationX(-_rotation.X);
            rotMat *= Matrix4.CreateRotationY(-_rotation.Y);
            rotMat *= Matrix4.CreateRotationZ(_rotation.Z);

            return rotMat;
        }

        /// <summary>
        /// Recalculates the objects world matrix from its state. (Remember to call this when you change any state. Most methods already call this for you)
        /// </summary>
        internal void CalcMatFromState()
        {
            Matrix4 newLocalMat = Matrix4.Identity;

            newLocalMat *= Matrix4.CreateScale(this._scale) * this.RotationMatrix() * Matrix4.CreateScale(_flip);

            // position
            newLocalMat.M41 = _pos.X;
            newLocalMat.M42 = _pos.Y;
            newLocalMat.M43 = _pos.Z;

            // compute world transformation
            Matrix4 newWorldMat;

            newWorldMat = newLocalMat;

            if (this.parent == null)
                newWorldMat = newLocalMat;
            else
                newWorldMat = newLocalMat * this.Parent.worldMat;

            // apply the transformations
            this.localMat = newLocalMat;
            this.worldMat = newWorldMat;
            this.CalculateDirFromRotation();

            //NotifyPositionOrSizeChanged(); // still has no use
        }

        private void CalculateDirFromRotation()
        {

            _dir = new Vector3
            {
                X = -worldMat.M31,
                Y = -worldMat.M32,
                Z = -worldMat.M33
            };

            _up = new Vector3
            {
                X = worldMat.M21,
                Y = worldMat.M22,
                Z = worldMat.M23
            };

            _right = new Vector3
            {
                X = worldMat.M11,
                Y = worldMat.M12,
                Z = worldMat.M13
            };
        }

        #endregion
    }
}