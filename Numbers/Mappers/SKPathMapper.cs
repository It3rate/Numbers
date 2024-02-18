﻿namespace Numbers.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
        public SKPathMapper(MouseAgent agent, SKSegment guideline = null) : base(agent, new Polyline2DDomain(500), guideline)
        {
        }
        //public SKPathMapper(MouseAgent agent, PolyNumberChain numberSet, SKSegment xBasis) : base(agent, numberSet, xBasis)
        //{
        //}

        public override SKPath GetHighlightAt(Highlight highlight)
        {
            throw new NotImplementedException();
        }
        private List<SKPoint> _points = new List<SKPoint>();
        private SKPoint _lastPoint;

        public void BeginRecord()
        {
            _points.Clear();
        }
        public void EndRecord()
        {
            SmoothPositions();
        }
        public void AddPosition(SKPoint point)
        {
            if(_points.Count == 0)
            {
                _lastPoint = point;
            }
            else
            {
                PolylineDomain.AddPosition((long)_lastPoint.X, (long)_lastPoint.Y, (long)point.X, (long)point.Y);
                _lastPoint = point;
            }
            _points.Add(point);
        }
        // maybe smoothing should be in skia world as it happens on points not segments?
        public void SmoothPositions()
        {
            var positions = PolylineDomain.GetContiguousPositions();
            var points = PositionsToPoints(positions);
            points = DouglasPeuckerReduction(points);
            positions = PointsToPositions(points);
            PolylineDomain.ResetWithContiguousPositions(positions);
        }
        private SKPoint[] PositionsToPoints(long[] positions)
        {
            var result = new SKPoint[positions.Length / 2];
            for (int i = 0; i < positions.Length; i += 2)
            {
                result[i] = new SKPoint(positions[i], positions[i + 1]);
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
        public SKPoint[] DouglasPeuckerReduction(SKPoint[] points, double tolerance = 0.5)
        {
            var result = points;
            if (points.Length > 3)
            {
                int firstPoint = 0;
                int lastPoint = points.Length - 1;
                var pointIndexsToKeep = new List<int>() { firstPoint, lastPoint };

                while (points[firstPoint].Equals(points[lastPoint]) && lastPoint > firstPoint)
                {
                    lastPoint--;
                }

                DPReduction(points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);

                pointIndexsToKeep.Sort();
                points = pointIndexsToKeep.Select(index => points[index]).ToArray();
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
            var pen = Pens.NumberLinePen;
            Renderer.DrawPolyline(pen, _points.ToArray());
        }
    }
}
