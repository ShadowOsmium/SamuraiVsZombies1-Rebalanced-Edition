using System.Collections;
using PlayHaven;
using UnityEngine;

[AddComponentMenu("PlayHaven/VGP Handler")]
public class PlayHavenVGPHandler : MonoBehaviour
{
	public delegate void PurchaseEventHandler(int requestId, Purchase purchase);

	private static PlayHavenVGPHandler instance;

	private PlayHavenManager playHaven;

	private Hashtable purchases = new Hashtable(4);

	public static PlayHavenVGPHandler Instance
	{
		get
		{
			if (!instance)
			{
				instance = Object.FindObjectOfType(typeof(PlayHavenVGPHandler)) as PlayHavenVGPHandler;
			}
			return instance;
		}
	}

	public event PurchaseEventHandler OnPurchasePresented;

	private void Awake()
	{
		playHaven = PlayHavenManager.instance;
	}

	private void OnEnable()
	{
		playHaven.OnPurchasePresented += PlayHavenOnPurchasePresented;
	}

	private void OnDisable()
	{
		playHaven.OnPurchasePresented -= PlayHavenOnPurchasePresented;
	}

	private void PlayHavenOnPurchasePresented(int requestId, Purchase purchase)
	{
		if (this.OnPurchasePresented != null)
		{
			purchases.Add(requestId, purchase);
			this.OnPurchasePresented(requestId, purchase);
		}
	}

	public void ResolvePurchase(int requestId, PurchaseResolution resolution, bool track)
	{
		if (purchases.ContainsKey(requestId))
		{
			Purchase purchase = (Purchase)purchases[requestId];
			purchases.Remove(requestId);
			playHaven.ProductPurchaseResolutionRequest(resolution);
			if (track)
			{
				playHaven.ProductPurchaseTrackingRequest(purchase, resolution);
			}
		}
		else if (Debug.isDebugBuild)
		{
			Debug.LogWarning("PlayHaven VGP handler does not have a record of a purchase with the provided request identifier: " + requestId);
		}
	}

	public void ResolvePurchase(Purchase purchase, PurchaseResolution resolution, bool track)
	{
		if (!purchases.ContainsValue(purchase))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("PlayHaven VGP handler does not have a record of a purchase with the provided purchase object; will track only if requested.");
			}
			if (track)
			{
				playHaven.ProductPurchaseTrackingRequest(purchase, resolution);
			}
			return;
		}
		int num = -1;
		foreach (int key in purchases.Keys)
		{
			if (purchases[key] == purchase)
			{
				num = key;
				break;
			}
		}
		if (num > -1)
		{
			purchases.Remove(num);
			playHaven.ProductPurchaseResolutionRequest(resolution);
			if (track)
			{
				playHaven.ProductPurchaseTrackingRequest(purchase, resolution);
			}
		}
		else
		{
			Debug.LogError("Unable to determine request identifier for provided purchase object.");
		}
	}
}
