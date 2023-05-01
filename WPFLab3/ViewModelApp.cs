using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using ReactiveUI;
using System.Security.Cryptography;

namespace WPFLab3
{
	public enum Regime
	{
		Lines, Spline
	}

	public class ViewModelApp : ReactiveObject
	{
		public object CurrentTab { get; set; }
		public ObservableCollection<ModelApp> LinesCollection;
		public System.Windows.Point CurrentPoint;
		public System.Windows.Point ButtonDownPoint;
		private List<double> gridX, gridY;
		private System.Windows.Point dPoint;
		public ModelApp SelectedLineItem { get; set; }		
		public Regime regime = new Regime();
		private MainWindow mainWindow;
		private double X_min = 0, Y_min = 0, X_max = 0, Y_max = 0, zoomX = 1, zoomY = 1;
		private double Height;
		private double Width;
		private ModelCalulation modelCalulation = new ModelCalulation();
		public bool MouseDown { get; set; } = false;
		public double Scale { get; set; } = 1;

		public ViewModelApp(MainWindow mainWindow)
		{
			this.mainWindow = mainWindow;
			regime = Regime.Lines;			
			LinesCollection = new ObservableCollection<ModelApp>();
			LinesCollection.Add(new ModelApp("Legend " + (LinesCollection.Count() + 1).ToString()));
			LinesCollection.Last().GeoObject = new ObservableCollection<Point> { new Point(1, 2), new Point(9, 7) };
			dPoint = new System.Windows.Point(0, 0);
			Height = mainWindow.GraphicCurve.Height;
			Width = mainWindow.GraphicCurve.Width;
			Draw();						
		}

		private ICommand startDirectionCalulation;
		public ICommand StartDirectionCalulation => startDirectionCalulation ?? (startDirectionCalulation = new DelegateCommand(() =>
		{
			modelCalulation.StartCalulationDirect();
		}));

		private ICommand startInverceCalulation;
		public ICommand StartInverceCalulation => startInverceCalulation ?? (startInverceCalulation = new DelegateCommand(() =>
		{

		}));

		private ICommand setDirectoryCalulationCommand;
		public ICommand SetDirectoryCalulationCommand => setDirectoryCalulationCommand ?? (setDirectoryCalulationCommand = new DelegateCommand(() =>
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult result = dialog.ShowDialog();
				modelCalulation.SetInputCalculationData(dialog.SelectedPath);				
			}
		}));

		private ICommand addCommand;
		public ICommand AddCommand => addCommand ?? (addCommand = new DelegateCommand(() =>
		{
			if (!MouseDown)
			{
				LinesCollection.Add(new ModelApp("Legend " + (LinesCollection.Count() + 1).ToString()));
				SetStartView();
				Draw();
			}									
		}));

		private ICommand removeCommand;
		public ICommand RemoveCommand => removeCommand ?? (removeCommand = new DelegateCommand(() =>
		{
			if (SelectedLineItem != null)
			{
				LinesCollection.Remove(SelectedLineItem);
				boundsCalculation();
				SetStartView();				
				Draw();
				SelectedLineItem = null;
			}
		}));

		private ICommand addPointCommand;
		public ICommand AddPointCommand => addPointCommand ?? (addPointCommand = new DelegateCommand(() =>
		{
			if(SelectedLineItem != null)
			{
				SelectedLineItem.GeoObject.Add(new Point());				
			}			
		}));

		private ICommand openFileCommand;
		public ICommand OpenFileCommand => openFileCommand ?? (openFileCommand = new DelegateCommand(() =>
		{
			var dialog = new Microsoft.Win32.OpenFileDialog();
			dialog.Multiselect = true;
			dialog.FileName = "Document";
			dialog.DefaultExt = ".txt";
			dialog.Filter = "Text documents (.txt)|*.txt";

			bool? result = dialog.ShowDialog();

			if (result == true)
			{
				string filename = dialog.FileName;
				if (File.Exists(filename))
				{
					ObservableCollection<Point> points = new ObservableCollection<Point>();
					foreach (string line in File.ReadLines(filename))
					{
						var tmp = line.Split();
						points.Add(new Point(double.Parse(tmp[0]), double.Parse(tmp[1])));						
					}
					LinesCollection.Add(new ModelApp(points, "Legend " + (LinesCollection.Count() + 1).ToString()));
				}
				boundsCalculation();
				Draw();
			}
		}));

		private ICommand saveFileCommand;
		public ICommand SaveFileCommand => saveFileCommand ?? (saveFileCommand = new DelegateCommand(() =>
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "Text documents (.txt)|*.txt";
			dialog.DefaultExt = ".txt";
			bool? result = dialog.ShowDialog();

			if (result == true)
			{
				using (StreamWriter sw = File.CreateText(dialog.FileName))
				{
					foreach (Point item in SelectedLineItem.GeoObject)
					{
						sw.WriteLine(item.X.ToString() + "\t" + item.Y.ToString());
					}					
				}
			}			
		}));

		private void Text(Canvas canvasObj, double x, double y, string text, Color color)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Text = text;
			textBlock.Foreground = new SolidColorBrush(color);
			Canvas.SetLeft(textBlock, x);
			Canvas.SetTop(textBlock, y);
			canvasObj.Children.Add(textBlock);
		}

		private void CalculateGrid()
		{
			boundsCalculation();
			double cur_x = X_min, cur_y = Y_min;
			gridX = new List<double>();
			gridY = new List<double>();
			
			double stepX = Math.Pow(10, Math.Floor(Math.Log10((X_max - X_min - 1))));
			double stepY = Math.Pow(10, Math.Floor(Math.Log10((Y_max - Y_min - 1))));

			if (stepX == 0) stepX = 2;
			if (stepY == 0) stepY = 2;

			while (cur_x <= X_max + 1)
			{
				gridX.Add(cur_x);
				cur_x += stepX;
			}

			while (cur_y <= Y_max + 1)
			{
				gridY.Add(cur_y);
				cur_y += stepY;
			}
		}

		public void SetMousePosition(System.Windows.Point Point)
		{

			double X = Point.X;
			double Y = Point.Y;

			if(LinesCollection.Count > 0)
			{
				X = (X / Scale + dPoint.X) / (zoomX );
				Y = (Height - Y / Scale - dPoint.Y) / (zoomY );

				double xdiv = (X_max - X_min) / gridX.Count;				
				double ydiv = (Y_max - Y_min) / gridY.Count;				
			}
			var _X = Convert.ToSingle(X).ToString();
			var _Y = Convert.ToSingle(Y).ToString();			
			mainWindow.LabelX.Content = _X;
			mainWindow.LabelY.Content = _Y;
		}

		private void RecalculateZoom()
		{			
			zoomX = Width / Math.Abs(X_max - X_min);
			zoomY = Height / Math.Abs(Y_max - Y_min);
		}

		private void DrawAxis(Canvas Axis, double min, double max, bool AxisFlag)
		{
			if (LinesCollection.Count > 0)
			{

				if (Axis.Children.Count > 0) Axis.Children.Clear();

				if (AxisFlag)
				{
					foreach (double x in gridX)
					{
						//if (x * zoomX < Width - 5 && x * zoomX > 10)
							Text(Axis, (x * zoomX - 2 - dPoint.X) * Scale, 8, x.ToString(), Color.FromRgb(0, 0, 0));
					}
				}
				else
				{
					foreach (double y in gridY)
					{
						//if (y * zoomY < Height - 5 && y * zoomY > 5)
							Text(Axis, 15, (Height - y * zoomY - 5 - dPoint.Y) * Scale, y.ToString(), Color.FromRgb(0, 0, 0));
					}
				}
			}
		}

		private void boundsCalculation()
		{
			Point Min, Max;

			X_min = 0;
			Y_min = 0;
			X_max = 0;
			Y_max = 0;

			foreach (ModelApp item in LinesCollection)
			{
				
				(Min, Max) = item.FindMinMax();

				if(Min != null && Max != null)
				{
					X_min = Min.X < X_min ? Min.X : X_min;
					Y_min = Min.Y < Y_min ? Min.Y : Y_min;
					X_max = Max.X > X_max ? Max.X : X_max;
					Y_max = Max.Y > Y_max ? Max.Y : Y_max;					
				}
			}
			RecalculateZoom();
		}
					
		private void DrawGrid()
		{
			dPoint.X += (CurrentPoint.X - ButtonDownPoint.X) / Width * 20;
			dPoint.Y += (CurrentPoint.Y - ButtonDownPoint.Y) / Height * 20;
			double dX = Width / gridX.Count;
			double dY = Height / gridY.Count;
			
			for (int i = 0; i < gridX.Count; ++i)
			{
				Line myLine = new Line();
				myLine.Stroke = System.Windows.Media.Brushes.LightGray;				
				double x_cur = gridX[i];
				myLine.X1 = (x_cur * zoomX - dPoint.X) * Scale;				
				myLine.Y1 = 0;
				myLine.X2 = (x_cur * zoomX - dPoint.X) * Scale;				
				myLine.Y2 = Height;
				mainWindow.GraphicCurve.Children.Add(myLine);
			}

			for (int i = 0; i < gridY.Count; ++i)
			{
				Line myLine = new Line();
				myLine.Stroke = System.Windows.Media.Brushes.LightGray;				
				double y_cur = gridY[i];
				myLine.X1 = 0;
				myLine.Y1 = (Height - y_cur * zoomY - dPoint.Y) * Scale;
				myLine.X2 = Width;
				myLine.Y2 = (Height - y_cur * zoomY  - dPoint.Y) * Scale;
				mainWindow.GraphicCurve.Children.Add(myLine);
			}
		}

		private void DrawLines(ModelApp modelApp)
		{						
			double dx = X_min < 0 ? X_min : 0;
			double dy = Y_min < 0 ? Y_min : 0;			

			if(modelApp.GeoObject != null && modelApp.GeoObject.Count > 0)
			{				
				Polyline myPolyline = new Polyline();				
				myPolyline.Stroke = modelApp.ColorLine;
				myPolyline.StrokeThickness = 3;
				myPolyline.FillRule = FillRule.EvenOdd;				
				PointCollection myPointCollection = new PointCollection();

				var SortPoints = modelApp.SortedPoints();

				foreach (Point item in SortPoints)
				{
					myPointCollection.Add(new System.Windows.Point(((item.X - dx) * zoomX - dPoint.X) * Scale, ((Height - (item.Y - dy) * zoomY) - dPoint.Y) * Scale));
				}
				myPolyline.Points = myPointCollection;
				mainWindow.GraphicCurve.Children.Add(myPolyline);
			}
		}		

		public void Draw()
		{
			if (mainWindow.GraphicCurve.Children.Count > 0) mainWindow.GraphicCurve.Children.Clear();
			boundsCalculation();
			CalculateGrid();
			RecalculateZoom();
			DrawGrid();			

			switch (regime)
			{
				case Regime.Lines:
					{
						foreach (ModelApp item in LinesCollection)
						{
							DrawLines(item);
						}
						break;
					}									
				default:
					break;
			}
					
			DrawAxis(mainWindow.OXCurve, X_min, X_max, true);
			DrawAxis(mainWindow.OYCurve, Y_min, Y_max, false);			
		}

		public void SetStartView()
		{
			dPoint.X = 0;
			dPoint.Y = 0;
			Scale = 1;
			boundsCalculation();
			Draw();
		}		
	}
}
