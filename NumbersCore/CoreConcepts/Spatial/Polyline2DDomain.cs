namespace NumbersCore.CoreConcepts.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.Primitives;

    public class Polyline2DDomain : SpatialDomain
    {
        private NumberChain _x;
        private NumberChain _y;
        public PolyNumberChain XYValues { get; }
        public Polyline2DDomain(Focal basisFocal, Focal maxFocal) : base(basisFocal, maxFocal, "Polyline2D")
        {
        }
        public void AddPosition(Number x, Number y)
        {
            XYValues.AddPosition(x, y);
        }
        public void AddPosition(Focal x, Focal y)
        {
            XYValues.AddPosition(x, y);
        }
        public void AddPosition(long xi, long xr, long yi, long yr)
        {
            XYValues.AddPosition(xi, xr, yi, yr);
        }
        public void AddIncrementalPosition(long x, long y)
        {
            XYValues.AddIncrementalPosition(x, y);
        }
        public (Number,Number) NumbersAt(int index)
        {
            var result =  XYValues.NumbersAt(index);
            return (result[0], result[1]);
        }
        public (Focal, Focal) FocalsAt(int index)
        {
            var result = XYValues.FocalsAt(index);
            return (result[0], result[1]);
        }
        public override Number CreateDefaultNumber(bool addToStore = true)
        {
            var num = new Number(new Focal(0, 1));
            return AddNumber(num, addToStore);
        }
        public float[][] GetContiguousValues() => XYValues.GetContiguousValues();
        public void ResetWithContiguousValues(IEnumerable<float> positions) => XYValues.ResetWithContiguousValues(positions);
        public long[] GetContiguousPositions() => XYValues.GetContiguousPositions();
        public void ResetWithContiguousPositions(IEnumerable<long> positions) => XYValues.ResetWithContiguousPositions(positions);
        public void SmoothPositions()
        {
            var positions = GetContiguousPositions();
            var smoothedPositions = new List<long>();
            // smooth
            ResetWithContiguousPositions(smoothedPositions);
        }
        //        public SKPoint[] DouglasPeuckerReduction(SKPoint[] points, double tolerance = 0.5)
        //        {
        //            var result = points;
        //            if (points.Length > 3)
        //            {
        //                int firstPoint = 0;
        //                int lastPoint = points.Length - 1;
        //                var pointIndexsToKeep = new List<int>() { firstPoint, lastPoint };

        //                while (points[firstPoint].Equals(points[lastPoint]) && lastPoint > firstPoint)
        //                {
        //                    lastPoint--;
        //                }

        //                DPReduction(points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);

        //                pointIndexsToKeep.Sort();
        //                points = pointIndexsToKeep.Select(index => points[index]).ToArray();
        //            }
        //            return points;
        //        }
        private void DPReduction(float[] points, Int32 firstPoint, Int32 lastPoint, Double tolerance, ref List<Int32> pointIndexesToKeep)
        {
            //double maxDistance = 0;
            //int indexFarthest = 0;

            //for (int index = firstPoint; index < lastPoint; index += 2)
            //{
            //    var distance = points[index].DistanceToLine(points[firstPoint], points[lastPoint]);
            //    if (distance > maxDistance)
            //    {
            //        maxDistance = distance;
            //        indexFarthest = index;
            //    }
            //}

            //if (maxDistance > tolerance && indexFarthest != 0)
            //{
            //    pointIndexesToKeep.Add(indexFarthest);
            //    DPReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexesToKeep);
            //    DPReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexesToKeep);
            //}
        }
        public void Reset()
        {
            foreach (var num in NumberStore.Values)
            {
            }
        }
    }
}
