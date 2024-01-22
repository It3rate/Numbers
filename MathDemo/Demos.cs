﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private int _testIndex = 0;
        private readonly int[] _tests = new int[] { 0, 1, 2, 3, 4 };
        public Demos(Brain brain, CoreRenderer renderer)
        {
	        Brain = brain;
	        Renderer = renderer;
        }

        private SKWorkspaceMapper test0(MouseAgent mouseAgent)
        {
            var hDomain = Domain.CreateDomain("test0", 100, 10);
            var vDomain = Domain.CreateDomain("test0", 100, 10);
            var hNum = hDomain.CreateNumberFromFloats(2, 9);
            var vNum = vDomain.CreateNumberFromFloats(3, 6);
        

            var hSel = new Selection(hNum);
            var transform = Brain.AddTransform(hSel, vNum, TransformKind.Blend);

            var wm = new SKWorkspaceMapper(mouseAgent, 150, 0, 800, 800);
            wm.AddHorizontal(hDomain);
            wm.AddVertical(vDomain);

            return wm;
        }

        private SKWorkspaceMapper testTrig(MouseAgent mouseAgent)
        {
            Trait trait = Trait.CreateIn(Brain, "testTrig");
            var unitSize = 100;
            var unit = Focal.CreateByValues(0, unitSize);
            var wm = new SKWorkspaceMapper(mouseAgent, 10, 50, 1200, 600);

            var domains = CreateDomainLines((MouseAgent)mouseAgent, trait, unit, 1, 101, -100, 0, -50, 50);
            var d0 = domains[0];
            var seg = d0.GetNumber(d0.NumberIds()[2]);
            //var d0n1 = d0.GetNumber(d0.NumberIds()[1]);
            //var d0n2 = d0.GetNumber(d0.NumberIds()[2]);
            //d0n0.InterpolateTo(d0n1, d0n0, d0n2);
            //var nn = d0.CreateNumber(d0n0.Focal);
            //mouseAgent.Workspace.AddElements(nn);

            //var ar = new List<Range>();
            //ar.Add(seg.ExpansiveForce);
            //for (int i = 0; i < unitSize; i++)
            //{
            //    seg.ShiftTicks(-1);
            //    var ef = seg.ExpansiveForce;
            //    ar.Add(ef);
            //    Trace.WriteLine(ef.MaxF - ef.MinF);
            //}

            var speed = 0f;
            var scale = 1f/1900f;
            var l = 0f;
            var r = 1f;
            for (int i = 0; i < unitSize; i++)
            {
                var len = (float)Math.Sqrt(l * l + r * r);

                var rr = Math.Abs(r) / Math.Abs(r - l);
                var lr = Math.Abs(l) / Math.Abs(r - l);
                //var lr = ratio <= 0 ? Math.Abs(ratio) : (1f - ratio);
                //var rr = ratio <= 0 ? Math.Abs(ratio + 1f) : (ratio);
                var dl = len * lr;
                var dr = len * rr;
                speed += (dr - dl);

                //Trace.WriteLine(speed);
                r -= speed * scale;
                l -= speed * scale;
                //speed += (float)Math.Sqrt(r * r) * scale;// (dr - dl);
                //r -= 0.01f;//speed * scale;// len * scale;// 
                //l -= 0.01f;//speed * scale;// len * scale;//
            }

            return wm;
        }

        private SKWorkspaceMapper test3(MouseAgent mouseAgent)
        {
	        Trait trait = Trait.CreateIn(Brain, "test3");
	        long unitSize = 64;
            var wm = new SKWorkspaceMapper(mouseAgent, 20, 20, 1000, 400);
            var guideline = new SKSegment(100,100,700,100);
            var unitSeg = guideline.SegmentAlongLine(0.4f, 0.6f);
            var dc = new AddSKDomainCommand(trait, 0, unitSize, -800, 800, guideline, unitSeg);
            mouseAgent.Stack.Do(dc);
            var num = new AddSKNumberCommand(dc.DomainMapper, new Range(-1.5, 2.4));
            mouseAgent.Stack.Do(num);
            num = new AddSKNumberCommand(dc.DomainMapper, new Range(.5, -1.2));
            mouseAgent.Stack.Do(num);

            var guideline2 = new SKSegment(100, 200, 700, 200);
            var dc2 = new AddSKDomainCommand(trait, 0, unitSize, -800, 800, guideline2, unitSeg);
            mouseAgent.Stack.Do(dc2);
            var num2 = new AddSKNumberCommand(dc2.DomainMapper, new Range(-1.2, -1.4));
            num2.DefaultDelay = -600;
            mouseAgent.Stack.Do(num2);

            var guideline3 = new SKSegment(100, 300, 700, 300);
            var dc3 = new AddSKDomainCommand(trait, 0, unitSize, -800, 800, guideline3, unitSeg);
            mouseAgent.Stack.Do(dc3);
            var numSet = new NumberSet(dc3.CreatedDomain, new[] { new Focal(5, 20), new Focal(-20, -10), new Focal(-40, -30) });
            dc3.CreatedDomain.AddNumberSet(numSet);

            return wm;
        }

        private SKWorkspaceMapper test2(MouseAgent mouseAgent)
        {
	        Trait trait = Trait.CreateIn(Brain, "test2");
            var unitSize = 10;
            var unit = Focal.CreateByValues(0, unitSize);
            var wm = new SKWorkspaceMapper(mouseAgent, 10, 50, 1200, 600);
            var domains = CreateDomainLines((MouseAgent)mouseAgent, trait, unit, 15, 10, -40, -30, 35, 24, 4, -13);
            var d2 = domains[2];
            var d1 = domains[1];
            var d1n2 = d1.GetNumber(d1.NumberIds()[2]);
            var nn =d2.CreateNumber(d1n2.Focal);
            mouseAgent.Workspace.AddElements(nn);
            return wm;
        }
        private SKWorkspaceMapper test1(MouseAgent mouseAgent)
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
            //var sel = new Source(num2);
            //var transform = t0.AddTransform(sel, num3, TransformKind.Blend);

            mouseAgent.Workspace.AddDomains(true, domain);//, domain2);
            var wm = new SKWorkspaceMapper(mouseAgent, 20, 20, 800, 800);
            var dm = wm.GetOrCreateDomainMapper(domain, wm.GetHorizontalSegment(.3f, 100));
            dm.ShowGradientNumberLine = true;
            dm.ShowNumberOffsets = true;
            dm.ShowBasisMarkers = true;
            dm.ShowBasis = true;
            wm.EnsureRenderers();
            //var nm = dm.NumberMapper(num2.Id);
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
	                wm = test3(mouseAgent);
                    break;
                default:
	                wm = testTrig(mouseAgent);
                    break;
            }
            _testIndex = _testIndex >= _tests.Length - 1 ? 0 : _testIndex + 1;

            wm.EnsureRenderers();
            mouseAgent.IsPaused = false;
            return wm;
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
	        var range = Focal.CreateByValues(minPos, maxPos);
	        var rangeLen = (double)range.LengthInTicks;
	        var yt = 0.1f;
	        var ytStep = (float)(0.8 / Math.Floor(focalPositions.Length / 2.0));
		    //var domain = trait.AddDomain(basisFocal, range);
		    ////domain.BasisIsReciprocal = true;
		    //result.Add(domain);
	        for (int i = 1; i < focalPositions.Length; i += 2)
	        {
		        var focal = Focal.CreateByValues(focalPositions[i - 1], focalPositions[i]);

		        var domain = trait.AddDomain(basisFocal, focal);
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
		        dm.ShowNumberOffsets = true;
		        dm.ShowBasisMarkers = true;
		        dm.ShowBasis = true;
		        yt += ytStep;
	        }

	        return result;
        }

    }
}
