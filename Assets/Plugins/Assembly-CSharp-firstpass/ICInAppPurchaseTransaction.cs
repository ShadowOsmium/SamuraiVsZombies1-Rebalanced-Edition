public abstract class ICInAppPurchaseTransaction
{
	public enum TransactionState
	{
		TransactionStatePurchasing = 0,
		TransactionStatePurchased = 1,
		TransactionStateFailed = 2,
		TransactionStateRestored = 3
	}

	public abstract TransactionState GetTransactionState();

	public abstract string GetProductIdentifier();

	public abstract string GetTransactionIdentifier();

	public abstract string GetErrorDescription();

	public abstract string GetReceiptDescription();

	public abstract string GetDate();
}
