using System;
using System.Collections;
using LitJson;
using UnityEngine;

namespace PlayHaven
{
	public class PlayHavenBinding : IDisposable
	{
		public enum RequestType
		{
			Open = 0,
			Metadata = 1,
			Content = 2,
			Preload = 3,
			CrossPromotionWidget = 4
		}

		public interface IPlayHavenRequest
		{
			int HashCode { get; }

			event GeneralHandler OnWillDisplay;

			event GeneralHandler OnDidDisplay;

			event SuccessHandler OnSuccess;

			event ErrorHandler OnError;

			event DismissHandler OnDismiss;

			event RewardHandler OnReward;

			event PurchaseHandler OnPurchasePresented;

			void Send();

			void Send(bool showsOverlayImmediately);

			void TriggerEvent(string eventName, JsonData eventData);
		}

		public class OpenRequest : IPlayHavenRequest
		{
			private int hashCode;

			public int HashCode
			{
				get
				{
					return hashCode;
				}
			}

			public event SuccessHandler OnSuccess = delegate
			{
			};

			public event ErrorHandler OnError = delegate
			{
			};

			public event DismissHandler OnDismiss;

			public event RewardHandler OnReward;

			public event PurchaseHandler OnPurchasePresented;

			public event GeneralHandler OnWillDisplay;

			public event GeneralHandler OnDidDisplay;

			public OpenRequest(string customUDID)
			{
			}

			public OpenRequest()
			{
				sRequests.Add(GetHashCode(), this);
			}

			public void Send()
			{
				Send(false);
			}

			public void Send(bool showsOverlayImmediately)
			{
				hashCode = GetHashCode();
				if (Application.isEditor)
				{
					Debug.Log("PlayHaven: open request");
					PlayHavenManager instance = PlayHavenManager.instance;
					if (instance != null)
					{
						Hashtable hashtable = new Hashtable();
						hashtable["notification"] = new Hashtable();
						Hashtable hashtable2 = new Hashtable();
						hashtable2["data"] = hashtable;
						hashtable2["hash"] = hashCode;
						hashtable2["name"] = "success";
						string json = JsonMapper.ToJson(hashtable2);
						instance.HandleNativeEvent(json);
					}
				}
			}

			public void TriggerEvent(string eventName, JsonData eventData)
			{
				if (string.Compare(eventName, "success") == 0)
				{
					Debug.Log("PlayHaven: Open request success!");
					if (Debug.isDebugBuild)
					{
						Debug.Log("JSON (trigger event): " + eventData.ToJson());
					}
					this.OnSuccess(this, eventData);
				}
				else if (string.Compare(eventName, "error") == 0)
				{
					Debug.LogError("PlayHaven: Open request failed!");
					if (Debug.isDebugBuild)
					{
						Debug.Log("JSON (trigger event): " + eventData.ToJson());
					}
					this.OnError(this, eventData);
				}
			}
		}

		public class MetadataRequest : IPlayHavenRequest
		{
			protected string mPlacement;

			private int hashCode;

			public int HashCode
			{
				get
				{
					return hashCode;
				}
			}

			public event SuccessHandler OnSuccess = delegate
			{
			};

			public event ErrorHandler OnError = delegate
			{
			};

			public event DismissHandler OnDismiss;

			public event RewardHandler OnReward;

			public event PurchaseHandler OnPurchasePresented;

			public event GeneralHandler OnWillDisplay = delegate
			{
			};

			public event GeneralHandler OnDidDisplay = delegate
			{
			};

			public MetadataRequest(string placement)
			{
				mPlacement = placement;
				sRequests.Add(GetHashCode(), this);
			}

			public void Send()
			{
				Send(false);
			}

			public void Send(bool showsOverlayImmediately)
			{
				hashCode = GetHashCode();
				if (Application.isEditor)
				{
					Debug.Log("PlayHaven: metadata request (" + mPlacement + ")");
					PlayHavenManager instance = PlayHavenManager.instance;
					if (instance != null)
					{
						Hashtable hashtable = new Hashtable();
						hashtable["type"] = "badge";
						hashtable["value"] = "1";
						Hashtable hashtable2 = new Hashtable();
						hashtable2["notification"] = hashtable;
						Hashtable hashtable3 = new Hashtable();
						hashtable3["data"] = hashtable2;
						hashtable3["hash"] = hashCode;
						hashtable3["name"] = "success";
						hashtable3["content"] = mPlacement;
						string json = JsonMapper.ToJson(hashtable3);
						instance.HandleNativeEvent(json);
					}
				}
			}

			public void TriggerEvent(string eventName, JsonData eventData)
			{
				if (string.Compare(eventName, "success") == 0)
				{
					Debug.Log("PlayHaven: Metadata request success!");
					if (Debug.isDebugBuild)
					{
						Debug.Log("JSON (trigger event): " + eventData.ToJson());
					}
					this.OnSuccess(this, eventData);
				}
				else if (string.Compare(eventName, "willdisplay") == 0)
				{
					this.OnWillDisplay(this);
				}
				else if (string.Compare(eventName, "diddisplay") == 0)
				{
					this.OnDidDisplay(this);
				}
				else if (string.Compare(eventName, "error") == 0)
				{
					Debug.LogError("PlayHaven: Metadata request failed!");
					if (Debug.isDebugBuild)
					{
						Debug.Log("JSON (trigger event): " + eventData.ToJson());
					}
					this.OnError(this, eventData);
				}
			}
		}

		public class ContentRequest : IPlayHavenRequest
		{
			protected string mPlacement;

			private int hashCode;

			public int HashCode
			{
				get
				{
					return hashCode;
				}
			}

			public event SuccessHandler OnSuccess;

			public event DismissHandler OnDismiss = delegate
			{
			};

			public event ErrorHandler OnError = delegate
			{
			};

			public event RewardHandler OnReward = delegate
			{
			};

			public event PurchaseHandler OnPurchasePresented = delegate
			{
			};

			public event GeneralHandler OnWillDisplay = delegate
			{
			};

			public event GeneralHandler OnDidDisplay = delegate
			{
			};

			public ContentRequest(string placement)
			{
				mPlacement = placement;
				sRequests.Add(GetHashCode(), this);
			}

			public void Send()
			{
				Send(false);
			}

			public void Send(bool showsOverlayImmediately)
			{
				hashCode = GetHashCode();
				if (Application.isEditor)
				{
					Debug.Log("PlayHaven: content request (" + mPlacement + ")");
					PlayHavenManager instance = PlayHavenManager.instance;
					if (instance != null)
					{
						Hashtable hashtable = new Hashtable();
						hashtable["notification"] = new Hashtable();
						Hashtable hashtable2 = new Hashtable();
						hashtable2["data"] = hashtable;
						hashtable2["hash"] = hashCode;
						hashtable2["name"] = "dismiss";
						string json = JsonMapper.ToJson(hashtable2);
						instance.HandleNativeEvent(json);
					}
				}
			}

			public void TriggerEvent(string eventName, JsonData eventData)
			{
				if (string.Compare(eventName, "reward") == 0)
				{
					Debug.Log("PlayHaven: Reward unlocked");
					if (Debug.isDebugBuild)
					{
						Debug.Log("JSON (trigger event): " + eventData.ToJson());
					}
					this.OnReward(this, eventData);
				}
				else if (string.Compare(eventName, "purchasePresentation") == 0)
				{
					Debug.Log("PlayHaven: Purchase presented");
					if (Debug.isDebugBuild)
					{
						Debug.Log("JSON (trigger event): " + eventData.ToJson());
					}
					this.OnPurchasePresented(this, eventData);
				}
				else if (string.Compare(eventName, "dismiss") == 0)
				{
					Debug.Log("PlayHaven: Content was dismissed!");
					if (Debug.isDebugBuild)
					{
						Debug.Log("JSON (trigger event): " + eventData.ToJson());
					}
					this.OnDismiss(this, eventData);
				}
				else if (string.Compare(eventName, "willdisplay") == 0)
				{
					this.OnWillDisplay(this);
				}
				else if (string.Compare(eventName, "diddisplay") == 0)
				{
					this.OnDidDisplay(this);
				}
				else if (string.Compare(eventName, "error") == 0)
				{
					Debug.LogError("PlayHaven: Content error!");
					if (Debug.isDebugBuild)
					{
						Debug.Log("JSON (trigger event): " + eventData.ToJson());
					}
					this.OnError(this, eventData);
				}
			}
		}

		public class ContentPreloadRequest : IPlayHavenRequest
		{
			protected string mPlacement;

			private int hashCode;

			public int HashCode
			{
				get
				{
					return hashCode;
				}
			}

			public event SuccessHandler OnSuccess = delegate
			{
			};

			public event DismissHandler OnDismiss;

			public event ErrorHandler OnError = delegate
			{
			};

			public event RewardHandler OnReward;

			public event PurchaseHandler OnPurchasePresented;

			public event GeneralHandler OnWillDisplay;

			public event GeneralHandler OnDidDisplay;

			public ContentPreloadRequest(string placement)
			{
				mPlacement = placement;
				sRequests.Add(GetHashCode(), this);
			}

			public void Send()
			{
				Send(false);
			}

			public void Send(bool showsOverlayImmediately)
			{
				hashCode = GetHashCode();
				if (Application.isEditor)
				{
					Debug.Log("PlayHaven: content preload request (" + mPlacement + ")");
					PlayHavenManager instance = PlayHavenManager.instance;
					if (instance != null)
					{
						Hashtable hashtable = new Hashtable();
						hashtable["notification"] = new Hashtable();
						Hashtable hashtable2 = new Hashtable();
						hashtable2["data"] = hashtable;
						hashtable2["hash"] = hashCode;
						hashtable2["name"] = "dismiss";
						string json = JsonMapper.ToJson(hashtable2);
						instance.HandleNativeEvent(json);
					}
				}
			}

			public void TriggerEvent(string eventName, JsonData eventData)
			{
				if (string.Compare(eventName, "gotcontent") == 0)
				{
					Debug.Log("PlayHaven: Preloaded content");
					if (Debug.isDebugBuild)
					{
						Debug.Log("JSON (trigger event): " + eventData.ToJson());
					}
					this.OnSuccess(this, eventData);
				}
				else if (string.Compare(eventName, "error") == 0)
				{
					Debug.LogError("PlayHaven: Content error!");
					if (Debug.isDebugBuild)
					{
						Debug.Log("JSON (trigger event): " + eventData.ToJson());
					}
					this.OnError(this, eventData);
				}
			}
		}

		public delegate void SuccessHandler(IPlayHavenRequest request, JsonData responseData);

		public delegate void ErrorHandler(IPlayHavenRequest request, JsonData errorData);

		public delegate void RewardHandler(IPlayHavenRequest request, JsonData rewardData);

		public delegate void PurchaseHandler(IPlayHavenRequest request, JsonData purchaseData);

		public delegate void DismissHandler(IPlayHavenRequest request, JsonData dismissData);

		public delegate void GeneralHandler(IPlayHavenRequest request);

		public static string token;

		public static string secret;

		public static IPlayHavenListener listener;

		protected static Hashtable sRequests = new Hashtable();

		private static bool optOutStatus;

		public static bool OptOutStatus
		{
			get
			{
				return optOutStatus;
			}
			set
			{
				optOutStatus = value;
			}
		}

		public void Dispose()
		{
		}

		public static void Initialize()
		{
			if (!PlayHavenManager.IsAndroidSupported)
			{
				return;
			}
		}

		public static void SetKeys(string token, string secret)
		{
			PlayHavenBinding.token = token;
			PlayHavenBinding.secret = secret;
		}

		public static int Open()
		{
			return SendRequest(RequestType.Open, string.Empty);
		}

		public static int Open(string customUDID)
		{
			return SendRequest(RequestType.Open, customUDID);
		}

		public static void CancelRequest(int requestId)
		{
		}

		public static void RegisterActivityForTracking(bool register)
		{
		}

		public static void SendProductPurchaseResolution(PurchaseResolution resolution)
		{
		}

		public static void SendIAPTrackingRequest(Purchase purchase, PurchaseResolution resolution)
		{
		}

		public static int SendRequest(RequestType type, string placement)
		{
			return SendRequest(type, placement, false);
		}

		public static int SendRequest(RequestType type, string placement, bool showsOverlayImmediately)
		{
			IPlayHavenRequest playHavenRequest = null;
			switch (type)
			{
			case RequestType.Open:
				playHavenRequest = new OpenRequest(placement);
				playHavenRequest.OnSuccess += HandleOpenRequestOnSuccess;
				playHavenRequest.OnError += HandleOpenRequestOnError;
				break;
			case RequestType.Metadata:
				playHavenRequest = new MetadataRequest(placement);
				playHavenRequest.OnSuccess += HandleMetadataRequestOnSuccess;
				playHavenRequest.OnError += HandleMetadataRequestOnError;
				playHavenRequest.OnWillDisplay += HandleMetadataRequestOnWillDisplay;
				playHavenRequest.OnDidDisplay += HandleMetadataRequestOnDidDisplay;
				break;
			case RequestType.Content:
				playHavenRequest = new ContentRequest(placement);
				playHavenRequest.OnError += HandleContentRequestOnError;
				playHavenRequest.OnDismiss += HandleContentRequestOnDismiss;
				playHavenRequest.OnReward += HandleContentRequestOnReward;
				playHavenRequest.OnPurchasePresented += HandleRequestOnPurchasePresented;
				playHavenRequest.OnWillDisplay += HandleContentRequestOnWillDisplay;
				playHavenRequest.OnDidDisplay += HandleContentRequestOnDidDisplay;
				break;
			case RequestType.Preload:
				playHavenRequest = new ContentPreloadRequest(placement);
				playHavenRequest.OnError += HandleContentRequestOnError;
				playHavenRequest.OnSuccess += HandlePreloadRequestOnSuccess;
				break;
			case RequestType.CrossPromotionWidget:
				playHavenRequest = new ContentRequest("more_games");
				playHavenRequest.OnError += HandleCrossPromotionWidgetRequestOnError;
				playHavenRequest.OnDismiss += HandleCrossPromotionWidgetRequestOnDismiss;
				playHavenRequest.OnWillDisplay += HandleCrossPromotionWidgetRequestOnWillDisplay;
				playHavenRequest.OnDidDisplay += HandleCrossPromotionWidgetRequestOnDidDisplay;
				break;
			}
			if (playHavenRequest != null)
			{
				playHavenRequest.Send(showsOverlayImmediately);
				return playHavenRequest.HashCode;
			}
			return 0;
		}

		private static void HandlePreloadRequestOnSuccess(IPlayHavenRequest request, JsonData responseData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				listener.NotifyPreloadSuccess(request.HashCode);
			}
		}

		private static Error CreateErrorFromJSON(JsonData errorData)
		{
			Error error = new Error();
			try
			{
				error.code = (int)errorData["code"];
			}
			catch (Exception ex)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(ex.Message);
				}
			}
			try
			{
				error.description = (string)errorData["description"];
			}
			catch (Exception ex2)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(ex2.Message);
				}
			}
			return error;
		}

		private static void HandleCrossPromotionWidgetRequestOnDismiss(IPlayHavenRequest request, JsonData dismissData)
		{
			if (listener != null)
			{
				listener.NotifyCrossPromotionWidgetDismissed();
			}
		}

		private static void HandleCrossPromotionWidgetRequestOnWillDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				listener.NotifyWillDisplayContent(request.HashCode);
			}
		}

		private static void HandleCrossPromotionWidgetRequestOnDidDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
			{
				listener.NotifyDidDisplayContent(request.HashCode);
			}
		}

		private static void HandleCrossPromotionWidgetRequestOnError(IPlayHavenRequest request, JsonData errorData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				Error error = CreateErrorFromJSON(errorData);
				listener.NotifyCrossPromotionWidgetError(request.HashCode, error);
			}
		}

		private static void HandleContentRequestOnDismiss(IPlayHavenRequest request, JsonData dismissData)
		{
			DismissType dismissType = DismissType.Unknown;
			try
			{
				switch ((string)dismissData["type"])
				{
				case "ApplicationTriggered":
					dismissType = DismissType.PHPublisherApplicationBackgroundTriggeredDismiss;
					break;
				case "ContentUnitTriggered":
					dismissType = DismissType.PHPublisherContentUnitTriggeredDismiss;
					break;
				case "CloseButtonTriggered":
					dismissType = DismissType.PHPublisherNativeCloseButtonTriggeredDismiss;
					break;
				case "NoContentTriggered":
					dismissType = DismissType.PHPublisherNoContentTriggeredDismiss;
					break;
				}
			}
			catch (Exception ex)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(ex.Message);
				}
			}
			if (listener != null)
			{
				listener.NotifyContentDismissed(request.HashCode, dismissType);
			}
		}

		private static void HandleContentRequestOnWillDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				listener.NotifyWillDisplayContent(request.HashCode);
			}
		}

		private static void HandleContentRequestOnDidDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
			{
				listener.NotifyDidDisplayContent(request.HashCode);
			}
		}

		private static void HandleContentRequestOnReward(IPlayHavenRequest request, JsonData rewardData)
		{
			Reward reward = new Reward();
			try
			{
				reward.receipt = (string)rewardData["receipt"];
			}
			catch (Exception ex)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(ex.Message);
				}
			}
			try
			{
				reward.name = (string)rewardData["name"];
				reward.quantity = (int)rewardData["quantity"];
				if (listener != null)
				{
					listener.NotifyRewardGiven(request.HashCode, reward);
				}
			}
			catch (Exception ex2)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(ex2.Message);
				}
			}
		}

		private static void HandleRequestOnPurchasePresented(IPlayHavenRequest request, JsonData purchaseData)
		{
			Purchase purchase = new Purchase();
			try
			{
				purchase.receipt = (string)purchaseData["receipt"];
			}
			catch (Exception ex)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(ex.Message);
				}
			}
			try
			{
				purchase.productIdentifier = (string)purchaseData["productIdentifier"];
				purchase.quantity = (int)purchaseData["quantity"];
				if (listener != null)
				{
					listener.NotifyPurchasePresented(request.HashCode, purchase);
				}
			}
			catch (Exception ex2)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(ex2.Message);
				}
			}
		}

		private static void HandleContentRequestOnError(IPlayHavenRequest request, JsonData errorData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				Error error = CreateErrorFromJSON(errorData);
				listener.NotifyContentError(request.HashCode, error);
			}
		}

		private static void HandleMetadataRequestOnError(IPlayHavenRequest request, JsonData errorData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				Error error = CreateErrorFromJSON(errorData);
				listener.NotifyMetaDataError(request.HashCode, error);
			}
		}

		private static void HandleMetadataRequestOnSuccess(IPlayHavenRequest request, JsonData responseData)
		{
			string text;
			try
			{
				text = (string)responseData["notification"]["type"];
			}
			catch (Exception ex)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(ex.Message);
				}
				text = string.Empty;
			}
			if (!(text == "badge"))
			{
				return;
			}
			try
			{
				string badge = (string)responseData["notification"]["value"];
				if (listener != null)
				{
					listener.NotifyBadgeUpdate(request.HashCode, badge);
				}
			}
			catch (Exception ex2)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(ex2.Message);
				}
			}
		}

		private static void HandleMetadataRequestOnWillDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				listener.NotifyWillDisplayContent(request.HashCode);
			}
		}

		private static void HandleMetadataRequestOnDidDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
			{
				listener.NotifyDidDisplayContent(request.HashCode);
			}
		}

		private static void HandleOpenRequestOnError(IPlayHavenRequest request, JsonData errorData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				Error error = CreateErrorFromJSON(errorData);
				listener.NotifyOpenError(request.HashCode, error);
			}
		}

		private static void HandleOpenRequestOnSuccess(IPlayHavenRequest request, JsonData responseData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				listener.NotifyOpenSuccess(request.HashCode);
			}
		}

		public static IPlayHavenRequest GetRequestWithHash(int hash)
		{
			if (sRequests.ContainsKey(hash))
			{
				return (IPlayHavenRequest)sRequests[hash];
			}
			return null;
		}

		public static void ClearRequestWithHash(int hash)
		{
			if (sRequests.ContainsKey(hash))
			{
				sRequests.Remove(hash);
			}
		}
	}
}
