using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPFLab3.Model
{
	public class ModelObject : IModelObject
	{
		public ModelObject(List<Point> points, Brush color, double value)
		{
			Points = points;
			Color = color;
			Value = value;
		}
		public ModelObject(List<Point> points, Brush color)
		{
			Points = points;
			Color = color;
		}

		public List<Point> Points { get; set; }
		public Brush Color { get; set; }
		public double Value { get; set; } = 0;
	}
}
