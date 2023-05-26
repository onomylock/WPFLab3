using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFLab3.Model
{
	public class Vector3d
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public Vector3d() { }
		public Vector3d(Point XY, double z)
		{
			X = XY.X;
			Y = XY.Y;
			Z = z;
		}

		public Vector3d(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Point GetPoint(Axis axis)
		{
			Point point = new Point();
			switch (axis)
			{
				case Axis.XY:
					{
						point.X = X;
						point.Y = Y;
						break;
					}
				case Axis.XZ:
					{
						point.X = X;
						point.Y = Z;
						break;
					}
				case Axis.YZ:
					{
						point.X = Y;
						point.Y = Z;
						break;
					}
			}
			return point;
		}

		public List<double> ToList() => new List<double> { this.X, this.Y, this.Z };

		#region operators
		public static double Dot(Vector3d vec1, Vector3d vec2) => vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
		public static double Norm(Vector3d point) => Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2) + Math.Pow(point.Z, 2));
		public static double UnaryMult(Vector3d point) => point.X * point.Y * point.Z;
		public static Vector3d operator +(Vector3d vec1, Vector3d vec2) => new Vector3d(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
		public static Vector3d operator -(Vector3d vec1, Vector3d vec2) => new Vector3d(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
		public static Vector3d operator *(Vector3d vec1, Vector3d vec2) => new Vector3d(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);
		public static Vector3d operator /(Vector3d vec1, Vector3d vec2) => new Vector3d(vec1.X / vec2.X, vec1.Y / vec2.Y, vec1.Z / vec2.Z);
		public static Vector3d operator +(Vector3d vec, double b) => new Vector3d(vec.X + b, vec.Y + b, vec.Z + b);
		public static Vector3d operator /(Vector3d vec, double b) => new Vector3d(vec.X / b, vec.Y / b, vec.Z / b);
		public static Vector3d operator *(Vector3d vec, double b) => new Vector3d(vec.X * b, vec.Y * b, vec.Z * b);
		public static Vector3d operator -(Vector3d vec, double b) => new Vector3d(vec.X - b, vec.Y - b, vec.Z - b);
		#endregion
	}
}
