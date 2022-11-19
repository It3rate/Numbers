using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Numbers.Core;
using Numbers.Views;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Numbers.UI
{
	public class CoreRenderer
    {
	    public Brain MyBrain => Brain.ActiveBrain;
        public List<Workspace> Workspaces { get; } = new List<Workspace>();
	    public Workspace CurrentWorkspace { get; private set; }
	    public SKWorkspaceMapper CurrentWorkspaceMapper => MyBrain.WorkspaceMappers[CurrentWorkspace.Id];
	    public SKAgentMapper CurrentAgentMapper { get; set; }

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
	        CurrentWorkspaceMapper.Draw();
	        CurrentAgentMapper?.Draw();
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
	    public void DrawText(SKPoint center, string text, SKPaint paint, SKPaint background)
	    {
		    var rect = GetTextBackgroundSize(center.X, center.Y, text, paint);
		    Canvas.DrawRoundRect(rect, 5, 5, background ?? Pens.TextBackgroundPen);
		    Canvas.DrawText(text, center.X, center.Y, paint);
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
	    public void GeneratePens(ColorTheme colorTheme = ColorTheme.Normal)
	    {
		    Pens = new CorePens(1, colorTheme);
	    }
	    protected SKRect GetTextBackgroundSize(float x, float y, String text, SKPaint paint)
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
		    foreach (var workspace in MyBrain.Workspaces)
		    {
			    if (workspace.IsActive)
			    {
				    CurrentWorkspace = workspace;
				    BeginDraw();
				    Draw();
				    EndDraw();
				    CurrentWorkspace = null;
			    }
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
