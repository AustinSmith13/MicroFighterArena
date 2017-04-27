using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LightCyclesAI.Graphics
{
    /// <summary>
    /// Global rendering configuration.
    /// </summary>
    public static class RenderConfig
    {
        public static bool DEBUG = false;
        //public static SceneGraph scene;                               // The sender [Deprecated]
        public static Camera camera = new OrthographicCamera();         // camera which exists as its own subsystem
        public static int width, height;                                // Viewport width and height
        public static byte mode = 0x0002;                               // Mode: 0x00 - Perspective; 0x01 - Orthographic; 0x02 - NULL
        public static bool drawGLSL = true;                             // Not implemented
        public static bool enableFaceCulling = true;
        public static bool enableDepthTest = false;                     // Will not depthtest PCB objects
        public static CullFaceMode cullFaceMode = CullFaceMode.Back;
        public static PolygonMode polygonMode = PolygonMode.Fill;
        public static bool enableDepthClamp = true;
        public static bool smoothLines = false;                         // Anti-aliasing filter
        public static bool frustumCulling = false;                      // Not implemented
        public static float deltatime = 0f;                             // The time since the last frame was rendered in ms
    }
}
