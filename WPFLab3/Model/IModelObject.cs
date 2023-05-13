using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFLab3.Model
{
	public interface IModelObject
	{
		List<Point> Points { get; set; }
		Brush Color { get; set; }
	}
}
