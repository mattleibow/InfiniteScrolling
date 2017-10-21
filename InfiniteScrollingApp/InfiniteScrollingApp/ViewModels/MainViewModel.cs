using Xamarin.Forms;

namespace InfiniteScrollingApp
{
	public class MainViewModel : BindableObject
	{
		public static readonly BindableProperty IsWorkingProperty =
			BindableProperty.Create(nameof(IsWorking), typeof(bool), typeof(MainViewModel), default(bool));

		public MainViewModel()
		{
			Items = new InfiniteDataCollection();
		}

		public bool IsWorking
		{
			get { return (bool)GetValue(IsWorkingProperty); }
			set { SetValue(IsWorkingProperty, value); }
		}

		public InfiniteDataCollection Items { get; }
	}
}
