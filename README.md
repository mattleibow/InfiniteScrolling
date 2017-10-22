# Infinite Scrolling

[![Build status](https://ci.appveyor.com/api/projects/status/1vnc3u270h71sbyc/branch/master?svg=true)](https://ci.appveyor.com/project/mattleibow/infinitescrolling/branch/master)  [![NuGet](https://img.shields.io/nuget/dt/InfiniteScrolling.svg)](https://www.nuget.org/packages/InfiniteScrolling)  [![NuGet Pre Release](https://img.shields.io/nuget/vpre/InfiniteScrolling.svg)](https://www.nuget.org/packages/InfiniteScrolling)

A small library to quickly and easily add infinite/endless scrolling support to any Xamarin.Forms ListView.

## Usage

Adding infinite scrolling support to a `ListView` is done in two steps. First we add the `InfiniteScrollBehavior` to
the `ListView`'s behaviors:

```xml
<!-- xmlns:scroll="clr-namespace:InfiniteScrolling;assembly=InfiniteScrolling" -->

<!-- a normal list view -->
<ListView CachingStrategy="RecycleElement" ItemsSource="{Binding Items}">

    <!-- the behavior that will enable infinite scrolling -->
    <ListView.Behaviors>
        <scroll:InfiniteScrollBehavior />
    </ListView.Behaviors>

    ...

</ListView>
```

And, then we need to make sure our items collection implements the `IInfiniteScrollLoader` interface:

```csharp
public interface IInfiniteScrollLoader
{
    // is there more data
    bool CanLoadMore { get; }

    // load more data
    Task LoadMoreAsync();
}
```

This interface can be implemented on an `ObservableCollection<T>`:

```csharp
public class InfiniteDataCollection : ObservableCollection<DataItem>, IInfiniteScrollLoader
{
    private readonly FakeDataSource dataSource;

    public InfiniteDataCollection()
    {
        // the data source from a service
        dataSource = new FakeDataSource();
    }

    // we can handle anything
    public bool CanLoadMore => true;

    // we need to add more items to the collection
    public async Task LoadMoreAsync()
    {
        // download the new set of items
        var items = await dataSource.GetMoreItemsAsync();

        // add the new items to this collection
        foreach (var item in items)
        {
            Add(item);
        }

        // TODO: instead of adding each item individually, we could make use of
        //       the RangedObservableCollection from the NuGet:
        //       https://github.com/mattleibow/RangedObservableCollection
        //       this would become:
        //
        //           var items = await dataSource.GetMoreItemsAsync();
        //           AddRange(items);
    }
}
```

## Advanced Usage

The default behavior is to wait until the last item is loaded into memory before requesting the next set of items. If
we need to have more control over this, we can also implement the `IInfiniteScrollDetector` interface:

```csharp
public interface IInfiniteScrollDetector
{
    // does the specified item indicate more data is needed
    bool ShouldLoadMore(object currentItem);

    // the list is probably empty, so should we load more data
    bool ShouldLoadMore();
}
```

This interface also needs to be implemented on the data source:

```csharp
public class InfiniteDataCollection : ..., IInfiniteScrollDetector
{
    ...

    // this is the last item, so load
    public bool ShouldLoadMore(object currentItem)
    {
        return this[Count - 1] == currentItem;
    }

    // there are no items, so load
    public bool ShouldLoadMore()
    {
        return Count == 0;
    }
}
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
