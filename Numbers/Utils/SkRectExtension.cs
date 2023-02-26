using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Utils
{
	public static class SkRectExtension
	{
		public static SKSegment TopLine(this SKRect self) => new SKSegment(self.Left, self.Top, self.Right, self.Top);
		public static SKSegment LeftLine(this SKRect self) => new SKSegment(self.Left, self.Top, self.Left, self.Bottom);
		public static SKSegment BottomLine(this SKRect self) => new SKSegment(self.Left, self.Bottom, self.Right, self.Bottom);
		public static SKSegment RightLine(this SKRect self) => new SKSegment(self.Right, self.Top, self.Right, self.Bottom);
	}
}
