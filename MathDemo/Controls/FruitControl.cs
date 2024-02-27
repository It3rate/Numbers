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
    using NumbersCore.Utils;
    using SkiaSharp;

    public class FruitControl : UIControlBase
    {
        public ExtentDomain Position { get; private set; } = new ExtentDomain(500, 500);
        public Number Size { get; private set; }
        public Number AspectRatio { get; private set; }
        public HSLDomain Fill { get; private set; } = new HSLDomain();
        public NumberChain Convexity { get;private set; }
        private SpatialDomain _convexityDomain = SpatialDomain.Get2DCenteredDomain(32, 1, "Convexity");

        private float[] _samplesX => _samples[0];
        private float[] _samplesY => _samples[1];
        private float[] _samplesSize => _samples[2];
        private float[] _samplesAspectRatio => _samples[3];
        private float[] _samplesHue => _samples[4];
        private float[] _samplesSaturation => _samples[5];
        private float[] _samplesLightness => _samples[6];
        private float[] _samplesConvexity => _samples[7];
        public FruitControl(MouseAgent agent, int top, int left, int width, int height, long count = 20) : base(agent, count)
        {
            _sampleCounter.SetValue(20);

            // apple orange banana lemon lime cherry blueberry kiwi cherry peach
            AddPolyProperty(Position, left, left + width, top, top + height);

            Size = CreateProperty("Size", 40, 50, 0, 100);

            var aspectDomain = Domain.CreateDomain("Aspect", 50, -1, 1, 0, "Aspect", false);
            AspectRatio = aspectDomain.CreateNumber(new Range(0.2f, 0.2f));

            Convexity = new NumberChain(_convexityDomain, Polarity.Aligned);
            Convexity.AddItem(32, 32);
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
            _numbers.Add(Fill.Hue);
            _numbers.Add(Fill.Saturation);
            _numbers.Add(Fill.Lightness);
            _numbers.Add(Convexity);

            _samples = new float[_numbers.Count][];
        }

        private SKPaint _defaultStroke = CorePens.GetPen(SKColors.Black, 2);
        protected override void GeneratePath(long idx)
        {
            var aspect = _samplesAspectRatio[idx];
            var rect = new SKRect(_samplesX[idx], _samplesY[idx], _samplesX[idx] + _samplesSize[idx], _samplesY[idx] + (_samplesSize[idx]  + _samplesSize[idx] * aspect));
            var path = SKPathMapper.GenerateArcOval(rect, _samplesConvexity[idx]);
            _paths[idx].Reset();
            _paths[idx].AddPath(path);
            _fills[idx] = CorePens.GetBrush(SKColor.FromHsl(_samplesHue[idx], _samplesSaturation[idx], _samplesLightness[idx]));
            _strokes[idx] = _defaultStroke;
        }
    }
}
