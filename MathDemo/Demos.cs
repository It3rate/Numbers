using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Numbers;
using Numbers.Agent;
using Numbers.Commands;
using Numbers.Mappers;
using Numbers.Renderer;
using Numbers.Utils;
using NumbersAPI.CommandEngine;
using NumbersCore.Primitives;
using NumbersCore.Utils;

namespace MathDemo
{
	public class Demos : IDemos
    {
	    private Brain Brain { get; }
        private CoreRenderer Renderer { get; }
        private MouseAgent _currentMouseAgent;

        public Demos(Brain brain, CoreRenderer renderer)
        {
	        Brain = brain;
	        Renderer = renderer;
        }

        private int _testIndex = 4;
        private int _prevIndex = -1;
        private readonly int[] _tests = new int[] { 0, 1, 2, 3, 4 };
        public SKWorkspaceMapper NextTest(MouseAgent mouseAgent, bool isReload = false)
        {
            if(isReload && _prevIndex != -1)
            {
                _testIndex = _prevIndex;
            }
            _currentMouseAgent = mouseAgent;
            _currentMouseAgent.IsPaused = true;
            _currentMouseAgent.ClearAll();

            SKWorkspaceMapper wm;
            switch (_tests[_testIndex])
            {
                case 0:
                    wm = test2DMult();
                    break;
                case 1:
                    wm = testOneLine();
                    break;
                case 2:
                    wm = test2();
                    break;
                case 3:
                    wm = test3();
                    break;
                default:
                    wm = testMult();
                    break;
            }
            _prevIndex = _testIndex;
            _testIndex = _testIndex >= _tests.Length - 1 ? 0 : _testIndex + 1;

            wm.EnsureRenderers();
            _currentMouseAgent.IsPaused = false;
            return wm;
        }
        public SKWorkspaceMapper Reload(MouseAgent mouseAgent)
        {
            return NextTest(mouseAgent, true);
        }
        private SKWorkspaceMapper test2DMult()
        {
            var hDomain = Domain.CreateDomain("test2DMult", 100, 10);
            var vDomain = Domain.CreateDomain("test2DMult", 100, 10);
            var hNum = hDomain.CreateNumberFromFloats(2, 9);
            var vNum = vDomain.CreateNumberFromFloats(3, 6);

            Transform transform = Brain.AddTransform(hNum, vNum, TransformKind.Multiply);

            var wm = new SKWorkspaceMapper(_currentMouseAgent, 300, 0, 600, 600);
            wm.AddHorizontal(hDomain);
            wm.AddVertical(vDomain);

            CreateSimilarDomain(hDomain, 1f, 20, hNum.Focal);
            CreateSimilarDomain(hDomain, 1.08f, 20, vNum.Focal);
            CreateSimilarDomain(hDomain, 1.2f, 100, transform.Result.Focal);

            return wm;
        }

        private SKWorkspaceMapper testMult()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            var hDomain = CreateLowResDomain(4, 0);// Domain.CreateDomain("test0", unitSize, 16);
            var vDomain = CreateLowResDomain(4, .1f);// Domain.CreateDomain("test0", unitSize, 16);
            var mDomain = CreateLowResDomain(4, .3f);
            mDomain.OffsetNumbers = true;

            vDomain.BasisNumber.Focal = hDomain.BasisNumber.Focal;
            mDomain.BasisNumber.Focal = hDomain.BasisNumber.Focal;

            //var hNum2 = hDomain.CreateNumberFromFloats(1.75f, -3);
            //hNum2.Number.Polarity = Alignment.Inverted;
            //var vNum = vDomain.CreateNumberFromFloats(1.75f, -3);
            //vDomain.FlipPerspective();

            var hNum = hDomain.CreateNumberFromFloats(0, -1);
            hNum.Number.Polarity = Polarity.Inverted;

            var vNum = vDomain.CreateNumberFromFloats(0, 1.25f);

            Transform transform = Brain.AddTransform(hNum.Number, vNum.Number, TransformKind.Add);
            var tm = wm.GetOrCreateTransformMapper(transform);
            tm.DoRender = false;
            var mNum = mDomain.Domain.AddNumber(transform.Result);

            wm.Workspace.AddDomains(true, hDomain.Domain, vDomain.Domain, mDomain.Domain);
            //var speed = 0f;
            //var scale = 1f/1900f;
            //var l = 0f;
            //var r = 1f;
            //for (int i = 0; i < unitSize; i++)
            //{
            //    var len = (float)Math.Sqrt(l * l + r * r);

            //    var rr = Math.Abs(r) / Math.Abs(r - l);
            //    var lr = Math.Abs(l) / Math.Abs(r - l);
            //    //var lr = ratio <= 0 ? Math.Abs(ratio) : (1f - ratio);
            //    //var rr = ratio <= 0 ? Math.Abs(ratio + 1f) : (ratio);
            //    var dl = len * lr;
            //    var dr = len * rr;
            //    speed += (dr - dl);

            //    //Trace.WriteLine(speed);
            //    r -= speed * scale;
            //    l -= speed * scale;
            //    //speed += (float)Math.Sqrt(r * r) * scale;// (dr - dl);
            //    //r -= 0.01f;//speed * scale;// len * scale;// 
            //    //l -= 0.01f;//speed * scale;// len * scale;//
            //}

            return wm;
        }

        private SKWorkspaceMapper test3()
        {
	        Trait trait = Trait.CreateIn(Brain, "test3");
	        long unitSize = 64;
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 20, 20, 1000, 400);
            var guideline = new SKSegment(100,100,700,100);
            var unitSeg = guideline.SegmentAlongLine(0.4f, 0.6f);
            var dc = new AddSKDomainCommand(trait, 0, unitSize, -800, 800, guideline, unitSeg);
            _currentMouseAgent.Stack.Do(dc);
            var num = new AddSKNumberCommand(dc.DomainMapper, new Range(-1.5, 2.4));
            _currentMouseAgent.Stack.Do(num);
            num = new AddSKNumberCommand(dc.DomainMapper, new Range(.5, -1.2));
            _currentMouseAgent.Stack.Do(num);

            var guideline2 = new SKSegment(100, 200, 700, 200);
            var dc2 = new AddSKDomainCommand(trait, 0, unitSize, -800, 800, guideline2, unitSeg);
            _currentMouseAgent.Stack.Do(dc2);
            var num2 = new AddSKNumberCommand(dc2.DomainMapper, new Range(-1.2, -1.4));
            num2.DefaultDelay = -600;
            _currentMouseAgent.Stack.Do(num2);

            var guideline3 = new SKSegment(100, 300, 700, 300);
            var dc3 = new AddSKDomainCommand(trait, 0, unitSize, -800, 800, guideline3, unitSeg);
            _currentMouseAgent.Stack.Do(dc3);
            var numSet = new NumberSet(dc3.Domain.MinMaxNumber, new[] { new Focal(5, 20), new Focal(-20, -10), new Focal(-40, -30) });
            dc3.CreatedDomain.AddNumberSet(numSet);

            return wm;
        }

        private SKWorkspaceMapper test2()
        {
	        Trait trait = Trait.CreateIn(Brain, "test2");
            var unitSize = 10;
            var unit = new Focal(0, unitSize);
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 10, 50, 1200, 600);
            var domains = CreateDomainLines((MouseAgent)_currentMouseAgent, trait, unit, 15, 10, -40, -30, 35, 24, 4, -13);
            var d2 = domains[2];
            var d1 = domains[1];
            var d1n2 = d1.GetNumber(d1.NumberIds()[2]);
            var nn =d2.CreateNumber(d1n2.Focal);
            _currentMouseAgent.Workspace.AddElements(nn);
            return wm;
        }
        private SKWorkspaceMapper testOneLine()
        {
	        Trait trait = Trait.CreateIn(Brain, "testOneLine");
            var unitSize = 4;
            var basis = new Focal(3, 3 + unitSize);
            var range = new Focal(-40, 40);
            var domain = trait.AddDomain(basis, range);
            domain.BasisNumber.Polarity = Polarity.Inverted;
            //var domain2 = t0.AddDomain(unit.Id, range.Id);
            var val2 = new Focal(-15, 30);
            //var val3 = new Focal(t0, -40, 60);
            //var val2 = new Focal(t0, unitSize, unitSize);
            //var val3 = new Focal(t0, unitSize, unitSize);

            var num2 = domain.CreateNumber(val2);
            //var num3 = new Number(domain2, val3.Id);
            //var sel = new Source(num2);
            //var transform = t0.AddTransform(sel, num3, TransformKind.Blend);

            _currentMouseAgent.Workspace.AddDomains(true, domain);//, domain2);
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 20, 20, 800, 800);
            var dm = wm.GetOrCreateDomainMapper(domain, wm.GetHorizontalSegment(.3f, 100));
            dm.ShowGradientNumberLine = true;
            dm.OffsetNumbers = true;
            dm.ShowBasisMarkers = true;
            dm.ShowBasis = true;
            wm.EnsureRenderers();
            //var nm = dm.NumberMapper(num2.Id);
            //dm.EndPoint += new SKPoint(0, -50);
            return wm;
        }

        private SKDomainMapper CreateLowResDomain(int rangeSize, float offset, params Focal[] focals)
        {
            var newDomain = Domain.CreateDomain("lowres", 4, rangeSize);
            foreach (var focal in focals)
            {
                newDomain.CreateNumber(focal);
            }
            var wm = _currentMouseAgent.WorkspaceMapper;
            var result = wm.AddDomain(newDomain, offset, true);
            result.ShowGradientNumberLine = true;
            result.ShowBasis = true;
            result.ShowBasisMarkers = true;
            result.ShowMinorTicks = true;
            return result;
        }
        private SKDomainMapper CreateSimilarDomain(Domain domain, float offset, int rangeSize, params Focal[] focals)
        {
            var newDomain = Domain.CreateDomain(domain.Trait.Name, (int)domain.BasisFocal.LengthInTicks, rangeSize);
            foreach (var focal in focals)
            {
                newDomain.CreateNumber(focal);
            }
            var result = _currentMouseAgent.WorkspaceMapper.AddDomain(newDomain, offset, true, -200);
            result.ShowBasis = true;
            result.ShowBasisMarkers = true;
            result.ShowMinorTicks = false;
            return result;
        }
        /// <summary>
        /// Create lines with focal pairs.
        /// </summary>
        private List<Domain> CreateDomainLines(MouseAgent mouseAgent, Trait trait, Focal basisFocal, params long[] focalPositions)
        {
	        var result = new List<Domain>();
	        var wm = mouseAgent.WorkspaceMapper;
	        var padding = 1.4;
	        long maxPos = (long)Math.Max((focalPositions.Max() * padding), basisFocal.AbsLengthInTicks * padding);
	        long minPos = (long)Math.Min((focalPositions.Min() * padding), -basisFocal.AbsLengthInTicks * padding);
	        var range = new Focal(minPos, maxPos);
	        var rangeLen = (double)range.LengthInTicks;
	        var yt = 0.1f;
	        var ytStep = (float)(0.8 / Math.Floor(focalPositions.Length / 2.0));
		    //var domain = trait.AddDomain(basisFocal, range);
		    ////domain.BasisIsReciprocal = true;
		    //result.Add(domain);
	        for (int i = 1; i < focalPositions.Length; i += 2)
	        {
		        var focal = new Focal(focalPositions[i - 1], focalPositions[i]);

		        var domain = trait.AddDomain(basisFocal.Clone(), focal); // clone to avoid duplicate focals.
		        //domain.BasisIsReciprocal = true;
		        result.Add(domain);

		        var num = domain.CreateNumber(focal);
		        mouseAgent.Workspace.AddDomains(true, domain);
		        var displaySeg = wm.GetHorizontalSegment(yt, 100);
		        var y = displaySeg.StartPoint.Y;

		        var sz = domain.BasisIsReciprocal ? 0.01f : 0.05f;
		        var unitSeg = displaySeg.SegmentAlongLine(0.5f - sz, 0.5f + sz);
		        //var unitSeg = new SKSegment((float)unitStart, y, (float)unitStart + 20f, y);
                var dm = wm.GetOrCreateDomainMapper(domain, displaySeg, unitSeg);
		        dm.ShowGradientNumberLine = true;
		        dm.OffsetNumbers = true;
		        dm.ShowBasisMarkers = true;
		        dm.ShowBasis = true;
		        yt += ytStep;
	        }

	        return result;
        }

    }
}
