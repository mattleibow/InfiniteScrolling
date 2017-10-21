namespace InfiniteScrolling
{
	public interface IInfiniteScrollDetector
	{
		bool ShouldLoadMore(object currentItem);

		bool ShouldLoadMore();
	}
}
