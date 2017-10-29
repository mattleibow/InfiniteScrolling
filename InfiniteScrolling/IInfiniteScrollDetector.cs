namespace Xamarin.Forms.Extended.InfiniteScrolling
{
	public interface IInfiniteScrollDetector
	{
		bool ShouldLoadMore(object currentItem);
	}
}
