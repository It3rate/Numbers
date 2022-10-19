using System;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Numbers.Core;

namespace Numbers.Renderer
{
	public abstract class RendererBase
	{
		//public RenderStatus Status { get; set; }
		public int Width { get; protected set; }
		public int Height { get; protected set; }
		public event EventHandler DrawingComplete;

		//public UIData Data { get; set; }

		public SKCanvas Canvas;

		public CorePens Pens { get; set; }
		public SKBitmap Bitmap { get; set; }
		public bool ShowBitmap { get; set; }

		public RendererBase()
		{
			GeneratePens();
		}

		protected bool hasControl = false;

		public Control AddAsControl(Control parent, bool useGL = false)
		{
			Control result;
			if (useGL)
			{
				result = new SKGLControl();
				((SKGLControl) result).PaintSurface += DrawOnGLSurface;
			}
			else
			{
				result = new SKControl();
				((SKControl) result).PaintSurface += DrawOnPaintSurface;
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
			BeginDraw();
			Draw();
			EndDraw();
			//WorkingPad = null;
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
				Canvas.Clear(SKColor.FromHsl(200f, 14f, 8f));
			}

		}

		public virtual void Draw()
		{
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

		public abstract void DrawRoundBox(SKPoint point, SKPaint paint, float radius = 8f);
		public abstract void DrawPolyline(SKPoint[] polyline, SKPaint paint);
		public abstract void DrawPath(SKPoint[] polyline, SKPaint paint);
		public abstract void DrawDirectedLine(Number seg, SKPaint paint);
		public abstract void DrawText(SKPoint center, string text, SKPaint paint);
		public abstract void DrawBitmap(SKBitmap bitmap);

		public abstract void GeneratePens();

		protected SKRect GetTextBackgroundSize(float x, float y, String text, SKPaint paint)
		{
			var fm = paint.FontMetrics;
			float halfTextLength = paint.MeasureText(text) / 2 + 4;
			return new SKRect((int) (x - halfTextLength), (int) (y + fm.Top + 3), (int) (x + halfTextLength), (int) (y + fm.Bottom - 1));
		}

		public SKBitmap GenerateBitmap(int width, int height)
		{
			Bitmap = new SKBitmap(width, height);
			return Bitmap;
		}


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
