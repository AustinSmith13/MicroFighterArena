using System;
using CaboodleES;
using LightCyclesAI.Graphics;

namespace LightCyclesAI.Components
{
    // Renders the sceneObject associated with the entity
    class MeshRenderer : Component
    {
        public Mesh mesh;                   // Mesh Data to be rendererd
        public Action render;               // Auxiliary render method
        public RenderState renderState;     // Rendering config for mesh

        public MeshRenderer()
        {
            // Warning: not having an initialized renderstate or mesh will throw errors
            renderState = new RenderState();
            mesh = new Graphics.Mesh();
        }

        /// <summary>
        /// When a component is released to the internal pooling system, this method
        /// resets the components data to its default state.
        /// </summary>
        public override void Reset()
        {
            render = null;
            renderState = new RenderState();
            mesh = new Graphics.Mesh();
        }
    }
}
