using PlayHaven;
using UnityEngine;

[AddComponentMenu("PlayHaven/Content Requester")]
public class PlayHavenContentRequester : MonoBehaviour
{
	public enum WhenToRequest
	{
		Awake = 0,
		Start = 1,
		OnEnable = 2,
		OnDisable = 3,
		Manual = 4
	}

	public enum MessageType
	{
		None = 0,
		Send = 1,
		Broadcast = 2,
		Upwards = 3
	}

	public enum ExhaustedAction
	{
		None = 0,
		DestroySelf = 1,
		DestroyGameObject = 2,
		DestroyRoot = 3
	}

	public WhenToRequest whenToRequest = WhenToRequest.Manual;

	public string placement = string.Empty;

	public bool showsOverlayImmediately;

	public bool rewardMayBeDelivered;

	public MessageType rewardMessageType = MessageType.Broadcast;

	public bool useDefaultTestReward;

	public string defaultTestRewardName = string.Empty;

	public int defaultTestRewardQuantity = 1;

	public float requestDelay;

	public bool limitedUse;

	public int maxUses;

	public ExhaustedAction exhaustAction;

	private PlayHavenManager playHaven;

	private bool rewardHandlerDefined;

	private bool exhausted;

	private int uses;

	private int requestId;

	private bool requestIsInProgress;

	private PlayHavenManager Manager
	{
		get
		{
			if (!playHaven)
			{
				playHaven = PlayHavenManager.instance;
			}
			return playHaven;
		}
	}

	public int RequestId
	{
		get
		{
			return requestId;
		}
	}

	public bool IsExhausted
	{
		get
		{
			return limitedUse && uses > maxUses;
		}
	}

	private void Awake()
	{
		if (whenToRequest == WhenToRequest.Awake)
		{
			if (requestDelay > 0f)
			{
				Invoke("Request", requestDelay);
			}
			else
			{
				Request();
			}
		}
	}

	private void OnEnable()
	{
		if (whenToRequest == WhenToRequest.OnEnable)
		{
			if (requestDelay > 0f)
			{
				Invoke("Request", requestDelay);
			}
			else
			{
				Request();
			}
		}
	}

	private void OnDisable()
	{
		if (whenToRequest == WhenToRequest.OnDisable)
		{
			Request();
		}
	}

	private void OnDestroy()
	{
		if ((bool)Manager)
		{
			Manager.OnRewardGiven -= HandlePlayHavenManagerOnRewardGiven;
			Manager.OnDismissContent -= HandlePlayHavenManagerOnDismissContent;
		}
	}

	private void Start()
	{
		if (whenToRequest == WhenToRequest.Start)
		{
			if (requestDelay > 0f)
			{
				Invoke("Request", requestDelay);
			}
			else
			{
				Request();
			}
		}
	}

	private void RequestPlayHavenContent()
	{
		if (requestDelay > 0f)
		{
			Invoke("Request", requestDelay);
		}
		else
		{
			Request();
		}
	}

	public void Request()
	{
		if (requestIsInProgress)
		{
			if (Application.isEditor)
			{
				Debug.Log("request is in progress; not making another request");
			}
			return;
		}
		if (exhausted)
		{
			if (Application.isEditor)
			{
				Debug.LogWarning("content requester has been exhausted");
			}
			return;
		}
		if ((bool)Manager)
		{
			if (placement.Length > 0)
			{
				if (rewardMayBeDelivered && !rewardHandlerDefined)
				{
					Manager.OnRewardGiven += HandlePlayHavenManagerOnRewardGiven;
					Manager.OnDismissContent += HandlePlayHavenManagerOnDismissContent;
					rewardHandlerDefined = true;
				}
				requestId = Manager.ContentRequest(placement, showsOverlayImmediately);
			}
			else
			{
				Debug.LogError("placement value not set in PlayHaventContentRequester");
			}
		}
		else
		{
			Debug.LogError("PlayHaven manager is not available in the scene. Content requests cannot be initiated.");
		}
		uses++;
		if (limitedUse && !rewardMayBeDelivered && uses >= maxUses)
		{
			Exhaust();
		}
	}

	private void Exhaust()
	{
		exhausted = true;
		switch (exhaustAction)
		{
		case ExhaustedAction.DestroySelf:
			Object.Destroy(this);
			break;
		case ExhaustedAction.DestroyGameObject:
			Object.Destroy(base.gameObject);
			break;
		case ExhaustedAction.DestroyRoot:
			Object.Destroy(base.transform.root.gameObject);
			break;
		}
	}

	private void HandlePlayHavenManagerOnDismissContent(int hashCode, DismissType dismissType)
	{
		if (requestId == hashCode)
		{
			requestIsInProgress = false;
			if ((bool)Manager)
			{
				Manager.OnRewardGiven -= HandlePlayHavenManagerOnRewardGiven;
				Manager.OnDismissContent -= HandlePlayHavenManagerOnDismissContent;
			}
			rewardHandlerDefined = false;
			switch (rewardMessageType)
			{
			case MessageType.Broadcast:
				BroadcastMessage("OnPlayHavenContentDismissed", dismissType, SendMessageOptions.DontRequireReceiver);
				break;
			case MessageType.Send:
				SendMessage("OnPlayHavenContentDismissed", dismissType, SendMessageOptions.DontRequireReceiver);
				break;
			case MessageType.Upwards:
				SendMessageUpwards("OnPlayHavenContentDismissed", dismissType, SendMessageOptions.DontRequireReceiver);
				break;
			}
		}
	}

	public void HandlePlayHavenManagerOnRewardGiven(int hashCode, Reward reward)
	{
		if (requestId == hashCode)
		{
			switch (rewardMessageType)
			{
			case MessageType.Broadcast:
				BroadcastMessage("OnPlayHavenRewardGiven", reward);
				break;
			case MessageType.Send:
				SendMessage("OnPlayHavenRewardGiven", reward);
				break;
			case MessageType.Upwards:
				SendMessageUpwards("OnPlayHavenRewardGiven", reward);
				break;
			}
			if ((bool)Manager)
			{
				Manager.OnRewardGiven -= HandlePlayHavenManagerOnRewardGiven;
				Manager.OnDismissContent -= HandlePlayHavenManagerOnDismissContent;
			}
			rewardHandlerDefined = false;
			if (!exhausted && limitedUse && uses > maxUses)
			{
				Exhaust();
			}
		}
	}
}
