using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using static System.Resources.ResXFileRef;

namespace WPFLab3
{
	public class Cell
	{
		public double I { get; set; }
		public Point Center { get; set; } = new Point();
		public Point P { get; set; } = new Point();
		public Point[] Nodes { get; set; } = new Point[2];

		public Cell(double i, Point center, Point p, Point[] nodes)
		{
			I = i;
			Center = center;
			P = p;
			Nodes = nodes;
		}
	}

	public class Receiver
	{
		public Point B { get; set; } = new Point();
		public Point XY { get; set; } = new Point();

		public Receiver(Point b, Point xY)
		{
			B = b;
			XY = xY;
		}
	}

	public enum FileType
	{
		Cells,
		Receivers,
		Output
	}

	public class ModelCalulation : ReactiveObject
	{				
		public List<Cell> Cells { get; set; }
		public List<Receiver> Receivers { get; set; }

		private Direct _direct;
		private Regex _regexCell = new Regex(@"\w*.cells.txt$");
		private Regex _regexReceiver = new Regex(@"\w*.receivers.txt$");
		public ModelCalulation()
		{
			Cells = new List<Cell>();
			Receivers = new List<Receiver>();
		}
		
		public void StartCalulationDirect()
		{
			_direct = new Direct();
			_direct.Calculate(Cells, Receivers);
		}

		public void SetInputCalculationData(string directoryPath)
		{
			string[] files = Directory.GetFiles(directoryPath);

			foreach (string file in files)
			{
				if(File.Exists(file))
				{

					if (_regexCell.Match(file).Success)
					{						
						using(var sr = new StreamReader(file))
						{
							int count = int.Parse(sr.ReadLine());

							for(int i = 0; i < count; i++)
							{
								String[] arr = sr.ReadLine().Split();
								Cells.Add(new Cell(double.Parse(arr[0]), new Point(double.Parse(arr[1]), double.Parse(arr[2])), 
									new Point(double.Parse(arr[3]), double.Parse(arr[4])), 
									new Point[2] { new Point(double.Parse(arr[5]), double.Parse(arr[6])), 
												   new Point(double.Parse(arr[7]), double.Parse(arr[8]))}));
							}
						}
					}
					else if (_regexReceiver.Match(file).Success)
					{						
						using (var sr = new StreamReader(file))
						{
							int count = int.Parse(sr.ReadLine());

							for (int i = 0; i < count; i++)
							{
								String[] arr = sr.ReadLine().Split();
								Receivers.Add(new Receiver(new Point(double.Parse(arr[0]), double.Parse(arr[1])),
														   new Point(double.Parse(arr[2]), double.Parse(arr[3]))));
							}
						}
					}
				}				
			}
		}		
	}
}
