using System.Threading.Tasks;

namespace Xamarin.Forms.Extended
{
	public interface IInfiniteScrollLoader
	{
		bool CanLoadMore { get; }

		Task LoadMoreAsync();
	}
}
