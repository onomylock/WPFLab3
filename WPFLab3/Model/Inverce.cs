using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WPFLab3
{
	public class Inverce
	{
		public bool UseAlpha { get; set; }
		public double Alpha { get; set; }
		public bool UseGamma { get; set; }
		public List<Vector3d> Gamma { get; set; }
		public double dAlpha { get; set; }
		public double dGamma { get; set; }
		private int N, K;
		private List<List<double>> A { get; set; }
		private double[] B { get; set; }
		private List<List<Vector3d>> L { get; set; }
		private List<Receiver> receivers { get; set; }
		private List<Cell> cells { get; set; }

		public Inverce(List<Receiver> receivers, int N)
		{
			this.receivers = receivers;
			K = receivers.Count();
			this.N = N;
			//A = new double[3 * K, 3 * K];
			//L = new Vector3[3 * K, N];
		}

		public void Calculate()
		{

		}

		private void Solver()
		{

		}

		private void MakeL()
		{
			double r, r1, tmp;
			for(int i = 0; i < K; i++)
			{
				for(int j = 0; j < N; j++)
				{
					for(int k = 0; k < 27; k++)
					{
						Vector3d dVec = new Vector3d
						(
							receivers[j].XYZ.X - cells[i].GaussPoints[k].X,
							receivers[j].XYZ.Y - cells[i].GaussPoints[k].Y,
							receivers[j].XYZ.Z - cells[i].GaussPoints[k].Z
						);
						r = Math.Sqrt(Vector3d.Dot(dVec, dVec));
						r1 = 1 / Math.Pow(r, 2);
						tmp = cells[i].J * cells[i].GaussWeights[k] / (4 * Math.PI * Math.Pow(r, 3));

						L[3 * i][j] += new Vector3d
						(
							tmp * (3 * Math.Pow(dVec.X, 2) * r1 - 1),
							tmp * (3 * dVec.Y * dVec.X * r1),
							tmp * (3 * dVec.Z * dVec.X * r1)
						);

						L[3 * i + 1][j] += new Vector3d
						(
							tmp * (3 * dVec.Y * dVec.X * r1),
							tmp * (3 * Math.Pow(dVec.Y, 2) * r1 - 1),
							tmp * (3 * dVec.Z * dVec.Y * r1)
						);

						L[3 * i + 2][j] += new Vector3d
						(
							tmp * (3 * dVec.X * dVec.Z * r1),
							tmp * (3 * dVec.Y * dVec.Z * r1),
							tmp * (3 * Math.Pow(dVec.Z, 2) * r1 - 1)
						);
					}
				}
			}
		}

		private void MakeA()
		{
			for(int i = 0; i < A.Count(); i++)
			{
				for(int j = 0; j < A[i].Count; j++)
				{
					A[i][j] = 0;
					for(int k = 0; k < N; k++)
					{
						A[i][j] += Vector3d.Dot(L[i][k], L[j][k]);						
					}
				}
			}

		}

		private void MakeB()
		{
			for(int i = 0; i < B.Count(); i++)
			{
				B[i] = 0;
				for(int j = 0; j < N; j++)
				{
					B[i] += Vector3d.Dot(L[i][j], receivers[j].B);
				}
			}
		}
	}
}
