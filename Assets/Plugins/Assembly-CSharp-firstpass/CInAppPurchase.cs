using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class CInAppPurchase
{
	private const string m_ExternalFileName = "libIAP";

	private static ICInAppPurchaseDelegate m_InAppPurchaseDelegate;

	private static float m_timePrevValue;

	static CInAppPurchase()
	{
		m_InAppPurchaseDelegate = null;
		m_timePrevValue = Time.time;
	}

	public static void Init(ICInAppPurchaseDelegate @delegate, string marketPublicKey, string gameObjectListenerName = "IAPListener")
	{
		m_InAppPurchaseDelegate = @delegate;
		if (m_InAppPurchaseDelegate != null)
		{
			IAP_Init(gameObjectListenerName, marketPublicKey);
		}
		else
		{
			Debug.Log("CInAppPurchase.Init failed: ICInAppPurchaseDelegate instance is null");
		}
	}

	public static void SyncTransactions()
	{
		if (m_InAppPurchaseDelegate != null)
		{
			IAP_SyncTransactions();
		}
		else
		{
			Debug.Log("CInAppPurchase.SyncTransactions failed: CInAppPurchase wasn't initialised. Call for CInAppPurchase.Init function first");
		}
	}

	public static void RequestProductData(string[] products, int timeout)
	{
		if (m_InAppPurchaseDelegate != null)
		{
			IAP_RequestProductData(products, timeout);
		}
		else
		{
			Debug.Log("CInAppPurchase.RequestProductData failed: CInAppPurchase wasn't initialised. Call for CInAppPurchase.Init function first");
		}
	}

	public static void BuyProduct(string product)
	{
		if (m_InAppPurchaseDelegate != null)
		{
			IAP_BuyProduct(product);
		}
		else
		{
			Debug.Log("CInAppPurchase.BuyProduct failed: CInAppPurchase wasn't initialised. Call for CInAppPurchase.Init function first");
		}
	}

	public static bool IsTransactionActive()
	{
		return IAP_IsTransactionActive();
	}

	public static bool IsAvailable()
	{
		return IAP_IsAvailable();
	}

	public static bool IsSubscriptionAvailable()
	{
		return IAP_IsSubscriptionAvailable();
	}

	public static bool AllowsConcurrentOperations()
	{
		return IAP_AllowsConcurrentOperations();
	}

	public static void HandleUpdate()
	{
		int num = 0;
		float time = Time.time;
		if (m_timePrevValue == 0f)
		{
			m_timePrevValue = time;
		}
		if (m_timePrevValue < time)
		{
			if ((double)Math.Abs(time - m_timePrevValue) > 0.001)
			{
				num = Convert.ToInt32((time - m_timePrevValue) * 1000f);
				m_timePrevValue = time;
			}
		}
		else
		{
			m_timePrevValue = time;
		}
		if (num > 0)
		{
			IAP_HandleUpdate(num);
			SyncTransactions();
		}
	}

	[DllImport("libIAP", CharSet = CharSet.Ansi)]
	private static extern void IAP_Init(string gameObject, string key);

	[DllImport("libIAP")]
	private static extern void IAP_SyncTransactions();

	[DllImport("libIAP", CharSet = CharSet.Ansi)]
	private static extern void IAP_RequestProductData(string[] products, int timeout);

	[DllImport("libIAP", CharSet = CharSet.Ansi)]
	private static extern void IAP_BuyProduct(string product);

	[DllImport("libIAP")]
	private static extern bool IAP_IsTransactionActive();

	[DllImport("libIAP")]
	private static extern bool IAP_IsAvailable();

	[DllImport("libIAP")]
	private static extern bool IAP_AllowsConcurrentOperations();

	[DllImport("libIAP")]
	private static extern void IAP_HandleUpdate(int elapsedTime);

	[DllImport("libIAP")]
	private static extern void IAP_initSingleProduct(string productId);

	[DllImport("libIAP")]
	private static extern bool IAP_IsSubscriptionAvailable();

	public static void initSingleProduct(string productId)
	{
		IAP_initSingleProduct(productId);
	}

	public static void Callback_SyncTransactionsComplete(string param)
	{
		Debug.Log("CInAppPurchase.Callback_SyncTransactionsComplete is called, parameters [in] " + param);
		if (m_InAppPurchaseDelegate != null)
		{
			ICInAppPurchaseDelegate.SyncTransactionsState state = ICInAppPurchaseDelegate.SyncTransactionsState.ST_FAILED;
			ICInAppPurchaseTransaction[] array = null;
			switch (CStringUtils.ExtractFirstValueFromStringForKey(param, "SyncTransactionsState"))
			{
			default:
			{
				int num = 0;
				if (num == 1)
				{
					state = ICInAppPurchaseDelegate.SyncTransactionsState.ST_FAILED;
				}
				else
				{
					Debug.Log("IAP internal error, SyncTransactionsState code is unknown");
				}
				break;
			}
			case "ST_COMPLETE":
				state = ICInAppPurchaseDelegate.SyncTransactionsState.ST_COMPLETE;
				break;
			}
			int amountOfValuesFromStringForKey = CStringUtils.GetAmountOfValuesFromStringForKey(param, "Transaction");
			if (amountOfValuesFromStringForKey > 0)
			{
				array = new CInAppPurchaseTransaction[amountOfValuesFromStringForKey];
				for (int i = 0; i < amountOfValuesFromStringForKey; i++)
				{
					array[i] = new CInAppPurchaseTransaction(CStringUtils.ExtractFromStringForKeyValue(param, "Transaction", i + 1));
				}
			}
			m_InAppPurchaseDelegate.SyncTransactionsComplete(state, array);
		}
		else
		{
			Debug.Log("CInAppPurchase.Callback_SyncTransactionsComplete failed: CInAppPurchase isn't initialised. Call for CInAppPurchase.Init function first");
		}
	}

	public static void Callback_RequestProductDataComplete(string param)
	{
		Debug.Log("CInAppPurchase.Callback_RequestProductDataComplete is called, parameters [in] " + param);
		if (m_InAppPurchaseDelegate != null)
		{
			ICInAppPurchaseDelegate.RequestProductDataState state = ICInAppPurchaseDelegate.RequestProductDataState.RPD_FAILED;
			ICInAppPurchaseProduct[] array = null;
			switch (CStringUtils.ExtractFirstValueFromStringForKey(param, "RequestProductDataState"))
			{
			case "RPD_COMPLETE":
				state = ICInAppPurchaseDelegate.RequestProductDataState.RPD_COMPLETE;
				break;
			case "RPD_TIMEDOUT":
				state = ICInAppPurchaseDelegate.RequestProductDataState.RPD_TIMEDOUT;
				break;
			case "RPD_FAILED":
				state = ICInAppPurchaseDelegate.RequestProductDataState.RPD_FAILED;
				break;
			default:
				Debug.Log("IAP internal error, RequestProductDataState code is unknown");
				break;
			}
			int amountOfValuesFromStringForKey = CStringUtils.GetAmountOfValuesFromStringForKey(param, "Product");
			if (amountOfValuesFromStringForKey > 0)
			{
				array = new CInAppPurchaseProduct[amountOfValuesFromStringForKey];
				for (int i = 0; i < amountOfValuesFromStringForKey; i++)
				{
					array[i] = new CInAppPurchaseProduct(CStringUtils.ExtractFromStringForKeyValue(param, "Product", i + 1));
				}
			}
			m_InAppPurchaseDelegate.RequestProductDataComplete(state, array);
		}
		else
		{
			Debug.Log("CInAppPurchase.Callback_RequestProductDataComplete failed: CInAppPurchase isn't initialised. Call for CInAppPurchase.Init function first");
		}
	}

	public static void Callback_BuyProductComplete(string param)
	{
		Debug.Log("CInAppPurchase.Callback_BuyProductComplete is called, parameters [in] " + param);
		if (m_InAppPurchaseDelegate != null)
		{
			ICInAppPurchaseDelegate.BuyProductState state = ICInAppPurchaseDelegate.BuyProductState.BP_FAILED;
			ICInAppPurchaseTransaction transaction = null;
			switch (CStringUtils.ExtractFirstValueFromStringForKey(param, "BuyProductState"))
			{
			case "BP_COMPLETE":
				state = ICInAppPurchaseDelegate.BuyProductState.BP_COMPLETE;
				break;
			case "BP_CANCELLED":
				state = ICInAppPurchaseDelegate.BuyProductState.BP_CANCELLED;
				break;
			case "BP_FAILED":
				state = ICInAppPurchaseDelegate.BuyProductState.BP_FAILED;
				break;
			default:
				Debug.Log("IAP internal error, BuyProductState code is unknown");
				break;
			}
			if (CStringUtils.GetAmountOfValuesFromStringForKey(param, "Transaction") > 0)
			{
				transaction = new CInAppPurchaseTransaction(CStringUtils.ExtractFirstValueFromStringForKey(param, "Transaction"));
			}
			m_InAppPurchaseDelegate.BuyProductComplete(state, transaction);
		}
		else
		{
			Debug.Log("CInAppPurchase.Callback_BuyProductComplete failed: CInAppPurchase isn't initialised. Call for CInAppPurchase.Init function first");
		}
	}

	public static void Callback_OnSubscriptionReceived(string param)
	{
		m_InAppPurchaseDelegate.SubscriptionReceived(param);
	}
}
