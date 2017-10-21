using System;
using System.Collections;
using Xamarin.Forms;

namespace InfiniteScrolling
{
	public class InfiniteScrollBehavior : Behavior<ListView>
	{
		public static readonly BindableProperty IsLoadingMoreProperty =
			BindableProperty.Create(nameof(IsLoadingMore), typeof(bool), typeof(InfiniteScrollBehavior), default(bool), BindingMode.OneWayToSource);

		public bool IsLoadingMore
		{
			get { return (bool)GetValue(IsLoadingMoreProperty); }
			protected set { SetValue(IsLoadingMoreProperty, value); }
		}

		protected ListView AssociatedListView { get; private set; }

		protected override void OnAttachedTo(ListView bindable)
		{
			base.OnAttachedTo(bindable);

			AssociatedListView = bindable;

			bindable.BindingContextChanged += OnListViewBindingContextChanged;
			bindable.ItemAppearing += OnListViewItemAppearing;

			BindingContext = AssociatedListView.BindingContext;

			OnListViewLoaded();
		}

		protected override void OnDetachingFrom(ListView bindable)
		{
			bindable.BindingContextChanged -= OnListViewBindingContextChanged;
			bindable.ItemAppearing -= OnListViewItemAppearing;

			base.OnDetachingFrom(bindable);
		}

		private void OnListViewBindingContextChanged(object sender, EventArgs e)
		{
			BindingContext = AssociatedListView.BindingContext;

			OnListViewLoaded();
		}

		private async void OnListViewItemAppearing(object sender, ItemVisibilityEventArgs e)
		{
			if (IsLoadingMore)
				return;

			if (AssociatedListView.ItemsSource is IInfiniteScrollLoader loader)
			{
				if (loader.CanLoadMore && ShouldLoadMore(e.Item))
				{
					IsLoadingMore = true;
					await loader.LoadMoreAsync();
					IsLoadingMore = false;
				}
			}
		}

		private async void OnListViewLoaded()
		{
			if (IsLoadingMore)
				return;

			if (AssociatedListView.ItemsSource is IInfiniteScrollLoader loader)
			{
				if (loader.CanLoadMore && ShouldLoadMore())
				{
					IsLoadingMore = true;
					await loader.LoadMoreAsync();
					IsLoadingMore = false;
				}
			}
		}

		private bool ShouldLoadMore(object item)
		{
			if (AssociatedListView.ItemsSource is IInfiniteScrollDetector detector)
				return detector.ShouldLoadMore(item);
			if (AssociatedListView.ItemsSource is IList list)
				return list[list.Count - 1] == item;
			return false;
		}

		private bool ShouldLoadMore()
		{
			if (AssociatedListView.ItemsSource is IInfiniteScrollDetector detector)
				return detector.ShouldLoadMore();
			if (AssociatedListView.ItemsSource is IList list)
				return list.Count == 0;
			return false;
		}
	}
}
