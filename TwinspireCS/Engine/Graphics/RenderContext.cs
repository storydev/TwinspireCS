using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinspireCS.Engine.UI;

namespace TwinspireCS.Engine.Graphics
{
    public class RenderContext
    {

        private SimulationRegion interfaceSimulationRegion;
        private SimulationRegion canvasSimulationRegion;
        private List<Element> elements;

        private RenderScope currentScope;
        private List<RenderTarget> targets;

        public RenderContext()
        {
            interfaceSimulationRegion = new SimulationRegion(1.3f, float.PositiveInfinity);
            canvasSimulationRegion = new SimulationRegion(1.5f, float.PositiveInfinity);
            elements = new List<Element>();
            targets = new List<RenderTarget>();
        }

        /// <summary>
        /// Begin simulations on the given scope.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public bool Begin(RenderScope scope)
        {
            currentScope = scope;

            return true;
        }

        /// <summary>
        /// Initialise the render target for the current scope.
        /// </summary>
        /// <param name="target">Gets the <c>RenderTarget</c> for rendering.</param>
        /// <returns>Returns <c>false</c> if the target had to be created, otherwise <c>true</c.></returns>
        public bool InitTarget(out RenderTarget target)
        {
            var results = targets.Where((t) => t.ExpectedScope == currentScope);
            if (!results.Any())
            {
                target = new RenderTarget();
                target.ExpectedScope = currentScope;
                target.Type = RenderTargetType.Screen;
                targets.Add(target);

                return false;
            }
            else
            {
                target = results.FirstOrDefault();
            }

            return true;
        }



        public void Render()
        {

        }

    }
}
