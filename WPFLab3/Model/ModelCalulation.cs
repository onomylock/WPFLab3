using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace WPFLab3
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

		public static double Dot(Vector3d vec1, Vector3d vec2) => vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
		public static double Norm(Vector3d point) => Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2) + Math.Pow(point.Z, 2));
		public static double UnaryMult(Vector3d point) => point.X * point.Y * point.Z;
		public static Vector3d operator +(Vector3d vec1, Vector3d vec2) => new Vector3d(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
		public static Vector3d operator -(Vector3d vec1, Vector3d vec2) => new Vector3d(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
		public static Vector3d operator *(Vector3d vec1, Vector3d vec2) => new Vector3d(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);
		public static Vector3d operator /(Vector3d vec1, Vector3d vec2) => new Vector3d(vec1.X / vec2.X, vec1.Y / vec2.Y, vec1.Z / vec2.Z);
		public static Vector3d operator +(Vector3d vec, double b) => new Vector3d(vec.X + b, vec.Y + b, vec.Z + b);
		public static Vector3d operator /(Vector3d vec, double b) => new Vector3d(vec.X / b, vec.Y / b, vec.Z / b);
		public static Vector3d operator -(Vector3d vec, double b) => new Vector3d(vec.X - b, vec.Y - b, vec.Z - b);		

	}
	public class Cell
	{
		public double I { get; set; }
		public Vector3d Center { get; set; } = new Vector3d();
		public Vector3d P { get; set; } = new Vector3d();
		public Vector3d[] Nodes { get; set; } = new Vector3d[8];
		public Cell[] Sides { get; set; } = new Cell[6];
		public double[] GaussWeights { get; set; } = new double[27];
		public Vector3d[] GaussPoints { get; set; } = new Vector3d[27];
		public double J { get; set; }
		public double Mes { get; set; }
		private static double[] gaussPointsCoeff = new double[3] 
		{
			-Math.Sqrt(3 / 5),
			0,
			Math.Sqrt(3 / 5)
		};

		private static double[] gaussWeightsCoeff = new double[3]
		{
			5.0 / 9.0,
			8.0 / 9.0,
			5.0 / 9.0
		};

		public Cell(double i, Vector3d center, Vector3d p, Vector3d[] nodes)
		{
			I = i;
			Center = center;
			P = p;
			Nodes = nodes;
		}

		public Cell(){}

		public Vector3d GetB(Vector3d point)
		{
			double r, r1, mes;
			Vector3d dXYZ;
			Vector3d res = new Vector3d();
			for(int i = 0; i < 27; i++)
			{
				dXYZ = point - GaussPoints[i];
				r = Vector3d.Norm(dXYZ);
				r1 = 1 / Math.Pow(r, 2);
				mes = J * GaussWeights[i] / (4 * Math.PI * Math.Pow(r, 3));
				res.X += mes * (P.X * (3.0 * dXYZ.X * dXYZ.X * r1 - 1.0) + P.Y * (3.0 * dXYZ.X * dXYZ.Y * r1) + P.Z * (3.0 * dXYZ.X * dXYZ.Z * r1));
				res.Y += mes * (P.X * (3.0 * dXYZ.X * dXYZ.Y * r1) + P.Y * (3.0 * dXYZ.Y * dXYZ.Y * r1 - 1.0) + p.z * (3.0 * dXYZ.Y * dXYZ.Z * r1));
				res.Z += mes * (P.X * (3.0 * dXYZ.X * dXYZ.Z * r1) + P.Y * (3.0 * dXYZ.Y * dXYZ.Z * r1) + P.Z * (3.0 * dXYZ.Z * dXYZ.Z * r1 - 1.0));
			}
			return res;
		}

		public void InitCell()
		{
			Vector3d H = new Vector3d(Nodes[1].X - Nodes[0].X, 
									  Nodes[2].Y - Nodes[0].Y, 
									  Nodes[4].Z - Nodes[0].Z);

			Mes = Vector3d.UnaryMult(H);
			Center = new Vector3d(Nodes[0].X + H.X / 2, Nodes[0].Z + H.Z / 2, Nodes[0].Z + H.Z / 2);

			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++)
					for (int k = 0; k < 3; k++)
					{
						GaussWeights[(i * 3 + j) * 3 + k] = gaussWeightsCoeff[i] * gaussWeightsCoeff[j] * gaussWeightsCoeff[k];
						GaussPoints[(i * 3 + j) * 3 + k] = new Vector3d(gaussPointsCoeff[i], gaussPointsCoeff[j], gaussPointsCoeff[k]);
					}
			J = Mes / 8;

			for(int i = 0; i < GaussWeights.Length; i++)
			{
				GaussPoints[i] = (GaussPoints[i] + 1.0) * H / 2.0 + Nodes[0];
			}
		}

		public bool Inside(Vector3d min, Vector3d max) => this.Center.X >= min.X && this.Center.X <= max.X &&
														  this.Center.Y >= min.Y && this.Center.Y <= max.Y &&
														  this.Center.Z >= min.Z && this.Center.Z <= max.Z;
	}

	public class Receiver
	{
		public Vector3d B { get; set; } = new Vector3d();
		public Vector3d XYZ { get; set; } = new Vector3d();

		public Receiver(Vector3d b, Vector3d xY)
		{
			B = b;
			XYZ = xY;
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
								Cells.Add(new Cell(double.Parse(arr[0]), new Vector3d(double.Parse(arr[1]), double.Parse(arr[2]), 0),
									new Vector3d(double.Parse(arr[3]), double.Parse(arr[4])), 
									new Vector3d[2] { new Vector3d(double.Parse(arr[5]), double.Parse(arr[6])), 
												   new Vector3d(double.Parse(arr[7]), double.Parse(arr[8]))}));
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
								Receivers.Add(new Receiver(new Vector3d(double.Parse(arr[0]), double.Parse(arr[1])),
														   new Vector3d(double.Parse(arr[2]), double.Parse(arr[3]))));
							}
						}
					}
				}				
			}
		}		
	}
}
