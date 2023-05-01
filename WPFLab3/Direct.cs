using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace WPFLab3
{
	public class Direct
	{				
		public void Calculate(List<Cell> cells, List<Receiver> receivers)
		{
			double r, square, x, y, coef;

			foreach (Receiver receiver in receivers)
			{
				foreach (Cell cell in cells)
				{
					r = GetR(cell, receiver);
					square = GetCellArea(cell);
					x = receiver.XY.X - cell.Center.X;
					y = receiver.XY.Y - cell.Center.Y;
					coef = square * cell.I / (4 * Math.PI * Math.Pow(r, 2));

					receiver.B.X = coef * cell.P.X * 3 * Math.Pow(x, 2) / Math.Pow(r, 2)
						+ cell.P.Y * 3 * x * y / Math.Pow(r, 2);
					receiver.B.Y = coef * cell.P.X * 3 * x * y / Math.Pow(r, 2)
						+ cell.P.Y * (3 * Math.Pow(y, 2) / Math.Pow(r, 2) - 1);
				}
			}			
		}

		private double GetR(Cell cell, Receiver receiver)
		{
			return Math.Sqrt(Math.Pow(cell.Center.X - receiver.XY.X, 2) + Math.Pow(cell.Center.Y - receiver.XY.Y, 2));
		}

		private double GetCellArea(Cell cell)
		{
			return (cell.Nodes[1].X - cell.Nodes[0].X) * (cell.Nodes[1].Y - cell.Nodes[0].Y);
		}
	}
}
