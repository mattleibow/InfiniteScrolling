using System.Threading.Tasks;

namespace Xamarin.Forms.Extended.InfiniteScrolling
{
	public interface IInfiniteScrollLoader
	{
		bool CanLoadMore { get; }

		Task LoadMoreAsync();
	}
}
