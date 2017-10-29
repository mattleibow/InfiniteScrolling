using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Xamarin.Forms.Extended
{
	public class InfiniteScrollCollection<T> : ObservableCollection<T>, IInfiniteScrollLoader, IInfiniteScrollLoading
	{
		private bool isLoadingMore;

		public InfiniteScrollCollection()
		{
		}

		public InfiniteScrollCollection(IEnumerable<T> collection)
			: base(collection)
		{
		}

		public Action OnBeforeLoadMore { get; set; }

		public Action OnAfterLoadMore { get; set; }

		public Action<Exception> OnError { get; set; }

		public Func<bool> OnCanLoadMore { get; set; }

		public Func<Task<IEnumerable<T>>> OnLoadMore { get; set; }

		public virtual bool CanLoadMore => OnCanLoadMore?.Invoke() ?? true;

		public bool IsLoadingMore
		{
			get => isLoadingMore;
			private set
			{
				if (isLoadingMore != value)
				{
					isLoadingMore = value;
					OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsLoadingMore)));

					LoadingMore?.Invoke(this, new LoadingMoreEventArgs(IsLoadingMore));
				}
			}
		}

		public event EventHandler<LoadingMoreEventArgs> LoadingMore;

		public async Task LoadMoreAsync()
		{
			try
			{
				IsLoadingMore = true;
				OnBeforeLoadMore?.Invoke();

				var result = await OnLoadMore();

				if (result != null)
				{
					AddRange(result);
				}
			}
			catch (Exception ex) when (OnError != null)
			{
				OnError.Invoke(ex);
			}
			finally
			{
				IsLoadingMore = false;
				OnAfterLoadMore?.Invoke();
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));

			CheckReentrancy();

			var startIndex = Count;
			var changedItems = new List<T>(collection);

			foreach (var i in changedItems)
				Items.Add(i);

			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems, startIndex));
		}
	}
}
