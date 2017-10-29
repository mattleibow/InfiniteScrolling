using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

using Xamarin.Forms.Extended;

namespace InfiniteScrollingApp
{
	public class GroupedViewModel : BindableObject
	{
		private const int PageSize = 20;

		public static readonly BindableProperty IsWorkingProperty =
			BindableProperty.Create(nameof(IsWorking), typeof(bool), typeof(GroupedViewModel), default(bool));

		public GroupedViewModel()
		{
			var dataSource = new FakeDataSource();

			Items = new InfiniteScrollCollection<GroupCollection<DataItem>>
			{
				OnLoadMore = async () =>
				{
					// load the next page
					var page = Items.Sum(i => i.Count) / PageSize;
					var items = await dataSource.GetItemsAsync(page + 1, PageSize);

					// go through each group from the server
					foreach (var group in items.GroupBy(i => i.Group))
					{
						// look to see if these items belong to any existing groups
						var inList = Items.LastOrDefault(i => i.Group == group.Key);

						if (inList != null)
						{
							// this is an existing group, so add the items to that
							foreach (var item in group)
								inList.Add(item);

							// TODO: instead of adding each item individually, we could make use of
							//       the RangedObservableCollection from the NuGet:
							//       https://github.com/mattleibow/RangedObservableCollection
							//       this would become:
							//
							//           inList.AddRange(group);
						}
						else
						{
							// this is a new group
							Items.Add(new GroupCollection<DataItem>(group) { Group = group.Key });
						}
					}

					return null; // we have added the items ourselves
				}
			};

			RefreshCommand = new Command(() =>
			{
				// clear and start again
				Items.Clear();
				Items.LoadMoreAsync();
			});

			// load the initial data
			Items.LoadMoreAsync();
		}

		public bool IsWorking
		{
			get { return (bool)GetValue(IsWorkingProperty); }
			set { SetValue(IsWorkingProperty, value); }
		}

		public InfiniteScrollCollection<GroupCollection<DataItem>> Items { get; set; }

		public ICommand RefreshCommand { get; }

		public class GroupCollection<T> : ObservableCollection<T>
		{
			public GroupCollection()
			{
			}

			public GroupCollection(IEnumerable<T> collection)
				: base(collection)
			{
			}

			public int Group { get; set; }

			public string Heading => $"Group {Group}";
		}
	}
}
