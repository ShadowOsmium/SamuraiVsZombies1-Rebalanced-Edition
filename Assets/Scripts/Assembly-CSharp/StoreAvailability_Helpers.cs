using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class StoreAvailability_Helpers
{
	public static void Get(List<StoreData.Item> items)
	{
		string[] allIDs = Singleton<HelpersDatabase>.instance.allIDs;
		foreach (string text in allIDs)
		{
			bool flag = false;
			int num = Singleton<Profile>.instance.GetHelperLevel(text) + 1;
			SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Helpers/" + text);
			if (!Singleton<EventsManager>.instance.IsEventActive(sDFTreeNode["eventID"]))
			{
				continue;
			}
			if (num == 1 && Singleton<Profile>.instance.highestUnlockedWave < int.Parse(sDFTreeNode["waveToUnlock"]))
			{
				flag = true;
			}
			else
			{
				EnsureProperInitialHelperLevel(text);
			}
			SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", num));
			if (sDFTreeNode2 == null)
			{
				FillInfinityPath(text, sDFTreeNode, items);
			}
			else
			{
				if ((sDFTreeNode2.hasAttribute("hideInStore") && SUILayoutConv.GetBool(sDFTreeNode2["hideInStore"])) || (sDFTreeNode.hasAttribute("hideInStore") && SUILayoutConv.GetBool(sDFTreeNode["hideInStore"])))
				{
					continue;
				}
				string delegateArg = text;
				bool isLastUpgrade = sDFTreeNode.childCount == num;
				StoreData.Item item = new StoreData.Item(delegate
				{
					LevelUpHelper(delegateArg, isLastUpgrade);
				});
				string attribName = ((!sDFTreeNode2.hasAttribute("meleeDamage")) ? "bowDamage" : "meleeDamage");
				if (num == 1)
				{
					item.details.AddStat("health_stats", sDFTreeNode2["health"], sDFTreeNode2["health"]);
					item.details.AddStat("strength_stats", sDFTreeNode2[attribName], sDFTreeNode2[attribName]);
					item.details.SetColumns(num, num);
				}
				else
				{
					SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(string.Format("{0:000}", num - 1));
					if (sDFTreeNode3 != null)
					{
						item.details.AddStat("health_stats", sDFTreeNode3["health"], sDFTreeNode2["health"]);
						item.details.AddStat("strength_stats", sDFTreeNode3[attribName], sDFTreeNode2[attribName]);
						item.details.SetColumns(num - 1, num);
					}
				}
				item.id = text;
				item.icon = sDFTreeNode["HUDIcon"];
				item.isEvent = sDFTreeNode["eventID"] != string.Empty;
				item.locked = flag;
				item.unlockAtWave = int.Parse(sDFTreeNode["waveToUnlock"]);
				if (flag)
				{
					if (sDFTreeNode2.hasAttribute("specialUnlockText"))
					{
						item.unlockCondition = Singleton<Localizer>.instance.Parse(sDFTreeNode2["specialUnlockText"]);
						switch (item.id)
						{
						case "Rifleman":
						case "Swordsmith":
						case "IceArcher":
						case "Takeda":
						case "Necromancer":
						{
							item.isFoundInPresent = true;
							SDFTreeNode sDFTreeNode4 = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/resources");
							SDFTreeNode sDFTreeNode5 = sDFTreeNode4.to("presentA").to("normal").to(Singleton<PlayModesManager>.instance.selectedMode);
							SDFTreeNode sDFTreeNode6 = sDFTreeNode4.to("presentB").to("normal").to(Singleton<PlayModesManager>.instance.selectedMode);
							SDFTreeNode sDFTreeNode7 = sDFTreeNode4.to("presentC").to("normal").to(Singleton<PlayModesManager>.instance.selectedMode);
							SDFTreeNode sDFTreeNode8 = sDFTreeNode4.to("presentD").to("normal").to(Singleton<PlayModesManager>.instance.selectedMode);
							if (sDFTreeNode5.hasAttribute(item.id))
							{
								item.containedInPresent.Add("Sprites/Icons/present_red");
							}
							if (sDFTreeNode6.hasAttribute(item.id))
							{
								item.containedInPresent.Add("Sprites/Icons/present_blue");
							}
							if (sDFTreeNode7.hasAttribute(item.id) || sDFTreeNode8.hasAttribute(item.id))
							{
								item.containedInPresent.Add("Sprites/Icons/present_yellow");
							}
							break;
						}
						}
					}
					else
					{
						item.unlockCondition = string.Format(Singleton<Localizer>.instance.Get("store_unlockatwave"), item.unlockAtWave);
					}
				}
				item.cost = new Cost(sDFTreeNode2["cost"], Singleton<SalesDatabase>.instance.GetSaleFor(text));
				if (num == 1)
				{
					item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_recruit"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]));
				}
				else
				{
					item.title = string.Format(Singleton<Localizer>.instance.Get("store_format_levelup"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]), num);
				}
				item.unlockTitle = Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]);
				item.details.AddSmallDescription((!sDFTreeNode.hasAttribute("desc")) ? string.Empty : sDFTreeNode["desc"]);
				items.Add(item);
			}
		}
	}

	private static void FillInfinityPath(string helperID, SDFTreeNode mainData, List<StoreData.Item> items)
	{
	}

	private static void EnsureProperInitialHelperLevel(string helperID)
	{
		int helperLevel = Singleton<Profile>.instance.GetHelperLevel(helperID);
		if (helperLevel == 0)
		{
			Singleton<Profile>.instance.SetHelperLevel(helperID, helperLevel + 1, true);
			Singleton<Profile>.instance.Save();
		}
	}

	private static void LevelUpHelper(string helperID, bool isLastUpgrade)
	{
		Singleton<Profile>.instance.SetHelperLevel(helperID, Singleton<Profile>.instance.GetHelperLevel(helperID) + 1, true);
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("AbilityUpgradePurchased", helperID, Singleton<Profile>.instance.GetHelperLevel(helperID));
		if (!isLastUpgrade)
		{
			return;
		}
		SDFTreeNode sDFTreeNode = SDFTree.LoadFromResources("Registry/Helpers/" + helperID);
		if (sDFTreeNode == null)
		{
			Debug.Log("ERROR: Could not find helper data to retrieve Achievements infos: " + helperID);
			return;
		}
		string text = sDFTreeNode["achievementGC"];
		if (text != string.Empty)
		{
			SingletonMonoBehaviour<Achievements>.instance.Award(text);
		}
	}
}
