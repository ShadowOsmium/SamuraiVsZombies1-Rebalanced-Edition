using System;
using System.Collections;
using LitJson;
using PlayHaven;
using UnityEngine;

[AddComponentMenu("PlayHaven/Manager")]
public class PlayHavenManager : MonoBehaviour, IPlayHavenListener
{
	public enum WhenToOpen
	{
		Awake = 0,
		Start = 1,
		Manual = 2
	}

	public enum WhenToGetNotifications
	{
		Disabled = 0,
		Awake = 1,
		Start = 2,
		OnEnable = 3,
		Manual = 4,
		Poll = 5
	}

	public delegate void CancelRequestHandler(int requestId);

	public const int NO_HASH_CODE = 0;

	public static string KEY_LAUNCH_COUNT = "playhaven-launch-count";

	public string tokenAndroid = "31873c6e4392466284232fd9b5b6a9a5";

	[HideInInspector]
	public bool lockToken;

	public string secretAndroid = "cdaea7e9ab934703b28b9b1d37c090b6";

	[HideInInspector]
	public bool lockSecret;

	public bool doNotDestroyOnLoad = true;

	public bool defaultShowsOverlayImmediately;

	public bool maskShowsOverlayImmediately;

	public WhenToOpen whenToSendOpen;

	public WhenToGetNotifications whenToGetNotifications = WhenToGetNotifications.Start;

	public float notificationPollDelay = 1f;

	public float notificationPollRate = 15f;

	public bool cancelAllOnLevelLoad;

	private ArrayList requestsInProgress = new ArrayList(8);

	public int suppressContentRequestsForLaunches;

	public string[] suppressedPlacements;

	public string[] suppressionExceptions;

	private int launchCount;

	private string badge = string.Empty;

	private string customUDID = string.Empty;

	private bool networkReachable = true;

	public bool maskNetworkReachable;

	public bool isAndroidSupported;

	private static PlayHavenManager _instance;

	private static bool wasWarned;

	public static PlayHavenManager instance
	{
		get
		{
			if (!_instance)
			{
				_instance = FindInstance();
			}
			return _instance;
		}
	}

	public string CustomUDID
	{
		get
		{
			return customUDID;
		}
		set
		{
			customUDID = value;
		}
	}

	public bool OptOutStatus
	{
		get
		{
			return PlayHavenBinding.OptOutStatus;
		}
		set
		{
			PlayHavenBinding.OptOutStatus = value;
		}
	}

	public static bool IsAndroidSupported
	{
		get
		{
			PlayHavenManager playHavenManager = instance;
			if (playHavenManager != null)
			{
				return playHavenManager.isAndroidSupported;
			}
			return false;
		}
	}

	public string Badge
	{
		get
		{
			return badge;
		}
	}

	public event RequestCompletedHandler OnRequestCompleted;

	public event BadgeUpdateHandler OnBadgeUpdate;

	public event RewardTriggerHandler OnRewardGiven;

	public event PurchasePresentedTriggerHandler OnPurchasePresented;

	public event SimpleDismissHandler OnDismissCrossPromotionWidget;

	public event DismissHandler OnDismissContent;

	public event WillDisplayContentHandler OnWillDisplayContent;

	public event DidDisplayContentHandler OnDidDisplayContent;

	public event SuccessHandler OnSuccessOpenRequest;

	public event SuccessHandler OnSuccessPreloadRequest;

	public event ErrorHandler OnErrorOpenRequest;

	public event ErrorHandler OnErrorCrossPromotionWidget;

	public event ErrorHandler OnErrorContentRequest;

	public event ErrorHandler OnErrorMetadataRequest;

	public event CancelRequestHandler OnSuccessCancelRequest;

	public event CancelRequestHandler OnErrorCancelRequest;

	private static PlayHavenManager FindInstance()
	{
		PlayHavenManager playHavenManager = UnityEngine.Object.FindObjectOfType(typeof(PlayHavenManager)) as PlayHavenManager;
		if (!playHavenManager)
		{
			GameObject gameObject = GameObject.Find("PlayHavenManager");
			if (gameObject != null)
			{
				playHavenManager = gameObject.GetComponent<PlayHavenManager>();
			}
		}
		if (!playHavenManager && !wasWarned)
		{
			Debug.LogError("unable to locate a PlayHavenManager in the scene");
			wasWarned = true;
		}
		return playHavenManager;
	}

	private void Awake()
	{
	}

	private void OnEnable()
	{
		if (whenToGetNotifications == WhenToGetNotifications.OnEnable)
		{
			BadgeRequest();
		}
	}

	private void Start()
	{
		if (whenToSendOpen == WhenToOpen.Start)
		{
			OpenNotification();
		}
		if (whenToGetNotifications == WhenToGetNotifications.Start)
		{
			BadgeRequest();
		}
		else if (whenToGetNotifications == WhenToGetNotifications.Poll)
		{
			PollForBadgeRequests();
		}
	}

	private void DetectNetworkReachable()
	{
		networkReachable = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
		networkReachable &= !maskNetworkReachable;
	}

	private void OnApplicationPause(bool pause)
	{
		PlayHavenBinding.RegisterActivityForTracking(!pause);
	}

	private void OnLevelWasLoaded(int level)
	{
		if (cancelAllOnLevelLoad)
		{
			CancelAllPendingRequests();
		}
	}

	public bool IsPlacementSuppressed(string placement)
	{
		if (suppressContentRequestsForLaunches > 0 && launchCount < suppressContentRequestsForLaunches)
		{
			if (suppressedPlacements != null && suppressedPlacements.Length > 0)
			{
				string[] array = suppressedPlacements;
				foreach (string text in array)
				{
					if (text == placement)
					{
						return true;
					}
				}
				return false;
			}
			if (suppressionExceptions != null && suppressionExceptions.Length > 0)
			{
				string[] array2 = suppressionExceptions;
				foreach (string text2 in array2)
				{
					if (text2 == placement)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}
		return false;
	}

	public int OpenNotification(string customUDID)
	{
		if (networkReachable)
		{
			CustomUDID = customUDID;
			int num = PlayHavenBinding.Open(customUDID);
			requestsInProgress.Add(num);
			return num;
		}
		return 0;
	}

	public int OpenNotification()
	{
		if (networkReachable)
		{
			int num = PlayHavenBinding.Open(CustomUDID);
			requestsInProgress.Add(num);
			return num;
		}
		return 0;
	}

	public void CancelAllPendingRequests()
	{
		foreach (int item in requestsInProgress)
		{
			PlayHavenBinding.CancelRequest(item);
		}
		requestsInProgress.Clear();
	}

	public void ProductPurchaseResolutionRequest(PurchaseResolution resolution)
	{
		PlayHavenBinding.SendProductPurchaseResolution(resolution);
	}

	public void ProductPurchaseTrackingRequest(Purchase purchase, PurchaseResolution resolution)
	{
		PlayHavenBinding.SendIAPTrackingRequest(purchase, resolution);
	}

	public int ContentPreloadRequest(string placement)
	{
		if (networkReachable)
		{
			int num = PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.Preload, placement);
			requestsInProgress.Add(num);
			return num;
		}
		return 0;
	}

	public int ContentRequest(string placement)
	{
		if (IsPlacementSuppressed(placement))
		{
			return 0;
		}
		if (networkReachable)
		{
			int num = PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.Content, placement, defaultShowsOverlayImmediately);
			requestsInProgress.Add(num);
			return num;
		}
		return 0;
	}

	public int ContentRequest(string placement, bool showsOverlayImmediately)
	{
		if (IsPlacementSuppressed(placement))
		{
			return 0;
		}
		if (networkReachable)
		{
			int num = PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.Content, placement, showsOverlayImmediately && !maskShowsOverlayImmediately);
			requestsInProgress.Add(num);
			return num;
		}
		return 0;
	}

	public int ShowCrossPromotionWidget()
	{
		if (networkReachable)
		{
			int num = PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.CrossPromotionWidget, string.Empty, defaultShowsOverlayImmediately);
			requestsInProgress.Add(num);
			return num;
		}
		return 0;
	}

	public int BadgeRequest()
	{
		if (networkReachable && whenToGetNotifications != 0)
		{
			int num = PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.Metadata, "more_games");
			requestsInProgress.Add(num);
			return num;
		}
		return 0;
	}

	public void PollForBadgeRequests()
	{
	}

	public void NotifyRequestCompleted(int requestId)
	{
		requestsInProgress.Remove(requestId);
		if (this.OnRequestCompleted != null)
		{
			this.OnRequestCompleted(requestId);
		}
	}

	public void NotifyOpenSuccess(int requestId)
	{
		if (this.OnSuccessOpenRequest != null)
		{
			this.OnSuccessOpenRequest(requestId);
		}
	}

	public void NotifyOpenError(int requestId, Error error)
	{
		if (this.OnErrorOpenRequest != null)
		{
			this.OnErrorOpenRequest(requestId, error);
		}
	}

	public void NotifyWillDisplayContent(int requestId)
	{
		if (this.OnWillDisplayContent != null)
		{
			this.OnWillDisplayContent(requestId);
		}
	}

	public void NotifyDidDisplayContent(int requestId)
	{
		if (this.OnDidDisplayContent != null)
		{
			this.OnDidDisplayContent(requestId);
		}
	}

	public void NotifyPreloadSuccess(int requestId)
	{
		if (this.OnSuccessPreloadRequest != null)
		{
			this.OnSuccessPreloadRequest(requestId);
		}
	}

	public void NotifyBadgeUpdate(int requestId, string badge)
	{
		this.badge = badge;
		if (this.OnBadgeUpdate != null)
		{
			this.OnBadgeUpdate(requestId, badge);
		}
	}

	public void NotifyRewardGiven(int requestId, Reward reward)
	{
		if (this.OnRewardGiven != null)
		{
			this.OnRewardGiven(requestId, reward);
		}
	}

	public void NotifyPurchasePresented(int requestId, Purchase purchase)
	{
		if (this.OnPurchasePresented != null)
		{
			this.OnPurchasePresented(requestId, purchase);
		}
	}

	public void NotifyCrossPromotionWidgetDismissed()
	{
		if (this.OnDismissCrossPromotionWidget != null)
		{
			this.OnDismissCrossPromotionWidget();
		}
	}

	public void NotifyCrossPromotionWidgetError(int requestId, Error error)
	{
		if (this.OnErrorCrossPromotionWidget != null)
		{
			this.OnErrorCrossPromotionWidget(requestId, error);
		}
	}

	public void NotifyContentDismissed(int requestId, DismissType dismissType)
	{
		if (this.OnDismissContent != null)
		{
			this.OnDismissContent(requestId, dismissType);
		}
	}

	public void NotifyContentError(int requestId, Error error)
	{
		if (this.OnErrorContentRequest != null)
		{
			this.OnErrorContentRequest(requestId, error);
		}
	}

	public void NotifyMetaDataError(int requestId, Error error)
	{
		if (this.OnErrorMetadataRequest != null)
		{
			this.OnErrorMetadataRequest(requestId, error);
		}
	}

	public void ClearBadge()
	{
		badge = string.Empty;
	}

	public void HandleNativeEvent(string json)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log("JSON (native event): " + json);
		}
		JsonData jsonData = JsonMapper.ToObject(json);
		int hash = (int)jsonData["hash"];
		PlayHavenBinding.IPlayHavenRequest requestWithHash = PlayHavenBinding.GetRequestWithHash(hash);
		if (requestWithHash != null)
		{
			string text = (string)jsonData["name"];
			JsonData eventData = jsonData["data"];
			requestWithHash.TriggerEvent(text, eventData);
			if (text != "willdisplay" && text != "diddisplay" && text != "gotcontent")
			{
				PlayHavenBinding.ClearRequestWithHash(hash);
			}
		}
	}

	public void RequestCancelSuccess(string hashCodeString)
	{
		int num = Convert.ToInt32(hashCodeString);
		PlayHavenBinding.ClearRequestWithHash(num);
		if (this.OnSuccessCancelRequest != null)
		{
			this.OnSuccessCancelRequest(num);
		}
	}

	public void RequestCancelFailed(string hashCodeString)
	{
		if (this.OnErrorCancelRequest != null)
		{
			int requestId = Convert.ToInt32(hashCodeString);
			this.OnErrorCancelRequest(requestId);
		}
	}
}
