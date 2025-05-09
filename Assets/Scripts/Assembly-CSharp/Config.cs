public class Config : Singleton<Config>
{
	private SDFTreeNode mData;

	public SDFTreeNode root
	{
		get
		{
			return mData;
		}
	}

	public Config()
	{
		ResetCachedData();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ResetCachedData;
	}

	public void ResetCachedData()
	{
		mData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Config");
	}
}
