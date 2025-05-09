using UnityEngine;

namespace Glu.Burstly.Internal.API
{
	public sealed class JavaBurstlyAPI : IBurstlyAPI
	{
		public object CreateManager(string publisherId, string zoneId)
		{
			return null;
		}

		public void DestroyManager(object peer)
		{
		}

		public void PrecacheAd(object peer)
		{
		}

		public void RefreshAd(object peer)
		{
		}

		public bool IsPrecached(object peer)
		{
			return false;
		}

		public bool IsVisible(object peer)
		{
			return false;
		}

		public void SetVisible(object peer, bool visible)
		{
		}

		public bool IsPaused(object peer)
		{
			return false;
		}

		public void SetPaused(object peer, bool paused)
		{
		}

		public int GetBannerAlignment(object peer)
		{
			return 6;
		}

		public void SetBannerAlignment(object peer, int anchor)
		{
		}

		public Vector2 GetBannerOffset(object peer)
		{
			return Vector2.zero;
		}

		public void SetBannerOffset(object peer, Vector2 point)
		{
		}

		public int GetDefaultSessionLife(object peer)
		{
			return 0;
		}

		public void SetDefaultSessionLife(object peer, int timeout)
		{
		}

		public void SetPublisherTargetingData(object peer, string data)
		{
		}

		public bool IsFullScreen(object peer)
		{
			return false;
		}

		public void Update(object peer)
		{
		}

		public string GetSDKVersion()
		{
			return string.Empty;
		}

		public void SetServerURL(string url)
		{
		}
	}
}
