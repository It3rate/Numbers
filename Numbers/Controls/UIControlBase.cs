namespace Numbers.Controls
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Numbers.Agent;
    using Numbers.Mappers;
    using Numbers.Renderer;
    using NumbersCore.CoreConcepts.Counter;
    using NumbersCore.Primitives;
    using SkiaSharp;

    public abstract class UIControlBase : IDrawableElement
    {
        private static int _idCounter = 1; 
        public int Id { get; set; }
        public MouseAgent Agent { get; }
        public SKWorkspaceMapper WorkspaceMapper => Agent.WorkspaceMapper;
        public CoreRenderer Renderer => Agent.Renderer;
        public SKCanvas Canvas => Renderer.Canvas;

        public bool IsDirty
        {
            get => _sampleCounter.Segment.IsDirty;
            set => _sampleCounter.Segment.IsDirty = value;
        }

        public SegmentCounter _sampleCounter = new SegmentCounter(1, 50);
        public long SampleCount => _sampleCounter.Value;

        protected bool _pathsDirty = true;
        protected SKPath[] _paths;
        protected SKPaint[] _strokes;
        protected SKPaint[] _fills;
        protected List<Number> _numbers = new List<Number>();
        protected float[][] _samples;
        protected Random _rnd = new Random();

        public UIControlBase(MouseAgent agent, long count) 
        {
            Id = _idCounter++;
            Agent = agent;
            _sampleCounter.SetValue(count);
        }
        protected abstract void CreateDomainMaps();
        protected Number CreateProperty(string trait, int start, int end, int min, int max, int zeroPoint = 0)
        {
            var dm = Domain.CreateDomain(trait, 1, min, max, zeroPoint, trait, false);
            var result = new Number(new Focal(start + zeroPoint, end + zeroPoint));
            dm.AddNumber(result, false);
            return result;
        }
        protected void AddPolyProperty(PolyDomain pDomain, params long[] startEndPairs)
        {
            //var result = new Number(new Focal(start + domain.BasisFocal.StartPosition, end + domain.BasisFocal.StartPosition));
            pDomain.AddPosition(startEndPairs);
            //return result;
        }
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

        public virtual void Update()
        {
            var countDirty = IsDirty;
            for (int i = 0; i < _numbers.Count; i++)
            {
                if (countDirty || _numbers[i].IsDirty)
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
        protected virtual void GeneratePaths()
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
                }
                for (int i = 0; i < SampleCount; i++)
                {
                    GeneratePath(i);
                }
            }
            _pathsDirty = false;
        }
        protected abstract void GeneratePath(long idx);
        protected virtual void ClearDirty()
        {
            IsDirty = false;
            foreach (var num in _numbers)
            {
                num.IsDirty = false;
            }
        }

        public virtual void Draw()
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
