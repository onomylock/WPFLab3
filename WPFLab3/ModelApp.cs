using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WPFLab3
{
	public class Point : ReactiveObject
	{		
		[Reactive]
		public double X { get; set; }
		[Reactive]
		public double Y { get; set; }

		public Point()
		{
			X = 0;
			Y = 0;
		}

		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}
	}

	public class ModelApp : ReactiveObject
	{
		public ObservableCollection<Point> GeoObject { get; set; }

		[Reactive]
		public SolidColorBrush ColorLine { get; set; }
		[Reactive]
		public string Legend { get; set; }

		public ModelApp(string legend)
		{
			GeoObject = new ObservableCollection<Point>();
			Legend = legend;
			var r = new Random();
			var bytes = new byte[4];
			r.NextBytes(bytes);
			ColorLine = new SolidColorBrush(Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]));			
		}

		public ModelApp(ObservableCollection<Point> geoObject, string legend)
		{
			GeoObject = geoObject;
			Legend = legend;
			var r = new Random();
			var bytes = new byte[4];
			r.NextBytes(bytes);
			ColorLine = new SolidColorBrush(Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]));			
		}
		
		public Point[] SortedPoints()
		{
			Point[] tmpSortedPoints = new Point[GeoObject.Count];
			GeoObject.CopyTo(tmpSortedPoints, 0);
			
			for(int i = 0; i + 1 < tmpSortedPoints.Length; i++)
			{
				for(int j = 0; j + 1 < tmpSortedPoints.Length - i; j ++)
				{
					if(tmpSortedPoints[j + 1].X < tmpSortedPoints[j].X)
					{
						Point tmp = tmpSortedPoints[j + 1];
						tmpSortedPoints[j + 1] = tmpSortedPoints[j];
						tmpSortedPoints[j] = tmp;
					}
				}
			}
			return tmpSortedPoints;
		}

		public void SetRandomColor()
		{
			var r = new Random();
			var bytes = new byte[4];
			r.NextBytes(bytes);
			ColorLine = new SolidColorBrush(Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]));
		}

		public (Point, Point) FindMinMax()
		{
			if (GeoObject != null && GeoObject.Count > 0)
			{
				Point MaxPoint = new Point(GeoObject.First().X, GeoObject.First().Y);
				Point MinPoint = new Point(GeoObject.First().X, GeoObject.First().Y);

				foreach (Point item in GeoObject)
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
			else return (null, null);
		}
	}
}
