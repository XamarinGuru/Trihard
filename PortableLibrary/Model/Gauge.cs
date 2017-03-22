
using System.Collections.Generic;

namespace PortableLibrary
{
	public class Item
	{
		public Item()
		{
			name = "";
			value = "";
		}
		public string name { get; set; }
		public string value { get; set; }
	}

	public class Gauge
	{
		public List<Item> Bike { get; set; }
		public List<Item> Run { get; set; }
		public List<Item> Swim { get; set; }
	}
}
