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

        public int _lastSampleCount = 10;
        public int _sampleCount = 10;

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

        private bool _isDirty = true;
        private List<SKPath> _paths = new List<SKPath>();
        private List<SKPaint> _outlines = new List<SKPaint>();
        private List<SKPaint> _fills = new List<SKPaint>();

        public ShapeControl(MouseAgent agent) : base(agent)
        {
            Radius = new Number(new Focal(0, 255));
            RadiusOffset = new Number(new Focal(0, 255));
            Hue = new Number(new Focal(0, 360));
            Saturation = new Number(new Focal(0, 100));
            Lightness = new Number(new Focal(0, 100));
            StrokeHue = new Number(new Focal(0, 360));
            StrokeSaturation = new Number(new Focal(0, 100));
            StrokeLightness = new Number(new Focal(0, 100));
            StrokeWidth = new Number(new Focal(0, 50));
            X = new Number(new Focal(0, 1200));
            Y = new Number(new Focal(0, 800));
            Rotation = new Number(new Focal(0, 360));

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


            _lastSampleCount = _sampleCount;
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
            _isDirty = false;
        }
        public override void Draw()
        {

        }
    }
}
