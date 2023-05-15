using ReactiveUI;	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFLab3.Model;

namespace WPFLab3
{
	public class ViewModelView2D : ViewModelTab
	{		
		public ViewModelView2D(MainWindow mainWindow)
		{
			this.mainWindow = mainWindow;
			Height = mainWindow.GraphicView2D.Height;
			Width = mainWindow.GraphicView2D.Width;
		}
		public override void Draw()
		{
			if (mainWindow.GraphicView2D.Children.Count > 0) 
				mainWindow.GraphicView2D.Children.Clear();			
			CalculateGrid();
			DrawGrid();

			foreach (var modelObject in ModelObjectCollection)
			{
				foreach(var item in modelObject)
				{
					DrawModelObject(item);
					DrawTextGraphic(item);
				}
					
			}

			DrawAxis(mainWindow.OXView2D, minP.X, maxP.X, true);
			DrawAxis(mainWindow.OYView2D, minP.Y, maxP.Y, false);
		}

		private void DrawTextGraphic(IModelObject modelObject)
		{
			double dx = minP.X < 0 ? minP.X : 0;
			double dy = minP.Y < 0 ? minP.Y : 0;

			if (modelObject != null && modelObject.Points.Count() > 0)
			{
				Point pointCenter = new Point(modelObject.Points[1].X - modelObject.Points[0].X, 
					modelObject.Points[1].Y - modelObject.Points[0].Y);
				Text(mainWindow.GraphicView2D, (pointCenter.X * zoomP.X - dPoint.X) * Scale,
					(Height - pointCenter.Y * zoomP.Y - dPoint.Y) * Scale, modelObject.Value.ToString(), Color.FromRgb(0, 0, 0));
			}
		}

		protected override void AddGridLine(Line myLine)
		{
			mainWindow.GraphicView2D.Children.Add(myLine);
		}

		protected override void BoundsCalculation()
		{			
			double tmp;
			minP = new Point();
			maxP = new Point();

			foreach (var modelObject in ModelObjectCollection)
			{				
				List<Point> pointsXMinCollection = new List<Point>();
				foreach (var item in modelObject)
				{
					tmp = item.Points.Min(x => x.X);
					if (minP.X > tmp)
						minP.X = tmp;
					tmp = item.Points.Max(x => x.X);
					if(maxP.X < tmp)
						maxP.X = tmp;
					tmp = item.Points.Min(y => y.Y);
					if (minP.Y > tmp)
						minP.Y = tmp;
					tmp = item.Points.Max(y => y.Y);
					if (maxP.Y < tmp)
						maxP.Y = tmp;			
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

		protected override void DrawModelObject(IModelObject modelObject)
		{
			double dx = minP.X < 0 ? minP.X : 0;
			double dy = minP.Y < 0 ? minP.Y : 0;

			if (modelObject != null && modelObject.Points.Count() > 0)
			{
				PointCollection myPointCollection = new PointCollection();
				myPointCollection.Add(new Point(((modelObject.Points[0].X - dx) * zoomP.X - dPoint.X) * Scale, (Height - (modelObject.Points[0].Y - dy) * zoomP.Y - dPoint.Y) * Scale));
				myPointCollection.Add(new Point(((modelObject.Points[0].X - dx) * zoomP.X - dPoint.X) * Scale, (Height - (modelObject.Points[1].Y - dy) * zoomP.Y - dPoint.Y) * Scale));
				myPointCollection.Add(new Point(((modelObject.Points[1].X - dx) * zoomP.X - dPoint.X) * Scale, (Height - (modelObject.Points[1].Y - dy) * zoomP.Y - dPoint.Y) * Scale));
				myPointCollection.Add(new Point(((modelObject.Points[1].X - dx) * zoomP.X - dPoint.X) * Scale, (Height - (modelObject.Points[0].Y - dy) * zoomP.Y - dPoint.Y) * Scale));				

				Polygon myPolygon = new Polygon();
				myPolygon.HorizontalAlignment = HorizontalAlignment.Left;
				myPolygon.VerticalAlignment = VerticalAlignment.Center;
				myPolygon.Fill = modelObject.Color;
				myPolygon.Stroke = Brushes.Black;							
				myPolygon.Points = myPointCollection;							
				mainWindow.GraphicView2D.Children.Add(myPolygon);
			}
		}

		protected override (Point, Point) FindMinMax(IModelObject modelObject)
		{
			throw new NotImplementedException();			
		}

		protected override void SetLableContent(Point point)
		{
			mainWindow.LabelXView2D.Content = Convert.ToSingle(point.X).ToString();
			mainWindow.LabelYView2D.Content = Convert.ToSingle(point.Y).ToString();
		}		
	}
}
