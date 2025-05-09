using UnityEngine;

public class IAPListenerScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (CInAppPurchase.IsAvailable() && CInAppPurchase.IsTransactionActive())
		{
			CInAppPurchase.HandleUpdate();
		}
	}

	public void Callback_SyncTransactionsComplete(string param)
	{
		CInAppPurchase.Callback_SyncTransactionsComplete(param);
	}

	public void Callback_RequestProductDataComplete(string param)
	{
		CInAppPurchase.Callback_RequestProductDataComplete(param);
	}

	public void Callback_BuyProductComplete(string param)
	{
		CInAppPurchase.Callback_BuyProductComplete(param);
	}

	public void Callback_OnSubscriptionReceived(string param)
	{
		CInAppPurchase.Callback_OnSubscriptionReceived(param);
	}
}
