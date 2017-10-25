using Xamarin.Forms;

namespace InfiniteScrollingApp
{
	public partial class SimplePage : ContentPage
	{
		public SimplePage()
		{
			InitializeComponent();

			BindingContext = new SimpleViewModel();
		}
	}
}
