using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ResolutionGroupName("MyCompany")]
[assembly: ExportEffect(typeof(InfiniteScrollingApp.UWP.FocusEffect), "FocusEffect")]

namespace InfiniteScrollingApp.UWP
{
	public sealed partial class MainPage
	{
		public MainPage()
		{
			this.InitializeComponent();

			LoadApplication(new InfiniteScrollingApp.App());
		}
	}

	public class FocusEffect : PlatformEffect
	{
		protected override void OnAttached()
		{
			var list = Control as Windows.UI.Xaml.Controls.ListView;
			list.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var b = VisualTreeHelper.GetChild((DependencyObject)sender, 0) as Border;
			var sv = (ScrollViewer)b.Child;
			sv.ViewChanged += OnScrolled;
		}

		protected override void OnDetached()
		{
		}

		private void OnScrolled(object sender, ScrollViewerViewChangedEventArgs e)
		{
			//Debug.WriteLine((sender as ScrollViewer).VerticalOffset);

			var listview = Control as Windows.UI.Xaml.Controls.ListView;
			

			ScrollViewer sv = sender as ScrollViewer;
			GeneralTransform gt = sv.TransformToVisual(listview);
			var p = gt.TransformPoint(new Windows.Foundation.Point(0, 0));
			List<UIElement> list = new List<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(p, sv));
			ListViewItem item = list.OfType<ListViewItem>().FirstOrDefault();
			if (item != null)
			{
				int index = listview.IndexFromContainer(item);
				Debug.WriteLine("Visible item at top of list is " + index);
			}
		}
	}
}
