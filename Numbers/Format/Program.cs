using System;
using System.Collections.Generic;
using System.Linq;
using Numbers.Agent;
using Numbers.Commands;
using Numbers.Mappers;
using Numbers.Renderer;
using Numbers.Utils;
using NumbersAPI.CommandEngine;
using NumbersCore.Primitives;
using NumbersCore.Utils;

namespace Numbers.Format
{
	public class Program
    {
	    private Brain Brain { get; }
        private CoreRenderer Renderer { get; }
        private int _testIndex = 3;
        private readonly int[] _tests = new int[] { 0, 1, 2, 3 };
        public Program(Brain brain, CoreRenderer renderer)
        {
	        Brain = brain;
	        Renderer = renderer;
        }

        private SKWorkspaceMapper test3(MouseAgent mouseAgent)
        {
	        Trait trait = Trait.CreateIn(Brain, "test3");

            var wm = new SKWorkspaceMapper(mouseAgent, 20, 20, 1000, 400);
            var guideline = new SKSegment(100,100,700,100);
            var unitSeg = guideline.SegmentAlongLine(0.4f, 0.6f);
            var dc = new AddSKDomainCommand(trait, 0, 10, -800, 800, guideline, unitSeg);
            mouseAgent.Stack.Do(dc);
            var guideline2 = new SKSegment(100, 200, 700, 200);
            var dc2 = new AddSKDomainCommand(trait, 0, 10, -800, 800, guideline2, unitSeg);
            mouseAgent.Stack.Do(dc2);
            var num = new AddSKNumberCommand(dc.DomainMapper, new Range(1.5, -0.4));
            var num2 = new AddSKNumberCommand(dc2.DomainMapper, new Range(-2.2, 1.4));
            mouseAgent.Stack.Do(num, num2);
            //stack.Undo();
            return wm;
        }

        private SKWorkspaceMapper test2(Agent.MouseAgent mouseAgent)
        {
	        Trait trait = Trait.CreateIn(Brain, "test2");
            var unitSize = 10;
            var unit = Focal.CreateByValues(0, unitSize);
            var wm = new SKWorkspaceMapper(mouseAgent, 20, 20, 1000, 400);
            var domains = CreateDomainLines((Agent.MouseAgent)mouseAgent, trait, unit, 15, 10, -40, -30, 35, 24, 4, -13);
            var d2 = domains[2];
            var d1 = domains[1];
            var d1n2 = d1.GetNumber(d1.NumberIds()[2]);
            var nn =d2.CreateNumber(d1n2.Focal);
            mouseAgent.Workspace.AddElements(nn);
            return wm;
        }
        private SKWorkspaceMapper test0(Agent.MouseAgent mouseAgent)
        {
	        Trait trait = Trait.CreateIn(Brain, "test0");
            var unitSize = 8;
            var unit = Focal.CreateByValues(0, unitSize);
            var range = Focal.CreateByValues(-16 * unitSize, 16 * unitSize);
            var hDomain = trait.AddDomain(unit, range);
            var vDomain = trait.AddDomain(unit, range);
            var hFocal = Focal.CreateByValues(-2 * unitSize, 9 * unitSize);
            var vFocal = Focal.CreateByValues(3 * unitSize, 6 * unitSize);
            //var val2 = Focal.CreateByValues(t0, unitSize, unitSize);
            //var val3 = Focal.CreateByValues(t0, unitSize, unitSize);

            var hNum = hDomain.CreateNumber(hFocal);
            var vNum = vDomain.CreateNumber(vFocal);
            var hSel = new Selection(hNum);
            var transform = Brain.AddTransform(hSel, vNum, TransformKind.Blend);

            mouseAgent.Workspace.AddDomains(true, hDomain, vDomain);

            var wm = new SKWorkspaceMapper(mouseAgent, 150, 10, 800, 800);

            var dm = wm.GetOrCreateDomainMapper(hDomain, wm.GetHorizontalSegment(.5f, 50));
            dm.ShowGradientNumberLine = false;
            dm.ShowValueMarkers = true;
            dm.ShowBasisMarkers = false;
            dm.ShowBasis = false;

            var dm2 = wm.GetOrCreateDomainMapper(vDomain, wm.GetVerticalSegment(.5f, 50));
            dm2.ShowGradientNumberLine = false;
            dm2.ShowValueMarkers = true;
            dm2.ShowBasisMarkers = false;
            dm2.ShowBasis = false;
            return wm;
        }
        private SKWorkspaceMapper test1(Agent.MouseAgent mouseAgent)
        {
	        Trait trait = Trait.CreateIn(Brain, "test1");
            var unitSize = 4;
            var unit = Focal.CreateByValues(3, 3 + unitSize);
            var range = Focal.CreateByValues(-40, 40);
            var domain = trait.AddDomain(unit, range);
            //var domain2 = t0.AddDomain(unit.Id, range.Id);
            var val2 = Focal.CreateByValues(-15, 20);
            //var val3 = Focal.CreateByValues(t0, -40, 60);
            //var val2 = Focal.CreateByValues(t0, unitSize, unitSize);
            //var val3 = Focal.CreateByValues(t0, unitSize, unitSize);

            var num2 = domain.CreateNumber(val2);
            //var num3 = new Number(domain2, val3.Id);
            //var sel = new Selection(num2);
            //var transform = t0.AddTransform(sel, num3, TransformKind.Blend);

            mouseAgent.Workspace.AddDomains(true, domain);//, domain2);
            var wm = new SKWorkspaceMapper(mouseAgent, 20, 20, 800, 800);
            var dm = wm.GetOrCreateDomainMapper(domain, wm.GetHorizontalSegment(.3f, 100));
            dm.ShowGradientNumberLine = true;
            dm.ShowNumberOffsets = true;
            dm.ShowBasisMarkers = true;
            dm.ShowBasis = true;
            wm.EnsureRenderers();
            var nm = dm.NumberMapper(num2.Id);
            //dm.EndPoint += new SKPoint(0, -50);
            return wm;
        }

        public SKWorkspaceMapper NextTest(MouseAgent mouseAgent)
        {
	        mouseAgent.IsPaused = true;
	        mouseAgent.ClearAll();

	        SKWorkspaceMapper wm;
            switch (_tests[_testIndex])
            {
                case 0:
	                wm = test0(mouseAgent);
                    break;
                case 1:
	                wm = test1(mouseAgent);
	                break;
                case 2:
	                wm = test2(mouseAgent);
	                break;
                case 3:
                default:
	                wm = test3(mouseAgent);
                    break;
            }
            _testIndex = _testIndex >= _tests.Length - 1 ? 0 : _testIndex + 1;

            wm.EnsureRenderers();
            mouseAgent.IsPaused = false;
            return wm;
        }

        private List<Domain> CreateDomainLines(Agent.MouseAgent mouseAgent, Trait trait, Focal basisFocal, params long[] focalPositions)
        {
	        var result = new List<Domain>();
	        var wm = mouseAgent.WorkspaceMapper;
	        var padding = 1.4;
	        long maxPos = (long)Math.Max((focalPositions.Max() * padding), basisFocal.AbsLengthInTicks * padding);
	        long minPos = (long)Math.Min((focalPositions.Min() * padding), -basisFocal.AbsLengthInTicks * padding);
	        var range = Focal.CreateByValues(minPos, maxPos);
	        var rangeLen = (double)range.LengthInTicks;
	        var yt = 0.1f;
	        var ytStep = (float)(0.8 / Math.Floor(focalPositions.Length / 2.0));
	        for (int i = 1; i < focalPositions.Length; i += 2)
	        {
		        var domain = trait.AddDomain(basisFocal, range);
		        //domain.BasisIsReciprocal = true;
		        result.Add(domain);
		        var focal = Focal.CreateByValues(focalPositions[i - 1], focalPositions[i]);
		        var num = domain.CreateNumber(focal);
		        mouseAgent.Workspace.AddDomains(true, domain);
		        var displaySeg = wm.GetHorizontalSegment(yt, 100);
		        var y = displaySeg.StartPoint.Y;

		        var sz = domain.BasisIsReciprocal ? 0.01f : 0.05f;
		        var unitSeg = displaySeg.SegmentAlongLine(0.5f - sz, 0.5f + sz);
		        //var unitSeg = new SKSegment((float)unitStart, y, (float)unitStart + 20f, y);
                var dm = wm.GetOrCreateDomainMapper(domain, displaySeg, unitSeg);
		        dm.ShowGradientNumberLine = true;
		        dm.ShowNumberOffsets = true;
		        dm.ShowBasisMarkers = true;
		        dm.ShowBasis = true;
		        yt += ytStep;
	        }

	        return result;
        }

    }
}
