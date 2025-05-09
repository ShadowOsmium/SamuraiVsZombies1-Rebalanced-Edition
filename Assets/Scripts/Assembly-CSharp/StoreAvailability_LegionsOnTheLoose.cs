using System.Collections.Generic;

public class StoreAvailability_LegionsOnTheLoose
{
	public static void GetAll(List<StoreData.Item> items)
	{
		if (Singleton<PlayModesManager>.instance.selectedMode == "zombies")
		{
			Get("LegionOnTheLoose", items, Singleton<Profile>.instance.legionOnTheLooseLevel, UpgradeLegion);
			Get("DeathFromAbove", items, Singleton<Profile>.instance.deathFromAboveLevel, UpgradeDeathFromAbove);
		}
	}

	private static void Get(string powerID, List<StoreData.Item> items, int currentLevel, OnSUIGenericCallback onPurchase)
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/" + powerID);
		if (sDFTreeNode == null)
		{
			return;
		}
		bool flag = false;
		if (Singleton<Profile>.instance.highestUnlockedWave < int.Parse(sDFTreeNode["waveToUnlock"]))
		{
			flag = true;
		}
		int num = currentLevel + 1;
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
		if (sDFTreeNode2 == null)
		{
			return;
		}
		StoreData.Item item = new StoreData.Item(onPurchase);
		if (num != 1)
		{
			SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(string.Format("{0:000}", num - 1));
			if (sDFTreeNode3 == null)
			{
			}
		}
		item.id = powerID;
		item.icon = sDFTreeNode["icon"];
		item.locked = flag;
		item.unlockAtWave = int.Parse(sDFTreeNode["waveToUnlock"]);
		if (flag)
		{
			item.unlockCondition = string.Format(Singleton<Localizer>.instance.Get("store_unlockatwave"), item.unlockAtWave);
		}
		if (num == 1)
		{
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_purchase"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num);
		}
		else
		{
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_levelup"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num);
		}
		item.details.AddSmallDescription(Singleton<Localizer>.instance.Parse(sDFTreeNode["desc"]));
		item.unlockTitle = Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]);
		item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor(powerID));
		item.isEvent = sDFTreeNode["eventID"] != string.Empty;
		items.Add(item);
	}

	private static void UpgradeLegion()
	{
		Singleton<Profile>.instance.legionOnTheLooseLevel++;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("Upgrade_LegionOnTheLoose", string.Empty);
	}

	private static void UpgradeDeathFromAbove()
	{
		Singleton<Profile>.instance.deathFromAboveLevel++;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("Upgrade_DeathFromAbove", string.Empty);
	}
}
