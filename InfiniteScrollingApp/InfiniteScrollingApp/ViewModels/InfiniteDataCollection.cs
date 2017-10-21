using System.Collections.ObjectModel;
using System.Threading.Tasks;

using InfiniteScrolling;

namespace InfiniteScrollingApp
{
	public class InfiniteDataCollection : ObservableCollection<DataItem>, IInfiniteScrollLoader, IInfiniteScrollDetector
	{
		private const int PageSize = 20;
		private readonly FakeDataSource dataSource;

		public InfiniteDataCollection()
		{
			dataSource = new FakeDataSource();
		}

		public bool ShouldLoadMore(object currentItem) => this[Count - 1] == currentItem;

		public bool ShouldLoadMore() => Count == 0;

		public bool CanLoadMore => true;

		public async Task LoadMoreAsync()
		{
			var page = Count / PageSize;
			var items = await dataSource.GetItemsAsync(page + 1, PageSize);

			foreach (var item in items)
			{
				Add(new DataItem(item));
			}
		}
	}
}
