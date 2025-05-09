using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class PotionsDatabase : Singleton<PotionsDatabase>
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

	public PotionsDatabase()
	{
		ResetCachedData();
		CacheSimpleIDList();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ResetCachedData;
	}

	public void ResetCachedData()
	{
		mData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Potions");
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

	public string GetAttribute(string potionID, string attributeName)
	{
		SDFTreeNode sDFTreeNode = mData.to(potionID);
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

	public bool Execute(string id)
	{
		bool flag = false;
		SDFTreeNode sDFTreeNode = mData.to(id);
		if (sDFTreeNode.hasAttribute("heal"))
		{
			flag = Execute_Heal(sDFTreeNode["heal"]);
			if (flag)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_WHERES_THE_WASABI");
				Singleton<Analytics>.instance.LogEvent("HealthPotionUsed", id, Singleton<Profile>.instance.waveToBeat);
			}
		}
		else if (sDFTreeNode.hasAttribute("leadership"))
		{
			flag = Execute_Leadership(sDFTreeNode["leadership"]);
			if (flag)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_TEA_EARL_GREY_HOT");
				Singleton<Analytics>.instance.LogEvent("LeadershipPotionUsed", id, Singleton<Profile>.instance.waveToBeat);
			}
		}
		return flag;
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

	private bool Execute_Heal(string data)
	{
		switch (data)
		{
		default:
		{
			int num = 0;
			if (num == 1)
			{
				if (WeakGlobalSceneBehavior<InGameImpl>.instance.hero.health == WeakGlobalSceneBehavior<InGameImpl>.instance.hero.maxHealth)
				{
					return false;
				}
				WeakGlobalSceneBehavior<InGameImpl>.instance.hero.RecievedHealing(WeakGlobalSceneBehavior<InGameImpl>.instance.hero.maxHealth);
			}
			else
			{
				Debug.Log("ERROR: Unknown healing potion command: " + data);
			}
			break;
		}
		case "all":
		{
			bool flag = false;
			foreach (Character item in WeakGlobalInstance<CharactersManager>.instance.allAlive)
			{
				if (!item.isEnemy && !item.isBase && item.health < item.maxHealth)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				foreach (Character item2 in WeakGlobalInstance<CharactersManager>.instance.allAlive)
				{
					if (!item2.isEnemy && !item2.isBase && item2.health < item2.maxHealth)
					{
						item2.RecievedHealing(item2.maxHealth - item2.health);
					}
				}
				break;
			}
			return false;
		}
		}
		return true;
	}

	private bool Execute_Leadership(string data)
	{
		int num = 0;
		try
		{
			num = int.Parse(data);
		}
		catch
		{
			return false;
		}
		if (WeakGlobalInstance<Smithy>.instance.resources == WeakGlobalInstance<Smithy>.instance.maxResources)
		{
			return false;
		}
		WeakGlobalInstance<Smithy>.instance.resources += num;
		return true;
	}
}
