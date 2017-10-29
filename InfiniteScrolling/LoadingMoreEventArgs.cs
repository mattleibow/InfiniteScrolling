using System;

namespace Xamarin.Forms.Extended
{
	public class LoadingMoreEventArgs : EventArgs
	{
		public LoadingMoreEventArgs(bool isLoadingMore)
		{
			IsLoadingMore = isLoadingMore;
		}

		public bool IsLoadingMore { get; }
	}
}
