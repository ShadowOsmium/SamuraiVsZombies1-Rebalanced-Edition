public abstract class ICInAppPurchaseProduct
{
	public abstract string GetDescription();

	public abstract string GetTitle();

	public abstract string GetProductIdentifier();

	public abstract string GetPriceAsString();

	public abstract float GetPrice();

	public abstract string GetCurrencySymbol();

	public abstract string GetLocaleIdentifier();
}
