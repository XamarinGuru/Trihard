using EventKit;

namespace location2
{
	public class App
	{
		public static App Current
		{
			get { return current; }
		}
		private static App current;

		/// <summary>
		/// The EKEventStore is intended to be long-lived. It's expensive to new it up
		/// and can be thought of as a database, so we create a single instance of it
		/// and reuse it throughout the app
		/// </summary>
		public EKEventStore EventStore
		{
			get { return eventStore; }
		}
		protected EKEventStore eventStore;

		static App()
		{
			current = new App();
		}
		protected App()
		{
			eventStore = new EKEventStore();
		}
	}
}
