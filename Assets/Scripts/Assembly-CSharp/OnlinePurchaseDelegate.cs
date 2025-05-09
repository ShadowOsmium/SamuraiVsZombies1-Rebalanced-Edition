using PlayHaven;
using Debug = UnityEngine.Debug;

public class OnlinePurchaseDelegate : ICInAppPurchaseDelegate
{
	public OnIAPItemsInfoReceivedCallback onIAPItemsInfoReceived;

	public OnIAPPurchaseCompletedCallback onIAPPurchaseCompleted;

	public OnIAPErrorCallback onIAPError;

	public override void SyncTransactionsComplete(SyncTransactionsState state, ICInAppPurchaseTransaction[] transactions)
	{
		switch (state)
		{
		case SyncTransactionsState.ST_COMPLETE:
			Debug.Log("SyncTransactions() succedded, productIdentifiers:");
			if (transactions != null)
			{
				for (int i = 0; i < transactions.Length; i++)
				{
					CInAppPurchaseTransaction cInAppPurchaseTransaction = (CInAppPurchaseTransaction)transactions[i];
					Debug.Log(cInAppPurchaseTransaction.GetProductIdentifier());
				}
			}
			else
			{
				Debug.Log("There is NOT any transaction yet");
			}
			break;
		case SyncTransactionsState.ST_FAILED:
			SendError(0, Singleton<Localizer>.instance.Get("iap_error_syncfailed"));
			break;
		default:
			SendError(0, Singleton<Localizer>.instance.Get("iap_error_syncunknown"));
			break;
		}
	}

	public override void RequestProductDataComplete(RequestProductDataState state, ICInAppPurchaseProduct[] validProducts)
	{
		switch (state)
		{
		case RequestProductDataState.RPD_COMPLETE:
			Debug.Log("RequestProductData() succedded, productIdentifiers:");
			if (validProducts != null && validProducts.Length > 0)
			{
				if (onIAPItemsInfoReceived != null)
				{
					IAPItemDescriptor[] array = new IAPItemDescriptor[validProducts.Length];
					int num = 0;
					for (int i = 0; i < validProducts.Length; i++)
					{
						CInAppPurchaseProduct cInAppPurchaseProduct = (CInAppPurchaseProduct)validProducts[i];
						string text = FilterCurrencySymbol(cInAppPurchaseProduct.GetCurrencySymbol());
						array[num++] = new IAPItemDescriptor(cInAppPurchaseProduct.GetProductIdentifier(), cInAppPurchaseProduct.GetDescription(), text + cInAppPurchaseProduct.GetPriceAsString());
					}
					onIAPItemsInfoReceived(array);
				}
			}
			else
			{
				SendError(0, Singleton<Localizer>.instance.Get("iap_error_novalidproducts"));
			}
			break;
		case RequestProductDataState.RPD_TIMEDOUT:
			SendError(0, Singleton<Localizer>.instance.Get("iap_error_requesttimedout"));
			break;
		case RequestProductDataState.RPD_FAILED:
			SendError(0, Singleton<Localizer>.instance.Get("iap_error_requestfailed"));
			break;
		default:
			SendError(0, Singleton<Localizer>.instance.Get("iap_error_requestunknown"));
			break;
		}
	}

	public override void BuyProductComplete(BuyProductState state, ICInAppPurchaseTransaction transaction)
	{
		Singleton<PlayHavenTowerControl>.instance.inIAPMode = false;
		switch (state)
		{
		case BuyProductState.BP_COMPLETE:
			if (transaction != null)
			{
				Debug.Log("BuyProduct() succedded, productIdentifier: " + transaction.GetProductIdentifier());
				Singleton<PlayHavenTowerControl>.instance.NotifyIAPEvent(transaction.GetProductIdentifier(), PurchaseResolution.Buy);
				if (onIAPPurchaseCompleted != null)
				{
					onIAPPurchaseCompleted(transaction.GetProductIdentifier());
				}
			}
			else
			{
				Singleton<PlayHavenTowerControl>.instance.NotifyIAPEvent(transaction.GetProductIdentifier(), PurchaseResolution.Error);
				Debug.Log("Internal IAP error? BuyProduct() succedded, but there is NOT transaction actually");
			}
			break;
		case BuyProductState.BP_CANCELLED:
			Singleton<PlayHavenTowerControl>.instance.NotifyIAPEvent(transaction.GetProductIdentifier(), PurchaseResolution.Cancel);
			Singleton<PlayHavenTowerControl>.instance.InvokeContent("iap_cancelled_in_store");
			SendError(0, Singleton<Localizer>.instance.Get("iap_error_buycancelled"));
			break;
		case BuyProductState.BP_FAILED:
			Singleton<PlayHavenTowerControl>.instance.NotifyIAPEvent(transaction.GetProductIdentifier(), PurchaseResolution.Error);
			SendError(0, Singleton<Localizer>.instance.Get("iap_error_buyfailed"));
			break;
		default:
			Singleton<PlayHavenTowerControl>.instance.NotifyIAPEvent(transaction.GetProductIdentifier(), PurchaseResolution.Error);
			SendError(0, Singleton<Localizer>.instance.Get("iap_error_buyunknown"));
			break;
		}
	}

	private void SendError(int code, string msg)
	{
		if (!ApplicationUtilities.IsBuildType("amazon") && onIAPError != null)
		{
			onIAPError(code, msg);
		}
	}

	private string FilterCurrencySymbol(string currencySymbol)
	{
		if (currencySymbol.Length == 1 && currencySymbol[0] == '￥')
		{
			return new string(new char[1] { '¥' });
		}
		return currencySymbol;
	}

	public override void SubscriptionReceived(string receipt)
	{
	}
}
