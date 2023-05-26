using System;

namespace WPFLab3.Model
{
	public class Cell
	{
		#region values
		public double I { get; set; }
		public int NumCell { get; set; }
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
		#endregion

		public Cell(double i, Vector3d center, Vector3d p, Vector3d[] nodes)
		{
			I = i;
			Center = center;
			P = p;
			Nodes = nodes;
		}		

		public Vector3d GetB(Vector3d point)
		{
			double r, r1, mes;
			Vector3d dXYZ;
			Vector3d res = new Vector3d();
			for (int i = 0; i < 27; i++)
			{
				dXYZ = point - GaussPoints[i];
				r = Vector3d.Norm(dXYZ);
				r1 = 1 / Math.Pow(r, 2);
				mes = J * GaussWeights[i] / (4 * Math.PI * Math.Pow(r, 3));
				res.X += mes * (P.X * (3.0 * dXYZ.X * dXYZ.X * r1 - 1.0) + P.Y * (3.0 * dXYZ.X * dXYZ.Y * r1) + P.Z * (3.0 * dXYZ.X * dXYZ.Z * r1));
				res.Y += mes * (P.X * (3.0 * dXYZ.X * dXYZ.Y * r1) + P.Y * (3.0 * dXYZ.Y * dXYZ.Y * r1 - 1.0) + P.Z * (3.0 * dXYZ.Y * dXYZ.Z * r1));
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

			for (int i = 0; i < GaussWeights.Length; i++)
			{
				GaussPoints[i] = (GaussPoints[i] + 1.0) * H / 2.0 + Nodes[0];
			}
		}

		public bool Inside(Vector3d min, Vector3d max) => this.Center.X >= min.X && this.Center.X <= max.X &&
														  this.Center.Y >= min.Y && this.Center.Y <= max.Y &&
														  this.Center.Z >= min.Z && this.Center.Z <= max.Z;
	}
}
