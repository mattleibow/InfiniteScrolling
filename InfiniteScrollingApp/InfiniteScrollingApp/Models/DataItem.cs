namespace InfiniteScrollingApp
{
	public class DataItem
	{
		public DataItem(int item)
		{
			Item = item;
			Group = item / 30;
		}

		public int Item { get; }

		public int Group { get; }

		public string Name => $"Item {Item}";
	}
}
