using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Models
{
    public class ImageSettings
    {
        // LineThreshold
        public double LTres { get; set; } = 1f;
        // QuadraticSplineThreshold!
        public double QTres { get; set; } = 1f;
        public int PathOmit { get; set; } = 8;

        public double ColorSampling { get; set; } = 1f;
        public int NumberOfColors { get; set; } = 16;
        public double MinColorRatio { get; set; } = .02f;
        public int ColorQuantCycles { get; set; } = 3;

        public double Scale { get; set; } = 1f;
        public double SimplifyTolerance { get; set; } = 0f;
        public int RoundCoords { get; set; } = 1;
        // LinearControlPointRadius
        public double LCpr { get; set; } = 0f;
        // QuadraticControlPointRadius
        public double QCpr { get; set; } = 0f;
        public bool Viewbox { get; set; } = false;

        public int BlurRadius { get; set; } = 0;
        public double BlurDelta { get; set; } = 20f;
    }
}