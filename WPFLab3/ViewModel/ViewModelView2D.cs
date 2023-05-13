using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFLab3.Model;

namespace WPFLab3
{
	public class ViewModelView2D : ViewModelTab
	{
		public override void Draw()
		{
			if (mainWindow.GraphicView2D.Children.Count > 0) 
				mainWindow.GraphicView2D.Children.Clear();			
			CalculateGrid();
			DrawGrid();

			foreach (var item in ModelObjectCollection)
			{
				DrawModelObject(item);
			}

			DrawAxis(mainWindow.OXView2D, minP.X, maxP.X, true);
			DrawAxis(mainWindow.OYView2D, minP.Y, maxP.Y, false);
		}

		protected override void AddGridLine(Line myLine)
		{
			mainWindow.GraphicView2D.Children.Add(myLine);
		}

		protected override void BoundsCalculation()
		{
			throw new NotImplementedException();
		}

		protected override void CalculateGrid()
		{
			throw new NotImplementedException();
		}

		protected override void DrawModelObject(IModelObject modelObject)
		{
			if(modelObject != null && modelObject.Points.Count() > 0)
			{					
				PointCollection myPointCollection = new PointCollection(4);
				myPointCollection.Add(new System.Windows.Point(modelObject.Points[0].X, modelObject.Points[0].Y));
				myPointCollection.Add(new System.Windows.Point(modelObject.Points[0].X, modelObject.Points[1].Y));
				myPointCollection.Add(new System.Windows.Point(modelObject.Points[1].X, modelObject.Points[1].Y));
				myPointCollection.Add(new System.Windows.Point(modelObject.Points[0].X, modelObject.Points[1].Y));

				Polygon myPolygon = new Polygon();
				myPolygon.Points = myPointCollection;
				myPolygon.Fill = modelObject.Color;			
				myPolygon.Stretch = Stretch.Fill;
				myPolygon.Stroke = Brushes.Black;				

				mainWindow.GraphicView2D.Children.Add(myPolygon);
			}
		}

		protected override void SetLableContent(Point point)
		{
			mainWindow.LabelXView2D.Content = Convert.ToSingle(point.X).ToString();
			mainWindow.LabelYView2D.Content = Convert.ToSingle(point.Y).ToString();
		}
	}
}
