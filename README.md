# Infinite Scrolling

[![Build status](https://ci.appveyor.com/api/projects/status/1vnc3u270h71sbyc/branch/master?svg=true)](https://ci.appveyor.com/project/mattleibow/infinitescrolling/branch/master)  [![NuGet](https://img.shields.io/nuget/dt/Xamarin.Forms.Extended.InfiniteScrolling.svg)](https://www.nuget.org/packages/Xamarin.Forms.Extended.InfiniteScrolling)  [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Xamarin.Forms.Extended.InfiniteScrolling.svg)](https://www.nuget.org/packages/Xamarin.Forms.Extended.InfiniteScrolling)

A small library to quickly and easily add infinite/endless scrolling support to any Xamarin.Forms ListView.

## Usage

Adding infinite scrolling support to a `ListView` is done in two steps. First we add the `InfiniteScrollBehavior` to
the `ListView`'s behaviors:

```xml
<!-- xmlns:scroll="clr-namespace:Xamarin.Forms.Extended;assembly=Xamarin.Forms.Extended.InfiniteScrolling" -->

<!-- a normal list view -->
<ListView CachingStrategy="RecycleElement" ItemsSource="{Binding Items}">

    <!-- the behavior that will enable infinite scrolling -->
    <ListView.Behaviors>
        <scroll:InfiniteScrollBehavior />
    </ListView.Behaviors>

    ...

</ListView>
```

And then, we need to make use of the `InfiniteScrollCollection<T>` as our collection type (instead of 
`ObservableCollection<T>` or `List<T>`):

```csharp
using Xamarin.Forms.Extended.InfiniteScrolling;

public class MainViewModel : BaseViewModel
{
    private const int PageSize = 20;

    public MainViewModel()
    {
        Items = new InfiniteScrollCollection<DataItem>
        {
            OnLoadMore = async () =>
            {
                // load the next page
                var page = Items.Count / PageSize;
                var items = await dataSource.GetItemsAsync(page + 1, PageSize);

                // return the items that need to be added
                return items;
            }
        };
    }

    public InfiniteScrollCollection<DataItem> Items { get; }
}
```

That's it!
All we needed was the behavior and the collection type. The rest is automatically handled for us. If we need to
manually trigger the list to load more, for example to initialize the list, we can just use the `LoadMoreAsync`
method:

```csharp
Items.LoadMoreAsync();
```

## Loading Placeholder

Although this library does not provide any placeholders by default, we can easily add one by making use of the
`IsLoadingMore` property of `InfiniteScrollBehavior` and the `ListView` footer:

```xml
<ListView ...>

    <!-- the behavior bound to IsWorking -->
    <ListView.Behaviors>
        <scroll:InfiniteScrollBehavior IsLoadingMore="{Binding IsWorking}" />
    </ListView.Behaviors>

    <!-- the "loading..." view, with visibility bound to IsWorking -->
    <ListView.Footer>
        <Grid Padding="6" IsVisible="{Binding IsWorking}">
            <!-- set the footer to have a zero height when invisible -->
            <Grid.Triggers>
                <Trigger TargetType="Grid" Property="IsVisible" Value="False">
                    <Setter Property="HeightRequest" Value="0" />
                </Trigger>
            </Grid.Triggers>
            <!-- the loading content -->
            <Label Text="Loading..." VerticalOptions="Center" HorizontalOptions="Center" />
        </Grid>
    </ListView.Footer>

</ListView>
```

## Advanced Usage

The way the custom collection works is to implement various interfaces that the behavior is listening for. We can use
these interfaces to convert any collection into a infinite-loading collection:

### `IInfiniteScrollLoader`

The `IInfiniteScrollLoader` interface is **required** as it is the primary means in which the behavior lets the 
collection know to load more data:

```csharp
public interface IInfiniteScrollLoader
{
    // returns true if the list should load more
    bool CanLoadMore { get; }

    // the method that actually loads more data
    Task LoadMoreAsync();
}
```

Depending on what the user does with the list view, the behavior will first check the `CanLoadMore` property to see if
more data needs to be loaded. If this property returns `true`, then the behavior will call the `LoadMoreAsync` method.
This method can perform any loading from anywhere (a database or remote service) and then add new items to the list.

### `IInfiniteScrollLoading`

The `IInfiniteScrollLoading` interface is **optional** and provides the behavior with notifications if the starts 
loading more data - without the list view asking for more:

```csharp
public interface IInfiniteScrollLoading
{
    // the collection is loading more
    bool IsLoadingMore { get; }

    // the collection either began or completed a load
    event EventHandler<LoadingMoreEventArgs> LoadingMore;
}
```

If this interface is implemented, the behavior can be aware of events that were not triggered by the list view, but by
a view model or code behind. When the collection wants to load more data, it needs to first raise the `LoadingMore`
event with `new LoadingMoreEventArgs(true)`. This is used to let the behavior know that it should not load items if the
user scrolls since it is already happening. As soon as the load operation completes, the event should be raised again,
but this time with the arguments `new LoadingMoreEventArgs(false)`.

The `IsLoadingMore` property should accurately reflect the current state of loading. As soon as a load starts, the
`IsLoadingMore` property must return `true`, and when loading completes, return `false`. This is because the behavior
can, at any time, request the loading state without having to listen to the events. This is most important for the 
cases where the list obtains it's items via data binding, but the binding changes. At this point, the events may not be
attached and there is no way for the behavior to know if a load operation has started or completed.

### `IInfiniteScrollDetector`

The `IInfiniteScrollDetector` interface is **optional** and provides the behavior with an alternate means with which to
check if the list needs to load more items based on the current scroll position:

```csharp
public interface IInfiniteScrollDetector
{
    // returns true if the current item is at the point that more items are needed
    bool ShouldLoadMore(object currentItem);
}
```

Typically, this interface is not needed as the behavior will be watching the scroll can detect when more items are 
needed. However, the automatice detection requires that the items source implement the `IList` interface. This is 
usually true for an `ObservableCollection<T>` or `List<T>`, but there may be some instances where this is not the
collection type. Or, we wish to have more control over when more items are loaded.

### Implementing Interfaces

A possible implementation of all these interfaces might be:

```csharp
public class InfiniteScrollCollection<T> : 
    ObservableCollection<T>, // the base collection type
    IInfiniteScrollLoader,   // [required]
    IInfiniteScrollLoading,  // [optional]
    IInfiniteScrollDetector  // [optional]
{
    // for tracking the state of a load operation
    private bool isLoadingMore;

    public InfiniteScrollCollection()
    {
    }

    public InfiniteScrollCollection(IEnumerable<T> collection)
        : base(collection)
    {
    }

    // we can load forever
    public bool CanLoadMore => true;

    // the list has determined that we need more items
    public async Task LoadMoreAsync()
    {
        // we are starting a load, set the property and raise the event
        isLoadingMore = true;
        LoadingMore?.Invoke(this, new LoadingMoreEventArgs(true));

        // load more items
        var newItems = await LoadMoreDataFromSomewhereAsync();

        // add the new items to the list
        foreach (item in newItems)
        {
            Add(item);
        }

        // we have finished our load, set the property and raise the event
        isLoadingMore = false;
        LoadingMore?.Invoke(this, new LoadingMoreEventArgs(false));
    }

    // whether or not we are loading items
    public bool IsLoadingMore => isLoadingMore;

    // the event that informs the list that we are fetching more items
    public event EventHandler<LoadingMoreEventArgs> LoadingMore;

    // whether or not we have run out of items
    public bool ShouldLoadMore(object currentItem)
    {
        // there are no items in the list
        if (Count == 0)
            return true;

        // the current item is the last item in the list
        return this[Count - 1] == currentItem;
    }
}
```
