using Xamarin.Forms;

namespace InfiniteScrollingApp
{
	public partial class GroupedPage : ContentPage
	{
		public GroupedPage()
		{
			InitializeComponent();

			BindingContext = new GroupedViewModel();
		}
	}
}
