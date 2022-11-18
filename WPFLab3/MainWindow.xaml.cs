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
			//viewModel.CurrentPoint = Mouse.GetPosition(this.Graphic);			
			//viewModel.SetMousePosition(viewModel.CurrentPoint);
			
			if(viewModel.MouseDown)
			{				
				viewModel.ButtonDownPoint = Mouse.GetPosition(this.Graphic);				
				viewModel.Draw();
			}
			else
			{
				viewModel.CurrentPoint = Mouse.GetPosition(this.Graphic);
				viewModel.SetMousePosition(viewModel.CurrentPoint);
			}
		}

		private void dataGridPoints_CurrentCellChanged(object sender, EventArgs e)
		{
			viewModel.Draw();
		}

		private void Graphic_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			viewModel.MouseDown = true;
		}

		private void Graphic_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			viewModel.MouseDown = false;
		}

		private void Graphic_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			viewModel.SetStartView();
		}

		private void Graphic_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			double scal = e.Delta < 0 ? 1.05 : 1 / 1.05;
			viewModel.Scale *= scal;
			viewModel.Draw();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (dataGrid.SelectedItem != null)
			{
				viewModel.SelectedLineItem.SetRandomColor();
				viewModel.Draw();
			}
		}
	}
}
