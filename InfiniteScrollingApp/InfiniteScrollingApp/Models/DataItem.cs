namespace InfiniteScrollingApp
{
	public class DataItem
	{
		public DataItem(int item)
		{
			Item = item;
		}

		public int Item { get; }

		public string Name => $"Item {Item}";
	}
}
