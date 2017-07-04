
using System.Collections.Generic;

namespace PortableLibrary
{
	public class Comment
	{
		public string author { get; set; }
		public string authorId { get; set; }
		public string authorUrl { get; set; }
		public string commentText { get; set; }
		public string eventId { get; set; }
		public string commentId { get; set; }
		public string date { get; set;}
	}

	public class Comments
	{
		public string ownerId { get; set;}
		public string type { get; set;}
		public List<Comment> comments { get; set; }
	}
}
