using System;

namespace Xamarin.Forms.Extended
{
	public interface IInfiniteScrollLoading
	{
		bool IsLoadingMore { get; }

		event EventHandler<LoadingMoreEventArgs> LoadingMore;
	}
}
