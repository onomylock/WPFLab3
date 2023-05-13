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
	public enum Tab
	{
		Curve, View2D
	}

	public class ViewModelApp : ReactiveObject
	{
		public ObservableCollection<TabViewModelBase> Tabs;
		public Tab CurrentTab { get; set; }
		public ObservableCollection<ViewModelTab> viewModelTabs { get; set; }
		public ObservableCollection<ModelApp> LinesCollection;
		//public System.Windows.Point CurrentPoint;
		//public System.Windows.Point ButtonDownPoint;
		//private List<double> gridX, gridY;
		//private System.Windows.Point dPoint;
		//public ModelApp SelectedLineItem { get; set; }
		private MainWindow mainWindow;

		//private double X_min = 0, Y_min = 0, X_max = 0, Y_max = 0, zoomX = 1, zoomY = 1;
		//private double Height;
		//private double Width;
		private ModelCalulation modelCalulation = new ModelCalulation();
		//public bool MouseDown { get; set; } = false;
		//public double Scale { get; set; } = 1;

		public ViewModelApp(MainWindow mainWindow)
		{
			this.mainWindow = mainWindow;
			viewModelTabs = new ObservableCollection<ViewModelTab>();
			viewModelTabs.Add(new ViewModelCurve(mainWindow));
			viewModelTabs.Add(new ViewModelView2D());


			//LinesCollection = new ObservableCollection<ModelApp>();
			//LinesCollection.Add(new ModelApp("Legend " + (LinesCollection.Count() + 1).ToString()));
			//LinesCollection.Last().GeoObject = new ObservableCollection<Point> { new Point(1, 2), new Point(9, 7) };
			//dPoint = new System.Windows.Point(0, 0);
			//Height = mainWindow.GraphicCurve.Height;
			//Width = mainWindow.GraphicCurve.Width;
			//Draw();						
		}

		private ICommand startDirectCalulation;
		public ICommand StartDirectCalulation => startDirectCalulation ?? (startDirectCalulation = new DelegateCommand(() =>
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

		public void Draw()
		{

		}

		//private ICommand addCommand;
		//public ICommand AddCommand => addCommand ?? (addCommand = new DelegateCommand(() =>
		//{
		//	if (!MouseDown)
		//	{
		//		LinesCollection.Add(new ModelApp("Legend " + (LinesCollection.Count() + 1).ToString()));
		//		SetStartView();
		//		Draw();
		//	}									
		//}));

		//private ICommand removeCommand;
		//public ICommand RemoveCommand => removeCommand ?? (removeCommand = new DelegateCommand(() =>
		//{
		//	if (SelectedLineItem != null)
		//	{
		//		LinesCollection.Remove(SelectedLineItem);
		//		boundsCalculation();
		//		SetStartView();				
		//		Draw();
		//		SelectedLineItem = null;
		//	}
		//}));

		//private ICommand addPointCommand;
		//public ICommand AddPointCommand => addPointCommand ?? (addPointCommand = new DelegateCommand(() =>
		//{
		//	if(SelectedLineItem != null)
		//	{
		//		SelectedLineItem.GeoObject.Add(new Point());				
		//	}			
		//}));

		//private ICommand openFileCommand;
		//public ICommand OpenFileCommand => openFileCommand ?? (openFileCommand = new DelegateCommand(() =>
		//{
		//	var dialog = new Microsoft.Win32.OpenFileDialog();
		//	dialog.Multiselect = true;
		//	dialog.FileName = "Document";
		//	dialog.DefaultExt = ".txt";
		//	dialog.Filter = "Text documents (.txt)|*.txt";

		//	bool? result = dialog.ShowDialog();

		//	if (result == true)
		//	{
		//		string filename = dialog.FileName;
		//		if (File.Exists(filename))
		//		{
		//			ObservableCollection<Point> points = new ObservableCollection<Point>();
		//			foreach (string line in File.ReadLines(filename))
		//			{
		//				var tmp = line.Split();
		//				points.Add(new Point(double.Parse(tmp[0]), double.Parse(tmp[1])));						
		//			}
		//			LinesCollection.Add(new ModelApp(points, "Legend " + (LinesCollection.Count() + 1).ToString()));
		//		}
		//		boundsCalculation();
		//		Draw();
		//	}
		//}));

		//private ICommand saveFileCommand;
		//public ICommand SaveFileCommand => saveFileCommand ?? (saveFileCommand = new DelegateCommand(() =>
		//{
		//	SaveFileDialog dialog = new SaveFileDialog();
		//	dialog.Filter = "Text documents (.txt)|*.txt";
		//	dialog.DefaultExt = ".txt";
		//	bool? result = dialog.ShowDialog();

		//	if (result == true)
		//	{
		//		using (StreamWriter sw = File.CreateText(dialog.FileName))
		//		{
		//			foreach (Point item in SelectedLineItem.GeoObject)
		//			{
		//				sw.WriteLine(item.X.ToString() + "\t" + item.Y.ToString());
		//			}					
		//		}
		//	}			
		//}));

		
	}
}
