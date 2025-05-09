using System;
using PlayHaven;
using UnityEngine;

public static class PlayHavenInterface
{
	public delegate void DismissHandler(string placement);

	public delegate void ErrorHandler(string placement, string errorDescription);

	public delegate void PurchaseInitiatedHandler(string productId);

	private static string activePlacement = string.Empty;

	private static int lastContentRequestHash;

	private static bool showAlert;

	private static DateTime lastBadgeRequest;

	public static Version Version
	{
		get
		{
			return new Version(1, 1, 6);
		}
	}

	public static event DismissHandler OnDismiss;

	public static event ErrorHandler OnError;

	public static event PurchaseInitiatedHandler OnPurchaseInitiated;

	public static void Init(string appToken, string secretCode)
	{
	}

	public static void Destroy()
	{
		Component component = UnityEngine.Object.FindObjectOfType(typeof(PlayHavenManager)) as Component;
		if (component != null)
		{
			UnityEngine.Object.Destroy(component.gameObject);
			PlayHavenManager.instance.OnErrorContentRequest -= ContentErrorHandler;
			PlayHavenManager.instance.OnErrorCrossPromotionWidget -= ContentErrorHandler;
			PlayHavenManager.instance.OnDismissContent -= ContentDismissHandler;
			PlayHavenManager.instance.OnDismissCrossPromotionWidget -= OldDismissHandler;
			PlayHavenManager.instance.OnPurchasePresented -= PurchaseTriggered;
		}
	}

	public static void StartPublisherContentRequest(string placement, bool showOverlayImmediately, bool showErrorMessage)
	{
		if (lastContentRequestHash == 0 || PlayHavenBinding.GetRequestWithHash(lastContentRequestHash) == null)
		{
			activePlacement = placement;
			showAlert = showErrorMessage;
			lastContentRequestHash = PlayHavenManager.instance.ContentRequest(placement, showOverlayImmediately);
			if (lastContentRequestHash == 0 && showAlert)
			{
				displayAlert("PlayHaven error", "Network connection is required to use this feature");
			}
		}
	}

	public static void StartPublisherContentRequest(string placement, bool showOverlayImmediately)
	{
		StartPublisherContentRequest(placement, showOverlayImmediately, false);
	}

	public static void SendIapTrackingRequest(string productId, PurchaseResolution resolution)
	{
		Purchase purchase = new Purchase();
		purchase.productIdentifier = productId;
		purchase.quantity = 1;
		PlayHavenManager.instance.ProductPurchaseTrackingRequest(purchase, resolution);
	}

	public static void SendVgpResolutionRequest(PurchaseResolution resolution)
	{
		PlayHavenManager.instance.ProductPurchaseResolutionRequest(resolution);
	}

	private static void PurchaseTriggered(int requestId, Purchase purchase)
	{
		if (PlayHavenInterface.OnPurchaseInitiated != null)
		{
			PlayHavenInterface.OnPurchaseInitiated(purchase.productIdentifier);
		}
	}

	public static void CancelCurrentContentRequest()
	{
		PlayHavenBinding.CancelRequest(lastContentRequestHash);
		lastContentRequestHash = 0;
	}

	public static int GetBadgeNumber()
	{
		if ((DateTime.Now - lastBadgeRequest).TotalHours > 3.0)
		{
			PlayHavenManager.instance.BadgeRequest();
			lastBadgeRequest = DateTime.Now;
		}
		return Convert.ToInt32("0" + PlayHavenManager.instance.Badge);
	}

	private static void ContentErrorHandler(int requestId, Error error)
	{
		lastContentRequestHash = 0;
		if (PlayHavenInterface.OnError != null)
		{
			PlayHavenInterface.OnError(activePlacement, error.description);
		}
		if (showAlert)
		{
			displayAlert("PlayHaven error", "Network connection is required to use this feature");
		}
	}

	private static void ContentDismissHandler(int requestId, DismissType type)
	{
		lastContentRequestHash = 0;
		if (PlayHavenInterface.OnDismiss != null)
		{
			PlayHavenInterface.OnDismiss(activePlacement);
		}
	}

	private static void OldDismissHandler()
	{
		lastContentRequestHash = 0;
		if (PlayHavenInterface.OnDismiss != null)
		{
			PlayHavenInterface.OnDismiss(activePlacement);
		}
	}

	private static void displayAlert(string title, string text)
	{
	}
}
