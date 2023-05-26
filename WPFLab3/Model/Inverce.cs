using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WPFLab3.Model
{
	public class Inverce
	{		
		private int N, K;
		private List<List<double>> A { get; set; }
		private List<double> B { get; set; }
		private List<List<Vector3d>> L { get; set; }
		private List<Receiver> receivers { get; set; }
		private List<Vector3d> gamma { get; set; }
		private List<Cell> cells { get; set; }
		private Config config;
		public Inverce(List<Receiver> receivers, List<Cell> cells, Config config)
		{
			this.receivers = receivers;
			this.cells = cells;
			K = receivers.Count();
			N = cells.Count();
			this.config = config;			
		}

		public void Calculate()
		{
			double alpha, funCurr, funPrev;
			MakeL();
			MakeA();
			MakeB();

			List<List<double>> matrix = A;
			List<List<double>> matrixAlpha = A;
			List<double> b = B;
			List<double> X;
			

			X = Solver(matrix, b);			

			if(config.UseAlpha)
			{
				alpha = config.Alpha0;
				funCurr = funPrev = FuncFi(X);

				while(funPrev * config.AlphaCoeff >= funCurr)
				{
					matrix = A;
					b = B;
					for (int i = 0; i < A.Count(); i++)
						A[i][i] += alpha;
					X = Solver(matrix, b);
					funCurr = FuncFi(X);
					alpha *= config.dAlpha;
				}
			}
			else
			{
				alpha = 0;
				funCurr = funPrev = FuncFi(X);
			}

			alpha /= config.dAlpha;

			for(int i = 0; i < matrix.Count(); i++)
			{
				matrixAlpha[i][i] += alpha;
			}

			gamma = new List<Vector3d>();
			for(int i = 0; i < K; i++)
			{
				gamma.Add(new Vector3d(config.Gamma0, config.Gamma0, config.Gamma0));
			}

			matrix = matrixAlpha;
			b = B;
			X = Solver(matrix, b);
			funCurr = funPrev = FuncFi(X);

			if (config.UseGamma)
			{				
				bool isChanged = true;

				while ((Math.Abs(Math.Log10(funPrev) - Math.Log10(funCurr)) <= config.GammaCoeff || funCurr <= funPrev) && isChanged)
				{
					isChanged = false;

					matrix = matrixAlpha;
					b = B;
					for (int k = 0; k < K; k++)
					{
						// Сюда будем суммировать вклады в диагональ
						Vector3d add_di = new Vector3d();
						// А тут - считать число живых соседей
						double add_di_coeff = 0;
						// По всем соседям
						for (int a = 0; a < 6; a++)
						{
							// Воображаемых соседей рассматривать не будем
							if (cells[k].Sides[a] != null)
							{
								int m = cells[k].Sides[a].NumCell;
								// Диагональ отдельно будет
								if (k != m)
								{
									// Внедиагональные элементы
									Vector3d add = gamma[k] + gamma[m];
									matrix[k * 3][m * 3] -= add.X;
									matrix[k * 3 + 1][m * 3 + 1] -= add.Y;
									matrix[k * 3 + 2][m * 3 + 2] -= add.Z;
									// Кусочек диагонального элемента
									add_di += gamma[m];
									add_di_coeff++;
								}
							}
						}
						// Второй кусочек диагонального элемента
						add_di = add_di + gamma[k] * add_di_coeff;
						matrix[k * 3][k * 3] += add_di.X;
						matrix[k * 3 + 1][k * 3 + 1] += add_di.Y;
						matrix[k * 3 + 2][k * 3 + 2] += add_di.Z;
					}

					// Посчитаем СЛАУ и функционал
					X = Solver(matrix, b);
					funCurr = FuncFi(X);										

					// Посчитаем новые значения гаммы
					// По всем ячейкам
					for (int k = 0; k < K; k++)
					{
						// По всем соседям
						for (int a = 0; a < 6; a++)
						{
							// Воображаемых соседей рассматривать не будем
							if (cells[k].Sides[a] != null)
							{
								var tmpCell = cells[k].P.ToList();
								var tmpSide = cells[k].Sides[a].P.ToList();
								var tmpGamma = gamma[k].ToList();
								// По всем координатам																
								for(int g = 0; g < 3; g++)
								{ 
									// Если разница между соотв. компонентами P больше порядка
									// И при этом это не околонулевые значения
									// Тогда обновим гамму и запомним, что кто-то еще шевелится
									if (Math.Abs(Math.Log10(tmpCell[g]) - Math.Log10(tmpSide[g])) > 1.0 &&
									   Math.Abs(tmpCell[g]) > config.GammaDiff && Math.Abs(tmpSide[g]) > config.GammaDiff)
									{
										tmpGamma[g] *= config.dGamma;
										isChanged = true;
									}
								}
								gamma[k].X = tmpGamma[0];
								gamma[k].Y = tmpGamma[1];
								gamma[k].Z = tmpGamma[2];
							}
						}
					}
				}
			}
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

		private double FuncFi(List<double> X)
		{
			for(int i = 0; i < cells.Count; i++)
			{
				cells[i].P = new Vector3d
					(
						 X[3 * i],
						 X[3 * i + 1],
						 X[3 * i + 2]
					);
			}

			double f = 0;

			for(int i = 0; i < N; i++)
			{
				Vector3d tmpVec = receivers[i].B - cells[i].GetB(receivers[i].XYZ);
				f += Vector3d.Dot(tmpVec, tmpVec);
			}
			return f;
		}		

		private List<double> Solver(List<List<double>> matrix, List<double> b)
		{
			List<double> result = new List<double>();
			double tmp;

			for (int i = 0; i < N; i++)
			{
				if (matrix[i][i] != 0)
				{
					bool flag = false;
					for (int j = i + 1; j < N && !flag; j++)
					{
						if (matrix[i][j] != 0)
						{
							for (int k = i; k < N; k++)
							{
								tmp = matrix[i][k];
								matrix[i][k] = matrix[j][k];
								matrix[j][k] = tmp;
							}
							tmp = b[i];
							b[i] = b[j];
							b[j] = tmp;
							flag = true;
						}
					}
				}
				b[i] = b[i] / matrix[i][i];
				
				for(int j = N - 1, jj = N; jj > i; j--, jj--)
				{
					matrix[i][j] = matrix[i][j] / matrix[i][i];
				}

				for(int j = i + 1; j < N; j++)
				{
					b[j] -= b[i] * matrix[j][i];
					for(int k = N - 1, kk = N; kk > i; k--, kk--)
					{
						matrix[j][k] -= matrix[i][k] * matrix[j][i];
					}
				}
			}

			for(int i = N - 1; i > 0; i--)
			{
				for(int j = i - 1, jj = i; jj > 0; j--, jj--)
				{
					b[j] -= matrix[j][i] * b[i];
				}
			}

			b.CopyTo(result.ToArray());			

			return result;
		}
	}
}
