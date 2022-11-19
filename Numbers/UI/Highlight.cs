using Numbers.Views;
using SkiaSharp;

namespace Numbers.UI
{
    using Numbers.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Highlight is a sub-selection for UI purposes, can select parts of an equation to drag, like a orgPoint on a segment, or segment in a transform.
    /// Can probably be merged with selection eventually, should be similar mechanisms to select with segments.
    /// </summary>
    public class Highlight
    {
	    public SKPoint OrginalPoint { get; set; } = SKPoint.Empty;
	    public SKPoint _snapPoint = SKPoint.Empty;
	    public SKPoint SnapPoint
	    {
		    get => _snapPoint.IsEmpty ? OrginalPoint : _snapPoint;
		    set => _snapPoint = value;

	    }
        public SKMapper Mapper { get; set; }
        public float T { get; set; }
        public UIKind Kind { get; set; }
        //public Number RangeInMinMax { get; set; } // will set selection ranges with a length trait number eventually
        public bool IsSet => Mapper != null;

        public Highlight()
        {
        }
        private Highlight(SKPoint orgPoint, SKMapper mapper, float t, UIKind kind)
	    {
		    OrginalPoint = orgPoint;
		    Mapper = mapper;
		    T = t;
		    Kind = kind;
	    }

        public void Set(SKPoint orgPoint, SKPoint snapPoint, SKMapper mapper, float t, UIKind kind)
        {
	        OrginalPoint = orgPoint;
	        SnapPoint = snapPoint;
            Mapper = mapper;
		    T = t;
		    Kind = kind;
        }
	    public void Reset()
        {
	        OrginalPoint = SKPoint.Empty;
	        SnapPoint = SKPoint.Empty;
            Mapper = null;
            T = 0;
            Kind = UIKind.None;
        }

	    public Highlight Clone()
	    {
            return new Highlight(new SKPoint(OrginalPoint.X, OrginalPoint.Y), Mapper, T, Kind);
	    }

	    public SKPath HighlightPath()
	    {
		    return Mapper.HighlightAt(T, SnapPoint);
	    }
    }

    [Flags]
    public enum UIKind
    {
	    None = 0x00,
        Major = 0x01,

        Label = 0x10,
        Marker = 0x20,
        Tick = 0x40,
        Unit = 0x80,

        Number = 0x100,
        Domain = 0x200,
        Transform = 0x400,
        Shape = 0x800,

        Point = 0x1000,
        Line = 0x2000,
        Area = 0x4000,
        Volume = 0x8000,
    }
    public static class HighlightExtensions
    {
	    public static bool IsMajor(this UIKind kind)
	    {
		    return (kind & UIKind.Major) != UIKind.None;
	    }
	    public static bool IsUnit(this UIKind kind)
	    {
		    return (kind & UIKind.Unit) != UIKind.None;
	    }
        public static bool IsNumber(this UIKind kind)
	    {
		    return (kind & UIKind.Number) != UIKind.None;
	    }
	    public static bool IsDomain(this UIKind kind)
	    {
		    return (kind & UIKind.Domain) != UIKind.None;
	    }
        public static bool IsDomainPoint(this UIKind kind)
	    {
		    return kind.IsDomain() && (kind & UIKind.Point) != UIKind.None;
	    }
        public static bool IsMinorTick(this UIKind kind)
	    {
		    return !kind.IsMajor() && (kind & UIKind.Tick) != UIKind.None;
	    }
	    public static bool IsBoldTick(this UIKind kind)
	    {
		    return kind.IsMajor() && (kind & UIKind.Tick) != UIKind.None;
	    }
    }
}
