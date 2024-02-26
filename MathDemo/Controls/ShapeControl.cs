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
        public Number SampleCount { get; }

        public Number Radius { get; }
        public Number RadiusOffset { get; }


        public HSLDomain Fill { get; } = new HSLDomain();
        public HSLDomain Stroke { get; } = new HSLDomain();
        public PositionDomain Position { get; } = new PositionDomain(500, 500);

        //public Number Hue { get; }
        //public Number Saturation { get; }
        //public Number Lightness { get; }
        //public Number StrokeHue { get; }
        //public Number StrokeSaturation { get; }
        //public Number StrokeLightness { get; }
        public Number StrokeWidth { get; }
        //public Number X { get; }
        //public Number Y { get; }
        public Number Rotation { get; }
        public Number Points { get; }

        private Focal _lastRadius;
        private Focal _lastRadiusOffset;
        private Focal _lastHue;
        private Focal _lastSaturation;
        private Focal _lastLightness;
        private Focal _lastStrokeHue;
        private Focal _lastStrokeSaturation;
        private Focal _lastStrokeLightness;
        private Focal _lastStrokeWidth;
        private Focal _lastX;
        private Focal _lastY;
        private Focal _lastRotation;
        private Focal _lastPoints;

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

        private bool _isDirty = true;
        private SKPath[] _paths;
        private SKPaint[] _strokes;
        private SKPaint[] _fills;

        public int _lastSampleCount = 0;
        public int _sampleCount = 20;

        public ShapeControl(MouseAgent agent, int top, int left, int width, int height) : base(agent)
        {
            Radius = CreateProperty("Radius", 25, 50, 0, 50);
            RadiusOffset = CreateProperty("RadiusOffset", 20, 100, 0, 200);

            AddPolyProperty(Fill, 0, 360, 0, 100, 20, 80);
            //AddProperty(Fill.SaturationDomain, 0, 100);
            //AddProperty(Fill.LightnessDomain, 20, 80);
            //Hue = CreateProperty("Hue", 0, 360, 0, 360);
            //Saturation = CreateProperty("Saturation", 0, 100, 0, 100);
            //Lightness = CreateProperty("Lightness", 20, 80, 0, 100);

            AddPolyProperty(Stroke, 0, 360, 0, 100, 0, 40);
            //AddProperty(Stroke.SaturationDomain, 0, 100);
            //AddProperty(Stroke.LightnessDomain, 0, 40);
            //StrokeHue = CreateProperty("StrokeHue", 0, 360, 0, 360);
            //StrokeSaturation = CreateProperty("StrokeSaturation", 0, 100, 0, 100);
            //StrokeLightness = CreateProperty("StrokeLightness", 0, 40, 0, 100);

            StrokeWidth = CreateProperty("StrokeWidth", 0, 5, 0, 10);

            AddPolyProperty(Position, left, left + width, top, top + height);
            //AddProperty(Position.YDomain, top, top + height);
            //X = CreateProperty("X", left, left + width, left, left + width);
            //Y = CreateProperty("Y", top, top + height, top, top + height);

            Rotation = CreateProperty("Rotation", 0, 360, 0, 360);
            Points = CreateProperty("Points", 3, 8, 0, 15, 3);

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
            if (Radius.Focal != _lastRadius || _sampleCount != _lastSampleCount)
            {
                _samplesRadius = Resample(Radius);
                _lastRadius = Radius.Focal.Clone();
                _isDirty = true;
            }
            if (RadiusOffset.Focal != _lastRadiusOffset || _sampleCount != _lastSampleCount)
            {
                _samplesRadiusOffset = Resample(RadiusOffset);
                _lastRadiusOffset = RadiusOffset.Focal.Clone();
                _isDirty = true;
            }
            if (Fill.Hue.Focal != _lastHue || _sampleCount != _lastSampleCount)
            {
                _samplesHue = Resample(Fill.Hue);
                _lastHue = Fill.Hue.Focal.Clone();
                _isDirty = true;
            }
            if (Fill.Saturation.Focal != _lastSaturation || _sampleCount != _lastSampleCount)
            {
                _samplesSaturation = Resample(Fill.Saturation);
                _lastSaturation = Fill.Saturation.Focal.Clone();
                _isDirty = true;
            }
            if (Fill.Lightness.Focal != _lastLightness || _sampleCount != _lastSampleCount)
            {
                _samplesLightness = Resample(Fill.Lightness);
                _lastLightness = Fill.Lightness.Focal.Clone();
                _isDirty = true;
            }
            if (Stroke.Hue.Focal != _lastStrokeHue || _sampleCount != _lastSampleCount)
            {
                _samplesStrokeHue = Resample(Stroke.Hue);
                _lastStrokeHue = Stroke.Hue.Focal.Clone();
                _isDirty = true;
            }
            if (Stroke.Saturation.Focal != _lastStrokeSaturation || _sampleCount != _lastSampleCount)
            {
                _samplesStrokeSaturation = Resample(Stroke.Saturation);
                _lastStrokeSaturation = Stroke.Saturation.Focal.Clone();
                _isDirty = true;
            }
            if (Stroke.Lightness.Focal != _lastStrokeLightness || _sampleCount != _lastSampleCount)
            {
                _samplesStrokeLightness = Resample(Stroke.Lightness);
                _lastStrokeLightness = Stroke.Lightness.Focal.Clone();
                _isDirty = true;
            }
            if (StrokeWidth.Focal != _lastStrokeWidth || _sampleCount != _lastSampleCount)
            {
                _samplesStrokeWidth = Resample(StrokeWidth);
                _lastStrokeWidth = StrokeWidth.Focal.Clone();
                _isDirty = true;
            }
            if (Position.X.Focal != _lastX || _sampleCount != _lastSampleCount)
            {
                _samplesX = Resample(Position.X);
                _lastX = Position.X.Focal.Clone();
                _isDirty = true;
            }
            if (Position.Y.Focal != _lastY || _sampleCount != _lastSampleCount)
            {
                _samplesY = Resample(Position.Y);
                _lastY = Position.Y.Focal.Clone();
                _isDirty = true;
            }
            if (Rotation.Focal != _lastRotation || _sampleCount != _lastSampleCount)
            {
                _samplesRotation = Resample(Rotation);
                _lastRotation = Rotation.Focal.Clone();
                _isDirty = true;
            }
            if (Points.Focal != _lastPoints || _sampleCount != _lastSampleCount)
            {
                _samplesPoints = Resample(Points);
                _lastPoints = Points.Focal.Clone();
                _isDirty = true;
            }


            if (_isDirty)
            {
                GeneratePaths();
            }
        }
        private Random _rnd = new Random();
        public float[] Resample(Number source)
        {
            var result = new float[_sampleCount];
            for (int i = 0; i < _sampleCount; i++)
            {
                var val = source.ValueInRenderPerspective.SampleRandom(_rnd);
                result[i] = (float)Math.Round(val);
            }
            return result;
        }
        private void GeneratePaths()
        {
            if (_isDirty || _lastSampleCount != _sampleCount)
            {
                if (_lastSampleCount != _sampleCount)
                {
                    _paths = new SKPath[_sampleCount];
                    _strokes = new SKPaint[_sampleCount];
                    _fills = new SKPaint[_sampleCount];
                    for (int i = 0; i < _sampleCount; i++)
                    {
                        _paths[i] = new SKPath();
                    }
                    _lastSampleCount = _sampleCount;
                }

                for (int i = 0; i < _sampleCount; i++)
                {
                    var pts = SKPathMapper.GenerateStar(_samplesX[i], _samplesY[i], _samplesRadius[i], _samplesRadius[i], (int)_samplesPoints[i], _samplesRadiusOffset[i] / 100f);
                    _paths[i].Reset();
                    _paths[i].AddPoly(pts);
                    _fills[i] = CorePens.GetBrush(SKColor.FromHsl(_samplesHue[i], _samplesSaturation[i], _samplesLightness[i]));
                    _strokes[i] = CorePens.GetPen(SKColor.FromHsl(_samplesStrokeHue[i], _samplesStrokeSaturation[i], _samplesStrokeLightness[i]), _samplesStrokeWidth[i]);
                }
            }
            _isDirty = false;
        }
        public override void Draw()
        {
            Update();
            if (_paths != null && _paths.Length == _sampleCount)
            {
                for (int i = 0; i < _sampleCount; i++)
                {
                    Renderer.Canvas.DrawPath(_paths[i], _fills[i]);
                    Renderer.Canvas.DrawPath(_paths[i], _strokes[i]);
                }
            }
        }
    }
}
