using System.Collections.Generic;

public class StoreAvailability
{
	public enum Group
	{
		Hero = 0,
		Helpers = 1,
		Consumables = 2,
		Charms = 3,
		All = 4
	}

	public static List<StoreData.Item> GetList()
	{
		return GetList(Group.All);
	}

	public static List<StoreData.Item> GetList(Group g)
	{
		List<StoreData.Item> list = new List<StoreData.Item>();
		if (g == Group.Hero && ApplicationUtilities.IsGWalletAvailable())
		{
			GetVIP(list);
		}
		if (g == Group.Hero || g == Group.All)
		{
			GetBoosterPack(list);
			GetHeroLevelPurchase(list);
			GetSword(list);
			GetBow(list);
			GetSmithy(list);
			GetBaseUpgrade(list);
			GetBellUpgrade(list);
			GetVillageArchersUpgrade(list);
			GetUndeathUpgrade(list);
			StoreAvailability_LegionsOnTheLoose.GetAll(list);
			StoreAvailability_Abilities.GetAll(list);
		}
		if (g == Group.Helpers || g == Group.All)
		{
			StoreAvailability_Helpers.Get(list);
		}
		if (g == Group.Charms || g == Group.All)
		{
			GetCharms(list);
		}
		if (g == Group.Consumables || g == Group.All)
		{
			GetSpecialDoubleCoins(list);
			StoreAvailability_DealPacks.Get(list);
			GetPachinkoBalls(list);
			GetPotions(list);
		}
		int num = 0;
		foreach (StoreData.Item item in list)
		{
			item._reserved = num++;
		}
		list.Sort(new StoreData.ItemsListSorter());
		return list;
	}

	private static void GetBoosterPack(List<StoreData.Item> items)
	{
		if (!(Singleton<PlayModesManager>.instance.selectedMode != "classic"))
		{
			StoreData.Item item = new StoreData.Item(delegate
			{
			});
			item.id = "goto_boosters";
			item.icon = "Sprites/Icons/booster_pack";
			item.title = SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Get("normal_store_link_title"));
			items.Add(item);
		}
	}

	private static void GetHeroLevelPurchase(List<StoreData.Item> items)
	{
		int num = Singleton<Profile>.instance.heroLevel + 1;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Heroes/" + Singleton<Profile>.instance.heroID);
		bool isLastUpgrade = sDFTreeNode.childCount == num;
		StoreData.Item item = new StoreData.Item(delegate
		{
			LevelUpHero(isLastUpgrade);
		});
		item.details.SetColumns(num - 1, num);
		item.details.AddStat("health_stats", InfiniteUpgrades.Extrapolate<int>(sDFTreeNode, "infiniteUpgradeHealth", "health", num - 1).ToString(), InfiniteUpgrades.Extrapolate<int>(sDFTreeNode, "infiniteUpgradeHealth", "health", num).ToString());
		if (sDFTreeNode.hasChild(num))
		{
			SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(num);
			item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor("hero", num.ToString()));
		}
		else
		{
			item.cost = new Cost(sDFTreeNode["infiniteUpgradeCost"], Singleton<SalesDatabase>.instance.GetSaleFor("hero", num.ToString()));
		}
		item.id = Singleton<Profile>.instance.heroID;
		item.icon = sDFTreeNode["icon"];
		item.title = string.Format(Singleton<Localizer>.instance.Parse(sDFTreeNode["store_levelup"]), num);
		item.details.AddSmallDescription((!sDFTreeNode.hasAttribute("desc")) ? string.Empty : sDFTreeNode["desc"]);
		items.Add(item);
	}

	private static void GetSword(List<StoreData.Item> items)
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Weapons/" + Singleton<Profile>.instance.swordID);
		int num = Singleton<Profile>.instance.swordLevel + 1;
		bool isLastUpgrade = sDFTreeNode.childCount == num;
		StoreData.Item item = new StoreData.Item(delegate
		{
			LevelUpSword(isLastUpgrade);
		});
		item.details.SetColumns(num - 1, num);
		item.details.AddStat("strength_stats", InfiniteUpgrades.Extrapolate<int>(sDFTreeNode, "infiniteUpgradeDamage", "damage", num - 1).ToString(), InfiniteUpgrades.Extrapolate<int>(sDFTreeNode, "infiniteUpgradeDamage", "damage", num).ToString());
		item.icon = InfiniteUpgrades.SnapToHighest<string>(sDFTreeNode, "icon", num);
		item.details.AddSmallDescription(InfiniteUpgrades.SnapToHighest<string>(sDFTreeNode, "desc", num));
		item.title = string.Format(Singleton<Localizer>.instance.Parse(InfiniteUpgrades.SnapToHighest<string>(sDFTreeNode, "title", num)), num);
		if (sDFTreeNode.hasChild(num))
		{
			SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(num);
			item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor("sword", num.ToString()));
		}
		else
		{
			item.cost = new Cost(sDFTreeNode["infiniteUpgradeCost"], Singleton<SalesDatabase>.instance.GetSaleFor("sword", num.ToString()));
		}
		item.id = Singleton<Profile>.instance.swordID;
		items.Add(item);
	}

	private static void GetBow(List<StoreData.Item> items)
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Weapons/" + Singleton<Profile>.instance.bowID);
		int num = Singleton<Profile>.instance.bowLevel + 1;
		bool isLastUpgrade = sDFTreeNode.childCount == num;
		StoreData.Item item = new StoreData.Item(delegate
		{
			LevelUpBow(isLastUpgrade);
		});
		item.details.SetColumns(num - 1, num);
		item.details.AddStat("strength_stats", InfiniteUpgrades.Extrapolate<int>(sDFTreeNode, "infiniteUpgradeDamage", "damage", num - 1).ToString(), InfiniteUpgrades.Extrapolate<int>(sDFTreeNode, "infiniteUpgradeDamage", "damage", num).ToString());
		item.icon = InfiniteUpgrades.SnapToHighest<string>(sDFTreeNode, "icon", num);
		item.details.AddSmallDescription(InfiniteUpgrades.SnapToHighest<string>(sDFTreeNode, "desc", num));
		item.title = string.Format(Singleton<Localizer>.instance.Parse(InfiniteUpgrades.SnapToHighest<string>(sDFTreeNode, "title", num)), num);
		if (sDFTreeNode.hasChild(num))
		{
			SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(num);
			item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor("bow", num.ToString()));
		}
		else
		{
			item.cost = new Cost(sDFTreeNode["infiniteUpgradeCost"], Singleton<SalesDatabase>.instance.GetSaleFor("bow", num.ToString()));
		}
		item.id = Singleton<Profile>.instance.bowID;
		items.Add(item);
	}

	private static void GetSmithy(List<StoreData.Item> items)
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Smithy");
		int num = Singleton<Profile>.instance.initialSmithyLevel + 1;
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
		if (sDFTreeNode2 != null && (!sDFTreeNode2.hasAttribute("hideInStore") || !bool.Parse(sDFTreeNode2["hideInStore"])))
		{
			bool isLastUpgrade = 3 == num;
			StoreData.Item item = new StoreData.Item(delegate
			{
				LevelUpSmithy(isLastUpgrade);
			});
			item.id = "Smithy";
			item.icon = Singleton<PlayModesManager>.instance.selectedModeData["leadershipIcon"];
			item.title = string.Format(Singleton<Localizer>.instance.Parse(sDFTreeNode["purchase_" + Singleton<PlayModesManager>.instance.selectedMode]), num);
			item.cost = new Cost(sDFTreeNode2["storeCost"], Singleton<SalesDatabase>.instance.GetSaleFor("smithy", num.ToString()));
			string text = "desc_" + Singleton<PlayModesManager>.instance.selectedMode;
			item.details.AddSmallDescription((!sDFTreeNode.hasAttribute(text)) ? string.Empty : sDFTreeNode[text]);
			items.Add(item);
		}
	}

	private static void GetBaseUpgrade(List<StoreData.Item> items)
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Base");
		int num = Singleton<Profile>.instance.baseLevel + 1;
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
		if (sDFTreeNode2 != null)
		{
			bool isLastUpgrade = sDFTreeNode.childCount == num;
			StoreData.Item item = new StoreData.Item(delegate
			{
				LevelUpBase(isLastUpgrade);
			});
			SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(string.Format("{0:000}", num - 1));
			if (sDFTreeNode3 != null)
			{
				item.details.AddStat("health_stats", sDFTreeNode3["health"], sDFTreeNode2["health"]);
				item.details.SetColumns(num - 1, num);
			}
			item.id = "Base";
			item.icon = sDFTreeNode2["icon"];
			item.title = string.Format(Singleton<Localizer>.instance.Parse(sDFTreeNode["storeTitle"]), num);
			item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor("base", num.ToString()));
			item.details.AddSmallDescription((!sDFTreeNode.hasAttribute("desc")) ? string.Empty : sDFTreeNode["desc"]);
			items.Add(item);
		}
	}

	private static void GetBellUpgrade(List<StoreData.Item> items)
	{
		if (!Singleton<PlayModesManager>.instance.CheckFlag("useBell"))
		{
			return;
		}
		bool flag = false;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Bell");
		if (Singleton<Profile>.instance.highestUnlockedWave < int.Parse(sDFTreeNode["waveToUnlock"]))
		{
			flag = true;
		}
		int num = Singleton<Profile>.instance.bellLevel + 1;
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
		if (sDFTreeNode2 == null)
		{
			return;
		}
		bool isLastUpgrade = sDFTreeNode.childCount == num;
		StoreData.Item item = new StoreData.Item(delegate
		{
			LevelUpBell(isLastUpgrade);
		});
		if (num == 1)
		{
			item.details.AddStat("strength_stats", sDFTreeNode2["damage"], sDFTreeNode2["damage"]);
			item.details.SetColumns(num, num);
		}
		else
		{
			SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(string.Format("{0:000}", num - 1));
			if (sDFTreeNode3 != null)
			{
				item.details.AddStat("strength_stats", sDFTreeNode3["damage"], sDFTreeNode2["damage"]);
				item.details.SetColumns(num - 1, num);
			}
		}
		item.id = "Bell";
		item.locked = flag;
		item.icon = sDFTreeNode2["icon"];
		item.unlockAtWave = int.Parse(sDFTreeNode["waveToUnlock"]);
		if (flag)
		{
			item.unlockCondition = string.Format(Singleton<Localizer>.instance.Get("store_unlockatwave"), item.unlockAtWave);
		}
		item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor("bell", num.ToString()));
		if (Singleton<Profile>.instance.bellLevel == 0)
		{
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_purchase_atlevel"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num);
		}
		else
		{
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_upgrade_tolevel"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num);
		}
		item.unlockTitle = Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]);
		item.details.AddSmallDescription((!sDFTreeNode.hasAttribute("desc")) ? string.Empty : sDFTreeNode["desc"]);
		items.Add(item);
	}

	private static void GetVillageArchersUpgrade(List<StoreData.Item> items)
	{
		if (!Singleton<PlayModesManager>.instance.CheckFlag("useVillageArchers"))
		{
			return;
		}
		bool flag = false;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/VillageArchers");
		if (Singleton<Profile>.instance.highestUnlockedWave < int.Parse(sDFTreeNode["waveToUnlock"]))
		{
			flag = true;
		}
		int num = Singleton<Profile>.instance.archerLevel + 1;
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
		if (sDFTreeNode2 == null)
		{
			return;
		}
		bool isLastUpgrade = sDFTreeNode.childCount == num;
		StoreData.Item item = new StoreData.Item(delegate
		{
			LevelUpVillageArchers(isLastUpgrade);
		});
		if (num == 1)
		{
			item.details.AddStat("strength_stats", sDFTreeNode2["bowDamage"], sDFTreeNode2["bowDamage"]);
			item.details.SetColumns(num, num);
		}
		else
		{
			SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(string.Format("{0:000}", num - 1));
			if (sDFTreeNode3 != null)
			{
				item.details.AddStat("strength_stats", sDFTreeNode3["bowDamage"], sDFTreeNode2["bowDamage"]);
				item.details.SetColumns(num - 1, num);
			}
		}
		item.id = "VillageArchers";
		item.icon = sDFTreeNode["icon"];
		item.locked = flag;
		item.unlockAtWave = int.Parse(sDFTreeNode["waveToUnlock"]);
		if (flag)
		{
			item.unlockCondition = string.Format(Singleton<Localizer>.instance.Get("store_unlockatwave"), item.unlockAtWave);
		}
		item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor("villageArchers", num.ToString()));
		if (Singleton<Profile>.instance.archerLevel == 0)
		{
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_purchase_atlevel"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num);
		}
		else
		{
			item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_upgrade_tolevel"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num);
		}
		item.unlockTitle = Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]);
		item.details.AddSmallDescription((!sDFTreeNode.hasAttribute("desc")) ? string.Empty : sDFTreeNode["desc"]);
		items.Add(item);
	}

	private static void GetUndeathUpgrade(List<StoreData.Item> items)
	{
		if (Singleton<PlayModesManager>.instance.selectedMode != "zombies")
		{
			return;
		}
		bool flag = false;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Undeath");
		if (Singleton<Profile>.instance.highestUnlockedWave < int.Parse(sDFTreeNode["waveToUnlock"]))
		{
			flag = true;
		}
		int num = Singleton<Profile>.instance.undeathLevel + 1;
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
		if (sDFTreeNode2 == null)
		{
			return;
		}
		bool isLastUpgrade = sDFTreeNode.childCount == num;
		StoreData.Item item = new StoreData.Item(delegate
		{
			LevelUpUndeath(isLastUpgrade);
		});
		if (num == 1)
		{
			item.details.AddStat("health_stats", sDFTreeNode2["regenerateHealth"], sDFTreeNode2["regenerateHealth"]);
			item.details.SetColumns(num, num);
		}
		else
		{
			SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(string.Format("{0:000}", num - 1));
			if (sDFTreeNode3 != null)
			{
				item.details.AddStat("health_stats", sDFTreeNode3["regenerateHealth"], sDFTreeNode2["regenerateHealth"]);
				item.details.SetColumns(num - 1, num);
			}
		}
		item.id = "Undeath";
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
		item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor("undeath", num.ToString()));
		item.isEvent = sDFTreeNode["eventID"] != string.Empty;
		items.Add(item);
	}

	public static void GetSpecialDoubleCoins(List<StoreData.Item> items)
	{
		if (!Singleton<Profile>.instance.earnsDoubleCoins)
		{
			StoreData.Item item = new StoreData.Item(delegate
			{
				BuyDoubleCoins();
			});
			item.id = "special_earnsDoubleCoins";
			item.icon = Singleton<PlayModesManager>.instance.selectedModeData["doubleCoins"];
			item.title = Singleton<Localizer>.instance.Get("earnsdoublecoins_title");
			item.details.AddDescription(Singleton<Localizer>.instance.Get("earnsdoublecoins_desc"));
			item.cost = new Cost(Singleton<Config>.instance.root.to("miscStore")["earnsDoubleCoinsCost"], Singleton<SalesDatabase>.instance.GetSaleFor("doubleCoins"));
			items.Add(item);
		}
	}

	public static void GetPachinkoBalls(List<StoreData.Item> items)
	{
		int kNumBalls = int.Parse(Singleton<Config>.instance.root.to("miscStore")["ballsPackNum"]);
		StoreData.Item item = new StoreData.Item(delegate
		{
			BuyBalls(kNumBalls);
		});
		item.id = "special_pachinkoBallsPack";
		item.icon = Singleton<PlayModesManager>.instance.selectedModeData["pachinkoBalls"];
		item.title = string.Format(Singleton<Localizer>.instance.Get("pachinkopack_title"), kNumBalls);
		item.details.AddDescription(string.Format(Singleton<Localizer>.instance.Get("pachinkopack_desc"), kNumBalls));
		item.cost = new Cost(Singleton<Config>.instance.root.to("miscStore")["ballsPackCost"], Singleton<SalesDatabase>.instance.GetSaleFor("balls"));
		items.Add(item);
	}

	public static void GetPotions(List<StoreData.Item> items)
	{
		string[] allIDsForActivePlayMode = Singleton<PotionsDatabase>.instance.allIDsForActivePlayMode;
		foreach (string text in allIDsForActivePlayMode)
		{
			if (Singleton<EventsManager>.instance.IsEventActive(Singleton<PotionsDatabase>.instance.GetAttribute(text, "eventID")))
			{
				string delegateArg = text;
				StoreData.Item item = new StoreData.Item(delegate
				{
					BuyPotions(delegateArg);
				});
				item.id = text;
				item.icon = Singleton<PotionsDatabase>.instance.GetAttribute(text, "icon");
				item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_purchase_x_items"), Singleton<PotionsDatabase>.instance.GetAttribute(text, "storePack"), Singleton<PotionsDatabase>.instance.GetAttribute(text, "displayName"));
				item.details.AddDescription(Singleton<PotionsDatabase>.instance.GetAttribute(text, "storeDesc"));
				item.cost = new Cost(Singleton<PotionsDatabase>.instance.GetAttribute(text, "cost"), Singleton<SalesDatabase>.instance.GetSaleFor(text));
				item.isEvent = Singleton<PotionsDatabase>.instance.GetAttribute(text, "eventID") != string.Empty;
				items.Add(item);
			}
		}
	}

	public static void GetCharms(List<StoreData.Item> items)
	{
		string[] allIDsForActivePlayMode = Singleton<CharmsDatabase>.instance.allIDsForActivePlayMode;
		foreach (string text in allIDsForActivePlayMode)
		{
			if (Singleton<EventsManager>.instance.IsEventActive(Singleton<CharmsDatabase>.instance.GetAttribute(text, "eventID")) && Singleton<CharmsDatabase>.instance.GetAttribute(text, "store").ToLower() == "yes")
			{
				string delegateArg = text;
				StoreData.Item item = new StoreData.Item(delegate
				{
					BuyCharms(delegateArg);
				});
				item.id = text;
				item.icon = Singleton<CharmsDatabase>.instance.GetAttribute(text, "icon");
				item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_purchase"), Singleton<CharmsDatabase>.instance.GetAttribute(text, "displayName"));
				item.details.AddDescription(Singleton<CharmsDatabase>.instance.GetAttribute(text, "storeDesc"));
				item.cost = new Cost(Singleton<CharmsDatabase>.instance.GetAttribute(text, "cost"), Singleton<SalesDatabase>.instance.GetSaleFor(text));
				item.isEvent = Singleton<CharmsDatabase>.instance.GetAttribute(text, "eventID") != string.Empty;
				items.Add(item);
			}
		}
	}

	private static void LevelUpHero(bool isLastUpgrade)
	{
		Singleton<Profile>.instance.heroLevel++;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("AbilityUpgradePurchased", "Hero", Singleton<Profile>.instance.heroLevel);
		if (isLastUpgrade)
		{
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_FIGHT_WITH_HONOR");
		}
	}

	private static void LevelUpSmithy(bool isLastUpgrade)
	{
		Singleton<Profile>.instance.initialSmithyLevel++;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("AbilityUpgradePurchased", "Smithy", Singleton<Profile>.instance.initialSmithyLevel);
		if (isLastUpgrade)
		{
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_FOLLOW_ANYWHERE");
		}
	}

	private static void LevelUpSword(bool isLastUpgrade)
	{
		Singleton<Profile>.instance.swordLevel++;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("AbilityUpgradePurchased", "Sword", Singleton<Profile>.instance.swordLevel);
		if (isLastUpgrade)
		{
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_SO_SHARP");
		}
	}

	private static void LevelUpBow(bool isLastUpgrade)
	{
		Singleton<Profile>.instance.bowLevel++;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("AbilityUpgradePurchased", "Crossbow", Singleton<Profile>.instance.bowLevel);
		if (isLastUpgrade)
		{
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_TAKE_THE_SHOT");
		}
	}

	private static void LevelUpBase(bool isLastUpgrade)
	{
		Singleton<Profile>.instance.baseLevel++;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("AbilityUpgradePurchased", "Gate", Singleton<Profile>.instance.baseLevel);
		if (isLastUpgrade)
		{
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_LAST_STAND");
		}
	}

	private static void LevelUpBell(bool isLastUpgrade)
	{
		Singleton<Profile>.instance.bellLevel++;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("AbilityUpgradePurchased", "Bell", Singleton<Profile>.instance.bellLevel);
		if (isLastUpgrade)
		{
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_DING_DONG");
		}
	}

	private static void LevelUpUndeath(bool isLastUpgrade)
	{
		Singleton<Profile>.instance.undeathLevel++;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("AbilityUpgradePurchased", "Undeath", Singleton<Profile>.instance.undeathLevel);
	}

	private static void LevelUpVillageArchers(bool isLastUpgrade)
	{
		Singleton<Profile>.instance.archerLevel++;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("AbilityUpgradePurchased", "VillageArchers", Singleton<Profile>.instance.archerLevel);
		if (isLastUpgrade)
		{
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_GIVE_THEM_A_VOLLEY");
		}
	}

	private static void BuyPotions(string potionID)
	{
		int num = int.Parse(Singleton<PotionsDatabase>.instance.GetAttribute(potionID, "storePack"));
		Singleton<Profile>.instance.SetNumPotions(potionID, Singleton<Profile>.instance.GetNumPotions(potionID) + num);
		Singleton<Profile>.instance.Save();
		if (potionID.Contains("health"))
		{
			Singleton<Analytics>.instance.LogEvent("HealthPotionPurchased", string.Empty);
		}
		else if (potionID.Contains("leadership"))
		{
			Singleton<Analytics>.instance.LogEvent("LeadershipPotionPurchased", string.Empty);
		}
		else if (potionID.Contains("revive"))
		{
			Singleton<Analytics>.instance.LogEvent("RevivePotionPurchased", string.Empty);
		}
		else
		{
			Singleton<Analytics>.instance.LogEvent("MiscPotionPurchased", potionID);
		}
	}

	private static void BuyCharms(string charmID)
	{
		Singleton<Profile>.instance.SetNumCharms(charmID, Singleton<Profile>.instance.GetNumCharms(charmID) + 1);
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("CharmPurchased", charmID);
	}

	private static void BuyDoubleCoins()
	{
		Singleton<Profile>.instance.earnsDoubleCoins = true;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("SpecialDoubleCoinsPurchased", string.Empty);
	}

	private static void BuyBalls(int num)
	{
		Singleton<Profile>.instance.pachinkoBalls += num;
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("BallsPurchased", num.ToString());
	}

	public static void GetVIP(List<StoreData.Item> items)
	{
		int num = 0;
		if (ApplicationUtilities.IsGWalletAvailable())
		{
			if (ApplicationUtilities.GWalletIsSubscriberToPlan(".silver.monthly"))
			{
				num = 1;
			}
			else if (ApplicationUtilities.GWalletIsSubscriberToPlan(".gold.monthly"))
			{
				num = 2;
			}
		}
		StoreData.Item item = new StoreData.Item(delegate
		{
		});
		item.id = "vip";
		switch (num)
		{
		case 1:
			item.icon = "Sprites/Menus/gw_icon_silver";
			break;
		case 2:
			item.icon = "Sprites/Menus/gw_icon_gold";
			break;
		default:
			item.icon = "Sprites/Menus/gw_icon";
			break;
		}
		item.title = SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Get((num <= 0) ? "vip_btn_title" : "vip_btn_title_with_plan"));
		items.Add(item);
	}
}
