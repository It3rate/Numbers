using Numbers.Core;
using Numbers.Views;

namespace Numbers.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Program
    {
	    private Brain MyBrain { get; }
        private CoreRenderer Renderer { get; }
        public Program(Brain brain, CoreRenderer renderer)
        {
	        MyBrain = brain;
	        Renderer = renderer;
        }

        private SKWorkspaceMapper test2(IAgent agent)
        {
            Trait trait = new Trait();
            var unitSize = 10;
            var unit = FocalRef.CreateByValues(trait, 0, unitSize);
            var wm = new SKWorkspaceMapper(agent.Workspace, Renderer, 20, 20, 1000, 800);
            var domains = CreateDomainLines(agent.Workspace, trait, -15, 10, 30, -40, -35, 24, -4, -13);
            var d2 = domains[2];
            var d1n2 = MyBrain.NumberStore[domains[1].NumberIds[2]];
            var nn = new Number(d2, d1n2.FocalId);
            agent.Workspace.AddElements(nn);
            return wm;
        }
        private SKWorkspaceMapper test0(IAgent agent)
        {
            Trait trait = new Trait();
            var unitSize = 8;
            var unit = FocalRef.CreateByValues(trait, 0, unitSize);
            var range = FocalRef.CreateByValues(trait, -16 * unitSize, 16 * unitSize);
            var hDomain = trait.AddDomain(unit.Id, range.Id);
            var vDomain = trait.AddDomain(unit.Id, range.Id);
            var hFocal = FocalRef.CreateByValues(trait, -2 * unitSize, 9 * unitSize);
            var vFocal = FocalRef.CreateByValues(trait, 3 * unitSize, 6 * unitSize);
            //var val2 = FocalRef.CreateByValues(t0, unitSize, unitSize);
            //var val3 = FocalRef.CreateByValues(t0, unitSize, unitSize);

            var hNum = new Number(hDomain, hFocal.Id);
            var vNum = new Number(vDomain, vFocal.Id);
            var hSel = new Selection(hNum);
            var transform = trait.AddTransform(hSel, vNum, TransformKind.Blend);

            agent.Workspace.AddDomains(true, hDomain, vDomain);

            var wm = new SKWorkspaceMapper(agent.Workspace, Renderer, 150, 10, 800, 800);

            var dm = wm.GetOrCreateDomainMapper(hDomain, wm.GetHorizontalSegment(.5f, 50));
            dm.ShowGradientNumberLine = false;
            dm.ShowValueMarkers = true;
            dm.ShowBasisMarkers = false;
            dm.ShowUnits = false;

            var dm2 = wm.GetOrCreateDomainMapper(vDomain, wm.GetVerticalSegment(.5f, 50));
            dm2.ShowGradientNumberLine = false;
            dm2.ShowValueMarkers = true;
            dm2.ShowBasisMarkers = false;
            dm2.ShowUnits = false;
            return wm;
        }
        private SKWorkspaceMapper test1(IAgent agent)
        {
            Trait trait = new Trait();
            var unitSize = 4;
            var unit = FocalRef.CreateByValues(trait, 3, 3 + unitSize);
            var range = FocalRef.CreateByValues(trait, -40, 40);
            var domain = trait.AddDomain(unit.Id, range.Id);
            //var domain2 = t0.AddDomain(unit.Id, range.Id);
            var val2 = FocalRef.CreateByValues(trait, -15, 20);
            //var val3 = FocalRef.CreateByValues(t0, -40, 60);
            //var val2 = FocalRef.CreateByValues(t0, unitSize, unitSize);
            //var val3 = FocalRef.CreateByValues(t0, unitSize, unitSize);

            var num2 = new Number(domain, val2.Id);
            //var num3 = new Number(domain2, val3.Id);
            //var sel = new Selection(num2);
            //var transform = t0.AddTransform(sel, num3, TransformKind.Blend);

            agent.Workspace.AddDomains(true, domain);//, domain2);
            var wm = new SKWorkspaceMapper(agent.Workspace, Renderer, 20, 20, 800, 800);
            var dm = wm.GetOrCreateDomainMapper(domain, wm.GetHorizontalSegment(.3f, 100));
            dm.ShowGradientNumberLine = true;
            dm.ShowNumberOffsets = true;
            dm.ShowBasisMarkers = true;
            dm.ShowUnits = true;
            wm.EnsureRenderers();
            var nm = wm.NumberMapper(num2.Id);
            //dm.EndPoint += new SKPoint(0, -50);
            return wm;
        }
        private int _testIndex = 2;
        private readonly int[] _tests = new int[] { 0, 1, 2 };
        public SKWorkspaceMapper NextTest(IAgent agent)
        {
	        agent.IsPaused = true;
	        agent.ClearAll();

	        agent.Workspace = new Workspace();
            SKWorkspaceMapper wm;
            switch (_tests[_testIndex])
            {
                case 0:
                    wm = test0(agent);
                    break;
                case 1:
                    wm = test1(agent);
                    break;
                case 2:
                default:
                    wm = test2(agent);
                    break;
            }
            _testIndex = _testIndex >= _tests.Length - 1 ? 0 : _testIndex + 1;
            wm.EnsureRenderers();
            agent.IsPaused = false;
            return wm;
        }

        private List<Domain> CreateDomainLines(Workspace workspace, Trait trait, params long[] focalPositions)
        {
	        var result = new List<Domain>();
	        var wm = MyBrain.WorkspaceMappers[workspace.Id];
	        var unitFocal = trait.FocalStore.Values.First();
	        var padding = 1.4;
	        long maxPos = (long)Math.Max((focalPositions.Max() * padding), unitFocal.AbsLengthInTicks * padding);
	        long minPos = (long)Math.Min((focalPositions.Min() * padding), -unitFocal.AbsLengthInTicks * padding);
	        var range = FocalRef.CreateByValues(trait, minPos, maxPos);
	        var rangeLen = (double)range.LengthInTicks;
	        var yt = 0.1f;
	        var ytStep = (float)(0.8 / Math.Floor(focalPositions.Length / 2.0));
	        for (int i = 1; i < focalPositions.Length; i += 2)
	        {
		        var domain = trait.AddDomain(unitFocal.Id, range.Id);
		        //domain.BasisIsReciprocal = true;
		        result.Add(domain);
		        var focal = FocalRef.CreateByValues(trait, focalPositions[i - 1], focalPositions[i]);
		        var num = new Number(domain, focal.Id);
		        workspace.AddDomains(true, domain);
		        var displaySeg = wm.GetHorizontalSegment(yt, 100);
		        var y = displaySeg.StartPoint.Y;

		        var sz = domain.BasisIsReciprocal ? 0.01f : 0.05f;
		        var unitSeg = displaySeg.SegmentAlongLine(0.5f - sz, 0.5f + sz);
		        //var unitSeg = new SKSegment((float)unitStart, y, (float)unitStart + 20f, y);
                var dm = wm.GetOrCreateDomainMapper(domain, displaySeg, unitSeg);
		        dm.ShowGradientNumberLine = true;
		        dm.ShowNumberOffsets = true;
		        dm.ShowBasisMarkers = true;
		        dm.ShowUnits = true;
		        yt += ytStep;
	        }

	        return result;
        }

    }
}
