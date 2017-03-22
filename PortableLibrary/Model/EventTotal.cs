
using System.Collections.Generic;

namespace PortableLibrary
{
	public class EventTotal
	{
		public List<Item> totals { get; set;}

		public string GetValue(string key)
		{
			foreach (var item in totals)
			{
				if (item.name == key)
					return item.value;
			}
			return null;
		}
	}
}
