using System.Collections.Generic;

public class StoreAvailability_Abilities
{
	public static void GetAll(List<StoreData.Item> items)
	{
		string[] allIDs = Singleton<AbilitiesDatabase>.instance.allIDs;
		foreach (string abilityID in allIDs)
		{
			Singleton<AbilitiesDatabase>.instance.BuildStoreData(abilityID, items);
		}
	}

	public static void GetAbilityUpgrade_DamageOnly(string abilityID, List<StoreData.Item> items)
	{
		bool flag = false;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Abilities/" + abilityID);
		if (Singleton<EventsManager>.instance.IsEventActive(sDFTreeNode["eventID"]))
		{
			if (Singleton<Profile>.instance.highestUnlockedWave < int.Parse(sDFTreeNode["waveToUnlock"]))
			{
				flag = true;
			}
			else
			{
				EnsureProperInitialLevel(abilityID);
			}
			int num = Singleton<Profile>.instance.GetAbilityLevel(abilityID) + 1;
			bool isLastUpgrade = sDFTreeNode.childCount == num;
			StoreData.Item item = new StoreData.Item(delegate
			{
				LevelUpAbility(abilityID, isLastUpgrade);
			});
			if (num > 1)
			{
				item.details.SetColumns(num - 1, num);
				item.details.AddStat("strength_stats", InfiniteUpgrades.Extrapolate<int>(sDFTreeNode, "infiniteUpgradeDamage", "damage", num - 1).ToString(), InfiniteUpgrades.Extrapolate<int>(sDFTreeNode, "infiniteUpgradeDamage", "damage", num).ToString());
			}
			if (num > sDFTreeNode.childCount)
			{
				item.cost = new Cost(sDFTreeNode["infiniteUpgradeCost"], Singleton<SalesDatabase>.instance.GetSaleFor(abilityID));
			}
			else
			{
				SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(num);
				item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor(abilityID));
			}
			item.id = abilityID;
			item.icon = sDFTreeNode["icon"];
			item.isEvent = sDFTreeNode["eventID"] != string.Empty;
			item.locked = flag;
			item.unlockAtWave = int.Parse(sDFTreeNode["waveToUnlock"]);
			if (flag)
			{
				item.unlockCondition = string.Format(Singleton<Localizer>.instance.Get("store_unlockatwave"), item.unlockAtWave);
			}
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_upgrade_tolevel"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num.ToString());
			item.unlockTitle = Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]);
			item.details.AddSmallDescription((!sDFTreeNode.hasAttribute("desc")) ? string.Empty : sDFTreeNode["desc"]);
			items.Add(item);
		}
	}

	public static void GetAbilityUpgrade_DivineIntervention(string abilityID, List<StoreData.Item> items)
	{
		bool flag = false;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Abilities/" + abilityID);
		if (!Singleton<EventsManager>.instance.IsEventActive(sDFTreeNode["eventID"]))
		{
			return;
		}
		if (Singleton<Profile>.instance.highestUnlockedWave < int.Parse(sDFTreeNode["waveToUnlock"]))
		{
			flag = true;
		}
		else
		{
			EnsureProperInitialLevel(abilityID);
		}
		int num = Singleton<Profile>.instance.GetAbilityLevel(abilityID) + 1;
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
		if (sDFTreeNode2 != null)
		{
			bool isLastUpgrade = sDFTreeNode.childCount == num;
			StoreData.Item item = new StoreData.Item(delegate
			{
				LevelUpAbility(abilityID, isLastUpgrade);
			});
			SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(string.Format("{0:000}", num - 1));
			if (sDFTreeNode3 != null)
			{
				item.details.AddStat("summon_ally", GetPercentString(sDFTreeNode3["summonAllyChance"]), GetPercentString(sDFTreeNode2["summonAllyChance"]));
				item.details.SetColumns(num - 1, num);
			}
			item.id = abilityID;
			item.icon = sDFTreeNode["icon"];
			item.isEvent = sDFTreeNode["eventID"] != string.Empty;
			item.locked = flag;
			item.unlockAtWave = int.Parse(sDFTreeNode["waveToUnlock"]);
			if (flag)
			{
				item.unlockCondition = string.Format(Singleton<Localizer>.instance.Get("store_unlockatwave"), item.unlockAtWave);
			}
			item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor(abilityID));
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_upgrade_tolevel"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num.ToString());
			item.unlockTitle = Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]);
			item.details.AddSmallDescription((!sDFTreeNode.hasAttribute("desc")) ? string.Empty : sDFTreeNode["desc"]);
			items.Add(item);
		}
	}

	public static void GetAbilityUpgrade_Lethargy(string abilityID, List<StoreData.Item> items)
	{
		bool flag = false;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Abilities/" + abilityID);
		if (!Singleton<EventsManager>.instance.IsEventActive(sDFTreeNode["eventID"]))
		{
			return;
		}
		if (Singleton<Profile>.instance.highestUnlockedWave < int.Parse(sDFTreeNode["waveToUnlock"]))
		{
			flag = true;
		}
		else
		{
			EnsureProperInitialLevel(abilityID);
		}
		int num = Singleton<Profile>.instance.GetAbilityLevel(abilityID) + 1;
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
		if (sDFTreeNode2 != null)
		{
			bool isLastUpgrade = sDFTreeNode.childCount == num;
			StoreData.Item item = new StoreData.Item(delegate
			{
				LevelUpAbility(abilityID, isLastUpgrade);
			});
			SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(string.Format("{0:000}", num - 1));
			if (sDFTreeNode3 != null)
			{
				item.details.AddStat("duration", GetSecondsString(sDFTreeNode3["duration"]), GetSecondsString(sDFTreeNode2["duration"]));
				item.details.SetColumns(num - 1, num);
			}
			item.id = abilityID;
			item.icon = sDFTreeNode["icon"];
			item.isEvent = sDFTreeNode["eventID"] != string.Empty;
			item.locked = flag;
			item.unlockAtWave = int.Parse(sDFTreeNode["waveToUnlock"]);
			if (flag)
			{
				item.unlockCondition = string.Format(Singleton<Localizer>.instance.Get("store_unlockatwave"), item.unlockAtWave);
			}
			item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor(abilityID));
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_upgrade_tolevel"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num.ToString());
			item.unlockTitle = Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]);
			item.details.AddSmallDescription((!sDFTreeNode.hasAttribute("desc")) ? string.Empty : sDFTreeNode["desc"]);
			items.Add(item);
		}
	}

	public static void GetAbilityUpgrade_SummonLightning(string abilityID, List<StoreData.Item> items)
	{
		bool flag = false;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Abilities/" + abilityID);
		if (!Singleton<EventsManager>.instance.IsEventActive(sDFTreeNode["eventID"]))
		{
			return;
		}
		if (Singleton<Profile>.instance.highestUnlockedWave < int.Parse(sDFTreeNode["waveToUnlock"]))
		{
			flag = true;
		}
		else
		{
			EnsureProperInitialLevel(abilityID);
		}
		int num = Singleton<Profile>.instance.GetAbilityLevel(abilityID) + 1;
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
		if (sDFTreeNode2 != null)
		{
			bool isLastUpgrade = sDFTreeNode.childCount == num;
			StoreData.Item item = new StoreData.Item(delegate
			{
				LevelUpAbility(abilityID, isLastUpgrade);
			});
			SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(string.Format("{0:000}", num - 1));
			if (sDFTreeNode3 != null)
			{
				item.details.AddStat("strength_stats", CombineLightningDamage(sDFTreeNode3), CombineLightningDamage(sDFTreeNode2));
				item.details.AddStat("duration", GetSecondsString(sDFTreeNode3["DOTDuration"]), GetSecondsString(sDFTreeNode2["DOTDuration"]));
				item.details.SetColumns(num - 1, num);
			}
			item.id = abilityID;
			item.icon = sDFTreeNode["icon"];
			item.isEvent = sDFTreeNode["eventID"] != string.Empty;
			item.locked = flag;
			item.unlockAtWave = int.Parse(sDFTreeNode["waveToUnlock"]);
			if (flag)
			{
				item.unlockCondition = string.Format(Singleton<Localizer>.instance.Get("store_unlockatwave"), item.unlockAtWave);
			}
			item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor(abilityID));
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_upgrade_tolevel"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num.ToString());
			item.unlockTitle = Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]);
			item.details.AddSmallDescription((!sDFTreeNode.hasAttribute("desc")) ? string.Empty : sDFTreeNode["desc"]);
			items.Add(item);
		}
	}

	public static void GetAbilityUpgrade_NightOfTheDead(string abilityID, List<StoreData.Item> items)
	{
		bool flag = false;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Abilities/" + abilityID);
		if (!Singleton<EventsManager>.instance.IsEventActive(sDFTreeNode["eventID"]))
		{
			return;
		}
		if (Singleton<Profile>.instance.highestUnlockedWave < int.Parse(sDFTreeNode["waveToUnlock"]))
		{
			flag = true;
		}
		else
		{
			EnsureProperInitialLevel(abilityID);
		}
		int num = Singleton<Profile>.instance.GetAbilityLevel(abilityID) + 1;
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
		if (sDFTreeNode2 != null)
		{
			bool isLastUpgrade = sDFTreeNode.childCount == num;
			StoreData.Item item = new StoreData.Item(delegate
			{
				LevelUpAbility(abilityID, isLastUpgrade);
			});
			SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(string.Format("{0:000}", num - 1));
			if (sDFTreeNode3 != null)
			{
				item.details.AddStat("duration", GetSecondsString(sDFTreeNode3["duration"]), GetSecondsString(sDFTreeNode2["duration"]));
				item.details.SetColumns(num - 1, num);
			}
			item.id = abilityID;
			item.icon = sDFTreeNode["icon"];
			item.isEvent = sDFTreeNode["eventID"] != string.Empty;
			item.locked = flag;
			item.unlockAtWave = int.Parse(sDFTreeNode["waveToUnlock"]);
			if (flag)
			{
				item.unlockCondition = string.Format(Singleton<Localizer>.instance.Get("store_unlockatwave"), item.unlockAtWave);
			}
			item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor(abilityID));
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_upgrade_tolevel"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num.ToString());
			item.unlockTitle = Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]);
			item.details.AddSmallDescription((!sDFTreeNode.hasAttribute("desc")) ? string.Empty : sDFTreeNode["desc"]);
			items.Add(item);
		}
	}

	private static string GetPercentString(string floatString)
	{
		float num = float.Parse(floatString) * 100f;
		return string.Format("{0}%", ((int)num).ToString());
	}

	private static string GetSecondsString(string secs)
	{
		return secs;
	}

	private static string CombineLightningDamage(SDFTreeNode data)
	{
		return string.Format("{0}+{1}", data["InitalDamage"], data["DOTDamage"]);
	}

	private static void EnsureProperInitialLevel(string abilityID)
	{
		if (Singleton<Profile>.instance.GetAbilityLevel(abilityID) == 0)
		{
			Singleton<Profile>.instance.SetAbilityLevel(abilityID, Singleton<Profile>.instance.GetAbilityLevel(abilityID) + 1);
			List<string> selectedAbilities = Singleton<Profile>.instance.GetSelectedAbilities();
			if (selectedAbilities.Count < Singleton<Profile>.instance.maxSelectedAbilities)
			{
				selectedAbilities.Add(abilityID);
				Singleton<Profile>.instance.SetSelectedAbilities(selectedAbilities);
			}
			Singleton<Profile>.instance.Save();
		}
	}

	private static void LevelUpAbility(string abilityID, bool isLastUpgrade)
	{
		Singleton<Profile>.instance.SetAbilityLevel(abilityID, Singleton<Profile>.instance.GetAbilityLevel(abilityID) + 1);
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("AbilityUpgradePurchased", abilityID, Singleton<Profile>.instance.GetAbilityLevel(abilityID));
		if (isLastUpgrade)
		{
			switch (abilityID)
			{
			case "KatanaSlash":
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_MUSHASHI");
				break;
			case "DivineIntervention":
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_SOMEONE_UP_THERE");
				break;
			case "GiantWave":
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_ALL_OUT_MELEE");
				break;
			case "SummonTornadoes":
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_CATEGORY_5");
				break;
			case "Lethargy":
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_SLOTH");
				break;
			case "SummonLightning":
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_FROM_THE_HEAVENS");
				break;
			}
		}
	}
}
