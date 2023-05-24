﻿using System;
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
using WPFLab3.Model;

namespace WPFLab3
{
	public enum Tab
	{
		Curve, View2D
	}

	public class ViewModelApp : ReactiveObject
	{		
		public Tab CurrentTab { get; set; }		
		public Dictionary<Tab, ViewModelTab> ViewModelTabs { get; set; }
		private MainWindow _mainWindow;
		private ModelCalulation _modelCalulation;		

		public ViewModelApp(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;			
			ViewModelTabs = new Dictionary<Tab, ViewModelTab>();
			ViewModelView2D tmpView = new ViewModelView2D(mainWindow);
			ViewModelCurve tmpCurve = new ViewModelCurve(mainWindow);
			tmpView.Oid = Guid.NewGuid();
			tmpCurve.Oid = tmpView.Oid;
			ViewModelTabs.Add(Tab.Curve, tmpCurve);
			ViewModelTabs.Add(Tab.View2D, tmpView);			
			_modelCalulation = new ModelCalulation();						
		}

		public void Draw()
		{			
			foreach (var item in ViewModelTabs.Where(x => x.Value.IsSelected))
			{
				item.Value.Draw();
			}
		}

		private void SetCalclulationObjectsToDrawObjects(ModelCalulation modelCalculation, Tab tab)
		{
			if (tab == Tab.Curve)
			{
				foreach (var item in ViewModelTabs.Where(x => x.Key == Tab.Curve))
				{
					var r = new Random();
					var bytes = new byte[4];
					r.NextBytes(bytes);					
					item.Value.ModelObjectCollection.Add(new List<IModelObject>(){ new ModelObject(modelCalculation.Receivers.Select(x => x.XY).ToList(),
						new SolidColorBrush(Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3])))});
				}
			}
			if (tab == Tab.View2D)
			{
				foreach (var item in ViewModelTabs.Where(x => x.Key == Tab.View2D))
				{
					var r = new Random();
					var bytes = new byte[4];
					item.Value.ModelObjectCollection.Add(new List<IModelObject>());
					foreach (var cell in modelCalculation.Cells)
					{
						r.NextBytes(bytes);
						item.Value.ModelObjectCollection.Last().Add(new ModelObject(cell.Nodes.ToList(),
							new SolidColorBrush(Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3])), cell.I));
					}
					//r.NextBytes(bytes);
					//item.ModelObjectCollection.Add(new List<IModelObject>(){ new ModelObject(modelCalculation.Receivers.Select(x => x.XY).ToList(),
					//	new SolidColorBrush(Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3])))});
				}
			}	
		}

		private ICommand startDirectCalulation;
		public ICommand StartDirectCalulation => startDirectCalulation ?? (startDirectCalulation = new DelegateCommand(() =>
		{
			_modelCalulation.StartCalulationDirect();
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
				//dialog.RootFolder = Directory.GetCurrentDirectory();
				dialog.RootFolder = Environment.SpecialFolder.UserProfile;
				System.Windows.Forms.DialogResult result = dialog.ShowDialog();
				if(result == System.Windows.Forms.DialogResult.OK)
				{
					_modelCalulation.SetInputCalculationData(dialog.SelectedPath);
					if (_modelCalulation.Receivers.Count > 0)
						SetCalclulationObjectsToDrawObjects(_modelCalulation, Tab.Curve);
					if (_modelCalulation.Cells.Count > 0)
						SetCalclulationObjectsToDrawObjects(_modelCalulation, Tab.View2D);
					Draw();
				}				
			}
		}));

		

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
