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
        public ExtentDomain PolylineDomain => (ExtentDomain)MathElement;

        private List<SKPoint> _points = new List<SKPoint>();
        private SKPoint _lastPoint;
        private SKPoint[] _smoothPoints;
        private bool _isShape = false;
        private bool _pathDirty = false;
        public int Count => PolylineDomain.Count;
        public SKPaint Pen { get; set; }

        public SKPathMapper(MouseAgent agent, SKSegment guideline = null) : base(agent, new ExtentDomain(500, 500), guideline)
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
            var starPoints = GenerateStar(center.X, center.Y, center.X - p0.X, center.Y - p0.Y, points, 0.5f);
            foreach (var pt in starPoints)
            {
                AddPosition(pt.X, pt.Y);
            }
            _pathDirty = true;
        }
        public static SKPoint[] GenerateStar(float xCenter, float yCenter, float xRadius, float yRadius, int points, float innerRatio = 0.5f)
        {
            var num = points == -1 ? 5 * 2 : points * 2; // star points
            num = num < 0 ? 0 : num;
            var step = MathF.PI * 2 / num;
            var result = new SKPoint[num + 1]; // + 1 is link back to start point
            for (int i = 0; i < num + 1; i++)
            {
                var curXr = i % 2 == 1 ? xRadius : xRadius * innerRatio;
                var curYr = i % 2 == 1 ? yRadius : yRadius * innerRatio;
                var x = (float)(xCenter + Math.Sin(i * step) * curXr);
                var y = (float)(yCenter + Math.Cos(i * step) * curYr);
                result[i] = new SKPoint(x, y);
            }
            return result;
        }
        public static SKPath GenerateArcOval(SKRect bounds, float roundness, float convexity)
        {
            SKPath path = new SKPath();
            //roundness -= 1;// Math.Max(0, Math.Min(1, roundness));
            //convexity -= 1;// = Math.Max(0, Math.Min(1, convexity));
            float width = bounds.Width;

            //outerRect.Left -= roundness * bounds.Height / 2f;
            //outerRect.Right += roundness * bounds.Height / 2f;

            float startAngle = 90;
            float sweepAngle = 180;
            var outerRect = bounds;
            var skew = Math.Abs(convexity) * (bounds.Width / 2f);
            outerRect.Left -= skew / 4f;
            outerRect.Right -= skew / 4f;
            SKPoint center = new SKPoint(outerRect.MidX, outerRect.MidY);
            path.AddArc(outerRect, startAngle, sweepAngle);


            var innerRect = outerRect;

            innerRect.Left = center.X - skew;
            innerRect.Right = center.X + skew;
            //sweepAngle = Math.Sign(convexity) < 0 ? -1f - sweepAngle : sweepAngle;
            path.AddArc(innerRect, startAngle + sweepAngle, sweepAngle * Math.Sign(convexity));

            //path.Close();

            return path;
        }
        //    // Clamp the values to ensure they're within the expected range
        //    roundness = (roundness < 0) ? 0 : (roundness > 1) ? 1 : roundness;
        //    convexity = (convexity < 0) ? 0 : (convexity > 1) ? 1 : convexity;

        //    // Calculate control points for Bezier curves based on roundness and convexity
        //    float width = bounds.Width;
        //    float height = bounds.Height;

        //    // Adjust the height for roundness
        //    float adjustedHeight = height * roundness;

        //    // Calculate control points for convexity
        //    float convexityOffset = width * (1 - convexity) / 2;

        //    SKPath path = new SKPath();

        //    // Start at the bottom center
        //    path.MoveTo(bounds.MidX, bounds.Bottom);

        //    // Bottom arc
        //    SKPoint controlPoint1Bottom = new SKPoint(bounds.Left + convexityOffset, bounds.Bottom - adjustedHeight);
        //    SKPoint controlPoint2Bottom = new SKPoint(bounds.Left + convexityOffset, bounds.Top + adjustedHeight);
        //    SKPoint endPointBottom = new SKPoint(bounds.MidX, bounds.Top);
        //    path.CubicTo(controlPoint1Bottom, controlPoint2Bottom, endPointBottom);

        //    // Top arc
        //    SKPoint controlPoint1Top = new SKPoint(bounds.Right - convexityOffset, bounds.Top + adjustedHeight);
        //    SKPoint controlPoint2Top = new SKPoint(bounds.Right - convexityOffset, bounds.Bottom - adjustedHeight);
        //    SKPoint endPointTop = new SKPoint(bounds.MidX, bounds.Bottom);
        //    path.CubicTo(controlPoint1Top, controlPoint2Top, endPointTop);

        //    path.Close();

        //    return path;
        //}

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

        public override void Draw()
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
