namespace Numbers.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Numbers.Agent;
    using Numbers.Drawing;
    using Numbers.Utils;
    using NumbersCore.CoreConcepts.Spatial;
    using NumbersCore.Primitives;
    using SkiaSharp;

    public class SKPathMapper : SKMapper
    {
        public Polyline2DDomain PolylineDomain => (Polyline2DDomain)MathElement;

        private List<SKPoint> _points = new List<SKPoint>();
        private SKPoint _lastPoint;
        private SKPoint[] _smoothPoints;
        private bool _isShape = false;
        private bool _pathDirty = false;
        public int Count => PolylineDomain.XYValues.Count;
        public SKPaint Pen { get; set; }

        public SKPathMapper(MouseAgent agent, SKSegment guideline = null) : base(agent, new Polyline2DDomain(500), guideline)
        {
            Pen = Pens.DrawPen;
        }
        //public SKPathMapper(MouseAgent agent, PolyNumberChain numberSet, SKSegment xBasis) : base(agent, numberSet, xBasis)
        //{
        //}

        public override SKPath GetHighlightAt(Highlight highlight)
        {
            throw new NotImplementedException();
        }

        public void BeginRecord()
        {
            Reset();
        }
        public void EndRecord()
        {
            FinalizePath();
        }
        public void AddPosition(float x, float y)
        {
            AddPosition(new SKPoint(x, y));
        }
        public void AddPosition(double x, double y)
        {
            AddPosition(new SKPoint((float)x, (float)y));
        }
        public void AddPosition(SKPoint point)
            {
                if (_points.Count == 0)
            {
                _lastPoint = point;
            }
            else
            {
                PolylineDomain.AddPosition((long)_lastPoint.X, (long)_lastPoint.Y, (long)point.X, (long)point.Y);
                _lastPoint = point;
            }
            _points.Add(point);

            if ((_points.Count > 0) && (_points.Count % 32 == 0))
            {
                //SmoothPositions();
            }
        }

        public void SetRect(SKPoint p0, SKPoint p1)
        {
            Reset();
            _isShape = true;
            AddPosition(p0.X, p0.Y);
            AddPosition(p1.X, p0.Y);
            AddPosition(p1.X, p1.Y);
            AddPosition(p0.X, p1.Y);
            AddPosition(p0.X, p0.Y);
            _pathDirty = true;
        }

        public void SetOval(SKPoint p0, SKPoint p1)
        {
            Reset();
            _isShape = true;
            var center = p0.Midpoint(p1);
            var xr = center.X - p0.X;
            var yr = center.Y - p0.Y;
            var num = (int)(p0.UnsignedDistanceTo(p1) * .8); // steps in polyline
            var step = MathF.PI * 2 / num;
            for (int i = 0; i < num + 1; i++)
            {
                AddPosition(center.X + Math.Sin(i * step) * xr, center.Y + Math.Cos(i * step) * yr);
            }
            _pathDirty = true;
        }
        public void SetStar(SKPoint p0, SKPoint p1, int points = -1)
        {
            Reset();
            _isShape = true;
            var center = p0.Midpoint(p1);
            var xr = center.X - p0.X;
            var yr = center.Y - p0.Y;
            var innerRatio = 0.5f;
            var num = points == -1 ? 5 * 2 : points * 2; // star points
            var step = MathF.PI * 2 / num;
            for (int i = 0; i < num + 1; i++)
            {
                var curXr = i % 2 == 1 ? xr : xr * innerRatio;
                var curYr = i % 2 == 1 ? yr : yr * innerRatio;
                AddPosition(center.X + Math.Sin(i * step) * curXr, center.Y + Math.Cos(i * step) * curYr);
            }
            _pathDirty = true;
        }

        // maybe smoothing should be in skia world as it happens on points not segments?
        public void SmoothPositions()
        {
            if (!_isShape)
            {
                var positions = PolylineDomain.GetContiguousPositions();
                var pts = PositionsToPoints(positions);
                _smoothPoints = DouglasPeuckerReduction(pts);
            }
        }
        public void FinalizePath()
        {
            SmoothPositions();
            var positions = PointsToPositions(_smoothPoints);
            PolylineDomain.ResetWithContiguousPositions(positions);
        }
        private SKPoint[] PositionsToPoints(long[] positions)
        {
            var result = new SKPoint[positions.Length / 2];
            var j = 0;
            for (int i = 0; i < positions.Length; i += 2)
            {
                result[j++] = new SKPoint(positions[i], positions[i + 1]);
            }
            return result;
        }
        private long[] PointsToPositions(SKPoint[] points)
        {
            var result = new long[points.Length * 2];
            var j = 0;
            for (int i = 0; i < points.Length; i++)
            {
                result[j++] = (long)points[i].X;
                result[j++] = (long)points[i].Y;
            }
            return result;
        }
        public SKPoint[] DouglasPeuckerReduction(SKPoint[] points, double tolerance = 1.2)
        {
            var result = points;
            if (points.Length > 3)
            {
                int firstPoint = 0;
                int lastPoint = points.Length - 1;
                var pointIndexesToKeep = new List<int>() { firstPoint, lastPoint };

                while (points[firstPoint].Equals(points[lastPoint]) && lastPoint > firstPoint)
                {
                    lastPoint--;
                }

                DPReduction(points, firstPoint, lastPoint, tolerance, ref pointIndexesToKeep);

                pointIndexesToKeep.Sort();
                points = pointIndexesToKeep.Select(index => points[index]).ToArray();
            }
            return points;
        }
        private void DPReduction(SKPoint[] points, Int32 firstPoint, Int32 lastPoint, Double tolerance, ref List<Int32> pointIndexesToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            for (int index = firstPoint; index < lastPoint; index += 2)
            {
                var distance = points[index].DistanceToLine(points[firstPoint], points[lastPoint]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                pointIndexesToKeep.Add(indexFarthest);
                DPReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexesToKeep);
                DPReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexesToKeep);
            }
        }

        public void Draw()
        {
            SKPoint[] pts = _smoothPoints != null ? _smoothPoints : _points.ToArray();
            Renderer.DrawPolyline(Pen, pts);
        }

        public void Reset()
        {
            _points.Clear();
            _smoothPoints = null;
            _isShape = false;
        }
    }
}
