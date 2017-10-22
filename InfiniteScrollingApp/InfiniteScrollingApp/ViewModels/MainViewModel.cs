using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

using InfiniteScrolling;

namespace InfiniteScrollingApp
{
	public class MainViewModel : BindableObject
	{
		private const int PageSize = 20;

		public static readonly BindableProperty IsWorkingProperty =
			BindableProperty.Create(nameof(IsWorking), typeof(bool), typeof(MainViewModel), default(bool));

		public MainViewModel()
		{
			var dataSource = new FakeDataSource();

			Items = new InfiniteScrollCollection<DataItem>
			{
				OnLoadMore = async () =>
				{
					// load the next page
					var page = Items.Count / PageSize;
					var items = await dataSource.GetItemsAsync(page + 1, PageSize);
					return items.Select(item => new DataItem(item));
				}
			};

			RefreshCommand = new Command(() =>
			{
				// clear and start again
				Items.Clear();
				Items.LoadMoreAsync();
			});

			// load the initial data
			Items.LoadMoreAsync();
		}

		public bool IsWorking
		{
			get { return (bool)GetValue(IsWorkingProperty); }
			set { SetValue(IsWorkingProperty, value); }
		}

		public InfiniteScrollCollection<DataItem> Items { get; set; }

		public ICommand RefreshCommand { get; }
	}
}
