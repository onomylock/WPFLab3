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
		public ObservableCollection<ModelApp> LinesCollection;
		public ModelApp SelectedLineItem { get; set; }		
		public Regime regime = new Regime();
		private MainWindow mainWindow;
		private double X_min = 0, Y_min = 0, X_max = 0, Y_max = 0, zoomX = 1, zoomY = 1;
		private double Height;
		private double Width;

		public ViewModelApp(MainWindow mainWindow)
		{
			this.mainWindow = mainWindow;
			regime = Regime.Lines;			
			LinesCollection = new ObservableCollection<ModelApp>();
			LinesCollection.Add(new ModelApp("Legend " + (LinesCollection.Count() + 1).ToString()));
			//LinesCollection.Last().GeoObject = new ObservableCollection<Point> { new Point(0, 0), new Point(3, 4) };
			Height = mainWindow.Graphic.Height;
			Width = mainWindow.Graphic.Width;
			Draw();
		}

		private ICommand addCommand;
		public ICommand AddCommand => addCommand ?? (addCommand = new DelegateCommand(() =>
		{
			LinesCollection.Add(new ModelApp("Legend " + (LinesCollection.Count() + 1).ToString()));
			Draw();
			//mainWindow.dataGrid.RowBackground = new SolidColorBrush(LinesCollection.Last().ColorLine);
			
		}));

		private ICommand removeCommand;
		public ICommand RemoveCommand => removeCommand ?? (removeCommand = new DelegateCommand(() =>
		{
			if (SelectedLineItem != null)
			{
				LinesCollection.Remove(SelectedLineItem);
				Draw();
			}
		}));

		private ICommand addPointCommand;
		public ICommand AddPointCommand => addPointCommand ?? (addPointCommand = new DelegateCommand(() =>
		{
			if(SelectedLineItem != null)
			{
				SelectedLineItem.GeoObject.Add(new Point());
				//DrawLines(SelectedLineItem);
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

		public void SetMousePosition(System.Windows.Point Point)
		{
			int lenX = 3;
			int lenY = 3;

			double X = Point.X;
			double Y = Point.Y;

			if(LinesCollection.Count > 0)
			{
				X = X / zoomX;
				Y = (Height - Y) / zoomY;

				double xdiv = (X_max - X_min) / 10;
				lenX = xdiv.ToString().Length + 1;
				double ydiv = (Y_max - Y_min) / 10;
				lenX = ydiv.ToString().Length + 1;
			}

			X = Math.Round(X, lenX);
			Y = Math.Round(Y, lenY);
			var _X = Convert.ToSingle(X).ToString();
			var _Y = Convert.ToSingle(Y).ToString();
			if (_X.Length > lenX) _X = _X.Substring(0, lenX);
			if (_Y.Length > lenY) _Y = _Y.Substring(0, lenY);
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
				//double Height = mainWindow.StackGraphic.Height;
				//double Width = mainWindow.StackGraphic.Width;

				double dX = Width / 10;
				double dY = Height / 10;

				double dAxis = Math.Abs(max - min) / 10;				
				if (AxisFlag)
				{					
					for (int i = 0; i < 10; i++)
					{
						double cur = min + i * dAxis;
						string text = cur.ToString();

						if (text.Length > 5)
							text = text.Substring(0, 5);						
						
						Text(Axis, i * dX - 2, 8, text, Color.FromRgb(0, 0, 0));													
					}
				}
				else
				{						
					for (int i = 10; i > 0; i--)
					{
						double cur = max - min - i * dAxis;
						string text = cur.ToString();

						if (text.Length > 5)
							text = text.Substring(0, 5);						
						
						Text(Axis, 15, i * dY - 12, text, Color.FromRgb(0, 0, 0));						
					}
				}								
			}
		}

		private void boundsCalculation()
		{
			Point Min, Max;

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
			double dX = Width / 10;
			double dY = Height / 10;

			for (int i = 0; i < 10; ++i)
			{
				Line myLine = new Line();
				myLine.Stroke = System.Windows.Media.Brushes.LightGray;				
				double x_cur = i * dX;
				myLine.X1 = x_cur;
				myLine.Y1 = 0;
				myLine.X2 = x_cur;
				myLine.Y2 = Height;
				mainWindow.Graphic.Children.Add(myLine);
			}

			for (int i = 0; i < 10; ++i)
			{
				Line myLine = new Line();
				myLine.Stroke = System.Windows.Media.Brushes.LightGray;
				double y_cur = i * dY;
				myLine.X1 = 0;
				myLine.Y1 = y_cur;
				myLine.X2 = Width;
				myLine.Y2 = y_cur;
				mainWindow.Graphic.Children.Add(myLine);
			}
		}

		private void DrawLines(ModelApp modelApp)
		{						
			double dx = X_min < 0 ? X_min : 0;
			double dy = Y_min < 0 ? Y_min : 0;			

			if(modelApp.GeoObject != null && modelApp.GeoObject.Count > 0)
			{
				
				Polyline myPolyline = new Polyline();
				//myPolyline.Stroke = new SolidColorBrush(modelApp.ColorLine);
				myPolyline.Stroke = modelApp.ColorLine;
				myPolyline.StrokeThickness = 3;
				myPolyline.FillRule = FillRule.EvenOdd;				
				PointCollection myPointCollection = new PointCollection();

				var SortPoints = modelApp.SortedPoints();

				foreach (Point item in SortPoints)
				{
					myPointCollection.Add(new System.Windows.Point((item.X - dx) * zoomX, (Height - (item.Y - dy) * zoomY)));
				}
				myPolyline.Points = myPointCollection;
				mainWindow.Graphic.Children.Add(myPolyline);
			}
		}

		private void DrawSpline(ModelApp modelApp)
		{
		//	double dx = X_min < 0 ? X_min : 0;
		//	double dy = Y_min < 0 ? Y_min : 0;

		//	if (modelApp.GeoObject != null && modelApp.GeoObject.Count > 0)
		//	{

		//		//Polyline myPolyline = new Polyline();
		//		////myPolyline.Stroke = new SolidColorBrush(modelApp.ColorLine);
		//		//myPolyline.Stroke = modelApp.ColorLine;
		//		//myPolyline.StrokeThickness = 3;
		//		//myPolyline.FillRule = FillRule.EvenOdd;
		//		//PointCollection myPointCollection = new PointCollection();

		//		var SortPoints = modelApp.SortedPoints();
		//		List<ArcSegment> arcSegments = new List<ArcSegment>();

		//		foreach (Point item in SortPoints)
		//		{
					
		//			//myPointCollection.Add(new System.Windows.Point((item.X - dx) * zoomX, (Height - (item.Y - dy) * zoomY)));
		//		}
		//		Pen pen = new Pen(modelApp.ColorLine, 3);
				


		//		//System.Windows.Shapes.Path.DataProperty.PropertyType.
		//		//System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
				
		//		mainWindow.Graphic.Children.Add();



		//		//myPolyline.Points = myPointCollection;
		//		//mainWindow.Graphic.Children.Add(myPolyline);
		//	}
		}



		public void Draw()
		{
			if (mainWindow.Graphic.Children.Count > 0) mainWindow.Graphic.Children.Clear();
			boundsCalculation();
			RecalculateZoom();
			DrawGrid();
			//

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
				case Regime.Spline:
					{
						foreach (ModelApp item in LinesCollection)
						{
							DrawSpline(item);
						}
						break;
					}
				default:
					break;
			}
					
			DrawAxis(mainWindow.OX, X_min, X_max, true);
			DrawAxis(mainWindow.OY, Y_min, Y_max, false);			
		}
	}
}
