using SystemConfiguration;

namespace Reachability {
	public enum NetworkStatus {
		NotReachable,
		ReachableViaCarrierDataNetwork,
		ReachableViaWiFiNetwork
	}

	public class Reachability
	{
		public static string HostName = "www.google.com";

		public bool IsReachableWithoutRequiringConnection (NetworkReachabilityFlags flags)
		{
			// Is it reachable with the current network configuration?
			bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

			// Do we need a connection to reach it?
			bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0
				|| (flags & NetworkReachabilityFlags.IsWWAN) != 0;

			return isReachable && noConnectionRequired;
		}

		// Is the host reachable with the current network configuration
		public  bool IsHostReachable(string host)
		{
			if (string.IsNullOrEmpty(host))
				return false;

			using (var r = new NetworkReachability(host)) {
				NetworkReachabilityFlags flags;

				if (r.TryGetFlags(out flags))
					return IsReachableWithoutRequiringConnection(flags);
			}
			return false;
		}
	}
}
