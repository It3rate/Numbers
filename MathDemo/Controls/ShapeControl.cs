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
    using NumbersCore.CoreConcepts.Counter;
    using NumbersCore.CoreConcepts.Optical;
    using NumbersCore.CoreConcepts.Spatial;
    using NumbersCore.Primitives;
    using NumbersCore.Utils;
    using SkiaSharp;

    public class ShapeControl : UIControlBase
    {
        // radius
        // inner radius offset
        // outline color
        // fill color
        // points
        // rotation
        // x range
        // y range
        public bool IsDirty 
        { 
            get => _sampleCounter.Segment.IsDirty;
            set => _sampleCounter.Segment.IsDirty = value; 
        }

        public long _lastSampleCount = 0;
        public SegmentCounter _sampleCounter = new SegmentCounter(1, 50);
        public long SampleCount => _sampleCounter.Value;

        public Number Radius { get; }
        public Number RadiusOffset { get; }
        public HSLDomain Fill { get; } = new HSLDomain();
        public HSLDomain Stroke { get; } = new HSLDomain();
        public PositionDomain Position { get; } = new PositionDomain(500, 500);
        public Number StrokeWidth { get; }
        public Number Rotation { get; }
        public Number Points { get; }



        private bool _pathsDirty = true;
        private SKPath[] _paths;
        private SKPaint[] _strokes;
        private SKPaint[] _fills;

        private List<Number> _numbers = new List<Number>();
        private float[][] _samples;

        private float[] _samplesRadius => _samples[0];
        private float[] _samplesRadiusOffset => _samples[1];
        private float[] _samplesHue => _samples[2];
        private float[] _samplesSaturation => _samples[3];
        private float[] _samplesLightness => _samples[4];
        private float[] _samplesStrokeHue => _samples[5];
        private float[] _samplesStrokeSaturation => _samples[6];
        private float[] _samplesStrokeLightness => _samples[7];
        private float[] _samplesX => _samples[8];
        private float[] _samplesY => _samples[9];
        private float[] _samplesStrokeWidth => _samples[10];
        private float[] _samplesRotation => _samples[11];
        private float[] _samplesPoints => _samples[12];

        public ShapeControl(MouseAgent agent, int top, int left, int width, int height) : base(agent)
        {
            Radius = CreateProperty("Radius", 25, 50, 0, 50);
            RadiusOffset = CreateProperty("RadiusOffset", 20, 100, 0, 200);
            AddPolyProperty(Fill, 0, 360, 0, 100, 20, 80);
            AddPolyProperty(Stroke, 0, 360, 0, 100, 0, 40);
            StrokeWidth = CreateProperty("StrokeWidth", 0, 5, 0, 10);
            AddPolyProperty(Position, left, left + width, top, top + height);
            Rotation = CreateProperty("Rotation", 0, 360, 0, 360);
            Points = CreateProperty("Points", 3, 8, 0, 15, 3);

            _sampleCounter.SetValue(20);

            CreateDomainMaps();
            Update();
        }
        private void CreateDomainMaps()
        {
            _numbers.Clear();

            _numbers.Add(Radius);
            _numbers.Add(RadiusOffset);
            _numbers.Add(Fill.Hue);
            _numbers.Add(Fill.Saturation);
            _numbers.Add(Fill.Lightness);
            _numbers.Add(Stroke.Hue);
            _numbers.Add(Stroke.Saturation);
            _numbers.Add(Stroke.Lightness);
            _numbers.Add(Position.X);
            _numbers.Add(Position.Y);
            _numbers.Add(StrokeWidth);
            _numbers.Add(Rotation);
            _numbers.Add(Points);

            _samples = new float[_numbers.Count][];
        }
        private Number CreateProperty(string trait, int start, int end, int min, int max, int zeroPoint = 0)
        {
            var dm = Domain.CreateDomain(trait, 1, min, max, zeroPoint, trait, false);
            var result = new Number(new Focal(start + zeroPoint, end + zeroPoint));
            dm.AddNumber(result, false);
            return result;
        }
        private void AddPolyProperty(PolyDomain pDomain, params long[] startEndPairs)
        {
            //var result = new Number(new Focal(start + domain.BasisFocal.StartPosition, end + domain.BasisFocal.StartPosition));
            pDomain.AddPosition(startEndPairs);
            //return result;
        }
        public void Update()
        {
            var countDirty = IsDirty;
            for (int i = 0; i < _numbers.Count; i++)
            {
                if(countDirty || _numbers[i].IsDirty)
                {
                    _samples[i] = Resample(_numbers[i]);
                    _pathsDirty = true;
                }
            }

            if (_pathsDirty)
            {
                GeneratePaths();
            }

            ClearDirty();
        }
        private void ClearDirty()
        {
            IsDirty = false;
            foreach (var num in _numbers)
            {
                num.IsDirty = false;
            }
        }
        private Random _rnd = new Random();
        public float[] Resample(Number source)
        {
            var result = new float[SampleCount];
            for (int i = 0; i < SampleCount; i++)
            {
                var val = source.ValueInRenderPerspective.SampleRandom(_rnd);
                result[i] = (float)Math.Round(val);
            }
            return result;
        }


        private void GeneratePaths()
        {
            if (_pathsDirty || IsDirty)
            {
                if (IsDirty)
                {
                    _paths = new SKPath[SampleCount];
                    _strokes = new SKPaint[SampleCount];
                    _fills = new SKPaint[SampleCount];
                    for (int i = 0; i < SampleCount; i++)
                    {
                        _paths[i] = new SKPath();
                    }
                    _lastSampleCount = SampleCount;
                }
                for (int i = 0; i < SampleCount; i++)
                {
                    var pts = SKPathMapper.GenerateStar(_samplesX[i], _samplesY[i], _samplesRadius[i], _samplesRadius[i], (int)_samplesPoints[i], _samplesRadiusOffset[i] / 100f);
                    _paths[i].Reset();
                    _paths[i].AddPoly(pts);
                    _fills[i] = CorePens.GetBrush(SKColor.FromHsl(_samplesHue[i], _samplesSaturation[i], _samplesLightness[i]));
                    _strokes[i] = CorePens.GetPen(SKColor.FromHsl(_samplesStrokeHue[i], _samplesStrokeSaturation[i], _samplesStrokeLightness[i]), _samplesStrokeWidth[i]);
                }
            }
            _pathsDirty = false;
        }
        public override void Draw()
        {
            Update();
            if (_paths != null && _paths.Length == SampleCount)
            {
                for (int i = 0; i < SampleCount; i++)
                {
                    Renderer.Canvas.DrawPath(_paths[i], _fills[i]);
                    Renderer.Canvas.DrawPath(_paths[i], _strokes[i]);
                }
            }
        }
    }
}
