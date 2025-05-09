using System;
using System.Collections.Generic;
using UnityEngine;

public class HelpersDatabase : Singleton<HelpersDatabase>
{
	private class HelperData
	{
		public string id;

		public SDFTreeNode registryData;

		public HelperData(string abId)
		{
			id = abId;
		}

		public void EnsureDataCached()
		{
			if (registryData == null)
			{
				registryData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Helpers/" + id);
			}
		}
	}

	private List<HelperData> mData = new List<HelperData>();

	private string[] mAllIDs;

	public string[] allIDs
	{
		get
		{
			return mAllIDs;
		}
	}

	public HelpersDatabase()
	{
		ResetCachedData();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ResetCachedData;
	}

	public void ResetCachedData()
	{
		mData = new List<HelperData>();
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Helpers/AllHelpers");
		if (sDFTreeNode == null || sDFTreeNode.attributeCount == 0)
		{
			Debug.Log("ERROR: Cannot find the list of Helpers (AllHelpers.txt)");
		}
		for (int i = 0; sDFTreeNode.hasAttribute(i); i++)
		{
			mData.Add(new HelperData(sDFTreeNode[i]));
		}
		CacheSimpleIDList();
	}

	public bool Contains(string id)
	{
		string[] array = mAllIDs;
		foreach (string strA in array)
		{
			if (string.Compare(strA, id, true) == 0)
			{
				return true;
			}
		}
		return false;
	}

	public string GetAttribute(string helperID, string attributeName)
	{
		return GetAttribute(helperID, attributeName, GetHelperLevel(helperID), false);
	}

	public string GetNextLevelAttribute(string helperID, string attributeName)
	{
		return GetAttribute(helperID, attributeName, GetHelperLevel(helperID) + 1, false);
	}

	public string GetAttributeOrNull(string helperID, string attributeName)
	{
		return GetAttribute(helperID, attributeName, GetHelperLevel(helperID), true);
	}

	public int GetMaxLevel(string helperID)
	{
		HelperData helperData = Seek(helperID);
		if (helperData != null)
		{
			helperData.EnsureDataCached();
			for (int i = 1; i < 1000; i++)
			{
				if (helperData.registryData.to(i) == null)
				{
					return i - 1;
				}
			}
		}
		Debug.Log("ERROR: Unknown helper ID, or helper data malformed: " + helperID);
		return -1;
	}

	private void CacheSimpleIDList()
	{
		mAllIDs = new string[mData.Count];
		int num = 0;
		foreach (HelperData mDatum in mData)
		{
			mAllIDs[num++] = mDatum.id;
		}
	}

	private HelperData Seek(string helperID)
	{
		foreach (HelperData mDatum in mData)
		{
			if (mDatum.id.Equals(helperID, StringComparison.OrdinalIgnoreCase))
			{
				return mDatum;
			}
		}
		return null;
	}

	private string GetAttribute(string helperID, string attributeName, int level, bool noWarningIfNotFound)
	{
		HelperData helperData = Seek(helperID);
		if (helperData == null)
		{
			Debug.Log("WARNING: Could not find helper: " + helperID);
			return string.Empty;
		}
		helperData.EnsureDataCached();
		if (helperData.registryData.hasAttribute(attributeName))
		{
			return Singleton<Localizer>.instance.Parse(helperData.registryData[attributeName]);
		}
		level = Mathf.Max(level, 1);
		SDFTreeNode sDFTreeNode = helperData.registryData.to(level);
		if (sDFTreeNode != null && sDFTreeNode.hasAttribute(attributeName))
		{
			return Singleton<Localizer>.instance.Parse(sDFTreeNode[attributeName]);
		}
		if (!noWarningIfNotFound)
		{
			Debug.Log("WARNING: Could not find helper attribute: " + attributeName);
		}
		return string.Empty;
	}

	private int GetHelperLevel(string helperID)
	{
		HelperData helperData = Seek(helperID);
		string text = helperData.registryData["levelMatchOtherHelper"];
		if (text != string.Empty)
		{
			return Singleton<Profile>.instance.GetHelperLevel(text);
		}
		return Singleton<Profile>.instance.GetHelperLevel(helperID);
	}
}
