using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfiniteScrollingApp
{
	public class FakeDataSource
	{
		public async Task<IEnumerable<int>> GetItemsAsync(int page, int pageSize)
		{
			await Task.Delay(2000);

			var start = (page - 1) * pageSize;
			var count = pageSize;

			return Enumerable.Range(start, count);
		}
	}
}
