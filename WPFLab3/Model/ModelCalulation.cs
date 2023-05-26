using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using File = System.IO.File;

namespace WPFLab3.Model
{
	public enum FileType
	{
		Cells,
		Receivers,
		Config,
		Output
	}

	public class Config
	{		
		public bool UseAlpha { get; set; }
		public bool UseGamma { get; set; }
		public double Alpha0 { get; set; }
		public double dAlpha { get; set; }
		public double AlphaCoeff { get; set; }
		public double Gamma0 { get; set; }
		public double dGamma { get; set; }
		public double GammaCoeff { get; set; }
		public double GammaDiff { get; set; }		
	}

	public class ModelCalulation : ReactiveObject
	{				
		public List<Cell> Cells { get; set; }
		public List<Receiver> Receivers { get; set; }
		public Config Conf { get; set; }
		
		private Direct _direct;
		private Regex _regexCell = new Regex(@"\w*.cells.txt$");
		private Regex _regexReceiver = new Regex(@"\w*.receivers.txt$");
		private Regex _regexConfig = new Regex(@"\w*.config.txt");
		public ModelCalulation()
		{
			Cells = new List<Cell>();
			Receivers = new List<Receiver>();
		}
		
		public void StartCalulationDirect()
		{
			_direct = new Direct();
			// _direct.Calculate(Cells, Receivers);
		}

		public void SetInputCalculationData(string directoryPath)
		{
			string[] files = Directory.GetFiles(directoryPath);

			foreach (string file in files)
			{
				if (File.Exists(file))
				{

					if (_regexCell.Match(file).Success)
					{
						ReadCells(file);
					}
					else if (_regexReceiver.Match(file).Success)
					{
						ReadReceivers(file);
					}
					else if (_regexConfig.Match(file).Success)
					{
						ReadConfig(file);
					}
				}
			}
		}

		private void ReadConfig(string filePath)
		{
			using (var sr = new StreamReader(filePath))
			{				
				Conf.UseAlpha =		bool.Parse(sr.ReadLine());					
				Conf.UseGamma =		bool.Parse(sr.ReadLine());
				Conf.Alpha0 =		double.Parse(sr.ReadLine());
				Conf.dAlpha =		double.Parse(sr.ReadLine());
				Conf.AlphaCoeff =	double.Parse(sr.ReadLine());
				Conf.Gamma0 =		double.Parse(sr.ReadLine());
				Conf.dGamma =		double.Parse(sr.ReadLine());
				Conf.GammaCoeff =	double.Parse(sr.ReadLine());
				Conf.GammaDiff =	double.Parse(sr.ReadLine());
			}	
		}

		private void ReadCells(string filePath)
		{
			using (var sr = new StreamReader(filePath))
			{
				int count = int.Parse(sr.ReadLine());

				for (int i = 0; i < count; i++)
				{
					String[] arr = sr.ReadLine().Split();
					Cells.Add(new Cell
						(
							double.Parse(arr[0]), 
							new Vector3d(double.Parse(arr[1]), double.Parse(arr[2]), double.Parse(arr[3])),
							new Vector3d(double.Parse(arr[4]), double.Parse(arr[5]), double.Parse(arr[6])),
							new Vector3d[2] 
							{
								new Vector3d(double.Parse(arr[7]), double.Parse(arr[8]), double.Parse(arr[9])),
								new Vector3d(double.Parse(arr[10]), double.Parse(arr[11]), double.Parse(arr[12]))
							})
						);
				}
			}
		}

		private void ReadReceivers(string filePath)
		{
			using (var sr = new StreamReader(filePath))
			{
				int count = int.Parse(sr.ReadLine());

				for (int i = 0; i < count; i++)
				{
					String[] arr = sr.ReadLine().Split();

					Receivers.Add(new Receiver(
						new Vector3d(double.Parse(arr[0]), double.Parse(arr[1]), double.Parse(arr[2])),
						new Vector3d(double.Parse(arr[3]), double.Parse(arr[4]), double.Parse(arr[5]))));
				}
			}
		}
	}
}
