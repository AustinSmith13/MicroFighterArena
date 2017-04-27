using System;
using System.Collections.Generic;
using CaboodleES;
using CaboodleES.System;
using LightCyclesAI.Components;
using LightCyclesAI.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LightCyclesAI.Systems
{
    /// <summary>
    /// Responsible for rendering any entity that has a component of type renderable
    /// </summary>
    [CaboodleES.Attributes.ComponentUsage(100, CaboodleES.System.Aspect.Has,
        typeof(MeshRenderer), typeof(Transform))]       // query every entity that has a MeshRenderer and a transform
    internal class Renderer : Processor
    {
        // Render Loop
        public override void Process(IDictionary<int, Entity> entities)
        {
            foreach(Entity entity in entities.Values)
            {
                MeshRenderer meshRenderer = entity.GetComponent<MeshRenderer>();
                Transform transform = entity.GetComponent<Transform>();

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

                Matrix4 modelViewMat = transform.worldMat * RenderConfig.camera.ViewMatrix;

                if(meshRenderer.renderState.doClipping)
                {
                    if ((transform.AbsolutePosition() - RenderConfig.camera.Transform.Position).Length > meshRenderer.renderState.clipDistance)
                        meshRenderer.renderState.visible = false;
                    
                    else
                        meshRenderer.renderState.visible = true;
                        
                }
                if (meshRenderer.renderState.doBillBoarding)
                    modelViewMat = Utils.Helper.BillboardMatrix(ref modelViewMat);

                GL.FrontFace(meshRenderer.renderState.frontFaceMode);
                GL.PolygonMode(MaterialFace.Front, meshRenderer.renderState.polygonMode);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref modelViewMat);

                // Renders the vbo if present
                meshRenderer.mesh.VBO.Render(meshRenderer.renderState);

                // Alternative rendering or behavior
                if (meshRenderer.render != null)
                    meshRenderer.render();
            }
        }

        public override void Start()
        {
            // Renderer key events handler
            AddHandler<Events.KeyboardKeyEvent>((e) =>
            {
                if(e.keyboardKeyEvent.Key == OpenTK.Input.Key.W)
                {
                    if (RenderConfig.polygonMode == PolygonMode.Line)
                        RenderConfig.polygonMode = PolygonMode.Fill;
                    else
                        RenderConfig.polygonMode = PolygonMode.Line;

                    /*foreach(Entity entity in entities.Values)
                    {
                        entity.GetComponent<MeshRenderer>().renderState.polygonMode = PolygonMode.Line;
                    }*/
                }
            });
        }
    }
}
