using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFLab3
{
	public abstract class TabViewModelBase
	{
		 
		protected virtual void Text(Canvas canvasObj, double x, double y, string text, Color color)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Text = text;
			textBlock.Foreground = new SolidColorBrush(color);
			Canvas.SetLeft(textBlock, x);
			Canvas.SetTop(textBlock, y);
			canvasObj.Children.Add(textBlock);
		}

		protected virtual void CalculateGrid()
		{
			boundsCalculation();
			double cur_x = X_min, cur_y = Y_min;
			gridX = new List<double>();
			gridY = new List<double>();

			double stepX = Math.Pow(10, Math.Floor(Math.Log10((X_max - X_min - 1))));
			double stepY = Math.Pow(10, Math.Floor(Math.Log10((Y_max - Y_min - 1))));

			if (stepX == 0) stepX = 2;
			if (stepY == 0) stepY = 2;

			while (cur_x <= X_max + 1)
			{
				gridX.Add(cur_x);
				cur_x += stepX;
			}

			while (cur_y <= Y_max + 1)
			{
				gridY.Add(cur_y);
				cur_y += stepY;
			}
		}

		public void SetMousePosition(System.Windows.Point Point)
		{

			double X = Point.X;
			double Y = Point.Y;

			if (LinesCollection.Count > 0)
			{
				X = (X / Scale + dPoint.X) / (zoomX);
				Y = (Height - Y / Scale - dPoint.Y) / (zoomY);

				double xdiv = (X_max - X_min) / gridX.Count;
				double ydiv = (Y_max - Y_min) / gridY.Count;
			}
			var _X = Convert.ToSingle(X).ToString();
			var _Y = Convert.ToSingle(Y).ToString();
			mainWindow.LabelX.Content = _X;
			mainWindow.LabelY.Content = _Y;
		}

		protected virtual void RecalculateZoom()
		{
			zoomX = Width / Math.Abs(X_max - X_min);
			zoomY = Height / Math.Abs(Y_max - Y_min);
		}

		protected virtual void DrawAxis(Canvas Axis, double min, double max, bool AxisFlag)
		{
			if (LinesCollection.Count > 0)
			{

				if (Axis.Children.Count > 0) Axis.Children.Clear();

				if (AxisFlag)
				{
					foreach (double x in gridX)
					{
						//if (x * zoomX < Width - 5 && x * zoomX > 10)
						Text(Axis, (x * zoomX - 2 - dPoint.X) * Scale, 8, x.ToString(), Color.FromRgb(0, 0, 0));
					}
				}
				else
				{
					foreach (double y in gridY)
					{
						//if (y * zoomY < Height - 5 && y * zoomY > 5)
						Text(Axis, 15, (Height - y * zoomY - 5 - dPoint.Y) * Scale, y.ToString(), Color.FromRgb(0, 0, 0));
					}
				}
			}
		}

		protected virtual void boundsCalculation()
		{
			Point Min, Max;

			X_min = 0;
			Y_min = 0;
			X_max = 0;
			Y_max = 0;

			foreach (ModelApp item in LinesCollection)
			{

				(Min, Max) = item.FindMinMax();

				if (Min != null && Max != null)
				{
					X_min = Min.X < X_min ? Min.X : X_min;
					Y_min = Min.Y < Y_min ? Min.Y : Y_min;
					X_max = Max.X > X_max ? Max.X : X_max;
					Y_max = Max.Y > Y_max ? Max.Y : Y_max;
				}
			}
			RecalculateZoom();
		}

		protected virtual void DrawGrid()
		{
			dPoint.X += (CurrentPoint.X - ButtonDownPoint.X) / Width * 20;
			dPoint.Y += (CurrentPoint.Y - ButtonDownPoint.Y) / Height * 20;
			double dX = Width / gridX.Count;
			double dY = Height / gridY.Count;

			for (int i = 0; i < gridX.Count; ++i)
			{
				Line myLine = new Line();
				myLine.Stroke = System.Windows.Media.Brushes.LightGray;
				double x_cur = gridX[i];
				myLine.X1 = (x_cur * zoomX - dPoint.X) * Scale;
				myLine.Y1 = 0;
				myLine.X2 = (x_cur * zoomX - dPoint.X) * Scale;
				myLine.Y2 = Height;
				mainWindow.GraphicCurve.Children.Add(myLine);
			}

			for (int i = 0; i < gridY.Count; ++i)
			{
				Line myLine = new Line();
				myLine.Stroke = System.Windows.Media.Brushes.LightGray;
				double y_cur = gridY[i];
				myLine.X1 = 0;
				myLine.Y1 = (Height - y_cur * zoomY - dPoint.Y) * Scale;
				myLine.X2 = Width;
				myLine.Y2 = (Height - y_cur * zoomY - dPoint.Y) * Scale;
				mainWindow.GraphicCurve.Children.Add(myLine);
			}
		}

		protected virtual void DrawLines(ModelApp modelApp)
		{
			double dx = X_min < 0 ? X_min : 0;
			double dy = Y_min < 0 ? Y_min : 0;

			if (modelApp.GeoObject != null && modelApp.GeoObject.Count > 0)
			{
				Polyline myPolyline = new Polyline();
				myPolyline.Stroke = modelApp.ColorLine;
				myPolyline.StrokeThickness = 3;
				myPolyline.FillRule = FillRule.EvenOdd;
				PointCollection myPointCollection = new PointCollection();
				var SortPoints = modelApp.SortedPoints();

				foreach (Point item in SortPoints)
				{
					myPointCollection.Add(new System.Windows.Point(((item.X - dx) * zoomX - dPoint.X) * Scale, ((Height - (item.Y - dy) * zoomY) - dPoint.Y) * Scale));
				}
				myPolyline.Points = myPointCollection;
				mainWindow.GraphicCurve.Children.Add(myPolyline);
			}
		}

		public void Draw()
		{
			if (mainWindow.GraphicCurve.Children.Count > 0) mainWindow.GraphicCurve.Children.Clear();
			boundsCalculation();
			CalculateGrid();
			RecalculateZoom();
			DrawGrid();

			switch (regime)
			{
				case Regime.Lines:
					{
						foreach (ModelApp item in LinesCollection)
						{
							DrawLines(item);
						}
						break;
					}
				default:
					break;
			}

			DrawAxis(mainWindow.OXCurve, X_min, X_max, true);
			DrawAxis(mainWindow.OYCurve, Y_min, Y_max, false);
		}

		public void SetStartView()
		{
			dPoint.X = 0;
			dPoint.Y = 0;
			Scale = 1;
			boundsCalculation();
			Draw();
		}
	}
}
