using System.Collections.Generic;
using UnityEngine;

public class AbilitiesDatabase : Singleton<AbilitiesDatabase>
{
	private class AbilityData
	{
		public delegate void OnBuildStoreDataFunc(string id, List<StoreData.Item> items);

		public string id;

		public OnBuildStoreDataFunc OnBuildStoreData;

		public OnSUIGenericCallback OnExecute;

		public SDFTreeNode registryData;

		public AbilityData(string abId, OnSUIStringCallback execFunc, OnBuildStoreDataFunc storeBuildDataFunc)
		{
			AbilityData abilityData = this;
			id = abId;
			OnExecute = delegate
			{
				execFunc(abilityData.id);
			};
			OnBuildStoreData = storeBuildDataFunc;
		}

		public void EnsureDataCached()
		{
			if (registryData == null)
			{
				registryData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Abilities/" + id);
			}
		}
	}

	private List<AbilityData> mData;

	private string[] mAllIDs;

	public string[] allIDs
	{
		get
		{
			return mAllIDs;
		}
	}

	public AbilitiesDatabase()
	{
		ResetCachedData();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ResetCachedData;
	}

	public void ResetCachedData()
	{
		mData = new List<AbilityData>();
		if (Singleton<PlayModesManager>.instance.selectedMode == "classic")
		{
			mData.Add(new AbilityData("KatanaSlash", OnExecute_KatanaSlash, StoreAvailability_Abilities.GetAbilityUpgrade_DamageOnly));
			mData.Add(new AbilityData("SummonLightning", OnExecute_SummonLightning, StoreAvailability_Abilities.GetAbilityUpgrade_SummonLightning));
			mData.Add(new AbilityData("Lethargy", OnExecute_Lethargy, StoreAvailability_Abilities.GetAbilityUpgrade_Lethargy));
			mData.Add(new AbilityData("DivineIntervention", OnExecute_DivineIntervention, StoreAvailability_Abilities.GetAbilityUpgrade_DivineIntervention));
			mData.Add(new AbilityData("SummonTornadoes", OnExecute_SummonTornadoes, StoreAvailability_Abilities.GetAbilityUpgrade_DamageOnly));
			mData.Add(new AbilityData("GiantWave", OnExecute_GiantWave, StoreAvailability_Abilities.GetAbilityUpgrade_DamageOnly));
		}
		else
		{
			mData.Add(new AbilityData("LifeSteal", OnExecute_KatanaSlash, StoreAvailability_Abilities.GetAbilityUpgrade_DamageOnly));
			mData.Add(new AbilityData("NightOfTheDead", OnExecute_NightOfTheDead, StoreAvailability_Abilities.GetAbilityUpgrade_NightOfTheDead));
			mData.Add(new AbilityData("GraveHands", OnExecute_GraveHands, StoreAvailability_Abilities.GetAbilityUpgrade_SummonLightning));
			mData.Add(new AbilityData("LethargyZM", OnExecute_Lethargy, StoreAvailability_Abilities.GetAbilityUpgrade_Lethargy));
			mData.Add(new AbilityData("RaiseDead", OnExecute_DivineIntervention, StoreAvailability_Abilities.GetAbilityUpgrade_DivineIntervention));
			mData.Add(new AbilityData("DragonAttack", OnExecute_GiantWave, StoreAvailability_Abilities.GetAbilityUpgrade_DamageOnly));
		}
		CacheSimpleIDList();
	}

	private void OnExecute_KatanaSlash(string abilityID)
	{
		WeakGlobalSceneBehavior<InGameImpl>.instance.hero.DoKatanaSlash(abilityID);
	}

	private void OnExecute_SummonLightning(string abilityID)
	{
		WeakGlobalSceneBehavior<InGameImpl>.instance.hero.DoSummonLightning(abilityID);
	}

	private void OnExecute_GraveHands(string abilityID)
	{
		WeakGlobalSceneBehavior<InGameImpl>.instance.hero.DoGraveHands(abilityID);
	}

	private void OnExecute_Lethargy(string abilityID)
	{
		WeakGlobalSceneBehavior<InGameImpl>.instance.hero.DoLethargy(abilityID);
	}

	private void OnExecute_DivineIntervention(string abilityID)
	{
		WeakGlobalSceneBehavior<InGameImpl>.instance.hero.DoDivineIntervention(abilityID);
	}

	private void OnExecute_SummonTornadoes(string abilityID)
	{
		WeakGlobalSceneBehavior<InGameImpl>.instance.hero.DoSummonTornado(abilityID);
	}

	private void OnExecute_GroundShock(string abilityID)
	{
		WeakGlobalSceneBehavior<InGameImpl>.instance.hero.DoGroundShock(abilityID);
	}

	private void OnExecute_GiantWave(string abilityID)
	{
		WeakGlobalSceneBehavior<InGameImpl>.instance.hero.DoGiantWave(abilityID);
	}

	private void OnExecute_NightOfTheDead(string abilityID)
	{
		WeakGlobalSceneBehavior<InGameImpl>.instance.hero.DoNightOfTheDead(abilityID);
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

	public string GetAttribute(string abilityID, string attributeName)
	{
		AbilityData abilityData = Seek(abilityID);
		if (abilityData == null)
		{
			Debug.Log("WARNING: Could not find ability: " + abilityID);
			return string.Empty;
		}
		abilityData.EnsureDataCached();
		int level = Mathf.Clamp(Singleton<Profile>.instance.GetAbilityLevel(abilityID), 1, abilityData.registryData.childCount);
		return GetAttribute(abilityID, attributeName, level);
	}

	public string GetNextLevelAttribute(string abilityID, string attributeName)
	{
		AbilityData abilityData = Seek(abilityID);
		if (abilityData == null)
		{
			Debug.Log("WARNING: Could not find ability: " + abilityID);
			return string.Empty;
		}
		abilityData.EnsureDataCached();
		int level = Mathf.Clamp(Singleton<Profile>.instance.GetAbilityLevel(abilityID) + 1, 1, abilityData.registryData.childCount);
		return GetAttribute(abilityID, attributeName, level);
	}

	public T Extrapolate<T>(string abilityID, string infiniteUpgradableAttributeName, string attributeName)
	{
		AbilityData abilityData = Seek(abilityID);
		if (abilityData == null)
		{
			return default(T);
		}
		abilityData.EnsureDataCached();
		int abilityLevel = Singleton<Profile>.instance.GetAbilityLevel(abilityID);
		if (infiniteUpgradableAttributeName == string.Empty || !abilityData.registryData.hasAttribute(infiniteUpgradableAttributeName))
		{
			return InfiniteUpgrades.SnapToHighest<T>(abilityData.registryData, attributeName, abilityLevel);
		}
		return InfiniteUpgrades.Extrapolate<T>(abilityData.registryData, infiniteUpgradableAttributeName, attributeName, abilityLevel);
	}

	public void Execute(string abilityID)
	{
		AbilityData abilityData = Seek(abilityID);
		if (abilityData != null)
		{
			abilityData.OnExecute();
		}
	}

	public void BuildStoreData(string abilityID, List<StoreData.Item> items)
	{
		AbilityData abilityData = Seek(abilityID);
		if (abilityData != null)
		{
			abilityData.OnBuildStoreData(abilityID, items);
		}
	}

	public int GetMaxLevel(string abilityID)
	{
		AbilityData abilityData = Seek(abilityID);
		if (abilityData != null)
		{
			abilityData.EnsureDataCached();
			for (int i = 1; i < 1000; i++)
			{
				if (abilityData.registryData.to(i) == null)
				{
					return i - 1;
				}
			}
		}
		Debug.Log("ERROR: Unknown ability ID, or ability data malformed: " + abilityID);
		return -1;
	}

	private void CacheSimpleIDList()
	{
		mAllIDs = new string[mData.Count];
		int num = 0;
		foreach (AbilityData mDatum in mData)
		{
			mAllIDs[num++] = mDatum.id;
		}
	}

	private AbilityData Seek(string abilityID)
	{
		foreach (AbilityData mDatum in mData)
		{
			if (mDatum.id == abilityID)
			{
				return mDatum;
			}
		}
		return null;
	}

	private string GetAttribute(string abilityID, string attributeName, int level)
	{
		AbilityData abilityData = Seek(abilityID);
		if (abilityData == null)
		{
			Debug.Log("WARNING: Could not find ability: " + abilityID);
			return string.Empty;
		}
		abilityData.EnsureDataCached();
		if (abilityData.registryData.hasAttribute(attributeName))
		{
			return Singleton<Localizer>.instance.Parse(abilityData.registryData[attributeName]);
		}
		SDFTreeNode sDFTreeNode = abilityData.registryData.to(level);
		if (sDFTreeNode != null && sDFTreeNode.hasAttribute(attributeName))
		{
			return Singleton<Localizer>.instance.Parse(sDFTreeNode[attributeName]);
		}
		Debug.Log("WARNING: Could not find ability attribute: " + attributeName);
		return string.Empty;
	}
}
