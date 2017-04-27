using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Input;


namespace LightCyclesAI.Graphics
{

    /// <summary>
    /// A hierarchy for organizing nodes for the purposes of positioning, scaling, rotating, and visibility.
    /// </summary>
    public sealed class SceneGraph : IDisposable
    {

        #region Properties

        // The current camera being used.
        public Camera ActiveCamera
        {
            get
            {
                return _cameraContext;
            }
            private set
            {
                _cameraContext = value;
            }
        }

        #endregion

        #region State

        // VBO - Vertex Buffer Object - contains all vertex data about this board.
        private Camera _cameraContext;
        private bool _initialized = false;
        private List<IRenderable> renderableObjects =
            new List<IRenderable>();
        private List<IStartable> startableObjects =
            new List<IStartable>();
        private List<IUpdateable> updateableObjects =
            new List<IUpdateable>();
        private List<SceneObject> objects =
            new List<SceneObject>();
        private List<SceneObject> objectQueue =
            new List<SceneObject>();
        private List<SceneObject> deleted =
            new List<SceneObject>();

        #endregion


        public SceneGraph()
        {

        }

        /// <summary>
        /// Adds a SceneObject to the Scene Graph.
        /// </summary>
        public void AddToScene(SceneObject sceneObject)
        {
            if (_initialized)
            {
                objectQueue.Add(sceneObject);
            }
            else
            {
                objects.Add(sceneObject);

                if (!RenderConfig.enableDepthTest)
                    objects = objects.OrderBy(o => o.Z_Index).ToList();
            }
        }

        /// <summary>
        /// Gets the active camera in the scene.
        /// </summary>
        /// <returns></returns>
        public Camera GetCameraContext()
        {
            return _cameraContext;
        }

        /// <summary>
        /// Sets the active camera in the scene.
        /// </summary>
        public void SetCameraContext(Camera context)
        {
            ActiveCamera = context;
            ActiveCamera.Start();
        }

        /// <summary>
        /// Clears all objects from the board.
        /// </summary>
        public void Clear()
        {
            foreach (var o in objects)
            {
                o.Transform.Parent = null;
                o.Transform.Children = null;
                o.Dispose();
            }

            RenderConfig.mode = 0x02;
            ActiveCamera = null;
            objects.Clear();
        }

        /// <summary>
        /// Must be called first before rendering.
        /// </summary>
        public void Initialize()
        {
            foreach (var sceneObject in objects)
            {
                if (sceneObject is IRenderable)
                    renderableObjects.Add(sceneObject as IRenderable);

                if (sceneObject is IStartable)
                {
                    startableObjects.Add(sceneObject as IStartable);

                    if (_initialized)
                        (sceneObject as IStartable).Start();
                }

                if (sceneObject is IUpdateable)
                    updateableObjects.Add(sceneObject as IUpdateable);
            }

            _initialized = true;
            foreach (var so in startableObjects)
                so.Start();

        }

        /// <summary>
        /// The update loop.
        /// </summary>
        public void Update(float deltatime)
        {
            // DEBUG DEPRECATED
            if (Keyboard.GetState().IsKeyDown(Key.R))
                RenderConfig.DEBUG = true;

            if (Keyboard.GetState().IsKeyDown(Key.T))
                RenderConfig.DEBUG = false;
            // END DEBUG

            foreach (var uo in updateableObjects)
                uo.Update(deltatime);
        }


        /// <summary>
        /// The render loop.
        /// </summary>
        public void Render(float deltatime)
        {
            RenderConfig.deltatime = deltatime;
            ActiveCamera.Update();

            if (RenderConfig.enableFaceCulling)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(RenderConfig.cullFaceMode);
            }
            else
            {
                GL.Disable(EnableCap.CullFace);
            }

            if (RenderConfig.enableDepthClamp)
            {
                GL.Enable(EnableCap.DepthClamp);
            }
            else
            {
                GL.Disable(EnableCap.DepthClamp);
            }

            if (RenderConfig.enableDepthTest)
            {
                GL.Enable(EnableCap.DepthTest);
            }
            else
            {
                GL.Disable(EnableCap.DepthTest);
            }

            if (RenderConfig.smoothLines)
            {
                GL.Enable(EnableCap.LineSmooth);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Hint(HintTarget.LineSmoothHint, HintMode.DontCare);
            }
            else
            {
                GL.Disable(EnableCap.LineSmooth);
                GL.Disable(EnableCap.Blend);
            }

            foreach (var ro in renderableObjects)
                ro.Render();


            // Add new scene objects at the end of each frame.
            ProcessAdded();
        }

        /// <summary>
        /// On resize, reizes the cameras context.
        /// </summary>
        public void Resize(int width, int height)
        {
            RenderConfig.width = width;
            RenderConfig.height = height;
            _cameraContext.Resize(width, height);
        }

        public void Dispose()
        {
            ActiveCamera.Dispose();

            foreach (var o in objects)
                o.Dispose();
        }

        private void ProcessAdded()
        {
            foreach (var sceneObject in objectQueue)
            {
                objects.Add(sceneObject);

                if (sceneObject is IStartable)
                    (sceneObject as IStartable).Start();

                if (sceneObject is IUpdateable)
                    updateableObjects.Add(sceneObject as IUpdateable);

                if (!RenderConfig.enableDepthTest)
                {
                    renderableObjects.Clear();

                    foreach (var o in objects)
                    {
                        if (sceneObject is IRenderable)
                            renderableObjects.Add(o as IRenderable);
                    }
                }
                else
                {
                    renderableObjects.Add(sceneObject as IRenderable);
                }
            }

            objectQueue.Clear();

            if (!RenderConfig.enableDepthTest)
                objects = objects.OrderBy(o => o.Z_Index).ToList();
        }
    }

    public enum MountSide
    {
        Top,
        Bottom,
        None
    }
}
