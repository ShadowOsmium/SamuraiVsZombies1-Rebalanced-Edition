namespace Glu.Burstly.Internal.API
{
	public static class BurstlyAPI
	{
		private static IBurstlyAPI api;

		public static IBurstlyAPI Instance
		{
			get
			{
				if (api == null)
				{
					api = new JavaBurstlyAPI();
				}
				return api;
			}
		}
	}
}
