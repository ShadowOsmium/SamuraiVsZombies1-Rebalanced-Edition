using UnityEngine;

namespace Glu.Burstly.Internal.API
{
	public interface IBurstlyAPI
	{
		object CreateManager(string publisherId, string zoneId);

		void DestroyManager(object peer);

		void RefreshAd(object peer);

		void PrecacheAd(object peer);

		bool IsPrecached(object peer);

		bool IsVisible(object peer);

		void SetVisible(object peer, bool visible);

		bool IsPaused(object peer);

		void SetPaused(object peer, bool paused);

		int GetBannerAlignment(object peer);

		void SetBannerAlignment(object peer, int anchor);

		Vector2 GetBannerOffset(object peer);

		void SetBannerOffset(object peer, Vector2 point);

		int GetDefaultSessionLife(object peer);

		void SetDefaultSessionLife(object peer, int timeout);

		void SetPublisherTargetingData(object peer, string data);

		bool IsFullScreen(object peer);

		void Update(object peer);

		string GetSDKVersion();

		void SetServerURL(string url);
	}
}
