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
        public ExtentDomain Size { get; private set; } = new ExtentDomain(50, 50); // need to be radius and aspect
        public HSLDomain Fill { get; private set; } = new HSLDomain();

        public NumberChain Flatness { get; private set; }
        private SpatialDomain _flatnessDomain = SpatialDomain.Get2DCenteredDomain(32, 1, "Flatness");
        public Number InnerFlatness { get;private set; }
        private SpatialDomain _innerFlatnessDomain = SpatialDomain.Get2DCenteredDomain(32, 1, "InnerFlatness");

        private float[] _samplesX => _samples[0];
        private float[] _samplesY => _samples[1];
        private float[] _samplesRadius => _samples[2];
        private float[] _samplesRadiusOffset => _samples[3];
        private float[] _samplesHue => _samples[4];
        private float[] _samplesSaturation => _samples[5];
        private float[] _samplesLightness => _samples[6];
        private float[] _samplesFlatness => _samples[7];
        private float[] _samplesInnerFlatness => _samples[8];
        public FruitControl(MouseAgent agent, int top, int left, int width, int height, long count = 20) : base(agent, count)
        {
            _sampleCounter.SetValue(20);

            // apple orange banana lemon lime cherry blueberry kiwi cherry peach
            AddPolyProperty(Position, left, left + width, top, top + height);
            AddPolyProperty(Size, 5, 50, 5, 50); // horz, vert
            Flatness = new NumberChain(_flatnessDomain, Polarity.Aligned);
            Flatness.AddItem(16, 32);
            InnerFlatness = new NumberChain(_innerFlatnessDomain, Polarity.Aligned);
            Flatness.AddItem(-10, 10);
            AddPolyProperty(Fill, 0, 360, 0, 100, 20, 80);

            CreateDomainMaps();
            Update();
        }

        protected override void CreateDomainMaps()
        {
            _numbers.Clear();

            _numbers.Add(Position.Horizontal);
            _numbers.Add(Position.Vertical);
            _numbers.Add(Size.Horizontal);
            _numbers.Add(Size.Vertical);
            _numbers.Add(Fill.Hue);
            _numbers.Add(Fill.Saturation);
            _numbers.Add(Fill.Lightness);
            _numbers.Add(Flatness);
            _numbers.Add(InnerFlatness);

            _samples = new float[_numbers.Count][];
        }

        private SKPaint _defaultStroke = CorePens.GetPen(SKColors.DarkGreen, 3);
        protected override void GeneratePath(long idx)
        {
            var pts = SKPathMapper.GenerateStar(_samplesX[idx], _samplesY[idx], _samplesRadius[idx], _samplesRadius[idx], (int)7, _samplesRadiusOffset[idx] / 100f);
            _paths[idx].Reset();
            _paths[idx].AddPoly(pts);
            _fills[idx] = CorePens.GetBrush(SKColor.FromHsl(_samplesHue[idx], _samplesSaturation[idx], _samplesLightness[idx]));
            _strokes[idx] = _defaultStroke;
        }
    }
}
