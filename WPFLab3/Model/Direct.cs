using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace WPFLab3
{
	public class Direct
	{
		public List<Cell> Cells;
		private List<Vector3d> nodes;
		private Vector3d min, max, num;		
		//private int numX, numY, numZ;
		public Direct(string filePath)
		{

		}

		public Direct() { }

		private Vector3d GetB(Vector3d point)
		{
			var res = new Vector3d();
			for(int i = 0; i < Cells.Count(); i++)
			{
				res += Cells[i].GetB(point);
			}
			return res;
		}

		private double GetAbsB(Vector3d point)
		{
			return Vector3d.Norm(GetB(point));
		}

		private void GenerateData()
		{
			Vector3d H = (max - min) / num;
			Vector3d currPoint = new Vector3d();
			
			Cells = new List<Cell>();

			for(int i = 0; i <= (int)num.X; i++)
			{
				currPoint.X = min.X + H.X * i;
				for (int j = 0; i <= (int)num.Y; j++)
				{
					currPoint.Y = min.Y + H.Y * j;
					for (int k = 0; k <= (int)num.X; k++)
					{
						currPoint.Z = min.Z + H.Z * k;
						nodes.Add(currPoint);
					}
				}
			}			
		}

		public void Calculate()
		{
			GenerateData();

			Cells = new List<Cell>((int)Vector3d.UnaryMult(num));

			for (int i = 0, m = 0; i < (int)num.X; i++)
			{
				for (int j = 0; i < (int)num.Y; j++)
				{
					for (int k = 0; k < (int)num.X; k++, m++)
					{						
						Cells[m].Nodes[0] = nodes[(i * ((int)num.Y + 1) + j) * ((int)num.Z + 1) + k];
						Cells[m].Nodes[1] = nodes[((i + 1) * ((int)num.Y + 1) + j) * ((int)num.Z + 1) + k];
						Cells[m].Nodes[2] = nodes[(i * ((int)num.Y + 1) + j + 1) * ((int)num.Z + 1) + k];
						Cells[m].Nodes[3] = nodes[((i + 1) * ((int)num.Y + 1) + j + 1) * ((int)num.Z + 1) + k];
						Cells[m].Nodes[4] = nodes[(i * ((int)num.Y + 1) + j) * ((int)num.Z + 1) + k + 1];
						Cells[m].Nodes[5] = nodes[((i + 1) * ((int)num.Y + 1) + j) * ((int)num.Z + 1) + k + 1];
						Cells[m].Nodes[6] = nodes[(i * ((int)num.Y + 1) + j + 1) * ((int)num.Z + 1) + k + 1];
						Cells[m].Nodes[7] = nodes[((i + 1) * ((int)num.Y + 1) + j + 1) * ((int)num.Z + 1) + k + 1];



						if (i > 0)
							Cells[m].Sides[0] = Cells[((i - 1) * (int)num.Y + j) * (int)num.Z + k];
						if (i < (int)num.X - 1)
							Cells[m].Sides[1] = Cells[((i + 1) * (int)num.Y + j) * (int)num.Z + k];
						if (j > 0)
							Cells[m].Sides[2] = Cells[(i * (int)num.Y + j - 1) * (int)num.Z + k];
						if (j < (int)num.Y - 1)
							Cells[m].Sides[3] = Cells[(i * (int)num.Y + j + 1) * (int)num.Z + k];
						if (k > 0)
							Cells[m].Sides[4] = Cells[(i * (int)num.Y + j) * (int)num.Z + k - 1];
						if (k < (int)num.Z - 1)
							Cells[m].Sides[5] = Cells[(i * (int)num.Y + j) * (int)num.Z + k + 1];
					}
				}
			}

		}
	}
	//public class Direct
	//{				
	//	public void Calculate(List<Cell> cells, List<Receiver> receivers)
	//	{
	//		double r, square, x, y, coef, bx, by;

	//		foreach (var receiver in receivers)
	//		{
	//			foreach (var cell in cells)
	//			{
	//				r = GetR(cell, receiver);
	//				square = GetCellArea(cell);
	//				x = receiver.XY.X - cell.Center.X;
	//				y = receiver.XY.Y - cell.Center.Y;
	//				coef = square * cell.I / (4 * Math.PI * Math.Pow(r, 2));
					
	//				bx = coef * cell.P.X * 3 * Math.Pow(x, 2) / Math.Pow(r, 2)
	//					+ cell.P.Y * 3 * x * y / Math.Pow(r, 2);
	//				by = coef * cell.P.X * 3 * x * y / Math.Pow(r, 2)
	//					+ cell.P.Y * (3 * Math.Pow(y, 2) / Math.Pow(r, 2) - 1);

	//				receiver.B = new Point(bx, by);
	//			}
	//		}			
	//	}

	//	private double GetR(Cell cell, Receiver receiver)
	//	{
	//		return Math.Sqrt(Math.Pow(cell.Center.X - receiver.XY.X, 2) + Math.Pow(cell.Center.Y - receiver.XY.Y, 2));
	//	}

	//	private double GetCellArea(Cell cell)
	//	{
	//		return (cell.Nodes[1].X - cell.Nodes[0].X) * (cell.Nodes[1].Y - cell.Nodes[0].Y);
	//	}
	//}
}
