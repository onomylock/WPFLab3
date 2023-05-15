using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static System.Windows.Forms.AxHost;

namespace WPFLab3
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ViewModelApp viewModel;	
		//private ObservableCollection<TabItem> _tabItems;

		public MainWindow()
		{
			InitializeComponent();
			OXCurve.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			OXCurve.Arrange(new Rect(0, 0, OXCurve.DesiredSize.Width, OXCurve.DesiredSize.Height));
			OYCurve.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			OYCurve.Arrange(new Rect(0, 0, OYCurve.DesiredSize.Width, OYCurve.DesiredSize.Height));
			GraphicCurve.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			GraphicCurve.Arrange(new Rect(0, 0, GraphicCurve.DesiredSize.Width, GraphicCurve.DesiredSize.Height));

			OXView2D.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			OXView2D.Arrange(new Rect(0, 0, OXView2D.DesiredSize.Width, OXView2D.DesiredSize.Height));
			OYView2D.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			OYView2D.Arrange(new Rect(0, 0, OYView2D.DesiredSize.Width, OYView2D.DesiredSize.Height));
			GraphicView2D.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			GraphicView2D.Arrange(new Rect(0, 0, GraphicView2D.DesiredSize.Width, GraphicView2D.DesiredSize.Height));

			try
			{
				viewModel = new ViewModelApp(this);
				DataContext = viewModel;
				
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}


			//TabControl.DataContext = viewModel;
			//dataGrid.ItemsSource = viewModel.LinesCollection;			
			//dataGrid.SelectionChanged += DataGrid_SelectionChanged;		
		}



		//private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//{
		//	if(dataGrid.SelectedItem != null)
		//	{
		//		viewModel.SelectedLineItem = dataGrid.SelectedItem as ModelApp;
		//		dataGridPoints.ItemsSource = viewModel.SelectedLineItem.GeoObject;
		//	}			
		//}

		private void StackPanel_MouseMove(object sender, MouseEventArgs e)
		{
			foreach(var item in viewModel.ViewModelTabs.Where(x => x.Value.IsSelected && x.Key == viewModel.CurrentTab))
			{
				if(item.Value.MouseDown)
				{
					if(viewModel.CurrentTab == Tab.Curve)
						item.Value.ButtonDownPoint = Mouse.GetPosition(this.GraphicCurve);					
					else
						item.Value.ButtonDownPoint = Mouse.GetPosition(this.GraphicView2D);
				}
				else
				{
					if (viewModel.CurrentTab == Tab.Curve)
					{
						item.Value.CurrentPoint = Mouse.GetPosition(this.GraphicCurve);
						item.Value.SetMousePosition(item.Value.CurrentPoint);
					}						
					else
					{
						item.Value.CurrentPoint = Mouse.GetPosition(this.GraphicView2D);
						item.Value.SetMousePosition(item.Value.CurrentPoint);
					}
						
					//item.SetMousePosition(viewModel.CurrentViewModelTub.CurrentPoint);
				}
			}
			if(viewModel.ViewModelTabs.Where(x => x.Value.IsSelected && x.Key == viewModel.CurrentTab && x.Value.MouseDown == true).Count() > 0)
				viewModel.Draw();
			//if(viewModel.CurrentViewModelTub.MouseDown)
			//{
			//	viewModel.CurrentViewModelTub.ButtonDownPoint = Mouse.GetPosition(this.GraphicCurve);
			//	viewModel.Draw();
			//}
			//else
			//{
			//	viewModel.CurrentViewModelTub.CurrentPoint = Mouse.GetPosition(this.GraphicCurve);
			//	viewModel.CurrentViewModelTub.SetMousePosition(viewModel.CurrentViewModelTub.CurrentPoint);
			//}			
		}

		private void dataGridPoints_CurrentCellChanged(object sender, EventArgs e)
		{
			viewModel.Draw();
		}

		private void Graphic_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//viewModel..MouseDown = true;
			//viewModel.ViewModelTabs.Where(x => x.TabView == viewModel.CurrentTab).First().MouseDown = true;
			foreach (var item in viewModel.ViewModelTabs.Where(x => x.Key == viewModel.CurrentTab))
			{
				item.Value.MouseDown = true;
			}
		}

		private void Graphic_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			//viewModel.CurrentViewModelTub.MouseDown = false;
			//viewModel.ViewModelTabs.Where(x => x.TabView == viewModel.CurrentTab).First().MouseDown = false;
			foreach (var item in viewModel.ViewModelTabs.Where(x => x.Key == viewModel.CurrentTab))
			{
				item.Value.MouseDown = false;
			}
		}

		private void Graphic_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			//viewModel.ViewModelTabs.Where(x => x.TabView == viewModel.CurrentTab).First().SetStartView();
			foreach (var item in viewModel.ViewModelTabs.Where(x => x.Key == viewModel.CurrentTab))
			{
				item.Value.SetStartView();
			}
		}

		private void Graphic_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			double scal = e.Delta < 0 ? 1.05 : 1 / 1.05;
			foreach (var item in viewModel.ViewModelTabs.Where(x => x.Key == viewModel.CurrentTab))
			{
				item.Value.Scale *= scal;
			}
			//viewModel.ViewModelTabs.(x =>
			//{
			//	if (x.TabView == viewModel.CurrentTab)
			//		x.Scale *= scal;
			//});
			viewModel.Draw();
		}

		//private void Button_Click(object sender, RoutedEventArgs e)
		//{
		//	if (dataGrid.SelectedItem != null)
		//	{
		//		viewModel.SelectedLineItem.SetRandomColor();
		//		viewModel.Draw();
		//	}
		//}

		//private void Graphic_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		//{
		//	viewModel.MouseDown = true;
		//}

		//private void Graphic_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		//{
		//	viewModel.MouseDown = false;
		//}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabControl tc = (TabControl)sender;
			string tabName = ((TabItem)tc.SelectedItem).Name;
			if (tabName == "Curve")
			{
				viewModel.CurrentTab = Tab.Curve;
				//foreach (var item in viewModel.ViewModelTabs.Where(x => x.Key == Tab.Curve))
				//{
				//	item.Value.IsSelected = true;
				//}
				foreach (var item in viewModel.ViewModelTabs)
				{
					if (item.Key == Tab.Curve)
						item.Value.IsSelected = true;
					else if(item.Key == Tab.View2D)
						item.Value.IsSelected = false;
				}
			}				
			else
			{
				viewModel.CurrentTab = Tab.View2D;
				//foreach (var item in viewModel.ViewModelTabs.Where(x => x.Key == Tab.View2D))
				//{
				//	item.Value.IsSelected = true;
				//}
				foreach (var item in viewModel.ViewModelTabs)
				{
					if (item.Key == Tab.Curve)
						item.Value.IsSelected = false;
					else if (item.Key == Tab.View2D)
						item.Value.IsSelected = true;
				}
			}				
		}
	}
}
