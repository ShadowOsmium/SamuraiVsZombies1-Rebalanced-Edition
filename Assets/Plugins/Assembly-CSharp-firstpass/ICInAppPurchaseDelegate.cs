public abstract class ICInAppPurchaseDelegate
{
	public enum SyncTransactionsState
	{
		ST_COMPLETE = 0,
		ST_FAILED = 1
	}

	public enum RequestProductDataState
	{
		RPD_COMPLETE = 0,
		RPD_TIMEDOUT = 1,
		RPD_FAILED = 2
	}

	public enum BuyProductState
	{
		BP_COMPLETE = 0,
		BP_CANCELLED = 1,
		BP_FAILED = 2
	}

	public abstract void SyncTransactionsComplete(SyncTransactionsState state, ICInAppPurchaseTransaction[] transactions);

	public abstract void RequestProductDataComplete(RequestProductDataState state, ICInAppPurchaseProduct[] validProducts);

	public abstract void BuyProductComplete(BuyProductState state, ICInAppPurchaseTransaction transaction);

	public abstract void SubscriptionReceived(string receipt);
}
