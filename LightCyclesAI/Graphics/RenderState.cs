using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace LightCyclesAI.Graphics
{
    /// <summary>
    /// Default render state of all objects.
    /// </summary>
    public class RenderState
    {
        public bool visible = true;
        public bool lighted = false;
        public bool depthTest = true;
        public bool alphaTest = false;
        public bool alphaBlendingOn = false;
        public bool doBillBoarding = false;
        public bool doClipping = false;
        public bool noShader = true;
        public bool isDeleted = false;
        public float clipDistance = 500f;
        public PolygonMode polygonMode = PolygonMode.Fill;
        public float lineWidth = 0f;
        public FrontFaceDirection frontFaceMode = FrontFaceDirection.Ccw;
    }
}
