using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFLab3
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ViewModelApp viewModel;
		public MainWindow()
		{
			InitializeComponent();
			OX.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			OX.Arrange(new Rect(0, 0, OX.DesiredSize.Width, OX.DesiredSize.Height));
			OY.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			OY.Arrange(new Rect(0, 0, OY.DesiredSize.Width, OY.DesiredSize.Height));
			Graphic.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Graphic.Arrange(new Rect(0, 0, Graphic.DesiredSize.Width, Graphic.DesiredSize.Height));
			viewModel = new ViewModelApp(this);
			DataContext = viewModel;
			dataGrid.ItemsSource = viewModel.LinesCollection;			
			dataGrid.SelectionChanged += DataGrid_SelectionChanged;		
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(dataGrid.SelectedItem != null)
			{
				viewModel.SelectedLineItem = dataGrid.SelectedItem as ModelApp;
				dataGridPoints.ItemsSource = viewModel.SelectedLineItem.GeoObject;
			}			
		}

		private void StackPanel_MouseMove(object sender, MouseEventArgs e)
		{
			var Point = Mouse.GetPosition(this.Graphic);			
			viewModel.SetMousePosition(Point);

			//int lenX = 3;
			//int lenY = 3;
			//if (Curves.Count > 0) 
			//{
			//	X = X / zoomX + xsign_off;
			//	Y = (Panel1.Height - Y) / zoomY + ysign_off;

			//	double xdiv = (maxX - minX) / Convert.ToDouble(10);
			//	lenX = (xdiv.ToString()).Length + 1;
			//	double ydiv = (maxY - minY) / Convert.ToDouble(10);
			//	lenY = (ydiv.ToString()).Length + 1;
			//}
			//X = Math.Round(X, lenX);
			//Y = Math.Round(Y, lenY);
			//var _X = Convert.ToSingle(X).ToString();
			//var _Y = Convert.ToSingle(Y).ToString();
			//if (_X.Length > lenX) _X = _X.Substring(0, lenX);
			//if (_Y.Length > lenY) _Y = _Y.Substring(0, lenY);
			//Label1.Content = _X;
			//Label2.Content = _Y;
		}

		private void dataGridPoints_CurrentCellChanged(object sender, EventArgs e)
		{
			viewModel.Draw();
		}
	}
}
