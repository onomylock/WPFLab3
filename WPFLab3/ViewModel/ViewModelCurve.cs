using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFLab3.Model;
using static System.Windows.Forms.LinkLabel;

namespace WPFLab3
{
	public class ViewModelCurve : ViewModelTab
	{
		public ViewModelCurve(MainWindow mainWindow)
		{
			base.mainWindow = mainWindow;
			CurrentTub = Tab.Curve;
			Height = mainWindow.GraphicCurve.Height;
			Width = mainWindow.GraphicCurve.Width;
		}
		public override void Draw()
		{
			if (mainWindow.GraphicCurve.Children.Count > 0)
				mainWindow.GraphicCurve.Children.Clear();
			CalculateGrid();
			DrawGrid();


			foreach (var item in ModelObjectCollection)
			{
				DrawModelObject(item);
			}

			DrawAxis(mainWindow.OXCurve, minP.X, maxP.X, true);
			DrawAxis(mainWindow.OYCurve, minP.Y, maxP.Y, false);
		}

		protected override void AddGridLine(Line myLine)
		{
			mainWindow.GraphicCurve.Children.Add(myLine);
		}

		protected override void BoundsCalculation()
		{
			Point Min, Max;

			minP.X = 0;
			minP.Y = 0;
			maxP.X = 0;
			maxP.Y = 0;

			foreach (var item in ModelObjectCollection)
			{

				(Min, Max) = FindMinMax(item);

				if (Min != null && Max != null)
				{
					minP.X = Min.X < minP.X ? Min.X : minP.X;
					minP.Y = Min.Y < minP.Y ? Min.Y : minP.Y;
					maxP.X = Max.X > maxP.X ? Max.X : maxP.X;
					maxP.Y = Max.Y > maxP.Y ? Max.Y : maxP.Y;
				}
			}
			RecalculateZoom();
		}

		protected override void CalculateGrid()
		{
			BoundsCalculation();
			double cur_x = maxP.X, cur_y = minP.Y;
			gridX = new List<double>();
			gridY = new List<double>();

			double stepX = Math.Pow(10, Math.Floor(Math.Log10((maxP.X - minP.X - 1))));
			double stepY = Math.Pow(10, Math.Floor(Math.Log10((maxP.Y - minP.Y - 1))));

			if (stepX == 0) stepX = 2;
			if (stepY == 0) stepY = 2;

			while (cur_x <= maxP.X + 1)
			{
				gridX.Add(cur_x);
				cur_x += stepX;
			}

			while (cur_y <= maxP.Y + 1)
			{
				gridY.Add(cur_y);
				cur_y += stepY;
			}
		}

		public Point[] SortedPoints(List<Point> points)
		{
			Point[] tmpSortedPoints = new Point[points.Count()];
			points.CopyTo(tmpSortedPoints, 0);

			for (int i = 0; i + 1 < tmpSortedPoints.Length; i++)
			{
				for (int j = 0; j + 1 < tmpSortedPoints.Length - i; j++)
				{
					if (tmpSortedPoints[j + 1].X < tmpSortedPoints[j].X)
					{
						Point tmp = tmpSortedPoints[j + 1];
						tmpSortedPoints[j + 1] = tmpSortedPoints[j];
						tmpSortedPoints[j] = tmp;
					}
				}
			}
			return tmpSortedPoints;
		}

		protected override void DrawModelObject(IModelObject modelObject)
		{
			double dx = minP.X < 0 ? minP.X : 0;
			double dy = minP.Y < 0 ? minP.Y : 0;

			if (modelObject != null && modelObject.Points.Count() > 0)
			{
				Polyline myPolyline = new Polyline();
				myPolyline.Stroke = modelObject.Color;
				myPolyline.StrokeThickness = 3;
				myPolyline.FillRule = FillRule.EvenOdd;
				PointCollection myPointCollection = new PointCollection();
				var SortPoints = SortedPoints(modelObject.Points);

				foreach (Point item in SortPoints)
				{
					myPointCollection.Add(new System.Windows.Point(((item.X - dx) * zoomP.X - dPoint.X) * Scale,
						(Height - (item.Y - dy) * zoomP.Y - dPoint.Y) * Scale));
				}
				myPolyline.Points = myPointCollection;
				mainWindow.GraphicCurve.Children.Add(myPolyline);
			}
		}

		protected override void SetLableContent(Point point)
		{
			mainWindow.LabelXCurve.Content = Convert.ToSingle(point.X).ToString();
			mainWindow.LabelYCurve.Content = Convert.ToSingle(point.Y).ToString();
		}

		protected override (Point, Point) FindMinMax(IModelObject modelObject)
		{
			if (modelObject != null && modelObject.Points.Count() > 0)
			{
				Point MaxPoint = new Point(modelObject.Points.First().X, modelObject.Points.First().Y);
				Point MinPoint = new Point(modelObject.Points.First().X, modelObject.Points.First().Y);

				foreach (Point item in modelObject.Points)
				{
					if (item.X > MaxPoint.X)
					{
						MaxPoint.X = item.X;
					}
					if (item.Y > MaxPoint.Y)
					{
						MaxPoint.Y = item.Y;
					}
					if (item.X < MinPoint.X)
					{
						MinPoint.X = item.X;
					}
					if (item.Y < MinPoint.Y)
					{
						MinPoint.Y = item.Y;
					}
				}
				return (MinPoint, MaxPoint);
			}
			return (null, null);
		}
	}
}
