using Xamarin.Forms;

namespace InfiniteScrollingApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			BindingContext = new MainViewModel();
		}
	}
}
