	using System;
using System.Windows.Forms;
using Numbers.Agent;
using Numbers.Mappers;
using Numbers.Utils;
using NumbersCore.Primitives;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Numbers.Renderer
{
	public class CoreRenderer
    {
	    public MouseAgent Agent { get; set; }
        public Brain Brain => Agent.Brain;
	    public Workspace Workspace => Agent.Workspace;
	    public SKWorkspaceMapper CurrentWorkspaceMapper => Agent.WorkspaceMapper;

	    public int Width { get; protected set; }
	    public int Height { get; protected set; }
	    public event EventHandler DrawingComplete;

	    public SKCanvas Canvas;
	    public CorePens Pens { get; set; }
	    public SKBitmap Bitmap { get; set; }
	    public bool ShowBitmap { get; set; }

        public CoreRenderer()
        {
	        GeneratePens();
        }

        public virtual void BeginDraw()
        {
	        Canvas.Save();
	        Canvas.SetMatrix(Matrix);
	        if (hasControl == false)
	        {
		        Canvas.Clear(SKColors.Black);
	        }
	        else
	        {
		        Canvas.Clear(Pens.BkgColor);
	        }

        }
        public virtual void Draw()
        {
	        CurrentWorkspaceMapper?.Draw();
	        Agent?.Draw();
        }
        public virtual void EndDraw()
        {
	        Canvas.Restore();
	        if (ShowBitmap && Bitmap != null)
	        {
		        DrawBitmap(Bitmap);
	        }

	        Canvas = null;
	        OnDrawingComplete();
        }
        protected void OnDrawingComplete()
        {
	        DrawingComplete?.Invoke(this, EventArgs.Empty);
        }

        public void DrawSegment(SKSegment seg, SKPaint paint)
        {
		    Canvas.DrawLine(seg.StartPoint, seg.EndPoint, paint);
	    }
	    public void DrawLine(SKPoint p0, SKPoint p1, SKPaint paint)
	    {
		    Canvas.DrawLine(p0, p1, paint);
	    }
	    public void DrawRoundBox(SKPoint point, SKPaint paint, float radius = 8f)
	    {
		    float round = radius / 3f;
		    var box = new SKRect(point.X - radius, point.Y - radius, point.X + radius, point.Y + radius);
		    Canvas.DrawRoundRect(box, round, round, paint);
	    }
	    public void DrawPolyline(SKPaint paint, params SKPoint[] polyline)
	    {
		    Canvas.DrawPoints(SKPointMode.Polygon, polyline, paint);
	    }
	    public void FillPolyline(SKPaint paint, params SKPoint[] polyline)
	    {
		    var path = new SKPath
		    {
			    FillType = SKPathFillType.EvenOdd
		    };
		    path.MoveTo(polyline[0]);
		    path.AddPoly(polyline, true);
		    Canvas.DrawPath(path, paint);
	    }
	    public void DrawDirectedLine(SKSegment seg, bool isUnitPerspective, SKPaint paint)
	    {
            DrawPolyline(paint, seg.Points);
            Canvas.DrawCircle(seg.StartPoint, 2, paint);
            var triPts = seg.EndArrow(8);
            Canvas.DrawPoints(SKPointMode.Polygon, triPts, paint);
		}
		public void DrawText(SKPoint center, string text, SKPaint paint, SKPaint background = null)
		{
			if (background != null)
			{
				var rect = GetTextBackgroundSize(center.X, center.Y, text, paint);
				DrawTextBackground(rect, background);
			}
			Canvas.DrawText(text, center.X, center.Y, paint);
		}
		public void DrawTextOnPath(SKSegment baseline, string text, SKPaint paint, SKPaint background = null)
		{
			var path = new SKPath();
			path.MoveTo(baseline.StartPoint);
			path.LineTo(baseline.EndPoint);
			if (background != null)
			{
				//var rect = GetTextBackgroundSize(center.X, center.Y, text, paint);
				//DrawTextBackground(rect, background);
			}
			Canvas.DrawTextOnPath(text, path, SKPoint.Empty, paint);
		}

		public void DrawTextBackground(SKRect rect, SKPaint background)
	    {
		    Canvas.DrawRoundRect(rect, 5, 5, background);
	    }
	    public void DrawBitmap(SKBitmap bitmap)
	    {
		    Canvas.DrawBitmap(bitmap, new SKRect(0, 0, Width, Height));
	    }

	    public SKPath GetCirclePath(SKPoint center, float radius = 10)
	    {
		    var path = new SKPath();
		    path.AddCircle(center.X, center.Y, radius);
		    return path;
	    }
	    public SKPath GetRectPath(SKPoint topLeft, SKPoint bottomRight)
	    {
		    var path = new SKPath();
		    path.AddRect(new SKRect(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y));
		    return path;
	    }
        public SKPath GetSegmentPath(SKSegment segment, float radius = 10)
	    {
		    var path = new SKPath();
		    var (pt0, pt1) = segment.PerpendicularLine(0, radius);
		    var ptDiff = pt1 - pt0;
            path.AddPoly(new SKPoint[]
	            {segment.StartPoint + ptDiff, segment.EndPoint + ptDiff, segment.EndPoint - ptDiff, segment.StartPoint - ptDiff}, true);
		    return path;
	    }
        public void GeneratePens(ColorTheme colorTheme = ColorTheme.Normal)
	    {
		    Pens = new CorePens(1, colorTheme);
	    }
	    public SKRect GetTextBackgroundSize(float x, float y, String text, SKPaint paint)
	    {
		    var fm = paint.FontMetrics;
		    float halfTextLength = paint.MeasureText(text) / 2 + 4;
		    return new SKRect((int)(x - halfTextLength), (int)(y + fm.Top + 3), (int)(x + halfTextLength), (int)(y + fm.Bottom - 1));
	    }

#region RenderSurface
	    protected bool hasControl = false;
	    public Control AddAsControl(Control parent, bool useGL = false)
	    {
		    Control result;
		    if (useGL)
		    {
			    result = new SKGLControl();
			    ((SKGLControl)result).PaintSurface += DrawOnGLSurface;
		    }
		    else
		    {
			    result = new SKControl();
			    ((SKControl)result).PaintSurface += DrawOnPaintSurface;
		    }

		    result.Width = parent.Width;
		    result.Height = parent.Height;
		    Width = result.Width;
		    Height = result.Height;
		    parent.Controls.Add(result);
		    hasControl = true;


		    return result;
	    }
	    private void DrawOnGLSurface(object sender, SKPaintGLSurfaceEventArgs e)
	    {
		    //if (Status != null)
		    //{
		    DrawOnCanvas(e.Surface.Canvas);
		    //}
	    }
	    private void DrawOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
	    {
		    //if (true || Status != null)
		    //{
		    DrawOnCanvas(e.Surface.Canvas);
		    //}
	    }
	    public void DrawOnBitmapSurface()
	    {
		    if (Bitmap != null)
		    {
			    using (SKCanvas canvas = new SKCanvas(Bitmap))
			    {
				    DrawOnCanvas(canvas);
			    }
		    }
	    }
	    public void DrawOnCanvas(SKCanvas canvas)
	    {
		    Canvas = canvas;
		    if (Workspace != null && Workspace.IsActive)
		    {
			    BeginDraw();
			    Draw();
			    EndDraw();
		    }
	    }
	    public void DrawFraction((string, string) parts, SKPoint txtPoint, SKPaint txtPaint, SKPaint txtBkgPen)
	    {
		    var whole = parts.Item1;
		    var fraction = parts.Item2;
		    var fractionPen = Pens.TextFractionPen;
		    if (fraction != "")
		    {
			    fractionPen.Color = txtPaint.Color;
			    if (whole == "")
			    {
				    fractionPen.TextAlign = SKTextAlign.Center;
				    DrawText(txtPoint, fraction, fractionPen, txtBkgPen);
				    fractionPen.TextAlign = SKTextAlign.Left;
			    }
			    else
			    {
				    var txtAlign = txtPaint.TextAlign;
				    txtPaint.TextAlign = SKTextAlign.Right;
				    var wRect = GetTextBackgroundSize(0, 0, whole, txtPaint);
				    var fRect = GetTextBackgroundSize(0, 0, fraction, Pens.TextFractionPen);
				    DrawText(txtPoint, whole, txtPaint, null);
				    var fPoint = new SKPoint(txtPoint.X - 2, txtPoint.Y);
				    DrawText(fPoint, fraction, fractionPen, null);
				    wRect.Union(fRect);
				    DrawTextBackground(wRect, txtBkgPen);
				    txtPaint.TextAlign = txtAlign;
			    }
		    }
		    else
		    {
			    DrawText(txtPoint, whole, txtPaint, txtBkgPen);
		    }
	    }
        public SKBitmap GenerateBitmap(int width, int height)
	    {
		    Bitmap = new SKBitmap(width, height);
		    return Bitmap;
	    }
        #endregion
#region View Matrix

	    private SKMatrix _matrix = SKMatrix.CreateIdentity();
	    public SKMatrix Matrix
	    {
		    get => _matrix;
		    set => _matrix = value;
	    }
	    public float ScreenScale { get; set; } = 1f;

	    public void SetPanAndZoom(SKMatrix initalMatrix, SKPoint anchorPt, SKPoint translation, float scale)
	    {
		    var scaledAnchor = new SKPoint(anchorPt.X * ScreenScale, anchorPt.Y * ScreenScale);
		    var scaledTranslation = new SKPoint(translation.X * ScreenScale, translation.Y * ScreenScale);

		    var mTranslation = SKMatrix.CreateTranslation(scaledTranslation.X, scaledTranslation.Y);
		    var mScale = SKMatrix.CreateScale(scale, scale, scaledAnchor.X, scaledAnchor.Y);
		    var mIdent = SKMatrix.CreateIdentity();
		    SKMatrix.Concat(ref mIdent, ref mTranslation, ref mScale);
		    SKMatrix.Concat(ref _matrix, ref mIdent, ref initalMatrix);
	    }

	    public void ResetZoom()
	    {
		    Matrix = SKMatrix.CreateIdentity();
	    }

#endregion

    }
}
