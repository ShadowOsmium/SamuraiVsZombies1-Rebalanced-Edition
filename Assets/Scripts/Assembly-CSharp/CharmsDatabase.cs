using System.Collections.Generic;

public class CharmsDatabase : Singleton<CharmsDatabase>
{
	private SDFTreeNode mData;

	private Dictionary<string, string[]> mAllIDs = new Dictionary<string, string[]>();

	public List<string> allIDs;

	public string[] allIDsForActivePlayMode
	{
		get
		{
			return GetIDsForPlayMode(Singleton<PlayModesManager>.instance.selectedMode);
		}
	}

	public CharmsDatabase()
	{
		ResetCachedData();
		CacheSimpleIDList();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ResetCachedData;
	}

	public void ResetCachedData()
	{
		mData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Charms");
		CacheSimpleIDList();
	}

	public string[] GetIDsForPlayMode(string playmode)
	{
		return mAllIDs[playmode];
	}

	public bool Contains(string id)
	{
		foreach (KeyValuePair<string, string[]> mAllID in mAllIDs)
		{
			string[] value = mAllID.Value;
			foreach (string strA in value)
			{
				if (string.Compare(strA, id, true) == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public string GetAttribute(string charmID, string attributeName)
	{
		SDFTreeNode sDFTreeNode = mData.to(charmID);
		if (sDFTreeNode == null)
		{
			return string.Empty;
		}
		if (!sDFTreeNode.hasAttribute(attributeName))
		{
			return string.Empty;
		}
		return Singleton<Localizer>.instance.Parse(sDFTreeNode[attributeName]);
	}

	public string GetAttribute(string charmID, string subSection, string attributeName)
	{
		SDFTreeNode sDFTreeNode = mData.to(charmID);
		if (sDFTreeNode == null)
		{
			return string.Empty;
		}
		sDFTreeNode = sDFTreeNode.to(subSection);
		if (sDFTreeNode == null)
		{
			return string.Empty;
		}
		if (!sDFTreeNode.hasAttribute(attributeName))
		{
			return string.Empty;
		}
		return Singleton<Localizer>.instance.Parse(sDFTreeNode[attributeName]);
	}

	private void CacheSimpleIDList()
	{
		SDFTreeNode sDFTreeNode = mData.to("All");
		List<string> list = new List<string>();
		for (int i = 0; sDFTreeNode.hasAttribute(i); i++)
		{
			list.Add(sDFTreeNode[i]);
		}
		allIDs = list;
		mAllIDs.Clear();
		foreach (string item in list)
		{
			string text = mData.to(item)["playmode"];
			if (mAllIDs.ContainsKey(text))
			{
				continue;
			}
			int num = 0;
			foreach (string item2 in list)
			{
				if (text == mData.to(item2)["playmode"])
				{
					num++;
				}
			}
			int num2 = 0;
			string[] array = new string[num];
			foreach (string item3 in list)
			{
				if (text == mData.to(item3)["playmode"])
				{
					array[num2++] = item3;
				}
			}
			mAllIDs.Add(text, array);
		}
	}
}
