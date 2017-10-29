namespace Xamarin.Forms.Extended
{
	public interface IInfiniteScrollDetector
	{
		bool ShouldLoadMore(object currentItem);
	}
}
