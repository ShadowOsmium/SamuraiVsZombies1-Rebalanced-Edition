public struct IAPItemDescriptor
{
	public string id;

	public string desc;

	public string price;

	public IAPItemDescriptor(string _id, string _desc, string _price)
	{
		id = _id;
		desc = _desc;
		price = _price;
	}
}
