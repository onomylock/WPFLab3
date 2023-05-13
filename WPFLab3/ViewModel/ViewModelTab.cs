using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using WPFLab3.Model;
using static System.Windows.Forms.LinkLabel;

namespace WPFLab3
{
	public abstract class ViewModelTab : ReactiveObject
	{
		public Tab CurrentTub { get; set; }
		public ObservableCollection<IModelObject> ModelObjectCollection { get; set; } = new ObservableCollection<IModelObject>();
		public Point CurrentPoint { get; set; } = new Point();
		public Point ButtonDownPoint { get; set; } = new Point();
		public double Scale { get; set; } = 1;
		public bool MouseDown { get; set; } = false;
		public double Width { get; set; }
		public double Height { get; set; }
		public MainWindow mainWindow { get; protected set; }


		protected List<double> gridX = new List<double>();
		protected List<double> gridY = new List<double>();
		protected Point dPoint, zoomP, maxP, minP;		

		public abstract void Draw();

		public virtual void SetStartView()
		{
			dPoint.X = 0;
			dPoint.Y = 0;
			Scale = 1;
			BoundsCalculation();
			Draw();
		}

		public void SetMousePosition(System.Windows.Point Point)
		{

			double X = Point.X;
			double Y = Point.Y;

			if (ModelObjectCollection.Count > 0)
			{
				X = (X / Scale + dPoint.X) / (zoomP.X);
				Y = (Height - Y / Scale - dPoint.Y) / (zoomP.Y);

				double xdiv = (maxP.X - minP.X) / gridX.Count;
				double ydiv = (maxP.Y - minP.Y) / gridY.Count;
			}						
			SetLableContent(new Point(X, Y));			
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
				myLine.X1 = (x_cur * zoomP.X - dPoint.X) * Scale;
				myLine.Y1 = 0;
				myLine.X2 = (x_cur * zoomP.Y - dPoint.X) * Scale;
				myLine.Y2 = Height;				
				AddGridLine(myLine);
			}

			for (int i = 0; i < gridY.Count; ++i)
			{
				Line myLine = new Line();
				myLine.Stroke = System.Windows.Media.Brushes.LightGray;
				double y_cur = gridY[i];
				myLine.X1 = 0;
				myLine.Y1 = (Height - y_cur * zoomP.Y - dPoint.Y) * Scale;
				myLine.X2 = Width;
				myLine.Y2 = (Height - y_cur * zoomP.Y - dPoint.Y) * Scale;
				AddGridLine(myLine);
			}
		}
		
		protected virtual void DrawAxis(Canvas Axis, double min, double max, bool AxisFlag)
		{
			if (ModelObjectCollection.Count > 0)
			{

				if (Axis.Children.Count > 0) Axis.Children.Clear();

				if (AxisFlag)
				{
					foreach (double x in gridX)
					{
						//if (x * zoomX < Width - 5 && x * zoomX > 10)
						Text(Axis, (x * zoomP.X - 2 - dPoint.X) * Scale, 8, x.ToString(), Color.FromRgb(0, 0, 0));
					}
				}
				else
				{
					foreach (double y in gridY)
					{
						//if (y * zoomY < Height - 5 && y * zoomY > 5)
						Text(Axis, 15, (Height - y * zoomP.Y - 5 - dPoint.Y) * Scale, y.ToString(), Color.FromRgb(0, 0, 0));
					}
				}
			}
		}

		protected virtual void Text(Canvas canvasObj, double x, double y, string text, Color color)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Text = text;
			textBlock.Foreground = new SolidColorBrush(color);
			Canvas.SetLeft(textBlock, x);
			Canvas.SetTop(textBlock, y);
			canvasObj.Children.Add(textBlock);
		}

		protected virtual void RecalculateZoom()
		{
			zoomP.X = Width / Math.Abs(maxP.X - minP.X);
			zoomP.Y = Height / Math.Abs(maxP.Y - minP.Y);
		}

		protected abstract void SetLableContent(Point point);
		protected abstract void CalculateGrid();
		protected abstract void AddGridLine(Line myLine);
		protected abstract void BoundsCalculation();
		protected abstract void DrawModelObject(IModelObject modelObject);
		protected abstract (Point, Point) FindMinMax(IModelObject modelObject);
	}
}
