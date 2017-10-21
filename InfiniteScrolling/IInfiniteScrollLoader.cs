using System.Threading.Tasks;

namespace InfiniteScrolling
{
	public interface IInfiniteScrollLoader
	{
		bool CanLoadMore { get; }

		Task LoadMoreAsync();
	}
}
