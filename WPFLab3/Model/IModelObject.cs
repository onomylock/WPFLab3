﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WPFLab3.Model
{
	public interface IModelObject
	{
		List<Point> Points { get; set; }
		Brush Color { get; set; }
		double Value { get; set; }
	}
}
