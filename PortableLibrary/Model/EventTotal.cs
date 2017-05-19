
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
				{
					if (item.value == "-")
					{
						return "0";
					}
					else if (item.value.Split(new char[] { ' ' }).Length == 2)
					{
						return item.value.Split(new char[] { ' ' })[0];
					}
					else
					{
						return item.value;
					}
				}
			}
			return null;
		}
	}
}
