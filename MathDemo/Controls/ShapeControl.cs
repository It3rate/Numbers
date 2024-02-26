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

        private List<IMathElement> _elements = new List<IMathElement>();

        private float[] _samplesRadius;
        private float[] _samplesRadiusOffset;
        private float[] _samplesHue;
        private float[] _samplesSaturation;
        private float[] _samplesLightness;
        private float[] _samplesStrokeHue;
        private float[] _samplesStrokeSaturation;
        private float[] _samplesStrokeLightness;
        private float[] _samplesStrokeWidth;
        private float[] _samplesX;
        private float[] _samplesY;
        private float[] _samplesRotation;
        private float[] _samplesPoints;

        private bool _pathsDirty = true;
        private SKPath[] _paths;
        private SKPaint[] _strokes;
        private SKPaint[] _fills;


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

            _elements.AddRange(new IMathElement[] { Radius, RadiusOffset, Fill, Stroke, Position, StrokeWidth, Rotation, Rotation, Points } );

            _sampleCounter.SetValue(20);

            Update();
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
            if (IsDirty || Radius.IsDirty)
            {
                _samplesRadius = Resample(Radius);
                _pathsDirty = true;
            }
            if (IsDirty || RadiusOffset.IsDirty)
            {
                _samplesRadiusOffset = Resample(RadiusOffset);
                _pathsDirty = true;
            }
            if (IsDirty || Fill.Hue.IsDirty)
            {
                _samplesHue = Resample(Fill.Hue);
                _pathsDirty = true;
            }
            if (IsDirty || Fill.Saturation.IsDirty)
            {
                _samplesSaturation = Resample(Fill.Saturation);
                _pathsDirty = true;
            }
            if (IsDirty || Fill.Lightness.IsDirty)
            {
                _samplesLightness = Resample(Fill.Lightness);
                _pathsDirty = true;
            }
            if (IsDirty || Stroke.Hue.IsDirty)
            {
                _samplesStrokeHue = Resample(Stroke.Hue);
                _pathsDirty = true;
            }
            if (IsDirty || Stroke.Saturation.IsDirty)
            {
                _samplesStrokeSaturation = Resample(Stroke.Saturation);
                _pathsDirty = true;
            }
            if (IsDirty || Stroke.Lightness.IsDirty)
            {
                _samplesStrokeLightness = Resample(Stroke.Lightness);
                _pathsDirty = true;
            }
            if (IsDirty || StrokeWidth.IsDirty)
            {
                _samplesStrokeWidth = Resample(StrokeWidth);
                _pathsDirty = true;
            }
            if (IsDirty || Position.X.IsDirty)
            {
                _samplesX = Resample(Position.X);
                _pathsDirty = true;
            }
            if (IsDirty || Position.Y.IsDirty)
            {
                _samplesY = Resample(Position.Y);
                _pathsDirty = true;
            }
            if (IsDirty || Rotation.IsDirty)
            {
                _samplesRotation = Resample(Rotation);
                _pathsDirty = true;
            }
            if (IsDirty || Points.IsDirty)
            {
                _samplesPoints = Resample(Points);
                _pathsDirty = true;
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
            foreach (var element in _elements)
            {
                element.IsDirty = false;
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
