public class SampleList : ListInterface
{
	public SampleListData[] list = new SampleListData[0];

	public override object[] ListAccess
	{
		get
		{
			return list;
		}
		set
		{
			list = (SampleListData[])value;
		}
	}
}
