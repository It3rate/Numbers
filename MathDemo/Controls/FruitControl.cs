namespace MathDemo.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Numbers.Agent;
    using Numbers.Controls;
    using Numbers.Mappers;
    using Numbers.Renderer;
    using NumbersCore.CoreConcepts.Optical;
    using NumbersCore.CoreConcepts.Spatial;
    using NumbersCore.Primitives;
    using SkiaSharp;

    public class FruitControl : UIControlBase
    {
        public ExtentDomain Position { get; private set; } = new ExtentDomain(500, 500);
        //public ExtentDomain Size { get; private set; } = new ExtentDomain(50, 50); // need to be radius and aspect
        public Number Size { get; private set; }
        public Number AspectRatio { get; private set; }
        public HSLDomain Fill { get; private set; } = new HSLDomain();

        public NumberChain Roundness { get; private set; }
        private SpatialDomain _roundnessDomain = SpatialDomain.Get2DCenteredDomain(32, 1, "Roundness");
        public NumberChain Convexity { get;private set; }
        private SpatialDomain _convexityDomain = SpatialDomain.Get2DCenteredDomain(32, 1, "Convexity");

        private float[] _samplesX => _samples[0];
        private float[] _samplesY => _samples[1];
        private float[] _samplesSize => _samples[2];
        private float[] _samplesAspectRatio => _samples[3];
        private float[] _samplesHue => _samples[4];
        private float[] _samplesSaturation => _samples[5];
        private float[] _samplesLightness => _samples[6];
        private float[] _samplesRoundness => _samples[7];
        private float[] _samplesConvexity => _samples[8];
        public FruitControl(MouseAgent agent, int top, int left, int width, int height, long count = 20) : base(agent, count)
        {
            _sampleCounter.SetValue(20);

            // apple orange banana lemon lime cherry blueberry kiwi cherry peach
            AddPolyProperty(Position, left, left + width, top, top + height);
            //AddPolyProperty(Size, 5, 50, 5, 50); // horz, vert
            Size = CreateProperty("Size", 100, 150, 0, 200);
            AspectRatio = CreateProperty("Aspect Ratio", 60, 100, 0, 100);
            Roundness = new NumberChain(_roundnessDomain, Polarity.Aligned);
            Roundness.AddItem(0, 16);
            Convexity = new NumberChain(_convexityDomain, Polarity.Aligned);
            Convexity.AddItem(10, 20);
            AddPolyProperty(Fill, 0, 360, 70, 100, 40, 60);

            CreateDomainMaps();
            Update();
        }

        protected override void CreateDomainMaps()
        {
            _numbers.Clear();

            _numbers.Add(Position.Horizontal);
            _numbers.Add(Position.Vertical);
            _numbers.Add(Size);
            _numbers.Add(AspectRatio);
            //_numbers.Add(Size.Horizontal);
            //_numbers.Add(Size.Vertical);
            _numbers.Add(Fill.Hue);
            _numbers.Add(Fill.Saturation);
            _numbers.Add(Fill.Lightness);
            _numbers.Add(Roundness);
            _numbers.Add(Convexity);

            _samples = new float[_numbers.Count][];
        }

        private SKPaint _defaultStroke = CorePens.GetPen(SKColors.DarkGreen, 3);
        protected override void GeneratePath(long idx)
        {
            var aspect = (_samplesAspectRatio[idx]) / 50f;
            var rect = new SKRect(_samplesX[idx], _samplesY[idx], _samplesX[idx] + _samplesSize[idx], _samplesY[idx] + (_samplesSize[idx] * aspect));
            var path = SKPathMapper.GenerateArcOval(rect, _samplesRoundness[idx], _samplesConvexity[idx]);
            _paths[idx].Reset();
            _paths[idx].AddPath(path);
            _fills[idx] = CorePens.GetBrush(SKColor.FromHsl(_samplesHue[idx], _samplesSaturation[idx], _samplesLightness[idx]));
            _strokes[idx] = _defaultStroke;
        }
    }
}
