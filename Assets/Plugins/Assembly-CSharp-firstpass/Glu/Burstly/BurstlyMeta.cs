using System;
using Glu.Burstly.Internal.API;

namespace Glu.Burstly
{
	public static class BurstlyMeta
	{
		public static Version Version
		{
			get
			{
				return new Version("1.0.3.0");
			}
		}

		public static Version SDKVersion
		{
			get
			{
				string sDKVersion = BurstlyAPI.Instance.GetSDKVersion();
				return new Version(sDKVersion);
			}
		}
	}
}
