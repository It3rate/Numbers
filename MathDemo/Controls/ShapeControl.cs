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
        public Number Hue { get; }
        public Number Saturation { get; }
        public Number Lightness { get; }
        public Number StrokeHue { get; }
        public Number StrokeSaturation { get; }
        public Number StrokeLightness { get; }
        public Number StrokeWidth { get; }
        public Number X { get; }
        public Number Y { get; }
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
        public int _sampleCount = 30;

        public ShapeControl(MouseAgent agent, int top, int left, int width, int height) : base(agent)
        {
            Radius = new Number(new Focal(-10, 30));
            RadiusOffset = new Number(new Focal(0, 100));
            Hue = new Number(new Focal(0, 360));
            Saturation = new Number(new Focal(0, 100));
            Lightness = new Number(new Focal(0, 100));
            StrokeHue = new Number(new Focal(0, 360));
            StrokeSaturation = new Number(new Focal(0, 100));
            StrokeLightness = new Number(new Focal(0, 100));
            StrokeWidth = new Number(new Focal(0, 8));
            X = new Number(new Focal(-left, left + width));
            Y = new Number(new Focal(-top, top+height));
            Rotation = new Number(new Focal(0, 360));
            Points = new Number(new Focal(-3, 8));
            // temp
            var trait = agent.Brain.GetOrCreateTrait("shapeTrait");
            agent.Workspace.AddTraits(true, trait);
            var dm = new Domain(trait, Focal.OneFocal, Focal.MinMaxFocal, "shapeDomain");
            dm.IsVisible = false;
            agent.Workspace.AddDomains(true, dm);
            dm.AddNumber(Radius, false);
            dm.AddNumber(RadiusOffset, false);
            dm.AddNumber(Hue, false);
            dm.AddNumber(Saturation, false);
            dm.AddNumber(Lightness, false);
            dm.AddNumber(StrokeHue, false);
            dm.AddNumber(StrokeSaturation, false);
            dm.AddNumber(StrokeLightness, false);
            dm.AddNumber(StrokeWidth, false);
            dm.AddNumber(X, false);
            dm.AddNumber(Y, false);
            dm.AddNumber(Rotation, false);
            dm.AddNumber(Points, false);

            Update();
        }
        public void Update()
        {
            if (Radius.Focal != _lastRadius || _sampleCount != _lastSampleCount)
            {
                _samplesRadius = Resample(Radius);
                _isDirty = true;
            }
            if (RadiusOffset.Focal != _lastRadiusOffset || _sampleCount != _lastSampleCount)
            {
                _samplesRadiusOffset = Resample(RadiusOffset);
                _isDirty = true;
            }
            if (Hue.Focal != _lastHue || _sampleCount != _lastSampleCount)
            {
                _samplesHue = Resample(Hue);
                _isDirty = true;
            }
            if (Saturation.Focal != _lastSaturation || _sampleCount != _lastSampleCount)
            {
                _samplesSaturation = Resample(Saturation);
                _isDirty = true;
            }
            if (Lightness.Focal != _lastLightness || _sampleCount != _lastSampleCount)
            {
                _samplesLightness = Resample(Lightness);
                _isDirty = true;
            }
            if (StrokeHue.Focal != _lastStrokeHue || _sampleCount != _lastSampleCount)
            {
                _samplesStrokeHue = Resample(StrokeHue);
                _isDirty = true;
            }
            if (StrokeSaturation.Focal != _lastStrokeSaturation || _sampleCount != _lastSampleCount)
            {
                _samplesStrokeSaturation = Resample(StrokeSaturation);
                _isDirty = true;
            }
            if (StrokeLightness.Focal != _lastStrokeLightness || _sampleCount != _lastSampleCount)
            {
                _samplesStrokeLightness = Resample(StrokeLightness);
                _isDirty = true;
            }
            if (StrokeWidth.Focal != _lastStrokeWidth || _sampleCount != _lastSampleCount)
            {
                _samplesStrokeWidth = Resample(StrokeWidth);
                _isDirty = true;
            }
            if (X.Focal != _lastX || _sampleCount != _lastSampleCount)
            {
                _samplesX = Resample(X);
                _isDirty = true;
            }
            if (Y.Focal != _lastY || _sampleCount != _lastSampleCount)
            {
                _samplesY = Resample(Y);
                _isDirty = true;
            }
            if (Rotation.Focal != _lastRotation || _sampleCount != _lastSampleCount)
            {
                _samplesRotation = Resample(Rotation);
                _isDirty = true;
            }
            if (Points.Focal != _lastPoints || _sampleCount != _lastSampleCount)
            {
                _samplesPoints = Resample(Points);
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
                result[i] = source.Value.SampleRandom(_rnd);
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
